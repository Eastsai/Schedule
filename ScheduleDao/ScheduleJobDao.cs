using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduleDao.DataModel;

namespace ScheduleDao
{
    public class ScheduleJobDao : BaseDAO
    {
        public List<ScheduleStatus> ScheduleQueryList(string db,string Sno) {
            string cmdStr = " select ScheduleSno,JobSno,JobName,JobCron,JobStatus from ScheduleStatusTemp where ScheduleSno = @ScheduleSno";
            Dictionary<string, string> paras = new Dictionary<string, string> { { "ScheduleSno", Sno} };
            return ExecuteSqlDataReader<ScheduleStatus>(GetSqlCommand(cmdStr,paras),db);
        }

        public string GetServiceNameByScheduleSno(string db,string Sno) {
            string cmdStr = "select top 1 ServiceName from [Service] where HasSchedule='1' and ScheduleSno = @ScheduleSno";
            Dictionary<string, string> paras = new Dictionary<string, string> { { "ScheduleSno", Sno} };
            return ExecuteSqlScalar(GetSqlCommand(cmdStr,paras),db);
        }

        public string GetServiceNameByJobSno(string db,string jobSno) {
            string cmdStr = @"select c.ServiceName 
                                from JobDetail a
                                inner join Schedules b on b.sno = a.ScheduleNo
                                inner join [Service] c on c.ScheduleSno = b.Sno
                                where a.sno = @jobSno";
            Dictionary<string, string> paras = new Dictionary<string, string> { { "jobSno", jobSno } };
            return ExecuteSqlScalar(GetSqlCommand(cmdStr, paras), db);
        }
        public string GetJobNumber(string db,string jobSno) {
            string cmdStr = @"select JobNumber from JobNumbering where JobSno = @jobSno and Taked = '1'";
            Dictionary<string, string> paras = new Dictionary<string, string>{ { "jobSno", jobSno } };
            return ExecuteSqlScalar(GetSqlCommand(cmdStr,paras),db);
        }
    }
}
