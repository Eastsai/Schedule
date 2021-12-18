using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDao.DataModel
{
    public class ScheduleStatus
    {
        public string ScheduleSno { get; set; }

        [Display(Name = "工作編號")]
        public int JobSno { get; set; }

        [Display(Name = "工作名稱")]
        public string JobName { get; set; }

        [Display(Name = "Cron")]
        public string JobCron { get; set; }

        [Display(Name = "狀態")]
        public string JobStatus { get; set; }
    }
}
