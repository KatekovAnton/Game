using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace PhysX_test2
{
    public static class MyMath
    {
        public static bool Near(this Vector3 v, Vector3 v1)
        {
            return Math.Abs(v.X - v1.X) < 0.001f && Math.Abs(v.Y - v1.Y) < 0.001f && Math.Abs(v.Z - v1.Z) < 0.001f;
            // return Math.Abs(x - v.x) + Math.Abs(y - v.y) + Math.Abs(z - v.z) < 0.00001f;
        }
        public static bool NewNear(this Vector3 v, Vector3 v1, float zerolevel)
        {
            return (v-v1).LengthSquared()<zerolevel*zerolevel;
            // return Math.Abs(x - v.x) + Math.Abs(y - v.y) + Math.Abs(z - v.z) < 0.00001f;
        }
        public static Vector2 Vector2FromVector4(Vector4 value)
        {
            return new Vector2(value.X, value.Y);
        }
        public static bool Near(this Vector2 v, Vector2 v1)
        {
            return Math.Abs(v.X - v1.X) < 0.01f && Math.Abs(v.Y - v1.Y) < 0.01f;
            // return Math.Abs(x - v.x) + Math.Abs(y - v.y) + Math.Abs(z - v.z) < 0.00001f;
        }

    }
}
