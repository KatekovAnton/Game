using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Community.CsharpSqlite;

namespace PhysX_test2.SQliteShell
{
    public class SQliteResultSet
    {
        public string[] ColumnNames
        {
            get;
            private set;
        }
        public List<object[]> result
        {
            get;
            private set;
        }
        public SQliteResultSet(ref Sqlite3.dxCallback callback)
        {
            result = new List<object[]>();
            callback = sqcallback;
        }
        private int sqcallback(object pCallbackArg, long argc, object p2, object p3)
        {
            result.Add(p2 as object[]);
            if(ColumnNames==null)
                ColumnNames = p3 as string[];
            return 0;
        }
    }
}
