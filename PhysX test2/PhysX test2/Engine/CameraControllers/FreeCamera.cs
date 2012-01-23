using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PhysX_test2.Engine.Logic;
using System;

namespace PhysX_test2.Engine.CameraControllers
{
    public class FreeCamera : CameraControllerSuperClass
    {
        protected float cam_speed = 0.01f;
        

        public FreeCamera(Camera cam, Vector3 pos,  Vector3 dir) :
            base(cam, pos, dir)
        {
            _currentTarget = pos + dir;
            _tar = pos + dir;
            _pos = pos;
            cur_pos = new Point((int)MouseManager.Manager.state.X, (int)MouseManager.Manager.state.Y);
        }
        Point cur_pos;
        bool pass = false;
        public override void UpdateCamera()
        {
            base.UpdateCamera();
        }
        Vector3 an;
        public void Update(bool w, bool a, bool s, bool d)
        {
            if (!pass)
            {
                float _ax = an.X - MouseManager.Manager.d_mouse.X / 100;
                float _ay = an.Y - MouseManager.Manager.d_mouse.Y / 100;

                //if (_ay < 0 && _ay > -MathHelper.Pi) 
                an.Y = _ay;
                an.X = _ax;

                float cax = (float)(Math.Cos(an.X));
                float sax = (float)(Math.Sin(an.X));
                float cay = (float)(Math.Cos(an.Y));
                float say = (float)(Math.Sin(an.Y));

                Vector3 move = cam_speed *
                    new Vector3(
                        (w ? sax : s ? -sax : 0) + (a ? cax : 0) + (d ? -cax : 0),
                        (w ? an.Y : s ? -an.Y : 0),
                        (w ? cax : s ? -cax : 0) + (a ? -sax : 0) + (d ? sax : 0)
                        );
                if (move != Vector3.Zero)
                {
                    move.Normalize();
                    ((UserInterface.GameInterface)MyGame.Instance._engine.UI).debug_textbox.Text = move.ToString();
                    _pos += move;
                }

                _tar = _pos + new Vector3(sax, an.Y, cax);
                Microsoft.Xna.Framework.Input.Mouse.SetPosition(cur_pos.X, cur_pos.Y);
            }

           _currendPosition = MyMath.perehod_fps(_currendPosition, _pos, 0.9f);
            _currentTarget = MyMath.perehod_fps(_currentTarget, _tar, 0.9f);
            pass = !pass;
        }

        Vector3 _pos, _tar;

        public override void UpdateCamerafromUser(Vector3 _targetPoint)
        {
            
            Update( KeyboardManager.currentState.IsKeyDown(Keys.W),
                    KeyboardManager.currentState.IsKeyDown(Keys.A), 
                    KeyboardManager.currentState.IsKeyDown(Keys.S),
                    KeyboardManager.currentState.IsKeyDown(Keys.D));

            base.UpdateCamerafromUser(_targetPoint);
        }

    }
}