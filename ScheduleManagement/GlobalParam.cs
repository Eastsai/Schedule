using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScheduleManagement
{
    public class GlobalParam
    {

        public static List<SelectListItem> YesOrNo = new List<SelectListItem> {
            new SelectListItem{ Text="請選擇",Value="" },
            new SelectListItem{ Text="是",Value="1" },
            new SelectListItem{ Text="否",Value="0" }
        };



    }

    public enum JobAction
    {
        ExecuteOnce = 0,
        Pause = 1,
        Resume = 2,
        Interrupt = 3,
        Delete = 4,
        Schedule = 5,
        Renew = 6
    }
}