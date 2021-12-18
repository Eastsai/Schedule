using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScheduleDao;

namespace ScheduleManagement.BusinessLogics
{
    public class CommonContext : BaseContext
    {        
        CommonDao common = new CommonDao();
        public List<SelectListItem> GetJobGroupsList() {
            var datas = common.GetJobGroups(dbSchedule);
            List<SelectListItem> result = new List<SelectListItem>() { new SelectListItem { Text = "請選擇", Value = "請選擇" } };
            foreach (var item in datas)
            {
                result.Add(new SelectListItem { Text = item , Value = item });
            }
            return result;
        }

        public List<SelectListItem> GetJobNamesList() {
            var datas = common.GetJobNames(dbSchedule);
            List<SelectListItem> result = new List<SelectListItem>() { new SelectListItem { Text = "請選擇", Value = "請選擇" } };
            foreach (var item in datas)
            {
                result.Add(new SelectListItem { Text = item, Value = item });
            }
            return result;

        }

        public List<SelectListItem> GetScheduleList() {
            var datas = common.GetSchedules(dbSchedule);
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (var item in datas)
            {
                result.Add(new SelectListItem { Text = item.ScheduleName,Value=item.Sno.ToString()});
            }
            return result;
        }
    }
}