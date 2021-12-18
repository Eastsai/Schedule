using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using ScheduleDao.DataModel;

namespace ScheduleManagement.Models
{
    public class ScheduleStatusViewModel
    {
        [Display(Name = "排程名稱")]
        public List<SelectListItem> ScheduleList { get; set; }
        public string ScheduleSno { get; set; }
        public string ServiceName { get; set; } = "";
        public string ServiceStatus { get; set; } = "";    
        public List<ScheduleStatus> ScheduleStatus { get; set; }
        public string QueryResult { get; set; }

        public string ExecuteResult { get; set; }
    }
}