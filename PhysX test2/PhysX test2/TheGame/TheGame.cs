using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine;

using PhysX_test2.TheGame.Objects;
using PhysX_test2.TheGame.Level;

using PhysX_test2.TheGame.LogicControllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysX_test2.TheGame
{
    public class TheGame:IKeyboardUser
    {
        private List<HotKey> _hotkeys;

        public const string _playerCharacterKey = "player";
        public const string _playerWeaponKey = "playerWeapon";

        public Dictionary<string, CharacterLogicController> _characters;
        public Dictionary<string, WeaponLogicController> _weapons;
        
        CharacterLogicController _anotherChar;
        WeaponLogicController _anotherWeapon;

        public Dictionary<string, GameObject> _objects;
        public MyContainer<BaseLogicController> _allLogicObjects;

        public GameLevel _level;
        public GameEngine _engine;

        public TheGame(MyGame __game)
        {
            _engine = new GameEngine(__game);
            _level = new GameLevel(_engine.gameScene);

            _characters = new Dictionary<string, CharacterLogicController>();
            _weapons = new Dictionary<string, WeaponLogicController>();
            _allLogicObjects = new MyContainer<BaseLogicController>();
            _hotkeys = new List<HotKey>();
            _hotkeys.Add(new HotKey(new Keys[] { Keys.I }, SwichGunState));

            KeyboardManager.Manager.AddKeyboardUser(this);

            
        }

        public bool IsKeyboardCaptured()
        {
            return false;
        }

        public void SwichGunState()
        {
            _characters[_playerCharacterKey].SwitchGun();
        }

        public List<HotKey> hotkeys()
        {
            return _hotkeys;
        }


        public void Initialize()
        {
            _engine.Initalize();
            LoadSampleData();
        }

        public void LoadSampleData()
        {
            //загрузка всего
            _engine.Loaddata();


            GameCharacter myCharacter = new GameCharacter("SCMarineAlive\0", Matrix.CreateTranslation(new Vector3(0, 0, 0.1f)), _level);
            GameSimpleObject myHead = new GameSimpleObject("Head01\0", myCharacter._aliveObject, Engine.Logic.PivotObjectDependType.Head, _level, false, false);

            CharacterLogicController me = new CharacterLogicController(myCharacter, myHead,true);
            _characters.Add(_playerCharacterKey, me);
            me.SetAlive(true);
            me._hisInput = new InputManagers.InputProviderPlayer();

            GameWeapon myWeapon = new GameWeapon("SCGunHandLO\0", "SCGunFloorLO\0", "SСGunAddon\0", myCharacter._aliveObject, _level);
            WeaponLogicController myGun = new WeaponLogicController(myWeapon);
            _weapons.Add(_playerWeaponKey, myGun);
            me.SetGun(myGun);
            
            _engine.CreateCharCameraController(me._hisObject._aliveObject);

            _allLogicObjects.Add(me);
            _allLogicObjects.Add(myGun);

            GameCharacter myCharacter1 = new GameCharacter("SCMarineAlive\0", Matrix.CreateTranslation(new Vector3(0, 0, 0.1f)), _level);
            GameSimpleObject myHead1 = new GameSimpleObject("Head01\0", myCharacter1._aliveObject, Engine.Logic.PivotObjectDependType.Head, _level, false, false);
            _anotherChar = new CharacterLogicController(myCharacter1, myHead1, false);
            _anotherChar.SetAlive(true);
            GameWeapon myWeapon1 = new GameWeapon("SCGunHandLO\0", "SCGunFloorLO\0", "SСGunAddon\0", myCharacter1._aliveObject, _level);
            _anotherWeapon = new WeaponLogicController(myWeapon1);
            _anotherChar.SetGun(_anotherWeapon);

            LogicControllers.Parameters.WeaponParameters weaponStaticParams = new LogicControllers.Parameters.WeaponParameters(0, "Gauss rifle", 10, null);
            weaponStaticParams._fireRestartTime = 100.0f;

            LogicControllers.Parameters.WeaponParameters weaponParams = new LogicControllers.Parameters.WeaponParameters(weaponStaticParams, null);
            weaponParams._fireRestartTime = 100.0f;

            LogicControllers.Parameters.WeaponParameters.allItems.Add(weaponStaticParams._displayName, weaponStaticParams);
            myGun._baseParameters = weaponStaticParams;
            myGun._instanceParameters = weaponParams;
        }

        public void Update(GameTime __gameTime)
        {
            foreach (BaseLogicController controller in _allLogicObjects)
                controller.Update(__gameTime);
        }

    }
}
