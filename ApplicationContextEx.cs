using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLHelper
{
    public class ApplicationContextEx : ApplicationContext
    {
        public static ApplicationContextEx ApplicationContextInstance { get; private set; }

        public ApplicationContextEx()
        {
            ApplicationContextInstance = this;
        }

        /// <summary>
        /// 현재 폼을 닫고 다음 창을 열어서 활성화된 메인폼을 변경
        /// </summary>
        /// <param name="form"></param>
        public void CloseAndOpen(Form form)
        {
            var prevForm = this.MainForm;
            this.MainForm = form;
            form.Show();

            if (prevForm != null)
                prevForm.Close();
        }
    }
}
