using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.Objects;
using PhysX_test2.TheGame.Objects.StateGraphs;
using PhysX_test2.SQliteShell;

namespace PhysX_test2.TheGame
{
    public class StaticObjects
    {
        //single object
        private static StaticObjects instance;
        public static StaticObjects Instance()
        {
            if (instance == null)
                instance = new StaticObjects();
            return instance;
        }


        public const string _characterNodeIdle = "idle\0";
        public const string _characterNodeAction = "action\0";
        public const string _characterNodeDead = "dead\0";
        public const string _characterEdgeDeadToIdle = "setalive\0";
        public const string _characterEdgeIdleToDead = "setdead\0";

        public const string _characterEdgeActionToDead = "setdead\0";
        public const string _characterEdgeActionToIdle = "setidle\0";
        public const string _characterEdgeIdleToAction = "setaction\0";

        public const string _levelgeometryNodeIdle = "idle";


        public const int EngLanguageID = 1;
        public const int RusLanguageID = 2;
        

        public ObjectGraphStatic GraphCharacter
        {
            get;
            private set;
        }
        public SQliteConnector Database
        {
            get;
            private set;
        }
        private Dictionary<string, string> _localizations;

        private StaticObjects()
        { 
            //init all static objects here
            InitCharacterGraph();
            ConnectToDatabase("Data\\Data.sqlite");
            LoadLocalizations(EngLanguageID);
        }

        private void ConnectToDatabase(string __databaseName)
        {
            Database = new SQliteConnector(__databaseName);
        }

        private void LoadLocalizations(int __targetLanguage)
        {
            if (Database == null)
                return;
            _localizations = new Dictionary<string,string>();
            SQliteResultSet result = Database.executeSelect("select textKey, textValue from lang_text where langKey = " + __targetLanguage.ToString(), null);
            foreach(object[] arr in result.result)
            {
                _localizations.Add(arr[0].ToString(), arr[1].ToString());
            }
        }

        public string localization(string __key)
        {
            if (_localizations.Keys.Contains(__key))
                return _localizations[__key];

            LogProvider.logMessage("text not found: " + __key);
            return __key;
        }

        private void InitCharacterGraph()
        {
            CharacterGraphNode nodeIdle = new CharacterGraphNode();
            nodeIdle._name = _characterNodeIdle;
            nodeIdle._isOperable = true;
            nodeIdle._canReceiveControl = true;

            CharacterGraphNode nodeDead = new CharacterGraphNode();
            nodeDead._name = _characterNodeDead;
            nodeDead._isOperable = false;
            nodeDead._canReceiveControl = false;

            //all actions, jump, etc
            CharacterGraphNode nodeAction = new CharacterGraphNode();
            nodeDead._name = _characterNodeAction;
            nodeDead._isOperable = true;
            nodeDead._canReceiveControl = false;


            //edges
            CharacterGraphEdge deadToAlive = new CharacterGraphEdge();
            deadToAlive._chance = 1.0f;
            deadToAlive._eventName = _characterEdgeDeadToIdle;
            deadToAlive.SetNodeFrom(nodeDead);
            deadToAlive._nodeTo = nodeIdle;

            CharacterGraphEdge aliveToDead = new CharacterGraphEdge();
            deadToAlive._chance = 1.0f;
            deadToAlive._eventName = _characterEdgeIdleToDead;
            deadToAlive.SetNodeFrom(nodeIdle);
            deadToAlive._nodeTo = nodeDead;

            CharacterGraphEdge aliveToAction = new CharacterGraphEdge();
            deadToAlive._chance = 1.0f;
            deadToAlive._eventName = _characterEdgeIdleToAction;
            deadToAlive.SetNodeFrom(nodeIdle);
            deadToAlive._nodeTo = nodeDead;

            CharacterGraphEdge actionToAlive = new CharacterGraphEdge();
            deadToAlive._chance = 1.0f;
            deadToAlive._eventName = _characterEdgeActionToIdle;
            deadToAlive.SetNodeFrom(nodeIdle);
            deadToAlive._nodeTo = nodeDead;

            CharacterGraphEdge actionToDead = new CharacterGraphEdge();
            deadToAlive._chance = 1.0f;
            deadToAlive._eventName = _characterEdgeActionToDead;
            deadToAlive.SetNodeFrom(nodeIdle);
            deadToAlive._nodeTo = nodeDead;

            GraphCharacter = new ObjectGraphStatic(
                new ObjectGraphNode[] { nodeIdle, nodeDead,nodeAction }, 
                new ObjectGraphEdge[] { deadToAlive, aliveToDead, aliveToAction,actionToAlive,actionToDead}
                );
        }

        ~StaticObjects()
        {
            Database.Close();
        }
    }
}
