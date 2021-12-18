using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDao.DataModel
{
    public class ScheduleJob
    {
        public string Sno { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "排程名稱")]
        public string ScheduleName { get; set; }

        [Required]
        [MaxLength(20)]       
        [Display(Name = "工作名稱")]
        public string JobName { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "工作程式名稱")]
        public string JobProgramName { get; set; }
        
        [Required]
        [MaxLength(20)]
        [Display(Name = "工作群組")]
        public string JobGroup { get; set; }
        
        [MaxLength(50)]
        [Display(Name = "工作描述")]
        public string JobDescription { get; set; }

        [Required]        
        [Display(Name = "持久")]
        public string Durable { get; set; }

        [Required]
        [Display(Name = "覆蓋")]
        public string Recover { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "觸發時間")]
        public string TriggerCronExpression { get; set; }

        [Required]
        [Display(Name = "是否啟用")]
        public string IsActive { get; set; }
    }
}