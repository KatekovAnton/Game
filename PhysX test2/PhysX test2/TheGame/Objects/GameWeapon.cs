﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Engine;
using PhysX_test2.Engine.Logic;
using PhysX_test2.BaseExtensions.Graph;

using PhysX_test2.TheGame;

using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.Objects.StateGraphs;

namespace PhysX_test2.TheGame.Objects
{
    public class GameWeapon : GameObject
    {
        public PivotObject _inHandObject;
        public PivotObject _onFloorObject;

        public PivotObject _addonObject;

        public GameWeapon(GameLevel __level)
            : base(__level, true, false)
        { }

        public void DropOnFloor()
        { }

        public void TakeInHand(PivotObject __character)
        {
            
        }
    }
}