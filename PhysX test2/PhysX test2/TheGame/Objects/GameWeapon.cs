using System;
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
            _inHandObject = Engine.GameEngine.loadObject(__inHandObject, null, false, null, PivotObjectDependType.Weapon) as LevelObject;
            _onFloorObject = Engine.GameEngine.loadObject(__onFloorObject, null, true) as LevelObject;
            _addonObject = Engine.GameEngine.loadObject(__addonObject, null, false,  null, PivotObjectDependType.Weapon) as LevelObject;

            _state = GameWeaponState.None;
        }

        public void DropOnFloor()
        {
            switch (_state)
            {
                case GameWeaponState.None:
                    {
                        _hisLevel.AddObject(_onFloorObject);
                        _hisLevel.AddObject(_addonObject);
                        _hisLevel.AddObjectSequence(_onFloorObject, _addonObject);
                    } break;
                case GameWeaponState.InHand:
                    {
                        _hisLevel._scene.SwapObjects(_inHandObject, _onFloorObject, false);
                    } break;
                default: break;
            }
            _state = GameWeaponState.OnFloor;
        }

        public void TakeInHand(GameCharacter __character)
        {
            switch (_state)
            {
                case GameWeaponState.None:
                    {
                        _inHandObject.behaviourmodel.SetParentObject(__character._aliveObject);
                        _addonObject.behaviourmodel.SetParentObject(__character._aliveObject);
                        _hisLevel.AddObject(_inHandObject);
                        _hisLevel.AddObject(_addonObject);
                        _hisLevel.AddObjectSequence(__character._aliveObject, _inHandObject);
                        _hisLevel.AddObjectSequence(_inHandObject, _addonObject);
                        
                    } break;
                case GameWeaponState.OnFloor:
                    {
                        _hisLevel._scene.SwapObjects(_onFloorObject, _inHandObject, false);
                    } break;
                default: break;
            }

            _state = GameWeaponState.InHand;
        }

        public void RemoveFromScene()
        {
            switch (_state)
            {
                case GameWeaponState.OnFloor:
                    {
                        _hisLevel.RemoveObject(_addonObject);
                        _hisLevel.RemoveObject(_onFloorObject);
                    } break;
                case GameWeaponState.InHand:
                    {
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
    }
}
