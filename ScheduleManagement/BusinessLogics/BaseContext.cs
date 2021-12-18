using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using NLog;

namespace ScheduleManagement.BusinessLogics
{
    public class BaseContext
    {
        protected string dbSchedule = ConfigurationManager.ConnectionStrings["Schedule"].ToString();
        protected Logger logger = LogManager.GetCurrentClassLogger();
    }
}