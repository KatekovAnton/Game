using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.Objects;
using PhysX_test2.TheGame.ObjectLogic;
using PhysX_test2.TheGame.Level;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame
{
    public class TheGame
    {
        public const string _playerKey = "player";
        public const string _playerweaponKet = "playerWeapon";

        public Dictionary<string, LogicCharacter> _characters;
        public Dictionary<string, LogicWeapon> _weapons;

        public GameLevel _level;

        public TheGame()
        {
            _level = new GameLevel(new Engine.Logic.EngineScene());


            GameCharacter myCharacter = new GameCharacter(Engine.GameEngine.loadObject("MyNewCharacter\0", Matrix.CreateTranslation(new Vector3(0, 0, 0.1f)), true), null, _level);
            GameObject myHead = new GameSimpleObject(Engine.GameEngine.loadObject("Head01\0", Matrix.CreateTranslation(new Vector3(-0.00f, -0.01f, 0.84f)), false, myCharacter._aliveObject, Engine.Logic.PivotObjectDependType.Head), _level, false, false);
            LogicCharacter me = new LogicCharacter(myCharacter,myHead);
        }

    }
}
