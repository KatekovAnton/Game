using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace PhysX_test2.TheGame.InputManagers
{
    public class InputProviderPlayer : InputProviderSuperClass, IAllKeys, IKeyboardUser
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

            if (KeyboardManager.Manager.captured_user_all_keys == null)
            {
                _tryAttackFirst = MouseManager.Manager.lmbState == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
                _target = MyGame.Instance._mousepoint;
                _angle = CreateAngleForCharacter(_target, __position);
                float yaang = MyGame.Instance._engine._cameraController._yAngle;
                _moveVector = Vector3.Zero;
                _viewVector = __position - _target;

                float ydiff = _viewVector.Y;
                Vector3 middlePoint = new Vector3(_target.X, __position.Y, _target.Z);
                float sin = (middlePoint - __position).Length() / _viewVector.Length();
                _bodyRotation = Convert.ToSingle(Math.Acos((double)sin));
                if (ydiff < 0)
                    _bodyRotation *= -1;
                _bodyRotation += 0.05f;
                // обработка нажатий клавы
                if (KeyboardManager.currentState.IsKeyDown(Keys.W))
                {
                    Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Forward, yaang);
                    _moveVector += move;
                }

                if (KeyboardManager.currentState.IsKeyDown(Keys.S))
                {
                    Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Back, yaang);
                    _moveVector += move;
                }

                if (KeyboardManager.currentState.IsKeyDown(Keys.A))
                {
                    Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Left, yaang);
                    _moveVector += move;
                }

                if (KeyboardManager.currentState.IsKeyDown(Keys.D))
                {
                    Vector3 move = Extensions.VectorForCharacterMoving(Extensions.Route.Right, yaang);
                    _moveVector += move;
                }

                _oldInputState = _newInputState;
                if (_moveVector != Vector3.Zero)
                {
                    MyGame.Instance._engine.cameraneedsupdate = true;
                    _newInputState = calculateCurrentState(_moveVector, _viewVector);
                }
                else
                    _newInputState = CharacterMoveState.Stay;
            }
            else
            {
                _tryAttackFirst = false;
                _newInputState = CharacterMoveState.Stay;
            }
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

        public List<HotKey> _hotkeys;

        public virtual bool GlobalUser { get { return false; } }
        public virtual bool IsKeyboardCaptured { set; get; }
        public virtual List<HotKey> hotkeys { get { return _hotkeys; } }
        public virtual void KeyPress() { }
    }
}
