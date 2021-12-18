using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleDao.DataModel;

namespace ScheduleDao
{
    public class CommonDao : BaseDAO
    {
        public string[] GetJobGroups(string db) {
            string cmdStr = "select distinct JobGroup from JobDetail";
            string[] datas = ExecuteSqlStringArray(GetSqlCommand(cmdStr),db);
            return datas;            
        }

        public string[] GetJobNames(string db) {
            string cmdStr = "select distinct JobName from JobDetail ";
            string[] datas = ExecuteSqlStringArray(GetSqlCommand(cmdStr), db);
            return datas;
        }

        public List<ScheduleListItem> GetSchedules(string db) {

            string cmdStr = "select Sno,ScheduleName from Schedules";
            return ExecuteSqlDataReader<ScheduleListItem>(GetSqlCommand(cmdStr),db);
        }
    }

 
}
