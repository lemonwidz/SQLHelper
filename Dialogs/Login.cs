using SQLHelper.DataBases.Handler;
using System;
using System.Windows.Forms;

namespace SQLHelper.Dialogs
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();

            txtPassword.KeyDown += TxtPassword_KeyDown;
            this.ActiveControl = txtIP;

            txtIP.Text = "xcom.xcosmos.co.kr";
            txtID.Text = "sa";
            txtPassword.Text = "Xcosmos!234";
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.Equals(Keys.Enter))
            {
                btnLogin.PerformClick();
            }
            else if (e.KeyData.Equals(Keys.Escape))
            {
                btnClose.PerformClick();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var dbHandler = DBHandlerFactory.GetDBHandler(new DBHandlerFactory.DBHandlerFactoryOption
            {
                DbType = DataBases.Common.DB_TYPE.MSSQL,
                DbName = "master",
                DbIP = txtIP.Text,
                DbUser = txtID.Text,
                DbPassword = txtPassword.Text
            });
            try
            {
                var dt = dbHandler.ExecuteDataTable("SELECT @@VERSION VER");
                if (dt != null && dt.Rows.Count > 0)
                {
                    ApplicationContextEx.ApplicationContextInstance.CloseAndOpen(new MainContainer
                    {
                        DbIP = txtIP.Text,
                        DbID = txtID.Text,
                        DbPassword = txtPassword.Text
                    });
                    return;
                }
            }
            catch// (Exception ex)
            {
            }
            Utils.MessageBoxShow("DB에 연결할 수 없습니다.");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
