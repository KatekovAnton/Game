using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using StillDesign.PhysX;
using PhysX_test2.Content;
using System.IO;


namespace PhysX_test2
{
    public static class Extensions
    {
        private static Random _random;
        public static Random GetRandomizer()
        {
            if (_random == null)
                _random = new Random((int)DateTime.Now.Ticks);
            return _random;
        }

        public enum Route {
            Forward,
            Back,
            Left,
            Right
        }

        public static StillDesign.PhysX.MathPrimitives.Vector3 toPhysicV3(this Microsoft.Xna.Framework.Vector3 _vector)//стринг с нулём
        {
            return new StillDesign.PhysX.MathPrimitives.Vector3(_vector.X, _vector.Y, _vector.Z);
        }

        public static Microsoft.Xna.Framework.Vector3 toXNAV3(this  StillDesign.PhysX.MathPrimitives.Vector3 _vector)//стринг с нулём
        {
            return new Microsoft.Xna.Framework.Vector3(_vector.X, _vector.Y, _vector.Z);
        }

        public static StillDesign.PhysX.MathPrimitives.Matrix toPhysicM(this Microsoft.Xna.Framework.Matrix _matrix)//стринг с нулём
        {
            return new StillDesign.PhysX.MathPrimitives.Matrix(_matrix.M11, _matrix.M12, _matrix.M13, _matrix.M14,
                                                                _matrix.M21, _matrix.M22, _matrix.M23, _matrix.M24,
                                                                _matrix.M31, _matrix.M32, _matrix.M33, _matrix.M34,
                                                                _matrix.M41, _matrix.M42, _matrix.M43, _matrix.M44);
        }

        public static Microsoft.Xna.Framework.Matrix toXNAM(this StillDesign.PhysX.MathPrimitives.Matrix _matrix)//стринг с нулём
        {
            return new Microsoft.Xna.Framework.Matrix (_matrix.M11, _matrix.M12, _matrix.M13, _matrix.M14,
                                                                _matrix.M21, _matrix.M22, _matrix.M23, _matrix.M24,
                                                                _matrix.M31, _matrix.M32, _matrix.M33, _matrix.M34,
                                                                _matrix.M41, _matrix.M42, _matrix.M43, _matrix.M44);
        }

        public static string ReadPackString(this System.IO.BinaryReader self)//стринг с нулём
        {
            var length = self.ReadInt32();
            return new string(self.ReadChars(length + 1));
        }


        public static Matrix ReadMatrix(this BinaryReader self)
        {
            return new Matrix(
                self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), 0.0f,
                self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), 0.0f,
                self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), 0.0f,
                self.ReadSingle(), self.ReadSingle(), self.ReadSingle(), 1.0f
            );
        }
        public static Vector3 ReadVector3(this BinaryReader self)
        {
            return new Vector3(self.ReadSingle(), self.ReadSingle(), self.ReadSingle());
        }

        public static void WriteVector3(this BinaryWriter self, Vector3 v)
        {
            self.Write(v.X);
            self.Write(v.Y);
            self.Write(v.Z);
        }
        public static void Times(this int self, Action<int> action)
        {
            for (var i = 0; i < self; i++)
            {
                action(i);
            }
        }

        public static Microsoft.Xna.Framework.Ray FromScreenPoint(Viewport viewport, Vector2 mousePos, Matrix view, Matrix project)
        {
            Vector3 nearPoint = viewport.Unproject(new Vector3(mousePos, 0), project, view, Matrix.Identity);
            Vector3 farPoint = viewport.Unproject(new Vector3(mousePos, 1), project, view, Matrix.Identity);

            return new Microsoft.Xna.Framework.Ray(nearPoint, farPoint - nearPoint);
        }

        public static void GetCenter(this BoundingBox bbox, out Vector3 center)
        {
            center = new Vector3(
             (bbox.Min.X + bbox.Max.X) / 2,
             (bbox.Min.Y + bbox.Max.Y) / 2,
             (bbox.Min.Z + bbox.Max.Z) / 2);
        }

        public static void GetSize(this BoundingBox bbox, out Vector3 size)
        {
            size = new Vector3(
             bbox.Max.X - bbox.Min.X,
             bbox.Max.Y - bbox.Min.Y,
             bbox.Max.Z - bbox.Min.Z);
        }

        public static double Round(double value, int digits)
        {
            double scale = Math.Pow(10.0, digits);
            double round = Math.Floor(Math.Abs(value) * scale + 0.5);
            return (Math.Sign(value) * round / scale);
        }

        public static Vector3 VectorForCharacterMoving(Route way, float angle) {
            Vector3 vector;
            switch(way) {
                case Route.Forward:
                    vector = new Vector3((float)(Settings.movingSpeed * Math.Cos(angle)), 0, (float)(-Settings.movingSpeed * Math.Sin(angle)));
                    return vector;
                case Route.Back:
                    vector = new Vector3((float)(-Settings.movingSpeed * Math.Cos(angle)), 0, (float) (Settings.movingSpeed * Math.Sin(angle)));
                    return vector;
                case Route.Right:
                    vector = new Vector3((float)(Settings.movingSpeed * Math.Sin(angle)), 0, (float)(Settings.movingSpeed * Math.Cos(angle)));
                    return vector;
                case Route.Left:
                    vector = new Vector3((float)(-Settings.movingSpeed * Math.Sin(angle)), 0, (float)(-Settings.movingSpeed * Math.Cos(angle)));
                    return vector;
            }
            return new Vector3();
        }
    }
}