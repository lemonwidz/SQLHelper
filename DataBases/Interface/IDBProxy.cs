using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SQLHelper.DataBases.Interface
{
    /// <summary>
    /// DB 핸들링 인터페이스
    /// </summary>
    internal interface IDBProxy
    {
        /// <summary>
        /// 프로시저 실행
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        DataTable Invoke(Common.InvokeOption option);
        /// <summary>
        /// 쿼리 실행
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        DataTable InvokeQuery(Common.InvokeOption option);
        /// <summary>
        /// 데이터 셋을 리턴하는 프로시저 실행
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        DataSet InvokeDataSet(Common.InvokeOption option);
        /// <summary>
        /// INSERT, UPDATE, DELETE 의 적용 행 수(count)를 리턴하는 쿼리
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        int InvokeNonQuery(Common.InvokeOption option);
        /// <summary>
        /// 트랜잭션 시작
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// 트랜젝션 종료
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 트랜젝션 롤백
        /// </summary>
        void RollbackTransaction();
    }
}
