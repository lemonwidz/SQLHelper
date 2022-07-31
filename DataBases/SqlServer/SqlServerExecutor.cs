using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SQLHelper.DataBases.Common;

namespace SQLHelper.DataBases.SqlServer
{
    internal class SqlServerExecutor : Interface.IDBProxy
    {
        private DbAccess db = null;
        private string _connectionString;

        public SqlServerExecutor(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DataTable Invoke(Common.InvokeOption option)
        {
            return InvokeInner(option.CommandText, option.Parameters, ref option.OutputParameters, false, option.UseTransaction) as DataTable;
        }

        private DbAccess GetDbAccess()
        {
            if (db == null)
                db = new DbAccess(this._connectionString);
            return db;
        }

        private object InvokeInner(string procedureName, object[] parameters, ref string[] outputParameters, bool isDataSet, bool useTransaction)
        {
            db = GetDbAccess();
            DBParameter para = new DBParameter(parameters);
            object dtResult = null;

            try
            {
                db.UseTransaction = useTransaction;
                dtResult = db.ExecuteProcedure(procedureName, para, isDataSet);
            }
            catch// (Exception ex)
            {
                throw;
            }
            finally // 예외가 발생되더라도 output 파라메터를 처리하기 위해서 필요
            {
                if (para.OutputData.Count > 0)
                {
                    outputParameters = new string[para.OutputData.Count];
                    int i = 0;
                    foreach (KeyValuePair<string, object> keys in para.OutputData)
                    {
                        outputParameters[i++] = (keys.Value ?? string.Empty).ToString();
                    }
                }

                if (dtResult != null)
                    SetTableInfo(dtResult, 0);

                try
                {
                    StringBuilder log1 = new StringBuilder();
                    StringBuilder log2 = new StringBuilder();
                    log1.Append(procedureName);
                    if (parameters != null && parameters.Length > 0)
                    {
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            log1.AppendFormat("{0}{1}", i == 0 ? " " : ",", GetParameterString(parameters[i]));
                        }
                    }
                    if (outputParameters != null && outputParameters.Length > 0)
                    {
                        for (int i = 0; i < outputParameters.Length; i++)
                        {
                            log2.AppendFormat("{0}{1}", i == 0 ? string.Empty : ",", outputParameters[i]);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            return dtResult;
        }

        public DataTable InvokeQuery(Common.InvokeOption option)
        {
            db = GetDbAccess();

            DataTable dtResult = db.ExecuteCommand(option.CommandText);
            dtResult.TableName = "QueryStringResult";
            return dtResult;
        }

        public DataSet InvokeDataSet(Common.InvokeOption option)
        {
            return InvokeInner(option.CommandText, option.Parameters, ref option.OutputParameters, true, option.UseTransaction) as DataSet;
        }

        public int InvokeNonQuery(InvokeOption option)
        {
            db = GetDbAccess();
            return db.ExecuteNonCommand(option.CommandText);
        }

        private string GetParameterString(object value)
        {
            if (value == null)
                return "NULL";
            else if (value is decimal)
                return value.ToString();
            else
                return string.Format("'{0}'", value);
        }

        private void SetTableInfo(object obj, int index)
        {
            if (obj is DataTable)
            {
                DataTable dt = obj as DataTable;
                if (index <= 0)
                    dt.TableName = "StoredProcedureResult";
                else
                    dt.TableName = string.Format("StoredProcedureResult_{0}", index);

                // 각 칼럼의 데이터를 Null 허용함으로 변경 // 이걸 안해주면 AddRow 같은 곳에서 문제가 생김
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dt.Columns[i].AllowDBNull = true;
                }
            }
            else if (obj is DataSet)
            {
                DataSet ds = obj as DataSet;
                for (int i = 0; i < ds.Tables.Count; i++)
                    SetTableInfo(ds.Tables[i], i + 1);
            }
        }

        public void BeginTransaction()
        {
            GetDbAccess().BeginTransaction();
        }

        public void CommitTransaction()
        {
            GetDbAccess().CommitTransaction();
        }

        public void RollbackTransaction()
        {
            GetDbAccess().RollbackTransaction();
        }
    }
}
