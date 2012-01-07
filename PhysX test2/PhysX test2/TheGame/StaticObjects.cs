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

        private static Dictionary<string, CharacterParameters> _characterParameters;
        public static Dictionary<string, CharacterParameters> CharacterParameters
        {
            get
            {
                return _characterParameters;
            }
        }

        private static Dictionary<string, WeaponParameters> _weaponParameters;
        public static Dictionary<string, WeaponParameters> WeaponParameters
        {
            get
            {
                return _weaponParameters;
            }
        }

        private static Dictionary<int, Dictionary<int, AnimationInfo>> _animations;
        public static Dictionary<int, Dictionary<int, AnimationInfo>> AnimationsParameters
        {
            get
            {
                return _animations;
            }
        }

        private static Dictionary<string, EffectParameters> _effectParameters;
        public static Dictionary<string, EffectParameters> EffectParameters
        {
            get
            {
                return _effectParameters;
            }
        }

        private class EffectHash
        {
            public Engine.Logic.PivotObjectMaterialType _materialType;
            public PhysX_test2.TheGame.LogicControllers.Parameters.BulletType _bulletType;
            public PhysX_test2.TheGame.LogicControllers.Parameters.EffectParameters _resultEffect;
        }
        private static EffectHash[] _effectHashTable;

        private StaticObjects()
        { 
            //init all static objects here
            InitCharacterGraph();
            ConnectToDatabase("Data\\Data.sqlite");
            LoadLocalizations(EngLanguageID);
            LoadBulletInformation();
            LoadAnimations();
            LoadCharacterInformation();
            LoadWeaponInformation();
            LoadEffects();
            LoadEffectHashTable();
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

        private void LoadAnimations()
        {
            _animations = new Dictionary<int, Dictionary<int, AnimationInfo>>();
            SQliteResultSet anims = Database.executeSelect("select distinct character from character_animations", null);
            foreach (object[] charid in anims.result)
            {
                string id_ = charid[0].ToString();
                SQliteResultSet animations = Database.executeSelect("select * from character_animations where character = " + id_, null);
                Dictionary<int, AnimationInfo> animationsresult = new Dictionary<int, AnimationInfo>();
                foreach (object[] anim in animations.result)
                {
                    int type = Convert.ToInt32(anim[1]);
                    string stayAnim = anim[2].ToString();
                    string fireAnim = anim[3].ToString();
                    string actionAnim = anim[4].ToString();
                    string staybottomanim = anim[5].ToString();
                    AnimationInfo aa = new AnimationInfo(type, stayAnim, staybottomanim, fireAnim, actionAnim);
                    animationsresult.Add(type, aa);
                }
                _animations.Add(Convert.ToInt32(id_), animationsresult);
            }
        }

        private void LoadEffects()
        {
            if (_effectParameters != null)
                return;

            _effectParameters = new Dictionary<string, EffectParameters>();
            SQliteResultSet efects = Database.executeSelect("select * from effects", null);
            foreach (object[] data in efects.result)
            {
                string effectId = data[0].ToString();
                int effectIdInt = Convert.ToInt32(data[0]);
                SQliteResultSet efectsItems = Database.executeSelect("select * from effect_items where effectId = " + effectId, null);
                EffectParameters ep = new EffectParameters(effectIdInt, data[1] as string, efectsItems.result.ToArray());
                _effectParameters.Add(effectId, ep);
            }
        }

        private void LoadEffectHashTable()
        {
            _effectHashTable = new EffectHash[3 * 3];
            {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.Metal;
                _effectHashTable[0] = eh;

                eh._resultEffect = _effectParameters["1"];
            }
            {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.Stone;
                _effectHashTable[1] = eh;

                eh._resultEffect = _effectParameters["1"];
            }
            {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.Wood;
                _effectHashTable[2] = eh;

                eh._resultEffect = _effectParameters["1"];
            }
          /*  {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.DynamicWood;
                _effectHashTable[3] = eh;

                eh._resultEffect = _effectParameters["1"];
            }*/
            {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.DynamicStone;
                _effectHashTable[4] = eh;

                eh._resultEffect = _effectParameters["1"];
            }
            {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.DynamicMetal;
                _effectHashTable[5] = eh;

                eh._resultEffect = _effectParameters["1"];
            }
           /* {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.DynamicHuman;
                _effectHashTable[6] = eh;

                eh._resultEffect = _effectParameters["1"];
            }*/
            {
                EffectHash eh = new EffectHash();
                eh._bulletType = BulletType.Bullet;
                eh._materialType = Engine.Logic.PivotObjectMaterialType.DynamicAlien;
                _effectHashTable[7] = eh;

                eh._resultEffect = _effectParameters["1"];
            }
        }

        public static string GetEffectParameters(Engine.Logic.PivotObjectMaterialType __material, 
            PhysX_test2.TheGame.LogicControllers.Parameters.BulletType __bullet)
        {
            for (int i = 0; i < _effectHashTable.Length; i++)
            {
                if (_effectHashTable[i] == null)
                    continue;
                if (_effectHashTable[i]._bulletType == __bullet && _effectHashTable[i]._materialType == __material)
                    return _effectHashTable[i]._resultEffect._dbID.ToString();
            }
            return null;
        }

        private void LoadBulletInformation()
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
                double lifeTime = Convert.ToDouble((arr[4] as string).Replace('.', ','));
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

        private void LoadCharacterInformation()
        {
            if (_characterParameters != null)
                return;

            _characterParameters = new Dictionary<string, CharacterParameters>();
            SQliteResultSet result = Database.executeSelect("select NPC.id, characters.levelObject, characters.waklSpeed, characters.actionTime, NPC.headObject, NPC.humanName, NPC.isUniq, characters.id from NPC inner join characters on NPC.character = characters.id", null);
            foreach (object[] arr in result.result)
            {
                int id = Convert.ToInt32(arr[0]);
                string loname = arr[1].ToString();
                float speed = Convert.ToSingle((arr[2] as string).Replace('.', ','));
                float actionTime = Convert.ToSingle((arr[3] as string).Replace('.', ','));
                string honame = arr[4].ToString();
                string numanname = arr[5].ToString();

                bool uniq = arr[6].ToString().Equals("1");
                string id_ = id.ToString();
                int charid = Convert.ToInt32(arr[7]);

               
                Dictionary<int, AnimationInfo> animinfo = _animations[charid];

                CharacterParameters parameters = new CharacterParameters(id, numanname, loname, honame, uniq, speed,actionTime, animinfo);
                _characterParameters.Add(id_, parameters);
            }
        }

        private void LoadWeaponInformation()
        {
            if (_weaponParameters != null)
                return;

            _weaponParameters = new Dictionary<string, WeaponParameters>();
            SQliteResultSet result = Database.executeSelect("SELECT weapons.*, calibers.caliberName FROM weapons inner join calibers on weapons.caliber = calibers.id", null);
            foreach (object[] arr in result.result)
            {
                int id = Convert.ToInt32(arr[0]);
                string id_ = id.ToString();
                string displayName = arr[15].ToString();
                float mass = Convert.ToSingle((arr[17] as string).Replace('.', ','));
                string shortName = arr[1].ToString();
                int caliber = Convert.ToInt32(arr[2]);
                string inhandObject = arr[3].ToString();
                string onfloorObject = arr[4].ToString();
                string addonObject = arr[5].ToString();
                float reloadTime = Convert.ToSingle((arr[6] as string).Replace('.', ','));
                float fireTime = Convert.ToSingle((arr[7] as string).Replace('.', ','));
                float fireRestartTime = Convert.ToSingle((arr[8] as string).Replace('.', ','));
                string fireObject = arr[9].ToString();
                float damageBallMod = Convert.ToSingle((arr[10] as string).Replace('.', ','));
                float damageEnMod = Convert.ToSingle((arr[11] as string).Replace('.', ','));
                int magazineCapacity = Convert.ToInt32(arr[12]);
                int oneShootCount = Convert.ToInt32(arr[13]);
                float accuracy = Convert.ToSingle((arr[14] as string).Replace('.', ','));
                string description = arr[16].ToString();
                int type = Convert.ToInt32(arr[18]);
                string fireOffset = arr[19].ToString();
                string caliberName = arr[20].ToString();

                WeaponParameters parameters = new WeaponParameters(id, displayName, mass,
                    shortName, caliber, inhandObject, onfloorObject,
                    addonObject, reloadTime, fireTime, fireRestartTime,
                    fireObject, damageBallMod, damageEnMod, magazineCapacity,
                    oneShootCount, accuracy, description, caliberName, type, fireOffset);
                _weaponParameters.Add(id_, parameters);
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
