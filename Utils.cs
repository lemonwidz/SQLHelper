using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SQLHelper
{
    public static class Utils
    {
        public static readonly string DIALOG_TITLE = "SQL Helper";

        /// <summary>
        /// AppendFormat + NewLine
        /// </summary>
        public static StringBuilder AppendFormatLine(this StringBuilder stringBuilder, string format, params object[] args)
        {
            stringBuilder.AppendFormat(format, args);
            stringBuilder.Append(Environment.NewLine);
            return stringBuilder;
        }

        /// <summary>
        /// 메시지 박스. 매크로 매서드
        /// </summary>
        public static void MessageBoxShow(string message)
        {
            MessageBox.Show(message, Utils.DIALOG_TITLE);
        }

        /// <summary>
        /// 인자로 받은 string을 SHA512 해시로 리턴
        /// </summary>
        public static string EncryptSHA512(string Data)
        {
            var sha = new SHA512Managed();
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(Data));
            var stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
    }
}