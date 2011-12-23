using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.Objects;
using PhysX_test2.TheGame.Objects.StateGraphs;
using PhysX_test2.SQliteShell;
using PhysX_test2.TheGame.LogicControllers.Parameters;

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

        //load from database
        private static Dictionary<string, BulletParameters> _bulletParameters;
        public static Dictionary<string, BulletParameters> BulletParameters
        {
            get
            {
                return _bulletParameters;
            }
        }

        private StaticObjects()
        { 
            //init all static objects here
            InitCharacterGraph();
            ConnectToDatabase("Data\\Data.sqlite");
            LoadLocalizations(EngLanguageID);
            LoadBulletInformation();
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

        public void LoadBulletInformation()
        {
            if (_bulletParameters != null)
                return;

            _bulletParameters = new Dictionary<string, BulletParameters>();
            SQliteResultSet result = Database.executeSelect("select ammo.caliber, ammo.damage, ammo.pierce, ammo.speed, ammo.liveTime, ammo.accuracy, ammo.inboxCount, ammo.levelObjectName, ammo.inventoryImage, ammo.boxMass, ammo.bulletMass, ammo.name, ammo.description, calibers.caliberName, ammo.id from ammo inner join calibers on ammo.caliber = calibers.id", null);
            foreach (object[] arr in result.result)
            {
                int caliber = Convert.ToInt32(arr[0]);
                float damage = Convert.ToSingle((arr[1] as string).Replace('.', ','));
                float pierce = Convert.ToSingle((arr[2] as string).Replace('.', ','));
                float speed = Convert.ToSingle((arr[3] as string).Replace('.', ','));
                float lifeTime = Convert.ToSingle((arr[4] as string).Replace('.', ','));
                float accuracy = Convert.ToSingle((arr[5] as string).Replace('.', ','));

                int inboxCount = Convert.ToInt32((arr[6] as string).Replace('.', ','));

                string levelObjectName = arr[7].ToString();
                string inventImagename = arr[8].ToString();

                float boxMass = Convert.ToSingle((arr[9] as string).Replace('.', ','));
                float bulletMass = Convert.ToSingle((arr[10] as string).Replace('.', ','));
                //.name, .description, .caliberName 
                string name = arr[11].ToString();
                string desc = arr[12].ToString();
                string calibername = arr[13].ToString();
                int id = Convert.ToInt32(arr[14]);
                string id_ = id.ToString();
                BulletParameters parameters = new BulletParameters(id, name, boxMass, caliber, damage, pierce, speed, lifeTime, accuracy, levelObjectName, bulletMass);
                _bulletParameters.Add(id_, parameters);
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
