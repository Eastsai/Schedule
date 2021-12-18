using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDao.DataModel
{
    public class ScheduleJobStatus
    {
        [Display(Name = "編號")]
        public string Sno { get; }

        [Display(Name = "排程名稱")]
        public string ScheduleName { get; set; }
        
        [Display(Name = "工作名稱")]
        public string JobName { get; set; }

        [Display(Name = "工作群組")]
        public string JobGroup { get; set; }

        [Display(Name = "Cron")]
        public string TriggerCronExpression { get; set; }

        [Display(Name = "排程狀態")]
        public string IsActive { get; set; }

    }
}
