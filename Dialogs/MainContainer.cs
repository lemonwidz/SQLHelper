using SQLHelper.DataBases.Handler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SQLHelper.Dialogs
{
    public partial class MainContainer : Form
    {
        public string DbIP { get; set; }
        public string DbID { get; set; }
        public string DbPassword { get; set; }

        public MainContainer()
        {
            InitializeComponent();

            tabMain.TabPages.Clear();

            this.AllowDrop = true;
            this.cmbDBList.SelectedIndexChanged += CmbDBList_SelectedIndexChanged;
        }

        private void CmbDBList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            btnCloseTab.Enabled = false;

            var dbList = GetDBHandler("master").GetDBList();
            cmbDBList.Items.AddRange(dbList);
            cmbDBList.SelectedIndex = 0;
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            var fileNames = drgevent.Data.GetData(DataFormats.FileDrop) as string[];
            if (fileNames != null && fileNames.Length > 0)
            {
                drgevent.Effect = DragDropEffects.All;
            }
            base.OnDragEnter(drgevent);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            var fileNames = drgevent.Data.GetData(DataFormats.FileDrop) as string[];
            if (fileNames != null && fileNames.Length > 0)
            {
                ChangeDB(fileNames[0]);
            }
            base.OnDragDrop(drgevent);
        }

        private void btnNewTab_Click(object sender, EventArgs e)
        {
            AddNewTableTab();
        }

        private void btnCloseTab_Click(object sender, EventArgs e)
        {
            RemoveCurrentTab();
        }

        private void AddNewTableTab()
        {
            TabPage page = new TabPage();
            page.Text = string.Format("[{0}] Table", GetSelectedDBName());
            var con = new UserControls.TableBrowser();
            con.ParentTabPage = page;
            con.DbHandler = GetDBHandler();
            page.Controls.Add(con);
            con.Dock = DockStyle.Fill;
            tabMain.TabPages.Add(page);
            btnCloseTab.Enabled = true;
            tabMain.SelectedTab = page;
        }

        private void AddNewQueryTab()
        {
            TabPage page = new TabPage();
            page.Text = string.Format("[{0}] Query", GetSelectedDBName());
            var con = new UserControls.QueryEditor();
            con.DbHandler = GetDBHandler();
            page.Controls.Add(con);
            con.Dock = DockStyle.Fill;
            tabMain.TabPages.Add(page);
            btnCloseTab.Enabled = true;
            tabMain.SelectedTab = page;
        }

        private void RemoveCurrentTab()
        {
            if (tabMain.SelectedTab != null)
            {
                var toDelete = tabMain.SelectedTab;
                if (tabMain.TabPages.IndexOf(tabMain.SelectedTab) > 0)
                    tabMain.SelectedTab = tabMain.TabPages[tabMain.TabPages.IndexOf(tabMain.SelectedTab) - 1];
                tabMain.TabPages.Remove(toDelete);
            }
            btnCloseTab.Enabled = tabMain.TabPages.Count > 0;
        }

        private void btnChangeDB_Click(object sender, EventArgs e)
        {
            // TODO : SQLite 구현
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.InitialDirectory = new FileInfo(txtDBPath.Text).Directory.FullName;
            //if (DialogResult.OK != dlg.ShowDialog())
            //    return;
            //ChangeDB(dlg.FileName);
        }

        private void ChangeDB(string fileName)
        {
            // TODO : SQLite 구현
            //txtDBPath.Text = fileName;
            //DBHandlerSQLServer.dbFileName = fileName;

            //tabMain.TabPages.Clear();
        }

        private void btnQueryEditor_Click(object sender, EventArgs e)
        {
            AddNewQueryTab();
        }

        private string GetSelectedDBName()
        {
            return cmbDBList.SelectedItem.ToString();
        }

        private DataBases.Interface.IDBHandler GetDBHandler()
        {
            return GetDBHandler(GetSelectedDBName());
        }

        private DataBases.Interface.IDBHandler GetDBHandler(string dbName)
        {
            return DBHandlerFactory.GetDBHandler(new DBHandlerFactory.DBHandlerFactoryOption
            {
                DbType = DataBases.Common.DB_TYPE.MSSQL,
                DbName = dbName,
                DbIP = this.DbIP,
                DbUser = this.DbID,
                DbPassword = this.DbPassword
            });
        }
    }
}
