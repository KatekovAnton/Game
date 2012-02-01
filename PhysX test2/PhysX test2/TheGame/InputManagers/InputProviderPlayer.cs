using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using PhysX_test2.Engine.CameraControllers;

namespace PhysX_test2.TheGame.InputManagers
{
    public class InputProviderPlayer : InputProviderSuperClass, IAllKeys, IKeyboardUser
    {
        public float cospi8 = (float)Math.Cos(Math.PI / 8.0);
        public float cospi4 = (float)Math.Cos(Math.PI / 4.0);
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
            if (!Enabled)
            {
                _tryAttackFirst = false;
                _newInputState = CharacterMoveState.Stay;
                return;
            }

            if (KeyboardManager.IsMouseCaptured || KeyboardManager.Manager.captured_user_all_keys != null)
            {
                _tryAttackFirst = false;
                _newInputState = CharacterMoveState.Stay;
                return;
            }

            _tryAttackFirst = MouseManager.Manager.lmbState == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
            _angle = 0;

            _target = MyGame.Instance._mousepoint;
            _angle = CreateAngleForCharacter(_target, __position);


            _moveVector = Vector3.Zero;
            _viewVector = _target - __position;

            float ydiff = _viewVector.Y;
            Vector3 middlePoint = new Vector3(_target.X, __position.Y, _target.Z);
            float sin = (middlePoint - __position).Length() / _viewVector.Length();
            _bodyRotation = Convert.ToSingle(Math.Acos((double)sin));
            if (ydiff > 0)
                _bodyRotation *= -1;
            _bodyRotation += 0.05f;


            float yaang = CameraManager._cameraController._yAngle;

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
                //  MyGame.Instance._engine.cameraneedsupdate = true;
                _newInputState = calculateCurrentState(_moveVector, _viewVector);
            }
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

            if (value > cospi4)
                return CharacterMoveState.WalkForward;
            else if (value < -cospi4)
                return CharacterMoveState.WalkBackward;
            else
            {
                Vector3 v = Vector3.Cross(_move, _view);
                if (v.Y < 0)
                    return CharacterMoveState.WalkLeft;
                else
                    return CharacterMoveState.WalkRight;
            }
        }

        public List<HotKey> _hotkeys;

        public virtual bool GlobalUser { get { return false; } }
        public virtual bool IsKeyboardCaptured { set; get; }
        public virtual List<HotKey> hotkeys { get { return _hotkeys; } }
        public virtual void KeyPress() { }
    }
}
