using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.InputManagers
{
    public enum CharacterMoveState
    {
        _new,
        Stay,
        WalkForward,
        WalkBackward,
        WalkLeft,
        WalkRight,
        WalkForwardLeft,
        WalkForwardRight,
        WalkBackwardLeft,
        WalkBackwardRight
    };

    public class InputProviderSuperClass
    {
        public bool _tryAttackFirst;
        public bool _tryAttackSecond;

        public CharacterMoveState _oldInputState;
        public CharacterMoveState _newInputState;
        public Vector3 _moveVector;
        public Vector3 _viewVector;
        public Vector3 _target;
        public float _angle;

        public float _bodyRotation = 0.0f;

        public virtual void Update(Vector3 __position)
        {
 
        }
    }
}
