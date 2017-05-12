using System;
using System.Collections.Generic;
using System.Data.Common;

namespace GenericADO.DAL
{
    public interface IDataBase
    {
        List<Object> OExecProc(DbCommand commands);
        List<Dictionary<string, object>> IOExecProc(DbCommand commands);
        void IExecNonQueryProc(DbCommand commands);


    }
}
