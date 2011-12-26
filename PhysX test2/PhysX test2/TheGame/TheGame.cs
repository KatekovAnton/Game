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
        //public Dictionary<string, WeaponLogicController> _weapons;


        public StaticObjects _staticData;
        public GameLevel _level;
        public GameEngine _engine;

        public TheGame(MyGame __game)
        {
            _engine = new GameEngine(__game);
            _staticData = StaticObjects.Instance();
            _level = new GameLevel(_engine.gameScene);

            _characters = new Dictionary<string, CharacterLogicController>();
           // _weapons = new Dictionary<string, WeaponLogicController>();
            
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
            _engine.AfterLoading();
        }

        public void LoadSampleData()
        {
            _engine.CasheObject("SimpleBullet_LO\0", null, false, false);
            //загрузка всего
            _engine.Loaddata();



            CharacterLogicController me = CharacterLogicController.CreateCharacter("1", _level);
            me.SetAlive(true);
            me._hisInput = new InputManagers.InputProviderPlayer();

            WeaponLogicController myGun = WeaponLogicController.CreateWeapon("1",me,_level);
            me.SetGun(myGun);
            _level.AddController(me);
            _level.AddController(myGun);
            _characters.Add(_playerCharacterKey, me);
            _engine.CreateCharCameraController(me._hisObject._levelObject);


            GameCharacter myCharacter1 = new GameCharacter("SCMarineAlive\0", Matrix.Identity, _level);
            GameSimpleObject myHead1 = new GameSimpleObject("Head01\0", myCharacter1._levelObject, Engine.Logic.PivotObjectDependType.Head, _level, false, false);
            CharacterLogicController _anotherChar = new CharacterLogicController(myCharacter1, myHead1, false);
            _anotherChar.SetAlive(true);
            GameWeapon myWeapon1 = new GameWeapon("SCGunHandLO\0", "SCGunFloorLO\0", "SСGunAddon\0", myCharacter1._levelObject, _level);
            GameSimpleObject weaponFire1 = new GameSimpleObject("Fire01LO\0", myWeapon1._inHandObject, Engine.Logic.PivotObjectDependType.Body, _level, false, false);
            WeaponLogicController _anotherWeapon = new WeaponLogicController(myWeapon1, weaponFire1);
            _anotherChar.SetGun(_anotherWeapon);
            _level.AddController(_anotherChar);
            _level.AddController(_anotherWeapon);
        }

        public void Update(GameTime __gameTime)
        {
            _level.Update(__gameTime);
        }

    }
}
