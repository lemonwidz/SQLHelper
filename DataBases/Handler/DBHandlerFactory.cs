using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SQLHelper.DataBases.Handler
{
    public class DBHandlerFactory
    {
        public static Common.DB_TYPE DefaultDbType { get; set; } = Common.DB_TYPE.MSSQL;

        public static Interface.IDBHandler GetDBHandler(DBHandlerFactoryOption option)
        {
            if (option.DbType == Common.DB_TYPE.Default)
                option.DbType = DBHandlerFactory.DefaultDbType;

            Interface.IDBHandler result = null;

            if (option.DbType == Common.DB_TYPE.MSSQL)
                result = new DBHandlerSQLServer();
            else if (option.DbType == Common.DB_TYPE.SQLite)
                throw new NotImplementedException();

            result.DbIP = option.DbIP;
            result.DbName = option.DbName;
            result.DbUser = option.DbUser;
            result.DbPassword = option.DbPassword;

            return result;
        }

        public class DBHandlerFactoryOption
        {
            /// <summary>
            /// DB 종류
            /// </summary>
            public Common.DB_TYPE DbType { get; set; } = Common.DB_TYPE.Default;
            /// <summary>
            /// DB 접속 IP (Port가 기본이 아닌경우 쉼표로 구분하여 추가)
            /// </summary>
            public string DbIP { get; set; }
            /// <summary>
            /// DB명
            /// </summary>
            public string DbName { get; set; }
            /// <summary>
            /// DB 접속 USER
            /// </summary>
            public string DbUser { get; set; }
            /// <summary>
            /// DB 접속 PASSWORD
            /// </summary>
            public string DbPassword { get; set; }
        }
    }
}
