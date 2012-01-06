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
        public ButtonState lastState;

        public bool isJustPressed;
        public bool isJustReleased;

        public bool moved;
        bool ft = true;
        public MouseManager()
        {
            MouseState state = Mouse.GetState();
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
               MouseState state = Mouse.GetState();
            if (mousePos.X != state.X || mousePos.Y != state.Y)
                moved = true;
            else
                moved = false;
            mousePos.X = state.X;
            mousePos.Y = state.Y;

            scrollWheelDelta = scrollWheelValue - state.ScrollWheelValue;
            scrollWheelValue = state.ScrollWheelValue;

            lastState = lmbState;
            lmbState = state.LeftButton;

            isJustPressed = isJustReleased = false;
            if (lastState == ButtonState.Pressed && lmbState == ButtonState.Released)
                isJustReleased = true;
            else if (lastState == ButtonState.Released && lmbState == ButtonState.Pressed)
                isJustPressed = true;

            
        }
    }
}
