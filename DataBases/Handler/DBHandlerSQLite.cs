//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;

//namespace SQLHelper.Handler
//{
//    // TODO : SQLite 구현
//    public class DBHandlerSQLite
//    {
//        public static string dbFileName = @"C:\CMPOSTBOX\MST\MST.db";
//        private static string[] tableList = null;

//        public static string[] GetTableList()
//        {
//            if (tableList == null)
//            {
//                var dt = ExecuteDataTable("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;");
//                tableList = new string[dt.Rows.Count];
//                for (int i = 0; i < dt.Rows.Count; i++)
//                {
//                    tableList[i] = dt.Rows[i]["name"].ToString();
//                }
//            }
//            return (string[])tableList.Clone();
//        }

//        private static string connectionString
//        {
//            get { return string.Format(@"Data Source={0};Version=3;", dbFileName); }
//        }

//        public static DataTable ExecuteDataTable(string query)
//        {
//            DataTable dtResult = null;
//            using (var conn = new SQLiteConnection(connectionString))
//            {
//                conn.Open();
//                var cmd = new SQLiteCommand(query, conn);
//                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
//                DataSet ds = new DataSet();
//                da.Fill(ds);
//                dtResult = ds.Tables[0];
//                conn.Close();
//            }
//            return dtResult;
//        }

//        public static int ExecuteNonQuery(string query)
//        {
//            int result = -1;
//            using (var conn = new SQLiteConnection(connectionString))
//            {
//                conn.Open();
//                var cmd = new SQLiteCommand(query, conn);
//                result = cmd.ExecuteNonQuery();
//                conn.Close();
//            }
//            return result;
//        }
//    }
//}
