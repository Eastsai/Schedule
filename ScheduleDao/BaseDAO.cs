using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ScheduleDao
{    
    public class BaseDAO : ConvertDatas
    {
        //回傳sqlcmd
        //適用where條件的數量會變動時
        protected SqlCommand GetQuerySqlCommand(string sqlStr,List<CommandExpress> exps) {
            StringBuilder sb = new StringBuilder(sqlStr);
            sb.Append(GetWhereExpString(exps));
            SqlCommand sCmd = new SqlCommand(sb.ToString());
            foreach (var item in exps)
            {
                sCmd.Parameters.AddWithValue(item.ColumnName,item.ColumnValue);
            }            
            return sCmd;
        }

        //回傳sqlcmd
        //適用set、where條件的數量會變動時
        protected SqlCommand GetUpdateSqlCommand(string sqlStr,Dictionary<string,string> sets,List<CommandExpress> exps) {
            SqlCommand sCmd = new SqlCommand();
            StringBuilder sb = new StringBuilder(sqlStr);            
            if (sets != null) {
                sb.Append(GetSetExpString(sets));
                sb.Append(GetWhereExpString(exps));
                sCmd.CommandText = sb.ToString();
                foreach (var item in sets)
                {
                    sCmd.Parameters.AddWithValue(item.Key,item.Value);
                }
                foreach (var item in exps)
                {
                    sCmd.Parameters.AddWithValue(item.ColumnName,item.ColumnValue);
                }               
            }            
            return sCmd;
        }

        protected SqlCommand GetUpdateSqlCommand(string sqlStr, List<CommandExpress> sets, List<CommandExpress> exps)
        {
            SqlCommand sCmd = new SqlCommand();
            StringBuilder sb = new StringBuilder(sqlStr);
            if (sets != null)
            {
                sb.Append(GetSetExpString(sets));
                sb.Append(GetWhereExpString(exps));
                sCmd.CommandText = sb.ToString();
                foreach (var item in sets)
                {
                    sCmd.Parameters.AddWithValue(item.ColumnName, item.ColumnValue);
                }
                foreach (var item in exps)
                {
                    sCmd.Parameters.AddWithValue(item.ColumnName, item.ColumnValue);
                }
            }
            return sCmd;
        }

        //回傳sqlcmd
        //適用where條件的數量會變動時
        protected SqlCommand GetDeleteSqlCommand(string sqlStr, List<CommandExpress> exps) {
            StringBuilder sb = new StringBuilder(sqlStr);
            sb.Append(GetWhereExpString(exps));
            SqlCommand sCmd = new SqlCommand(sb.ToString());
            foreach (var item in exps)
            {
                sCmd.Parameters.AddWithValue(item.ColumnName,item.ColumnValue);
            }
            return sCmd;
        }

        //傳入sql script
        //回傳SqlCommand,可選擇是否傳入parameters
        protected SqlCommand GetSqlCommand(string sqlstr, [Optional] Dictionary<string, string> param)
        {
            SqlCommand sCmd = new SqlCommand(sqlstr);

            if (param != null)
            {
                foreach (var item in param)
                {
                    sCmd.Parameters.AddWithValue(item.Key, item.Value);
                }
            }
            return sCmd;
        }

        //傳入sql script 
        //回傳異動資料筆數(ExecuteNonQuery)
        protected int ExecuteSqlNonQuery(SqlCommand cmd,string db) {
            int result = 0;                     
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Open();
                result = cmd.ExecuteNonQuery();
                sc.Close();
            }
            return result;
        }

        //傳入sql script 
        //回傳第一列第一行(ExecuteScalar)        
        protected string ExecuteSqlScalar(SqlCommand cmd, string db)
        {
            string result = String.Empty;
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Open();
                result = cmd.ExecuteScalar().ToString();
                sc.Close();
            }
            return result;
        }

        //傳入sql script 
        //回傳List<T>        
        protected List<T> ExecuteSqlDataReader<T>(SqlCommand cmd, string db) where T : new()
        {
            List<T> result = new List<T>();
            
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Open();                
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    result = ConvertSqlDataReaderToList<T>(sdr);
                }
                sc.Close();                
            }
 
            return result;
        }

        //傳入sql scrip
        //回傳string[]
        protected string[] ExecuteSqlStringArray(SqlCommand cmd,string db) {
            string[] result = new string[] { };

            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    result = ConvertSqlDataReaderToStringArray(sdr);
                }
                sc.Close();
            }

            return result;
        }

        //傳入sql script 
        //回傳DataTable        
        protected DataTable ExecuteSqlDataTable<T>(SqlCommand cmd,string db)
        {
            DataTable result = new DataTable();
            using (SqlConnection sc = new SqlConnection(db))
            {
                cmd.Connection = sc;
                sc.Close();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    result = (DataTable)sdr.GetEnumerator();
                }
                sc.Close();
            }
            return result;
        }

        protected void ExecuteBulkCopy(DataTable datas, string tablename, string db){                        
            DataColumnCollection dtcols = datas.Columns;
            //SqlBulkCopy
            SqlConnection sc = new SqlConnection(db);
            SqlBulkCopy bk = new SqlBulkCopy(sc);           
            bk.DestinationTableName = tablename;                     
            for (int i = 0; i < dtcols.Count; i++)
            {
                SqlBulkCopyColumnMapping bkcm = new SqlBulkCopyColumnMapping(dtcols[i].ColumnName, dtcols[i].ColumnName);
                bk.ColumnMappings.Add(bkcm);
            }               
            sc.Open();
            bk.WriteToServer(datas);                
            bk.Close();
            sc.Close();            
        }

        //public int ExecuteInsert<T>(string tbName,T data,string db) {
        protected int ExecuteInsert<T>(string tbName, T data, string db)
        {
            int result = 0;       
            string colstr = "";
            string valstr = "";            
            PropertyInfo[] pi = data.GetType().GetProperties();
            //get sql script
            foreach (var item in pi)
            {
                var value = item.GetValue(data);
                if (value != System.DBNull.Value && value != null && value.ToString() != "") {
                    //insert
                    colstr += $"{item.Name},";
                    //value
                    valstr += $"@{item.Name},";
                }
            }
            colstr = colstr.Remove(colstr.Length-1,1);
            valstr = valstr.Remove(valstr.Length-1,1);
            string sqlstr = $"insert into {tbName} ({colstr}) values({valstr})";
            //sql command
            SqlConnection sc = new SqlConnection(db);
            SqlCommand sCmd = new SqlCommand(sqlstr,sc);
            foreach (var i in pi)
            {
                var value = i.GetValue(data);
                if (value != System.DBNull.Value && value != null && value.ToString() != "")
                {
                    sCmd.Parameters.AddWithValue(i.Name, value);
                }                    
            }
            //execute
            sc.Open();
            result = sCmd.ExecuteNonQuery();
            sc.Close();

            return result;
        }

        protected string GetSetExpString(Dictionary<string,string> sets) {
            StringBuilder sb = new StringBuilder();
            if (sets != null) {
                sb.Append(" set");
                foreach (var item in sets)
                {
                    sb.Append($" {item.Key} = @{item.Value},");
                }
                sb.Remove(sb.Length-1,1);
            }                                    
            return sb.ToString();
        }

        protected string GetSetExpString(List<CommandExpress> sets)
        {
            StringBuilder sb = new StringBuilder();
            if (sets != null)
            {
                sb.Append(" set");
                foreach (var item in sets)
                {
                    sb.Append($" {item.ColumnName} = @{item.ColumnName},");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        protected string GetWhereExpString(List<CommandExpress> exps) {
            StringBuilder sb = new StringBuilder();
            int cntExp = 0;
            foreach (var item in exps)
            {
                string colName = item.ColumnName;
                string colValue = item.ColumnValue;
                string behaviorAct = "";
                string behaviorOperator = item.ColBehaviorOperator;              
                behaviorAct = (cntExp == 0) ? " where" : behaviorAct;
      
                sb.Append($" {behaviorAct} {colName} {behaviorOperator} @{colName}");
                cntExp++;
            }

            return sb.ToString();
        }
    }
    
    public class CommandExpress
    {
        //查詢行為:AND,OR,
        //運算子 : =,>,<,like        
        public string ColBehaviorAct { get; set; }
        public string ColBehaviorOperator { get; set; }
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }

        //public static readonly Dictionary<string, string> BehaviorAct = new Dictionary<string, string>(){
        //    { "OR" , "OR" },
        //    { "AND" , "AND" }
        //};
        //public static readonly Dictionary<string, string> BehaviorOperator = new Dictionary<string, string>() {
        //    { "Equal" , "=" },
        //    { "Less" , "<" },
        //    { "Greater" , ">"},
        //    { "Like" , "like" }
        //};

        public class BehaviorAct { 
            public static readonly string OR = "OR";
            public static readonly string AND = "AND";
        }

        public class BehaviorOperator { 
            public static readonly string Equal = "=";
            public static readonly string NotEqual = "<>";
            public static readonly string Less = "<";
            public static readonly string LessEqual = "<=";
            public static readonly string Greater = ">";
            public static readonly string GreaterEqual = ">=";
            public static readonly string Like = "like";        
        }

    }

    
    
}
