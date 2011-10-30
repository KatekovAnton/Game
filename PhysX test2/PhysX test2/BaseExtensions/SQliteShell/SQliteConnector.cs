using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Community.CsharpSqlite;

namespace PhysX_test2.SQliteShell
{
    public delegate void ActionForError(string errorName);
    class SQliteConnector
    {
        Sqlite3.sqlite3 dbconnector;
        public SQliteConnector(string dbname)
        {
            Sqlite3.sqlite3_open(dbname, ref dbconnector);
        }
        /// <summary>
        /// executes simple query like update
        /// </summary>
        /// <param name="query"></param>
        /// <param name="errorAct"></param>
        public void executeNonQuery(string query, ActionForError errorAct)
        {
            string errorName = "";
            Sqlite3.sqlite3_exec(dbconnector, query, null, null, ref errorName);
            if (errorName != "" && errorAct != null)
                errorAct(errorName);
        }
        /// <summary>
        /// executes select query, returns selection result
        /// </summary>
        /// <param name="query"> select statement</param>
        /// <param name="errorAct">executes if error in statement are detected</param>
        /// <returns>SQliteResultSet with result of selection</returns>
        public SQliteResultSet executeSelect(string query, ActionForError errorAct)
        {
            Sqlite3.dxCallback callback = null;
            SQliteResultSet result = new SQliteResultSet(ref callback);

            string s = "";
            Sqlite3.sqlite3_exec(dbconnector, query, callback, null, ref s);
            if (s != "" && errorAct != null)
                errorAct(s);

            return result;
        }
        /// <summary>
        /// executes insert query, returns id of last inserted row
        /// </summary>
        /// <param name="query">insert statement</param>
        /// <param name="errorAct">executes if error in statement are detected</param>
        /// <returns>id of last inserted row</returns>
        public object executeInsert(string query, ActionForError errorAct)
        {
            string s = "";
            Sqlite3.sqlite3_exec(dbconnector, query, null, null, ref s);
            if (s != "")
                errorAct(s);
            else
            {
                SQliteResultSet sqlrs = executeSelect("select last_insert_rowid()", null);
                return sqlrs.result[0][0];
            }
            return null;
        }
        public void close()
        {
            Sqlite3.sqlite3_close(dbconnector);
        }
    }
}
