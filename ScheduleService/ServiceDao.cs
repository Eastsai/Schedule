using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleDao;
using ScheduleDao.DataModel;

namespace ScheduleService
{
    class ServiceDao : BaseDAO
    {
        public List<JobContent> GetScheduleJobs(string db,string serviceName) {
            List<JobContent> jobs = new List<JobContent>();
            //string cmdStr = "SELECT Sno,JobNumber,JobName,JobProgramName,JobGroup,JobDescription,Durable,Recover,TriggerCronExpression FROM [Schedule].[dbo].[JobDetail] where IsActive = '1' and ScheduleName = 'ScheduleService'";          
            string cmdStr = @"SELECT a.Sno,JobNumber,JobName,JobProgramName,JobGroup,JobDescription,Durable,Recover,TriggerCronExpression 
                                FROM JobDetail a
                                inner join JobNumbering b on a.Sno = b.JobSno 
                                inner join Schedules c on a.ScheduleNo = c.Sno
                                inner join [Service] d on c.Sno = d.ScheduleSno 
                                where a.IsActive='1' and  d.ServiceName=@ServiceName";
            Dictionary<string, string> paras = new Dictionary<string, string> { { "ServiceName", serviceName} };
            jobs = ExecuteSqlDataReader<JobContent>(GetSqlCommand(cmdStr,paras),db);
            return jobs;
        }

        public JobContent GetJobContent(string db,string jobSno) {

            JobContent job = new JobContent();
            string cmdStr = "SELECT Sno, JobNumber, JobName, JobProgramName, JobGroup, JobDescription, Durable, Recover, TriggerCronExpression FROM[Schedule].[dbo].[JobDetail] where IsActive = '1' and ScheduleName = 'ScheduleService' and Sno = @Sno";
            Dictionary<string, string> paras = new Dictionary<string, string> { { "Sno",jobSno} };
            job = ExecuteSqlDataReader<JobContent>(GetSqlCommand(cmdStr,paras),db).FirstOrDefault();
            return job;
        }

        public void WriteScheduleStatus(string db,List<ScheduleStatus> data) {
            ExecuteBulkCopy(ConvertListToDataTable(data), "ScheduleStatusTemp", db);
        }

        public string GetSchduleSnoByServiceName(string db,string serviceName) {
            string cmdStr = "select ScheduleSno from [Service] where ServiceName = @serviceName";
            Dictionary<string, string> paras = new Dictionary<string, string> { { "serviceName",serviceName } };
            return ExecuteSqlScalar(GetSqlCommand(cmdStr,paras),db);
        }

        public void DeleteOldScheduleStatus(string db,string scheduleSno)
        {
            string cmdStr = "delete from ScheduleStatusTemp where ScheduleSno = @scheduleSno";
            Dictionary<string, string> paras = new Dictionary<string, string> { { "scheduleSno", scheduleSno } };
            ExecuteSqlNonQuery(GetSqlCommand(cmdStr,paras),db);

        }
    }
}
