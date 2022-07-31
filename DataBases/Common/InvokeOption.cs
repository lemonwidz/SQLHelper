using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLHelper.DataBases.Common
{
    [Serializable]
    public class InvokeOption
    {
        /// <summary>
        /// 실행 명령(프로시저명 또는 쿼리)
        /// </summary>
        public string CommandText;
        /// <summary>
        /// 프로시저인 경우 파라메터
        /// </summary>
        public object[] Parameters;
        /// <summary>
        /// 아웃풋으로 받는 파라메터
        /// </summary>
        public string[] OutputParameters;
        /// <summary>
        /// 트랜잭션 사용여부
        /// </summary>
        public bool UseTransaction;

        public InvokeOption()
        {
            this.UseTransaction = true;
        }
    }
}
