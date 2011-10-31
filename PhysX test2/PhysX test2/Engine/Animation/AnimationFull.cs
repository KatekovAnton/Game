using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Animation
{
    public class FullAnimation : Animation // 
    {
        public DecomposedMatrix[][] matrices;
        public float length;

        public int BonesCount;

        public FullAnimation()
        {
            type = 0;
        }

        public override DecomposedMatrix[] GetMatrices(int __frame)
        {
            int frame = __frame % matrices.Length;
            return matrices[frame];
         
        }

        public override Matrix[] GetMatrices(float frame)
        {
            return DecomposedMatrix.ConvertToMartixArray(matrices[Convert.ToInt32(frame)]);
        }

        public static FullAnimation From3DMAXStream(System.IO.Stream stream, Skeleton skeleton)
        {
            Matrix[][] Frames;

            var clip = new FullAnimation();
            var reader = new System.IO.BinaryReader(stream);

            var start = reader.ReadInt32();
            var end = reader.ReadInt32();
            var length = end - start + 1;

            clip.BonesCount = reader.ReadInt32();
            var counter = reader.ReadInt32();



            Frames = new Matrix[length][];
            for (int i = 0; i < length; i++)
                Frames[i] = new Matrix[clip.BonesCount];


            for (int i = 0; i < clip.BonesCount; i++)
                for (int j = 0; j < length; j++)
                    Frames[j][i] = reader.ReadMatrix();


            //теперь надо вычислить дельты 
            //(идёт загрузка экспортированного из макса)
            for (int i = 0; i < clip.BonesCount; i++)
                for (int @in = 0; @in < length; @in++)
                    Frames[@in][i] = skeleton.bones[i].BaseMatrix * Frames[@in][i];


            Matrix[][] relatedMatrices = Animation.GetRelatedMatrices(Frames, skeleton);
            DecomposedMatrix[][] decomposedMatrices = GetDecomposedMatrices(skeleton, relatedMatrices);

            clip.matrices = decomposedMatrices;
            clip.length = clip.matrices.Length;
            return clip;
        }

        public static FullAnimation From3DMAXStream(System.IO.Stream stream, SkeletonExtended skeleton, bool reverse)
        {
            Matrix[][] Frames;
            var clip = new FullAnimation();
            var reader = new System.IO.BinaryReader(stream);
            var start = reader.ReadInt32();
            var end = reader.ReadInt32();
            var length = end - start + 1;
            clip.BonesCount = reader.ReadInt32();
            var counter = reader.ReadInt32();
            Frames = new Matrix[length][];
            for (int i = 0; i < length; i++)
                Frames[i] = new Matrix[clip.BonesCount];
            for (int i = 0; i < clip.BonesCount; i++)
                for (int j = 0; j < length; j++)
                    Frames[j][i] = reader.ReadMatrix();
            //теперь надо вычислить дельты 
            //(идёт загрузка экспортированного из макса)
            for (int i = 0; i < clip.BonesCount; i++)
                for (int @in = 0; @in < length; @in++)
                    Frames[@in][i] = skeleton.baseskelet.bones[i].BaseMatrix * Frames[@in][i];

            if (reverse)
            {
                //Matrix[][] FramesTmp = new Matrix[length][];
                Frames = Frames.Reverse().ToArray();
            }
            Matrix[][] relatedMatrices = Animation.GetRelatedMatrices(Frames, skeleton.baseskelet);
            DecomposedMatrix[][] decomposedMatrices = GetDecomposedMatrices(skeleton.baseskelet, relatedMatrices);

            clip.matrices = decomposedMatrices;
            return clip;
        }
        public static FullAnimation From3DMAXStream(System.IO.Stream stream, SkeletonExtended skeleton, bool reverse, int[] partIndexes)
        {
            Matrix[][] Frames;
            var clip = new FullAnimation();
            var reader = new System.IO.BinaryReader(stream);
            var start = reader.ReadInt32();
            var end = reader.ReadInt32();
            var length = end - start + 1;
            clip.BonesCount = reader.ReadInt32();
            var counter = reader.ReadInt32();
            Frames = new Matrix[length][];
            for (int i = 0; i < length; i++)
                Frames[i] = new Matrix[clip.BonesCount];
            for (int i = 0; i < clip.BonesCount; i++)
                for (int j = 0; j < length; j++)
                    Frames[j][i] = reader.ReadMatrix();
            //теперь надо вычислить дельты 
            //(идёт загрузка экспортированного из макса)
            for (int i = 0; i < clip.BonesCount; i++)
                for (int @in = 0; @in < length; @in++)
                    Frames[@in][i] = skeleton.baseskelet.bones[i].BaseMatrix * Frames[@in][i];

            if (reverse)
            {
                Frames = Frames.Reverse().ToArray();
            }
            Matrix[][] relatedMatrices = Animation.GetRelatedMatrices(Frames, skeleton.baseskelet);
            DecomposedMatrix[][] decomposedMatrices = GetDecomposedMatrices(skeleton.baseskelet, relatedMatrices);
            DecomposedMatrix[][] decomposedMatrices2 = GetDecomposedMatricesByPart(decomposedMatrices, partIndexes);
            clip.matrices = decomposedMatrices2;
            return clip;
        }
        public static DecomposedMatrix[][] GetDecomposedMatricesByPart(DecomposedMatrix[][] fullmarx, int[] partIndexes)
        {
            DecomposedMatrix[][] res;
            res = new DecomposedMatrix[fullmarx.Length][];
            for (int i = 0; i < fullmarx.Length; i++)
            {
                res[i] = new DecomposedMatrix[partIndexes.Length];
            }
            for (int i = 0; i < partIndexes.Length; i++)
            {
                for (int j = 0; j < fullmarx.Length; j++)
                {
                    res[j][i] = fullmarx[j][partIndexes[i]];
                }
            }
            return res;
        }

        public static void FullAnimationToStream(System.IO.BinaryWriter stream, FullAnimation animation)
        {
            stream.Write(animation.BonesCount);
            stream.Write(animation.length);

            stream.Write(animation.matrices.Length);
            for (int i = 0; i < animation.matrices.Length; i++)
            {
                stream.Write(animation.matrices[i].Length);
                for (int j = 0; j < animation.matrices[i].Length; j++)
                {
                    stream.Write(animation.matrices[i][j].rotation.W);
                    stream.Write(animation.matrices[i][j].rotation.X);
                    stream.Write(animation.matrices[i][j].rotation.Y);
                    stream.Write(animation.matrices[i][j].rotation.Z);
                    stream.WriteVector3(animation.matrices[i][j].translation);
                    stream.WriteVector3(animation.matrices[i][j].scale);
                }
            }
        }

        public static FullAnimation FullAnimationFromStream(System.IO.BinaryReader stream)
        {
            FullAnimation result = new FullAnimation();
            result.BonesCount = stream.ReadInt32();
            result.length = stream.ReadInt32();

            DecomposedMatrix[][] res;
            int length = stream.ReadInt32();
            res = new DecomposedMatrix[length][];
            for (int i = 0; i < length; i++)
            {
                int length2 = stream.ReadInt32();
                res[i] = new DecomposedMatrix[length2];
                for (int j = 0; j < length2; j++)
                {
                    res[i][j] = new DecomposedMatrix();
                    float W = stream.ReadSingle();
                    float X = stream.ReadSingle();
                    float Y = stream.ReadSingle();
                    float Z = stream.ReadSingle();
                    res[i][j].rotation = new Quaternion(X, Y, Z, W);
                    res[i][j].translation = stream.ReadVector3();
                    res[i][j].scale = stream.ReadVector3();
                }
            }

            result.matrices = res;
            result.animlength = result.matrices.Length;
            return result;
        }
    }
}
