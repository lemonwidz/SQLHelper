using SQLHelper.DataBases.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SQLHelper.DataBases.Handler
{
    public class DBHandlerSQLServer : Interface.IDBHandler
    {
        private string dbName = string.Empty;
        private string[] tableList = null;


        /// <summary>
        /// DB 접속 IP (Port가 기본이 아닌경우 쉼표로 구분하여 추가)
        /// </summary>
        public string DbIP { get; set; }
        /// <summary>
        /// DB명
        /// </summary>
        public string DbName
        {
            get { return dbName; }
            set
            {
                if (value == dbName)
                    return;
                dbName = value;
                tableList = null;
            }
        }
        /// <summary>
        /// DB 접속 USER
        /// </summary>
        public string DbUser { get; set; }
        /// <summary>
        /// DB 접속 PASSWORD
        /// </summary>
        public string DbPassword { get; set; }

        /// <summary>
        /// 테이블 전체 목록을 가져옴
        /// </summary>
        public string[] GetTableList()
        {
            if (tableList == null)
            {
                var dt = ExecuteDataTable("SELECT name FROM sysobjects where xtype = 'U' AND category = '0' ORDER BY name;");
                tableList = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tableList[i] = dt.Rows[i]["name"].ToString();
                }
            }
            return (string[])tableList.Clone();
        }

        /// <summary>
        /// 테이블에 설정된 PK를 리턴
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string[] GetTablePK(string tableName)
        {
            string query = string.Format("SELECT COLUMN_NAME as [name] FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1 AND TABLE_NAME = '{0}' AND TABLE_SCHEMA = 'dbo'", tableName);
            var dt = ExecuteDataTable(query);
            List<string> result = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                result.Add(dt.Rows[i]["name"].ToString());
            }
            return result.ToArray();
        }

        private string connectionString
        {
            get { return string.Format(@"Data Source={0};Initial Catalog={1};uid={2};Password={3}", DbIP, dbName, DbUser, DbPassword); }
        }

        /// <summary>
        /// 전달 받은 쿼리를 실행. Select 쿼리를 사용해서 테이블을 리턴받을때 사용
        /// </summary>
        public DataTable ExecuteDataTable(string query)
        {
            //DataBases.Proxy.DBProxyInstance.SetDefaultConnectionString(connectionString);
            //var dtResult = DataBases.Proxy.DBProxy.InvokeQuery(query);
            //return dtResult;
            var executor = new SqlServer.SqlServerExecutor(connectionString);
            var result = executor.InvokeQuery(new Common.InvokeOption
            {
                CommandText = query
            });
            return result;
        }

        /// <summary>
        /// 전달 받은 쿼리를 실행. Insert, Update, Delete 를 사용한 테이블 리턴이 아닌 경우에 사용
        /// </summary>
        public int ExecuteNonQuery(string query)
        {
            //int result = -1;
            //DataBases.Proxy.DBProxyInstance.SetDefaultConnectionString(connectionString);
            //var dtResult = DataBases.Proxy.DBProxy.InvokeQuery(query);
            var executor = new SqlServer.SqlServerExecutor(connectionString);
            var result = executor.InvokeNonQuery(new Common.InvokeOption
            {
                CommandText = query                
            });
            return result;
        }

        public string[] GetDBList()
        {
            var dt = this.ExecuteDataTable("SELECT name FROM sys.databases");
            List<string> result = new List<string>();
            for(int i=0; i<dt.Rows.Count; i++)
            {
                result.Add(dt.Rows[i]["name"].ToString());
            }
            return result.ToArray();
        }
    }
}
