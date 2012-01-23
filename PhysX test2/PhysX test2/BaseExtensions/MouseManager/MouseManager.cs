using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PhysX_test2
{
    public class MouseManager
    {

        public static MouseManager Manager;

        public int scrollWheelSTARTValue;
        public int scrollWheelValue;
        public int scrollWheelDelta;

        public Vector2 mousePos;

        public ButtonState lmbState;
        public ButtonState lmblastState;

        public bool isJustPressed;
        public bool isJustReleased;

        public MouseState state;
        public MouseState lastState;
        public Vector2 d_mouse;

        public bool moved;
        bool ft = true;
        public MouseManager()
        {
            state = Mouse.GetState();
            lastState = state;

            scrollWheelValue = state.ScrollWheelValue;
            scrollWheelSTARTValue = state.ScrollWheelValue;

            Manager = this;
        }

        public void Update()
        {
            if (ft)
            {
                ft = false;
                return;
            }
            state = Mouse.GetState();
            if (mousePos.X != state.X || mousePos.Y != state.Y)
                moved = true;
            else
                moved = false;
            mousePos.X = state.X;
            mousePos.Y = state.Y;

            d_mouse = new Vector2(state.X - lastState.X, state.Y - lastState.Y);

            scrollWheelDelta = scrollWheelValue - state.ScrollWheelValue;
            scrollWheelValue = state.ScrollWheelValue;

           
            lmbState = state.LeftButton;
            
            
            isJustPressed = isJustReleased = false;
            if (lmblastState == ButtonState.Pressed && lmbState == ButtonState.Released)
                isJustReleased = true;
            else if (lmblastState == ButtonState.Released && lmbState == ButtonState.Pressed)
                isJustPressed = true;


            lastState = state;
            
        }
    }
}
