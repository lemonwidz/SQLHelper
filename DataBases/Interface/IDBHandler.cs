using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SQLHelper.DataBases.Interface
{
    // TODO : 여러종류의 DB를 지원하기 위한 인터페이스. SQLite 개발시 구현
    public interface IDBHandler
    {
        string DbIP { get; set; }
        string DbName { get; set; }
        string DbUser { get; set; }
        string DbPassword { get; set; }
        string[] GetTableList();
        DataTable ExecuteDataTable(string query);
        int ExecuteNonQuery(string query);
        string[] GetTablePK(string tableName);
        string[] GetDBList();
    }
}
