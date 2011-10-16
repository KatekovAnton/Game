using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace PhysX_test2.Engine.Animation
{
    public class CharacterStatic                              // класс чарактера
    {
        public CharacterPart[] parts;                   // список частей

        public ExtendedSkeleton skeleton;

        public CharacterStatic()
        { }

        public  Matrix[] GetFrameMatrix(AnimationNode[] ans, Skeleton skeleton,  int[] frameNamber, Matrix chahgeRoot)
        {
            Matrix[]res = new Matrix[skeleton.bones.Count()];
            DecomposedMatrix[] temp = new DecomposedMatrix[skeleton.bones.Count()];

            for (int i = 0; i < ans.Length; i++)
            {
                 for (int j = 0;j < parts[i].animGraph.boneIndexes.Length;j++)
                 {
                     temp[parts[i].animGraph.boneIndexes[j]] = ((FullAnimation)ans[i].animation).matrices[frameNamber[i]][j];
                 }
                                             
            }
            res = DecomposedMatrix.ConvertToMartixArray(temp);
            res[skeleton.RootIndex] = res[skeleton.RootIndex] * chahgeRoot;
            res = Animation.GetIndependentMatrices(skeleton, res);

            return res;
        }
    }
}
