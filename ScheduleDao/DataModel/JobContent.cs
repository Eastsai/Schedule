using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ScheduleDao.DataModel
{
    public class JobContent
    {
        public string Sno { get; set; }
        public string JobNumber { get; set; }      
        public string JobName { get; set; }
        public string JobProgramName { get; set; }
        public string JobGroup { get; set; }            
        public string JobDescription { get; set; }
        public bool Durable { get; set; }       
        public bool Recover { get; set; }     
        public string TriggerCronExpression { get; set; }

    }
}