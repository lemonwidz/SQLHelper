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
    /// <summary>
    /// 쿼리 에디터 사용자 정의 컨트롤
    /// </summary>
    public partial class QueryEditor : UserControl
    {
        private readonly char[] queryTrimCharacter = new char[] { '\r', '\n', '\t', '\0', ' ' };
        public DataBases.Interface.IDBHandler DbHandler { get; set; }

        public QueryEditor()
        {
            InitializeComponent();

            txtQuery.Font = new Font("굴림체", 10);
            txtQuery.KeyDown += TxtQuery_KeyDown;
        }

        private void TxtQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.Equals(Keys.F5))
            {
                btnRun.PerformClick();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtQuery.Focus();
            this.ActiveControl = txtQuery;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string text = txtQuery.Text.Trim(queryTrimCharacter);
            if (txtQuery.SelectedText != null && txtQuery.SelectedText.Length > 0) // 텍스트 블록을 선택해서 실행하는 경우
            {
                text = txtQuery.SelectedText.Trim(queryTrimCharacter);
            }

            if (text.Length == 0)
                return;

            tabMain.TabPages.Clear();
            string[] queries = text.Split(';'); // 세미콜론이 포함되는 경우 나눠서 실행
            for (int i = 0; i < queries.Length; i++)
            {
                string query = queries[i].Trim(queryTrimCharacter);
                if (query.Length == 0)
                    continue;
                if (query.ToUpper().StartsWith("SELECT "))
                    RunSelectQuery(query);
                else if (
                    query.ToUpper().StartsWith("INSERT ") ||
                    query.ToUpper().StartsWith("UPDATE ") ||
                    query.ToUpper().StartsWith("DELETE "))
                    RunExecuteNonQuery(query);
                else
                    RunExecuteNonQuery(query);
            }
        }

        private void RunSelectQuery(string query)
        {
            try
            {
                var dt = this.DbHandler.ExecuteDataTable(query);
                var grid = CreateDataGridView();
                grid.DataSource = dt;
                TabPage page = new TabPage();
                page.Controls.Add(grid);
                page.Text = string.Format("Result {0}", tabMain.TabPages.Count + 1);
                tabMain.TabPages.Add(page);
                grid.AutoResizeColumns();
            }
            catch (Exception ex)
            {
                ExceptionResult(query, ex);
            }
        }

        private void RunExecuteNonQuery(string query)
        {
            try
            {
                var ret = this.DbHandler.ExecuteNonQuery(query);
                var txt = CreateTextBox();
                txt.AppendText(string.Format("Query : {0}\r\n\r\n{1} rows affected.", query, ret));
                TabPage page = new TabPage();
                page.Controls.Add(txt);
                page.Text = string.Format("Result {0}", tabMain.TabPages.Count + 1);
                tabMain.TabPages.Add(page);
            }
            catch (Exception ex)
            {
                ExceptionResult(query, ex);
            }
        }

        private void ExceptionResult(string query, Exception ex)
        {
            var txt = CreateTextBox();
            txt.AppendText(string.Format("Query : {0}\r\n\r\nException :\r\n{1}", query, ex.Message));
            TabPage page = new TabPage();
            page.Controls.Add(txt);
            page.Text = string.Format("Error {0}", tabMain.TabPages.Count + 1);
            tabMain.TabPages.Add(page);
        }

        private DataGridView CreateDataGridView()
        {
            var grid = new DataGridView();
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.RowHeadersVisible = false;
            grid.Dock = DockStyle.Fill;
            return grid;
        }

        private RichTextBox CreateTextBox()
        {
            var txt = new RichTextBox();
            txt.Dock = DockStyle.Fill;
            txt.ReadOnly = true;
            txt.BackColor = Color.White;
            txt.BorderStyle = BorderStyle.None;
            return txt;
        }
    }
}
