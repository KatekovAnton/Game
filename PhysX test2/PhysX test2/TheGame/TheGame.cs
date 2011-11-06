using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine;

using PhysX_test2.TheGame.Objects;
using PhysX_test2.TheGame.ObjectLogic;
using PhysX_test2.TheGame.Level;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysX_test2.TheGame
{
    public class TheGame:IKeyboardUser
    {
        private List<HotKey> _hotkeys;

        public const string _playerKey = "player";
        public const string _playerweaponKet = "playerWeapon";

        public Dictionary<string, LogicCharacter> _characters;
        public Dictionary<string, LogicWeapon> _weapons;

        public Dictionary<string, GameObject> _objects;

        public GameLevel _level;
        public GameEngine _engine;

        public TheGame(MyGame __game)
        {
            _engine = new GameEngine(__game);
            _level = new GameLevel(_engine.gameScene);

            _characters = new Dictionary<string, LogicCharacter>();
            _weapons = new Dictionary<string, LogicWeapon>();

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
            _characters[_playerKey].SwitchGun();
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



            LogicCharacter me = new LogicCharacter(myCharacter, myHead);
            _characters.Add(_playerKey, me);
            me.SetAlive(true);


            GameWeapon myWeapon = new GameWeapon("SCGunHandLO\0", "SCGunFloorLO\0", "SСGunAddon\0", myCharacter._aliveObject, _level);
            LogicWeapon myGun = new LogicWeapon(myWeapon);
            _weapons.Add(_playerweaponKet, myGun);


            me.SetGun(myGun);
            _engine.CreateCharCameraController();
        }

    }
}
