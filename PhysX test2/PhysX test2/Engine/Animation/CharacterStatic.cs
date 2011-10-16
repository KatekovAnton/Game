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

        public Matrix[] GetFrameMatrix(AnimationNode an, int frameNamber)
        {
            Matrix[]res = new Matrix[skeleton.baseskelet.bones.Count()];
            DecomposedMatrix[] temp = new DecomposedMatrix[skeleton.baseskelet.bones.Count()];

            for (int i = 0; i < parts.Length; i++)
            {
                 for (int j = 0;j < parts[i].animGraph.boneIndexes.Length;j++)
                 {
                     temp[parts[i].animGraph.boneIndexes[j]] = ((FullAnimation)an.animation).matrices[j][frameNamber];
                 }
                                             
            }
            res = DecomposedMatrix.ConvertToMartixArray(temp);
            res = Animation.GetIndependentMatrices(skeleton.baseskelet, res);

            return res;
        }
    }
}
