/*-------------------------------------------------------------------------------------------------
 * 제  목 : DataBase 처리에 관한 클래스 (MSSQL 버전)
 * 
 * 작성자 : 박재율
 * 작성일 : 2008-01-02
 * 
 * 특이점 : 
 ------------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SQLHelper.DataBases.SqlServer
{
    /// <summary>
    /// [#] DataBase 처리에 관한 클래스
    /// </summary>
    internal class DbAccess
    {
        private SqlConnection con = null;
        private SqlTransaction tran = null;
        private string _connectionString = string.Empty;
        public bool UseTransaction { get; set; }

        /// <summary>
        /// [#] 커넥션을 따로 설정하지않고 생성합니다. SetConnection 메서드를 통해 설정할 수 있습니다.
        /// </summary>
        public DbAccess() { }

        ///// <summary>
        ///// [#] 임의의 커넥션을 기본으로 생성합니다. SetConnection 메서드를 통해 바꿀 수 있습니다.
        ///// </summary>
        ///// <param name="Con">기본으로 사용할 SqlConnection</param>
        //public DbAccess(SqlConnection con)
        //{
        //    this.con = con;
        //}

        ///// <summary>
        ///// [#] 임의의 커넥션을 기본으로 생성합니다. SetConnection 메서드를 통해 바꿀 수 있습니다.
        ///// </summary>
        //public DbAccess(string server, string catalog, string user, string password)
        //{
        //    SetConnection(server, catalog, user, password);
        //}

        /// <summary>
        /// [#] 임의의 커넥션을 기본으로 생성합니다. SetConnection 메서드를 통해 바꿀 수 있습니다.
        /// </summary>
        /// <param name="connectionString">기본으로 사용할 connectionString</param>
        public DbAccess(string connectionString)
        {
            SetConnection(connectionString);
        }

        /// <summary>
        /// [#] 클래스 내부적으로 사용하는 SqlConnection을 만듭니다.
        /// </summary>
        /// <param name="connectionString"></param>
        public void SetConnection(string connectionString)
        {
            con = new SqlConnection();
            con.ConnectionString = connectionString;
            this._connectionString = connectionString;
        }

        ///// <summary>
        ///// [#] 클래스 내부적으로 사용하는 SqlConnection을 만듭니다.
        ///// </summary>
        ///// <param name="server"></param>
        ///// <param name="catalog"></param>
        ///// <param name="user"></param>
        ///// <param name="password"></param>
        //public void SetConnection(string server, string catalog, string user, string password)
        //{
        //    con = new SqlConnection();
        //    con.ConnectionString = string.Format(@"Data Source={0};Persist Security Info=True;Initial Catalog={1};User ID={2};Password={3}",
        //        server, catalog, user, password);
        //}

        /// <summary>
        /// [#] SqlConnection을 비롯한 모든 DB연결을 닫습니다.
        /// </summary>
        public void DisConnect()
        {
            if (con != null && con.State == ConnectionState.Open)
                con.Close();

            if (con != null)
            {
                con.Dispose();
                con = null;
            }
        }

        /// <summary>
        /// [#] 프로시저를 실행합니다.
        /// </summary>
        public object ExecuteProcedure(string procedureName, DBParameter para, bool isDataSet)
        {
            DataSet output = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procedureName;
            cmd.Parameters.AddRange(para.GetSqlParameter(_connectionString, procedureName));
            if (con.State == ConnectionState.Closed)
                cmd.Connection.Open();
            cmd.CommandTimeout = 10000;
            cmd.Transaction = tran;

            SqlDataReader rd = null;
            try
            {
                rd = cmd.ExecuteReader();
                while (true)
                {
                    DataTable dt = GetDataTableFromDataReader(rd);
                    if (dt != null && dt.Columns.Count > 0)
                        output.Tables.Add(dt);
                    if (false == isDataSet || false == rd.NextResult())
                        break;
                }
                rd.Close();
            }
            catch (SqlException ex)
            {
                if (rd != null && false == rd.IsClosed)
                    rd.Close();
                throw ex;
                //return ex.Message;
            }
            catch (Exception ex)
            {
                if (rd != null && false == rd.IsClosed)
                    rd.Close();
                throw ex;
                //return ex.Message;
            }
            finally
            {
                //cmd.Connection.Close();
                para.SetOutputData(cmd.Parameters);

                if (tran == null)
                    cmd.Connection.Close();
            }

            for (int k = 0; k < output.Tables.Count; k++)
                output.Tables[k].AcceptChanges();

            if (isDataSet)
                return output;
            else
                return output.Tables.Count > 0 ? output.Tables[0] : null;
        }

        /// <summary>
        /// [#] 함수를 실행합니다.
        /// </summary>
        public object ExecuteFunction(string functionName, params object[] para)
        {
            return ExecuteFunction(functionName, new DBParameter(para));
        }

        /// <summary>
        /// [#] 함수를 실행합니다.
        /// </summary>
        public object ExecuteFunction(string functionName, DBParameter para)
        {
            string owner = "dbo";
            if (functionName.Contains("."))
            {
                owner = functionName.Substring(0, functionName.IndexOf("."));
                functionName = functionName.Remove(0, owner.Length + 1);
            }

            return ExecuteFunction(owner, functionName, para);
        }

        /// <summary>
        /// [#] 함수를 실행합니다.
        /// </summary>
        private object ExecuteFunction(string owner, string functionName, DBParameter para)
        {
            DataTable output = new DataTable();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.Parameters.AddRange(para.GetSqlParameter(_connectionString, functionName));
            string paraString = DBParameter.GetParameterStringFromSqlParameter(cmd.Parameters);
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("SELECT {0}.{3}.{1}({2})", con.Database, functionName, paraString, owner);
            if (con.State == ConnectionState.Closed)
                cmd.Connection.Open();
            SqlDataReader rd = cmd.ExecuteReader();
            output = GetDataTableFromDataReader(rd);
            rd.Close();
            //cmd.Connection.Close();

            output.AcceptChanges();

            if (tran == null)
                cmd.Connection.Close();
            return output;
        }

        /// <summary>
        /// [#] 임의의 SQL 명령을 실행합니다.
        /// </summary>
        public DataTable ExecuteCommand(string query)
        {
            DataTable output = new DataTable();

            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            if (con.State == ConnectionState.Closed)
                cmd.Connection.Open();
            try
            {
                SqlDataReader rd = cmd.ExecuteReader();
                output = GetDataTableFromDataReader(rd);
                rd.Close();
            }
            catch (SqlException ex)
            {
                throw ex;
                //return ex.Message;
            }
            catch (Exception ex)
            {
                throw ex;
                //return ex.Message;
            }
            finally
            {
                if (tran == null)
                    cmd.Connection.Close();
            }

            output.AcceptChanges();
            return output;
        }

        /// <summary>
        /// [#] 임의의 SQL 명령을 실행합니다.
        /// </summary>
        public int ExecuteNonCommand(string query)
        {
            SqlCommand cmd = new SqlCommand();
            int result = -1;
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            if (con.State == ConnectionState.Closed)
                cmd.Connection.Open();
            try
            {
                result = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (tran == null)
                    cmd.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// [#] 프로시저의 소스코드를 가져옵니다.
        /// </summary>
        /// <param name="sourceCodeName"></param>
        /// <returns></returns>
        public string[] GetStoredProcedureSourceCode(string sourceCodeName)
        {
            List<string> output = new List<string>();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = "sp_helptext " + sourceCodeName;
                cmd.CommandType = System.Data.CommandType.Text;
                if (con.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.FieldCount > 0 && rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            output.Add(rd[0].ToString());
                        }
                    }
                    rd.Close();
                }
                //cmd.Connection.Close();
            }
            return output.ToArray();
        }

        /// <summary>
        /// [#] 객체의 정보(sp_help)를 가져옵니다.
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public DataTable[] GetObjectInfomation(string objectName)
        {
            List<DataTable> output = new List<DataTable>();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "sp_help " + objectName;
            cmd.CommandType = CommandType.Text;
            if (con.State == ConnectionState.Closed)
                cmd.Connection.Open();
            SqlDataReader rd = cmd.ExecuteReader();
            do
            {
                output.Add(GetDataTableFromDataReader(rd));
            } while (rd.NextResult());
            rd.Close();
            //cmd.Connection.Close();

            return output.ToArray();
        }

        private DataTable GetDataTableFromDataReader(SqlDataReader rd)
        {
            DataTable output = new DataTable();
            output.BeginInit();
            output.BeginLoadData();
            if (rd.FieldCount > 0)
            {
                output = GenerateDataTableFromSchemeTable(rd.GetSchemaTable());

                for (int i = 0; rd.Read(); i++)
                {
                    DataRow row = output.NewRow();
                    for (int col = 0; col < rd.FieldCount; col++)
                    {
                        row[col] = rd[col];
                    }
                    output.Rows.Add(row);
                }
            }
            output.EndLoadData();
            output.EndInit();
            return output;
        }

        private DataTable GenerateDataTableFromSchemeTable(DataTable scheme)
        {
            DataTable output = new DataTable();
            output.BeginInit();
            output.BeginLoadData();

            for (int row = 0; row < scheme.Rows.Count; row++)
            {
                DataColumn col = new DataColumn();

                // ColumnName은 유니크해야 하므로 중복되면 접미어를 붙인다.
                string colName = scheme.Rows[row]["ColumnName"].ToString();
                if (output.Columns.Contains(colName))
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        if (false == output.Columns.Contains(string.Format("{0}_{1}", colName, i)))
                        {
                            colName = string.Format("{0}_{1}", colName, i);
                            break;
                        }
                    }
                }
                col.ColumnName = colName;
                col.Unique = scheme.Rows[row]["IsUnique"].ToString().Equals("True");
                col.AllowDBNull = scheme.Rows[row]["AllowDBNull"].ToString().Equals("True");
                col.AutoIncrement = scheme.Rows[row]["IsAutoIncrement"].ToString().Equals("True");
                //col.ReadOnly = scheme.Rows[row]["IsReadOnly"].ToString().Equals("True");
                col.ReadOnly = false;
                col.DataType = Type.GetType(scheme.Rows[row]["DataType"].ToString());


                output.Columns.Add(col);
                col.SetOrdinal((int)scheme.Rows[row]["ColumnOrdinal"]); // ordinal은 Table에 Add한 뒤에 설정해야 함
            }

            output.EndLoadData();
            output.EndInit();
            return output;
        }

        /// <summary>
        /// [#] DataTable 을 SqlDataAdapter 를 사용하여 변경사항을 적용시켜줍니다.
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateAll(string tableName, DataTable dt)
        {
            string strQuery = string.Format("SELECT * FROM {0}", tableName);
            SqlDataAdapter adapt = new SqlDataAdapter(strQuery, con);
            SqlCommandBuilder cb = new SqlCommandBuilder(adapt);

            adapt.Update(dt);
        }

        /// <summary>
        /// [#] 현재 구성된 SQL Connection 을 가져옵니다.
        /// </summary>
        public SqlConnection Connection
        {
            get { return con; }
        }

        public void BeginTransaction()
        {
            con.Open();
            if (tran == null)
                tran = con.BeginTransaction(IsolationLevel.ReadCommitted);
            tran.Save(Guid.NewGuid().ToString());
        }

        public void CommitTransaction()
        {
            tran.Commit();
            con.Close();
        }

        public void RollbackTransaction()
        {
            tran.Rollback();
            con.Close();
        }

    }
}
