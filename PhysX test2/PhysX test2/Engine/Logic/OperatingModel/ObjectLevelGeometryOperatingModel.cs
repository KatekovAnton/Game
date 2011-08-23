using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Logic.OperatingModel
{
    class ObjectLevelGeometryOperatingModel : ObjectOperatingModel
    {
        public override void OnMouseHover(object User, object GameInterface, OnMouseArgs e)
        {
            //повернуть игрока, наклонить его, 
        }
        public override void OnMouseDown(object User, object GameInterface, OnMouseArgs e)
        {
            //выстрелить... чтото типа юзер.файр чтоб он сам там у себя возможность сельбы проверял
        }
        public override void OnMouseUp(object User, object GameInterface, OnMouseArgs e)
        {
            //хххм
        }
        public override void OnKeyDown(object User, object GameInterface)
        {
            //хххм
        }
        public override void OnKeyUp(object User, object GameInterface)
        {
            //хххм
        }
    }
}
