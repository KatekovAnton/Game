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
    public enum GameWeaponState
    {
        None,
        OnFloor,
        InHand
    };

    public class GameWeapon : GameObject, IGraphUser
    {
        public GameWeaponState _state;

        public LevelObject _inHandObject;
        public LevelObject _onFloorObject;
        public LevelObject _addonObject;


        /// <summary>
        /// pish pish
        /// </summary>
        public GameSimpleObject _shotFireObject;

        /// <summary>
        /// for future
        /// </summary>
        public LigthSource _lightSource;
        

        public GameWeapon(string __inHandObject, string __onFloorObject, string __addonObject, GameLevel __level)
            : base(__level, true, false)
        {
            _inHandObject = Engine.GameEngine.LoadObject(__inHandObject, null, false, false, PivotObjectDependType.Weapon) as LevelObject;
            _onFloorObject = Engine.GameEngine.LoadObject(__onFloorObject, null, true, false) as LevelObject;
            if(__addonObject.Length>1)
                _addonObject = Engine.GameEngine.LoadObject(__addonObject, null, false, false, PivotObjectDependType.Body) as LevelObject;

            _state = GameWeaponState.None;
        }

        public override void LocateToLevel(LevelObject __parent)
        {
            if(__parent == null)
            {
                DropOnFloor();
                return;
            }
            if(!__parent.renderaspect.isanimated)
            {
                DropOnFloor();
                return;
            }
            TakeInHand(__parent);
        }

        public void DropOnFloor()
        {
            switch (_state)
            {
                case GameWeaponState.None:
                    {
                        _hisLevel.AddEngineObject(_onFloorObject);
                        if(_addonObject!=null)
                            _hisLevel.AddEngineObject(_addonObject, _onFloorObject);
                    } break;
                case GameWeaponState.InHand:
                    {
                        _hisLevel._scene.SwapObjects(_inHandObject, _onFloorObject, false);
                        _onFloorObject.SetGlobalPose(_inHandObject.transform,true);
                    } break;
                default: break;
            }
            _state = GameWeaponState.OnFloor;
        }

        public void TakeInHand(LevelObject __character)
        {
            switch (_state)
            {
                case GameWeaponState.None:
                    {
                        _hisLevel.AddEngineObject(_inHandObject, __character);
                        if (_addonObject != null)
                            _hisLevel.AddEngineObject(_addonObject, _inHandObject);
                    } break;
                case GameWeaponState.OnFloor:
                    {
                        _hisLevel._scene.SwapObjects(_onFloorObject, _inHandObject, false);
                    } break;
                default: break;
            }

            _state = GameWeaponState.InHand;
        }

        public override void RemoveFromLevel()
        {
            switch (_state)
            {
                case GameWeaponState.OnFloor:
                    {
                        if (_addonObject != null)
                            _hisLevel.RemoveObject(_addonObject);
                        _hisLevel.RemoveObject(_onFloorObject);
                    } break;
                case GameWeaponState.InHand:
                    {
                        if (_addonObject != null)
                            _hisLevel.RemoveObject(_addonObject);
                        _hisLevel.RemoveObject(_inHandObject);
                    } break;
                default: break;
            }

            _state = GameWeaponState.None;
        }

        public void BeginFire()
        { }

        public void StopFire()
        { }

        public override void Unload()
        {
            RemoveFromLevel();

            _shotFireObject.RemoveFromLevel();
            _shotFireObject.Unload();

            Engine.ContentLoader.ContentLoader.UnloadPivotObject(_inHandObject);
            Engine.ContentLoader.ContentLoader.UnloadPivotObject(_onFloorObject);
            Engine.ContentLoader.ContentLoader.UnloadPivotObject(_addonObject);
        }
    }
}
