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

        static public bool limits(Vector2 val, Vector4 limit)
        { return val.X > limit.X && val.Y > limit.Y && val.X < limit.Z && val.Y < limit.W; }

        static public bool limits(Vector2 val, Vector2 position, Vector2 size)
        { return val.X > position.X && val.Y > position.Y && val.X < position.X + size.X && val.Y < position.Y + size.Y; }

        static public Color perehod(Color col1, Color col2, double time)
        {
            col1.R = perehod(col1.R, col2.R, time);
            col1.G = perehod(col1.G, col2.G, time);
            col1.B = perehod(col1.B, col2.B, time);
            col1.A = perehod(col1.A, col2.A, time);
            return col1;
        }

        static public void perehod(ref Color col1, Color col2, double time)
        {
            col1.R = perehod(col1.R, col2.R, time);
            col1.G = perehod(col1.G, col2.G, time);
            col1.B = perehod(col1.B, col2.B, time);
            col1.A = perehod(col1.A, col2.A, time);
        }

        static public byte perehod(byte val1, byte val2, double time)
        { return (byte)((val1 - val2) * time + val2); }

        static public byte perehod(ref byte val1, byte val2, double time)
        { return (byte)((val1 - val2) * time + val2); }

        static public void perehod(ref double val1, double val2, double time)
        { val1 = (val1 - val2) * time + val2; }
        static public void perehod(ref float val1, float val2, float time)
        { val1 = (val1 - val2) * time + val2; }

        static public void perehod(ref Vector2 val1, Vector2 val2, float time)
        { val1 = (val1 - val2) * time + val2; }

        static public void perehod(ref Vector3 val1, Vector3 val2, float time)
        { val1 = (val1 - val2) * time + val2; }

        static public double Angle(Vector2 v1, Vector2 v2)
        {
            if (v1.Y == v2.Y) return v1.Y < v2.Y ? Math.PI : 0;
            return (v1.Y < v2.Y ? Math.PI / 2 - Math.Atan((v1.X - v2.X) / (v1.Y - v2.Y)) : Math.Atan((v1.X - v2.X) / (-v1.Y + v2.Y)) - Math.PI / 2);
        }

        static public double Distance(Vector2 v1, Vector2 v2)
        { return Math.Sqrt((v1.X - v2.X) * (v1.X - v2.X) + (v1.Y - v2.Y) * (v1.Y - v2.Y)); }
        static public double Distance(Vector4 v)
        { return Math.Sqrt((v.X - v.Z) * (v.X - v.Z) + (v.Y - v.W) * (v.Y - v.W)); }
        static public double Distance(double x1, double y1, double x2, double y2)
        { return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); }
        static public float Distance(float x1, float y1, float x2, float y2)
        { return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); }

        static public int minimax_int(int val, int min, int max)
        {
            return val > max ? max : val < min ? min : val;
        }

    }

    public struct DecomposedMatrix
    {
        public Quaternion rotation;
        public Vector3 translation;
        public Vector3 scale;

        public DecomposedMatrix(Matrix _matrix)
        {
            _matrix.Decompose(out scale, out rotation, out translation);
        }

       

        public Matrix GetMartix()
        {
            Matrix mtrx = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(translation);
            return mtrx;
        }

        public static Matrix[] ConvertToMartixArray(DecomposedMatrix[] entermatrix)
        {
            Matrix[] result = new Matrix[entermatrix.Length];
            for (int i = 0; i < entermatrix.Length; i++)
            {
                result[i] = Matrix.CreateScale(entermatrix[i].scale) * Matrix.CreateFromQuaternion(entermatrix[i].rotation) * Matrix.CreateTranslation(entermatrix[i].translation);// entermatrix[i].GetMartix();
            }
            return result;
        }
    }
}
