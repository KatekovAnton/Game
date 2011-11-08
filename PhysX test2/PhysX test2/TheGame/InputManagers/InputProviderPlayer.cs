using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace PhysX_test2.TheGame.InputManagers
{
    public class InputProviderPlayer : InputProviderSuperClass
    {
        public float cospi18 = (float)Math.Cos(Math.PI * 1.0 / 8.0);
        public float _oldRotation = -MathHelper.PiOver2;

        private float CreateAngleForCharacter(Microsoft.Xna.Framework.Vector3 __target, Vector3 __position)
        {
            double buffer = 0;
            double result = 0;

            float cursorX = __target.X;
            float cursorZ = __target.Z;


            float charX = __position.X;
            float charZ = __position.Z;
            double difX = charX - cursorX;
            double difZ = charZ - cursorZ;
            if (difX > 0)
            {
                if (difX == 0)
                    difX = 0.01;
                buffer = Math.Atan(difZ / difX);
            }
            else
            {
                if (difX == 0)
                    difX = 0.01;
                buffer = Math.Atan(difZ / difX) - Math.PI;
            }
            result = _oldRotation - buffer;

            _oldRotation = (float)buffer;
            return (float)result;
        }

        public override void Update(Vector3 __position)
        {
            _tryAttackFirst = MouseManager.Manager.lmbState == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            _target = MyGame.Instance._mousepoint;
            _angle = CreateAngleForCharacter(_target, __position);
            float yaang = MyGame.Instance._engine._cameraController._yAngle;
            _moveVector = Vector3.Zero;
            _viewVector = __position - _target;

            // обработка нажатий клавы
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Forward, yaang);
                _moveVector += move;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Back, yaang);
                _moveVector += move;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Left, yaang);
                _moveVector += move;
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Right, yaang);
                _moveVector += move;
            }

            _oldInputState = _newInputState;
            if (_moveVector != Vector3.Zero)
                _newInputState = calculateCurrentState(_moveVector, _viewVector);
            else
                _newInputState = CharacterMoveState.Stay;
        }

        public CharacterMoveState calculateCurrentState(Vector3 _move, Vector3 _view)
        {
            Vector2 move = new Vector2(_move.X, _move.Z);
            Vector2 view = new Vector2(_view.X, _view.Z);
            move.Normalize();
            view.Normalize();


            float value = Vector2.Dot(move, view);

            if (1 - value > cospi18)
                return CharacterMoveState.WalkForward;
            if (-1 + value < cospi18)
                return CharacterMoveState.WalkBackward;

            return CharacterMoveState.WalkForward;
        }
    }
}
