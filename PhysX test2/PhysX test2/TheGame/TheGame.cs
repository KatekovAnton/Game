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

        public bool GlobalUser { set { } get { return false; } }

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
          //  _engine.CasheObject("SimpleBullet_LO\0", null, false, false);
          //  _engine.CasheObject("EffectLevelObject\0", null, false, false);
            //загрузка всего
            _engine.Loaddata();



            CharacterLogicController me = CharacterLogicController.CreateCharacter("1", _level, false);
            me.SetAlive(true);
            me._hisInput = new InputManagers.InputProviderPlayer();

            WeaponLogicController myGun = WeaponLogicController.CreateWeapon("2",_level);
            me.SetGun(myGun);
            
            _characters.Add(_playerCharacterKey, me);
            _engine.CreateCharCameraController(me._hisObject._levelObject);


            CharacterLogicController _anotherChar = CharacterLogicController.CreateCharacter("2", _level, true);
            _anotherChar.SetAlive(true);
            _anotherChar._hisInput = new InputManagers.InputProviderAI();
            WeaponLogicController _anotherWeapon = WeaponLogicController.CreateWeapon("1", _level);
            _anotherChar.SetGun(_anotherWeapon);

        }

        public void Update(GameTime __gameTime)
        {
            _level.Update(__gameTime);
        }

    }
}
