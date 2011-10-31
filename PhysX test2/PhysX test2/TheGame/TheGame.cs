using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.TheGame.ObjectLogic;
using PhysX_test2.TheGame.Level;

namespace PhysX_test2.TheGame
{
    public class TheGame
    {
        public LogicNPC _player;
        public LogicWeapon _playerWeapon;

        public GameLevel _level;

        public TheGame()
        {
            _level = new GameLevel(new Engine.Logic.EngineScene());
        }

    }
}
