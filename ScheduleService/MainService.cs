using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Configuration;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;
using ScheduleDao.DataModel;


namespace ScheduleService
{
    public class MainService
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        private IScheduler _scheduler;        
        private string _scheduleSetting = ConfigurationManager.AppSettings["ScheduleSetting"].ToString();
        private string connectStr = ConfigurationManager.ConnectionStrings["Schedule"].ToString();
        private static string serviceName = ConfigurationManager.AppSettings["ServiceName"].ToString();
        private ServiceDao serviceDao = new ServiceDao();
        List<ScheduleData> allScheduleJobs = new List<ScheduleData>();

        public MainService()
        {
            try
            {
                Console.WriteLine("Schedule Service");
                switch (_scheduleSetting)
                {
                    case "DB":
                        logger.Trace("Schedule Jobs From DB");
                        Console.WriteLine("Schedule Jobs From DB");
                        SimpleThreadPool threadPool = new SimpleThreadPool();
                        threadPool.Initialize();
                        RAMJobStore jobStore = new RAMJobStore();
                        DirectSchedulerFactory.Instance.CreateScheduler("Schedule1", "Schedule1Instance", threadPool, jobStore);
                        _scheduler = DirectSchedulerFactory.Instance.GetScheduler("Schedule1");
                        InitailSchedulesFromDB();                    
                        _scheduler.Start();
                        GetScheduleStatus();
                        WriteScheduleStatusToDB();
                        break;
                    case "Local":
                        logger.Trace("Schedule Jobs From Config");
                        _scheduler = StdSchedulerFactory.GetDefaultScheduler();        
                        _scheduler.Start();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
         
        }

        public void Start()
        {
            //start scheduler
            logger.Trace("Service Start");
            if (_scheduler != null) { 
                _scheduler.Start();
            }
                
        }

        public void Stop()
        {
            //shutdown scheduler
            logger.Trace("Service Stop");
            if (_scheduler != null) { 
                _scheduler.Shutdown();     
            }
                  
        }

        //128-256
        public void ExecuteCustomCommand(int cmd) {
            logger.Trace("custom");
            //Service function
            if (cmd >= 250)
            {
                switch (cmd)
                {
                    case 250:
                        logger.Trace("250");
                        GetScheduleStatus();
                        WriteScheduleStatusToDB();
                        break;
                    default:
                        break;
                }
            }//job function
            else if(cmd >= 130){
                //130~249,jobNumber:13~24,act:0~9
                string jobNumber = cmd.ToString().Substring(0, 2);
                string jobAct = cmd.ToString().Substring(2,1);
                ScheduleData scheduleData = allScheduleJobs.FirstOrDefault(x => x.JobNumber == jobNumber);
                switch (jobAct)
                {
                    case "0": //執行一次 trigger
                        _scheduler.TriggerJob(scheduleData.JobDetail.Key);
                        logger.Trace($"TriggerJob : {scheduleData.JobName}");
                        break;
                    case "1": 
                        _scheduler.PauseJob(scheduleData.JobDetail.Key);                        
                        allScheduleJobs.FirstOrDefault(x => x.JobDetail.Key == scheduleData.JobDetail.Key).JobStatus = "暫停";
                        logger.Trace($"PauseJob : {scheduleData.JobName}");
                        break;
                    case "2":
                        _scheduler.ResumeJob(scheduleData.JobDetail.Key);
                        logger.Trace($"ResumeJob : {scheduleData.JobName}");
                        allScheduleJobs.FirstOrDefault(x => x.JobDetail.Key == scheduleData.JobDetail.Key).JobStatus = "已排程";
                        break;
                    case "3":
                        _scheduler.Interrupt(scheduleData.JobDetail.Key);
                        logger.Trace($"Interrupt : {scheduleData.JobName}");
                        break;
                    case "4": 
                        _scheduler.DeleteJob(scheduleData.JobDetail.Key);
                        logger.Trace($"DeleteJob : {scheduleData.JobName}");
                        break;
                    case "5": 
                        _scheduler.ScheduleJob(scheduleData.JobDetail, scheduleData.JobTrigger);
                        logger.Trace($"ScheduleJob : {scheduleData.JobName}");
                        break;
                    case "6": //renew job conent from db
                        break;
                    default:
                        break;
                }

            }

        
        }
        public void InitailSchedulesFromDB() {
            logger.Trace("InitailSchedulesFromDB");
            if (_scheduler != null) { 
                if (_scheduler.IsStarted) { 
                    _scheduler.Shutdown(true); 
                }
                _scheduler.Clear();    
                logger.Trace("Clear All");             
            }

            //get jobcontent from db
            List<JobContent> jobs = serviceDao.GetScheduleJobs(connectStr, serviceName);            
            allScheduleJobs = GenScheduleData(jobs);
            foreach (var item in allScheduleJobs)
            {                        
                _scheduler.ScheduleJob(item.JobDetail,item.JobTrigger);
                item.JobStatus = JobStatus.OnSchedule;
                logger.Trace($"Schedule Job : {item.JobSno},{item.JobName}");
            }                        
        }

        public List<ScheduleData> GenScheduleData(List<JobContent> jobs)
        {
            List<ScheduleData> scheduleDatas = new List<ScheduleData>();
            foreach (var item in jobs)
            {                                            
                ScheduleData sd = new ScheduleData {
                    JobSno = item.Sno,
                    JobNumber = item.JobNumber,
                    JobName = item.JobName,
                    JobCron = item.TriggerCronExpression,
                    JobStatus = JobStatus.UnSchedule,
                    JobDetail = GenJobDetail(item),
                    JobTrigger = GenTrigger(item)
                };
                scheduleDatas.Add(sd);
            }
            return scheduleDatas;
        }

        public IJobDetail GenJobDetail(JobContent data) {            
            //JobDetail            
            IJobDetail job = JobBuilder.Create(Type.GetType(data.JobProgramName))
                .WithIdentity(data.JobName).WithDescription(data.JobDescription)
                .StoreDurably(data.Durable).RequestRecovery(data.Recover)
                .Build();
            return job;
        }

        public ITrigger GenTrigger(JobContent data) {
            //Trigger
            ITrigger trigger = TriggerBuilder.Create().ForJob(data.JobName)
                .WithCronSchedule(data.TriggerCronExpression).Build();
            return trigger;
        }

        public void GetScheduleStatus() {

            var executingJobs = _scheduler.GetCurrentlyExecutingJobs();
            foreach (var item in executingJobs)
            {
                var scheduleData = allScheduleJobs.FirstOrDefault(x => x.JobDetail.Key == item.JobDetail.Key);
                if (scheduleData != null) { 
                    scheduleData.JobStatus = JobStatus.Running;
                }                   
            }
            LogScheduleStatus();
        }

        public void LogScheduleStatus() {
            foreach (var item in allScheduleJobs)
            {
                logger.Trace($"{item.JobName},{item.JobStatus}");
            }                    
        }

        public void WriteScheduleStatusToDB() {
            List<ScheduleStatus> scheduleStatuses = new List<ScheduleStatus>();
            string scheduleSno = serviceDao.GetSchduleSnoByServiceName(connectStr,serviceName);
            GetScheduleStatus();
            foreach (var item in allScheduleJobs)
            {
                scheduleStatuses.Add(new ScheduleStatus{
                    ScheduleSno = scheduleSno,
                    JobSno = Convert.ToInt16(item.JobSno),
                    JobName = item.JobName,
                    JobCron = item.JobCron,
                    JobStatus = item.JobStatus                    
                });
            }
            serviceDao.DeleteOldScheduleStatus(connectStr,scheduleSno);
            serviceDao.WriteScheduleStatus(connectStr, scheduleStatuses);
        }

        public void RenewJob(string jobSno)
        {
            JobContent job = serviceDao.GetJobContent(connectStr, jobSno);
            ScheduleData scheduleData = allScheduleJobs.FirstOrDefault(x => x.JobSno == jobSno);
            if (scheduleData != null) {
                _scheduler.DeleteJob(scheduleData.JobDetail.Key);
                scheduleData.JobStatus = JobStatus.UnSchedule;
                scheduleData.JobNumber = job.JobNumber;
                scheduleData.JobName = job.JobName;
                scheduleData.JobDetail = GenJobDetail(job);
                scheduleData.JobTrigger = GenTrigger(job);
                _scheduler.ScheduleJob(scheduleData.JobDetail,scheduleData.JobTrigger);
                scheduleData.JobStatus = JobStatus.OnSchedule;
            }
        }
    }

    public class ScheduleData {         
        public string JobSno { get; set; }
        public string JobNumber { get; set; }
        public string JobName { get; set; }
        public string JobCron { get; set; }
        public string JobStatus { get; set; }
        public IJobDetail JobDetail { get; set; }
        public ITrigger JobTrigger { get; set; }

    }

    public class JobStatus {

        public static readonly string UnSchedule = "未排程";

        public static readonly string OnSchedule = "已排程";

        public static readonly string Running = "執行中";
    
    }

    public enum JobAction {     
        ExecuteOnce = 0,
        Pause = 1,
        Resume = 2,
        Interrupt = 3,
        Delete = 4,
        Schedule = 5,
        Renew = 6
    }
}
