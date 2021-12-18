using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.ServiceProcess;
using ScheduleManagement.Models;
using ScheduleDao;
using ScheduleDao.DataModel;

namespace ScheduleManagement.BusinessLogics
{
    public class ScheduleContext : BaseContext
    {
        ScheduleJobDao scheduleJobDao = new ScheduleJobDao();       
        public ScheduleStatusViewModel ScheduleQueryList(ScheduleStatusViewModel model)
        {           
            string serviceName = scheduleJobDao.GetServiceNameByScheduleSno(dbSchedule,model.ScheduleSno);
            model.ServiceName = serviceName;
            ServiceController sc = new ServiceController(serviceName);
            if (sc != null)
            {
                model.ServiceStatus = sc.Status.ToString();
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    model.ServiceStatus = sc.Status.ToString();
                    return model;
                }
                sc.ExecuteCommand(250);
            }
            else {
                model.ServiceStatus = "查無服務";
            }
            Thread.Sleep(500);
            model.ScheduleStatus = scheduleJobDao.ScheduleQueryList(dbSchedule, model.ScheduleSno);
            sc.Close();
            return model;
        }

        public bool ExecuteServiceCommand(string jobSno,int jobAction) {            
            string jobNumber = scheduleJobDao.GetJobNumber(dbSchedule,jobSno);            
            int serviceCmd = Convert.ToInt32(jobNumber + jobAction.ToString());            
            string serviceName = scheduleJobDao.GetServiceNameByJobSno(dbSchedule,jobSno);
            ServiceController sc = new ServiceController(serviceName);
            if (sc != null)
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.ExecuteCommand(serviceCmd);
                    return true;
                }
                sc.Close();
            }          
            return false;        
        }

    }
}