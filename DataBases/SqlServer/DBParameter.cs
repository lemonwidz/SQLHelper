/*-------------------------------------------------------------------------------------------------
 * 제  목 : SqlParameter를 처리하는 클래스
 * 
 * 작성자 : 박재율
 * 작성일 : 2008-01-02
 * 
 * 특이점 : 
 ------------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SQLHelper.DataBases.SqlServer
{
    /// <summary>
    /// [#] SQL 파라메터를 다루는 클래스
    /// </summary>
    internal class DBParameter
    {
        private List<object> para = new List<object>();
        internal Dictionary<int, string> outputPara = new Dictionary<int, string>();
        private Dictionary<string, object> outputData = new Dictionary<string, object>();

        /// <summary>
        /// [#] 생성자
        /// </summary>
        public DBParameter()
        {
        }

        /// <summary>
        /// [#] 지정된 파라메터를 기본으로하여 생성합니다.
        /// </summary>
        /// <param name="parameters">생성할 파라메터 컬렉션</param>
        public DBParameter(params object[] parameters)
        {
            this.AddRange(parameters);
        }

        /// <summary>
        /// [#] 출력형식(OUTPUT)의 파라메터를 설정합니다.
        /// </summary>
        /// <param name="parameterName"></param>
        public void SetOutputParameter(params string[] parameterName)
        {
            for (int i = 0; i < parameterName.Length; i++)
            {
                if (parameterName[i].StartsWith("@"))
                    outputPara.Add(outputPara.Count, "@" + parameterName[i].Remove(0, 1));
                else
                    outputPara.Add(outputPara.Count, parameterName[i]);
            }
        }



        internal void SetOutputData(SqlParameterCollection colPara)
        {
            foreach (SqlParameter para in colPara)
            {
                if (para.Direction == ParameterDirection.InputOutput || para.Direction == ParameterDirection.Output)
                {
                    if (para.ParameterName.StartsWith("@"))
                        this.outputData.Add(para.ParameterName.Remove(0, 1), para.Value);
                    else
                        this.outputData.Add(para.ParameterName, para.Value);
                }
            }
        }

        /// <summary>
        /// [#] 현제 파라메터의 컬렉션에 새로운 파라메터를 추가합니다.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="data"></param>
        public void Add(string columnName, object data)
        {
            SqlParameter para = new SqlParameter();

        }

        /// <summary>
        /// [#] 파라메터를 설정합니다.
        /// </summary>
        /// <param name="parameters"></param>
        public void AddRange(params object[] parameters)
        {
            para.Clear();
            if (parameters != null)
                para.AddRange(parameters);
        }

        /// <summary>
        /// [#] SQL 파라메터를 가져옵니다.
        /// </summary>  
        /// <returns></returns>
        public SqlParameter[] GetSqlParameter(string connectionString, string procedureName)
        {
            string owner = "dbo";
            if (procedureName.Contains("."))
            {
                owner = procedureName.Substring(0, procedureName.IndexOf("."));
                procedureName = procedureName.Remove(0, owner.Length + 1);
            }

            return GetSqlParameter(connectionString, owner, procedureName);
        }

        private SqlParameter[] GetSqlParameter(string connectionString, string owner, string procedureName)
        {
            string query = string.Format(@"
SELECT '{0}' [spname], C.NAME [Parameter_name],
       CASE WHEN C.xusertype = '35' THEN C.length ELSE C.prec END [Size],
       C.isoutparam [DIRECTION],
       C.xscale [Scale],
       C.xprec [Prec],
       T.[name] Type,
       C.colid Param_order
  FROM sysobjects O
       LEFT OUTER JOIN syscolumns C ON C.ID = O.ID
       LEFT OUTER JOIN systypes   T ON T.xusertype = C.xusertype
 WHERE O.type = 'P'
   AND O.name = '{0}'
   AND O.uid = (SELECT uid FROM sysusers WHERE [NAME]='{1}')
 ORDER BY C.colid ASC
", procedureName, owner);
            List<SqlParameter> output = new List<SqlParameter>();
            Dictionary<int, string> newOutputPara = new Dictionary<int, string>();

            DbAccess db = new DbAccess(connectionString);
            db.Connection.Open();
            DataTable dt = db.ExecuteCommand(query);

            if (dt == null || dt.Rows.Count == 0)
            {
                throw new Exception(string.Format("{0}\r\n\r\n요청한 프로시저가 존재하지 않습니다.", procedureName));
            }

            int indexPara = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["Parameter_name"].ToString().Equals(string.Empty))
                {
                    continue;
                }
                else if (indexPara >= para.Count) // indexPara 는 계속 증가하여 실제 프로시저의 파라메터 개수를 카운트해줌
                {
                    indexPara++;
                    continue;
                }

                SqlParameter sqlPara = new SqlParameter();
                sqlPara.ParameterName = dt.Rows[i]["Parameter_name"].ToString();
                sqlPara.Value = para[indexPara] != null ? para[indexPara] : DBNull.Value;

                //if (this.outputPara.ContainsValue(sqlPara.ParameterName.Remove(0, 1)))
                //{
                //    sqlPara.Direction = ParameterDirection.InputOutput;
                //    newOutputPara.Add((int)dt.Rows[i]["Param_order"], sqlPara.ParameterName.Remove(0, 1));
                //}

                int sizeTemp = int.TryParse(dt.Rows[i]["Size"].ToString(), out sizeTemp) ? sizeTemp : 0;
                byte sizePrec = byte.TryParse(dt.Rows[i]["Prec"].ToString(), out sizePrec) ? sizePrec : (byte)0;
                byte sizeScale = byte.TryParse(dt.Rows[i]["Scale"].ToString(), out sizeScale) ? sizeScale : (byte)0;

                switch (dt.Rows[i]["Type"].ToString().ToUpper())
                {
                    case "NVARCHAR":
                        sqlPara.SqlDbType = SqlDbType.NVarChar;
                        sqlPara.Size = sizeTemp;
                        break;
                    case "VARCHAR":
                        sqlPara.SqlDbType = SqlDbType.VarChar;
                        sqlPara.Size = sizeTemp;
                        break;
                    case "NUMERIC":
                    case "DECIMAL":
                        sqlPara.SqlDbType = SqlDbType.Decimal;
                        sqlPara.Size = sizeTemp;
                        sqlPara.Precision = sizePrec;
                        sqlPara.Scale = sizeScale;
                        break;
                    case "INT":
                        sqlPara.SqlDbType = SqlDbType.Int;
                        sqlPara.Size = sizeTemp;
                        break;
                    case "TEXT":
                        sqlPara.SqlDbType = SqlDbType.Text;
                        sqlPara.Size = 2147483647;
                        break;
                    case "IMAGE":
                        sqlPara.SqlDbType = SqlDbType.Image;
                        sqlPara.Size = 2147483647;
                        break;
                    case "DATETIME":
                        sqlPara.SqlDbType = SqlDbType.DateTime;
                        break;
                    default:
                        break;
                }

                switch (dt.Rows[i]["DIRECTION"].ToString())
                {
                    case "0":
                        sqlPara.Direction = ParameterDirection.Input;
                        break;
                    case "1":
                        {
                            sqlPara.Direction = ParameterDirection.InputOutput;
                            int order = int.TryParse(dt.Rows[i]["Param_order"].ToString(), out order) ? order : -1;
                            newOutputPara.Add(order, sqlPara.ParameterName.Remove(0, 1));
                            break;
                        }
                    default:
                        break;
                }

                // 후처리
                if (sqlPara.SqlDbType == SqlDbType.DateTime)
                {
                    if (sqlPara.Value is DateTime && (DateTime)sqlPara.Value == DateTime.MinValue ||
                        sqlPara.Value == null)
                        sqlPara.Value = DBNull.Value;
                }

                output.Add(sqlPara);
                indexPara++;
            }

            if (indexPara != para.Count)
            {
                throw new Exception(string.Format("{0}\r\n\r\n요청한 프로시저와 실제 프로시저의 파라메터 개수가 일치하지 않습니다.\r\n(요청:{1}개, 실제:{2}개)", procedureName, para.Count, indexPara));
            }

            this.outputPara = newOutputPara;
            return output.ToArray();
        }

        /// <summary>
        /// [#][S] SqlParameter로 부터 문자열 형식의 파라메터를 만들어 냅니다.
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        internal static string GetParameterStringFromSqlParameter(SqlParameterCollection para)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < para.Count; i++)
            {
                output.Append(para[i].ParameterName);
                if (i < para.Count - 1)
                    output.Append(", ");
            }

            return output.ToString();
        }

        public Dictionary<string, object> OutputData
        {
            get { return outputData; }
        }
    }
}
