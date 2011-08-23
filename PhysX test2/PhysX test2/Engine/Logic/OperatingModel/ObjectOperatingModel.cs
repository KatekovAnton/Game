using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.OperatingModel
{
    public class OnMouseArgs
    {
        public Vector3 GlobalMousePosition;
        public Vector2 ScreenMousePosition;
    }
  //  public delegate void UserActionEventHandler();

    //юзер - это чар игрока.
    //гейм интерфейс это интерфейс игры - перерисовать прицел, указатель мыши...

    public abstract class ObjectOperatingModel
    {
        /// <summary>
        /// Вызывать при наведении мыши
        /// </summary>
        public abstract void OnMouseHover(object User, object GameInterface, OnMouseArgs e);
        /// <summary>
        /// Вызывать при нажатии кнопки мыши
        /// </summary>
        public abstract void OnMouseDown(object User, object GameInterface, OnMouseArgs e);
        /// <summary>
        /// Вызывать при отпускании кнопки мыши
        /// </summary>
        public abstract void OnMouseUp(object User, object GameInterface, OnMouseArgs e);
        /// <summary>
        /// а этот вообще надо??
        /// </summary>
        public abstract void OnKeyDown(object User, object GameInterface);
        /// <summary>
        /// а этот вообще надо??
        /// </summary>
        public abstract void OnKeyUp(object User, object GameInterface);

    }
}
