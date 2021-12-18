using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceProcess;
using System.Web.Mvc;
using ScheduleManagement.Models;
using ScheduleManagement.BusinessLogics;

namespace ScheduleManagement.Controllers
{
    public class ScheduleController : Controller
    {
        CommonContext common = new CommonContext();
        ScheduleContext schedule = new ScheduleContext();
     
        public ActionResult ScheduleStatus()
        {
            ScheduleStatusViewModel model = new ScheduleStatusViewModel();
            model.ScheduleList = common.GetScheduleList();
            return View(model);
        }

        public ActionResult ScheduleStatusList(ScheduleStatusViewModel model) {
            model.ScheduleList = common.GetScheduleList();                   
            model = schedule.ScheduleQueryList(model);            
            if (model.ScheduleStatus == null) {
                model.QueryResult = MessageCode.NoData;
            }else if (model.ScheduleStatus.Count <= 0) {
                model.QueryResult = MessageCode.NoData;
            }
            return View("ScheduleStatus",model);
        }

        public ActionResult ExecuteJobOnce(string JobSno,string ScheduleSno,string JobName) {
            bool executed = schedule.ExecuteServiceCommand(JobSno,(int)JobAction.ExecuteOnce);            
            ScheduleStatusViewModel model = new ScheduleStatusViewModel
            {                
                ScheduleSno = ScheduleSno,
                ExecuteResult = JobName + ((executed) ? "已執行" : "未執行")
            };
            return RedirectToAction("ScheduleStatusList",model);            
        }

        public ActionResult PauseJob(string JobSno, string ScheduleSno, string JobName) {
            bool executed = schedule.ExecuteServiceCommand(JobSno, (int)JobAction.Pause);
            ScheduleStatusViewModel model = new ScheduleStatusViewModel
            {
                ScheduleSno = ScheduleSno,
                ExecuteResult = JobName + ((executed) ? "已暫停" : "未暫停")
            };
            return RedirectToAction("ScheduleStatusList", model);
        }

        public ActionResult ResumeJob(string JobSno, string ScheduleSno, string JobName)
        {
            bool executed = schedule.ExecuteServiceCommand(JobSno, (int)JobAction.Resume);
            ScheduleStatusViewModel model = new ScheduleStatusViewModel
            {
                ScheduleSno = ScheduleSno,
                ExecuteResult = JobName + ((executed) ? "已繼續" : "未繼續")
            };
            return RedirectToAction("ScheduleStatusList", model);
        }

        public ActionResult InterrupJob(string JobSno, string ScheduleSno, string JobName)
        {
            bool executed = schedule.ExecuteServiceCommand(JobSno, (int)JobAction.Interrupt);
            ScheduleStatusViewModel model = new ScheduleStatusViewModel
            {
                ScheduleSno = ScheduleSno,
                ExecuteResult = JobName + ((executed) ? "已中斷" : "未中斷")
            };
            return RedirectToAction("ScheduleStatusList", model);
        }
    }
}