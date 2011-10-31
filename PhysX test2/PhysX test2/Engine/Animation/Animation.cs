using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Animation
{
    public abstract class Animation // абстракный общий класс анимации
    {
        public abstract Matrix[] GetMatrices(float frame);
        public bool isTransition;
        public int type;
        public float animlength;

        public abstract DecomposedMatrix[] GetMatrices(int frame);

        public static Matrix[][] GetRelatedMatrices(Matrix[][] Frames, Skeleton _skeleton)
        {
            int ss = _skeleton.RootIndex;
            Matrix[][] otnframes = new Matrix[Frames.Length][];
            for (int i = 0; i < Frames.Length; i++)
            {
                otnframes[i] = new Matrix[Frames[i].Length];
                Frames[i].CopyTo(otnframes[i], 0);
            }


            foreach (Bone bns in _skeleton.bones[ss].Childrens)
                MakeForwardBypass(bns, Frames, otnframes, _skeleton);


            return otnframes;
        }

        public static DecomposedMatrix[][] GetDecomposedMatrices(Skeleton _skeleton, Matrix[][] otnframes)
        {

            DecomposedMatrix[][] Qframes = new DecomposedMatrix[otnframes.Length][];
            for (int i = 0; i < otnframes.Length; i++)
                Qframes[i] = new DecomposedMatrix[_skeleton.bones.Length];

            int j = 0;
            foreach (Matrix[] frame in otnframes)
            {
                int k = 0;
                foreach (Matrix currentFrame in frame)
                {
                    Qframes[j][k] = new DecomposedMatrix(currentFrame);
                    k++;
                }
                j++;
            }
            return Qframes;
        }

        public static Matrix[] GetIndependentMatrices(Skeleton _Skeleton, Matrix[] otnosm)
        {
            int rootIndex = _Skeleton.RootIndex;

            Matrix[] frames = new Matrix[_Skeleton.bones.Length];
            //otnosm[15] = otnosm[i][15] * (Matrix.CreateRotationX((float)(Math.PI / 8)));
            frames[rootIndex] = otnosm[rootIndex];

            foreach (Bone bns in _Skeleton.bones[rootIndex].Childrens)
                MakeBackBypass(bns, otnosm, frames);


            return frames;
        }

        private static void MakeForwardBypass(Bone curbone, Matrix[][] Frames, Matrix[][] otnframes, Skeleton _skeleton)
        {

            int index = curbone.index;
            int parind = curbone.Parent.index;

            for (int i = 0; i < Frames.Length; i++)
                otnframes[i][index] = Frames[i][index] * Matrix.Invert(Frames[i][parind]);


            foreach (Bone bns in _skeleton.bones[index].Childrens)
                MakeForwardBypass(bns, Frames, otnframes, _skeleton);

        }

        private static void MakeBackBypass(Bone curbone, Matrix[] otnosm, Matrix[] frames)
        {
            int index = curbone.index;
            int parind = curbone.Parent.index;
            frames[index] = otnosm[index] * frames[parind];

            foreach (Bone bns in curbone.Childrens)
                MakeBackBypass(bns, otnosm, frames);

        }

    }
}
