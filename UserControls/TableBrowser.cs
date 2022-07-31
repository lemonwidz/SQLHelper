using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLHelper.UserControls
{
    public partial class TableBrowser : UserControl
    {
        public TabPage ParentTabPage { get; set; }
        public DataBases.Interface.IDBHandler DbHandler { get; set; }

        private bool isEditMode = false;

        public TableBrowser()
        {
            InitializeComponent();

            lstTable.Font = new Font("consolas", 10);

            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.RowHeadersVisible = false;
            grid.Font = new Font("consolas", 8);

            SetEditMode();

            lstTable.SelectedValueChanged += LstTable_SelectedValueChanged;
            grid.DataError += Grid_DataError;
            grid.CellBeginEdit += Grid_CellBeginEdit;
        }

        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is System.FormatException)
            {
                Utils.MessageBoxShow("테이블의 데이터 형식과 다른 형식으로 수정할 수 없습니다.");
                e.Cancel = true;
                return;
            }
            else
            {
                // Utils.MessageBoxShow(e.Exception.Message);
            }
        }

        private void LstTable_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lstTable.SelectedItem != null)
                SelectDataTable(lstTable.SelectedItem.ToString());
            else
                ParentTabPage.Text = "SQL";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var tables = this.DbHandler.GetTableList();
            lstTable.DataSource = tables;
        }

        private void SelectDataTable(string tableName)
        {
            ParentTabPage.Text = string.Format("[{0}] {1}", this.DbHandler.DbName, tableName);
            int limitCount = int.TryParse(txtRowLimit.Text, out limitCount) ? limitCount : 0;
            string query = string.Format("SELECT {1} * FROM {0};", tableName, limitCount == 0 ? string.Empty : string.Format(" TOP {0}", limitCount));
            var dt = this.DbHandler.ExecuteDataTable(query);
            grid.DataSource = dt;
            if (dt.Rows.Count < 5000)
                grid.AutoResizeColumns();

            var pks = this.DbHandler.GetTablePK(tableName);
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                string columnName = grid.Columns[i].Name;
                if (Array.IndexOf<string>(pks, columnName) >= 0)
                {
                    grid.Columns[i].HeaderText = string.Format("* {0}", columnName);
                    grid.Columns[i].Tag = "PK";
                }
                else
                {
                    grid.Columns[i].HeaderText = string.Format("{0}", columnName);
                    grid.Columns[i].Tag = string.Empty;
                }
            }

            lblStatus.Text = string.Format("{0} Rows Selected.", dt.Rows.Count);
        }

        private void btnApplyRowLimit_Click(object sender, EventArgs e)
        {
            if (lstTable.SelectedItem == null)
                return;
            SelectDataTable(lstTable.SelectedItem.ToString());
        }

        private void SetEditMode()
        {
            btnEditMode.Text = string.Format("데이터 수정 : {0}", isEditMode ? "ON" : "OFF");
            btnCommit.Enabled = btnRollback.Enabled = btnAddRow.Enabled = btnDeleteRow.Enabled = isEditMode;
            if (isEditMode)
                grid.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            else
                grid.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void btnEditMode_Click(object sender, EventArgs e)
        {
            isEditMode = false == isEditMode;
            SetEditMode();
        }

        private void Grid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (grid.Columns[e.ColumnIndex].Tag != null && grid.Columns[e.ColumnIndex].Tag.ToString() == "PK")
            {
                Utils.MessageBoxShow("PK칼럼은 수정할 수 없습니다.");
                e.Cancel = true;
                return;
            }
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            var dt = grid.DataSource as DataTable;
            var dtChanged = dt.GetChanges();

            if (dtChanged == null || dtChanged.Rows.Count == 0)
            {
                Utils.MessageBoxShow("변경 내역이 없습니다.");
                return;
            }

            var pkList = this.DbHandler.GetTablePK(lstTable.SelectedItem.ToString());

            if (pkList == null || pkList.Length == 0)
            {
                Utils.MessageBoxShow("테이블에 PK가 없어서 수정할 수 없습니다.");
                return;
            }

            for (int rowIndex = 0; rowIndex < dtChanged.Rows.Count; rowIndex++)
            {
                var rowState = dtChanged.Rows[rowIndex].RowState;
                if (rowState == DataRowState.Modified)
                {
                    ExecuteUpdateQuery(dtChanged, pkList, rowIndex);
                }
                else if (rowState == DataRowState.Deleted)
                {
                    ExecuteDeleteQuery(dtChanged, pkList, rowIndex);
                }
                else if (rowState == DataRowState.Added)
                {
                    ExecuteInsertQuery(dtChanged, rowIndex);
                }
            }
            Utils.MessageBoxShow("저장되었습니다.");
        }

        private void ExecuteUpdateQuery(DataTable dtChanged, string[] pkList, int row)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("UPDATE {0} SET ", lstTable.SelectedItem);
            bool isFirst = true;
            for (int colIndex = 0; colIndex < dtChanged.Columns.Count; colIndex++)
            {
                var columnName = dtChanged.Columns[colIndex].ColumnName;
                if (dtChanged.Rows[row][columnName, DataRowVersion.Original].ToString() != dtChanged.Rows[row][columnName, DataRowVersion.Current].ToString())
                {
                    var changedValue = dtChanged.Rows[row][columnName];
                    if (dtChanged.Columns[colIndex].DataType == typeof(string))
                        query.AppendFormat("{0}{1} = N'{2}'", isFirst ? string.Empty : ", ", columnName, changedValue?.ToString().Replace("'", "''"));
                    else
                        query.AppendFormat("{0}{1} = {2}", isFirst ? string.Empty : ", ", columnName, changedValue);
                    isFirst = false;
                }
            }

            query.AppendFormat(" WHERE 1=1");
            for (int i = 0; i < pkList.Length; i++)
            {
                var dataType = dtChanged.Columns[pkList[i]].DataType;
                if (dataType == typeof(int) || dataType == typeof(decimal) ||
                    dataType == typeof(double) || dataType == typeof(float))
                {
                    query.AppendFormat(" AND {0} = {1}", pkList[i], dtChanged.Rows[row][pkList[i]]);
                }
                else
                {
                    query.AppendFormat(" AND {0} = '{1}'", pkList[i], dtChanged.Rows[row][pkList[i]]);
                }
            }
            this.DbHandler.ExecuteNonQuery(query.ToString());
        }

        private void ExecuteInsertQuery(DataTable dtChanged, int row)
        {
            StringBuilder queryColumns = new StringBuilder();
            StringBuilder queryValues = new StringBuilder();
            queryColumns.AppendFormat("INSERT INTO {0} (", lstTable.SelectedItem);
            bool isFirst = true;
            for (int col = 0; col < dtChanged.Columns.Count; col++)
            {
                var columnName = dtChanged.Columns[col].ColumnName;
                var value = dtChanged.Rows[row][columnName];
                if (columnName == "rowid")
                    continue;

                queryColumns.AppendFormat("{0}{1}", isFirst ? string.Empty : ", ", columnName);
                if (dtChanged.Columns[col].DataType == typeof(string))
                    queryValues.AppendFormat("{0}'{1}'", isFirst ? string.Empty : ", ", value.ToString().Replace("'", "''"));
                else
                    queryValues.AppendFormat("{0}{1}", isFirst ? string.Empty : ", ", value);
                isFirst = false;
            }
            queryColumns.AppendFormat(") VALUES ({0})", queryValues);
            this.DbHandler.ExecuteNonQuery(queryColumns.ToString());
        }

        private void ExecuteDeleteQuery(DataTable dtChanged, string[] pkList, int row)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat("DELETE FROM {0}", lstTable.SelectedItem);
            query.AppendFormat(" WHERE 1=1");
            for (int i = 0; i < pkList.Length; i++)
            {
                var dataType = dtChanged.Columns[pkList[i]].DataType;
                var originValue = dtChanged.Rows[row][pkList[i], DataRowVersion.Original];
                if (dataType == typeof(int) || dataType == typeof(decimal) ||
                    dataType == typeof(double) || dataType == typeof(float))
                {
                    query.AppendFormat(" AND {0} = {1}", pkList[i], originValue);
                }
                else
                {
                    query.AppendFormat(" AND {0} = '{1}'", pkList[i], originValue);
                }
            }
            this.DbHandler.ExecuteNonQuery(query.ToString());
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            SelectDataTable(lstTable.SelectedItem.ToString());
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            DataTable dt = grid.DataSource as DataTable;
            dt.Rows.Add(dt.NewRow());
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            DataTable dt = grid.DataSource as DataTable;
            List<DataRow> rows = new List<DataRow>();
            for (int i = 0; i < grid.SelectedCells.Count; i++)
            {
                var row = dt.DefaultView[grid.SelectedCells[i].RowIndex].Row;
                if (rows.Contains(row))
                    continue;
                rows.Add(row);
            }
            for (int i = 0; i < rows.Count; i++)
            {
                rows[i].Delete();
            }
        }
    }
}
