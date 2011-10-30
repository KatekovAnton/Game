using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.SQliteShell;

namespace PhysX_test2
{
    public class DatabaseManager
    {
        private sealed class DataBaseManagerCreator
        {
            private static readonly DatabaseManager instance = new DatabaseManager();

            public static DatabaseManager _Manager
            {
                get { return instance; }
            }
        }

        public static DatabaseManager Manager
        {
            get { return DataBaseManagerCreator._Manager; }
        }

        private SQliteConnector connector;
        
        protected DatabaseManager()
        {
            connector = new SQliteConnector("Data\\Data.sqlite");    
        }
        
        ~DatabaseManager()
        {
            connector.close();
        }
    }
}
