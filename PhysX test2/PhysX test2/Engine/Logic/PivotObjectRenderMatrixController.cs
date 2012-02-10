using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Logic
{
    interface PivotObjectRenderMatrixController
    {
        Matrix GetRenderMatrix(Matrix __baseTransformMatrix);
    }

    class BillboardMatrixController : PivotObjectRenderMatrixController
    {
        public Matrix GetRenderMatrix(Matrix __baseTransformMatrix)
        {
            return Matrix.CreateBillboard(__baseTransformMatrix.Translation, CameraControllers.CameraManager.Camera._position, Vector3.Up, CameraControllers.CameraManager.Camera._direction);
        }
    }

    class ConstrainedBillboardMatrixController : PivotObjectRenderMatrixController
    {
        private PivotObject _baseObject;
        public ConstrainedBillboardMatrixController(PivotObject __baseObject)
        {
            _baseObject = __baseObject;
        }

        public Matrix GetRenderMatrix(Matrix __baseTransformMatrix)
        {
            return Matrix.CreateConstrainedBillboard(__baseTransformMatrix.Translation, CameraControllers.CameraManager.Camera._position, _baseObject._objectConstrAxis, CameraControllers.CameraManager.Camera._direction, _baseObject._objectConstrForward);
        }
    }

    class SkewMatrixController : PivotObjectRenderMatrixController
    {
        private double _x, _y;

        private static Vector2 _winddirection = new Vector2(-0.1f, 0.1f);
        private static float _windPower = 1.5f;
        private float _scale = 0.1f;
        private static Vector2 _maxDispersion = new Vector2(0.1f, 0.1f);

        public SkewMatrixController(float __scale)
        {
            _scale = __scale;
        }

        public Matrix GetRenderMatrix(Matrix __baseTransformMatrix)
        {
            double deltax = MyRandom.NextDouble(_maxDispersion.X);
            double deltay = MyRandom.NextDouble(_maxDispersion.Y);

            double time = MyGame.UpdateTime.ElapsedGameTime.TotalSeconds * 600 * _scale;

            _x += deltax * time;
            _y += deltay * time;
            return Extensions.CreateSkew((_winddirection.X * _windPower + (float)Math.Sin(_x)) * _scale, (_winddirection.Y * _windPower + (float)Math.Sin(_y)) * _scale) * __baseTransformMatrix;
        }
    }
}
