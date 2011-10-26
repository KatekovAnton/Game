using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Animation
{
    public class SkeletonExtended //: PackContent//int SkeletonWithAddInfo = 31;
    {
        #region packcontent methods
       
        public void loadbody(System.IO.BinaryReader br)
        {
            baseskelet = Skeleton.FromStream(br);
            HeadIndex = br.ReadInt32();
            WeaponIndex = br.ReadInt32();
            RootIndex = br.ReadInt32();
            TopRootIndex = br.ReadInt32();
            BottomRootIndex = br.ReadInt32();
            botomindexes = new int[br.ReadInt32()];
            for (int i = 0; i < botomindexes.Length; i++)
                botomindexes[i] = br.ReadInt32();

            topindexes = new int[br.ReadInt32()];
            for (int i = 0; i < topindexes.Length; i++)
                topindexes[i] = br.ReadInt32();


            //TODO load matrices
        }
        #endregion

        #region data
        public int[] botomindexes;

        public int[] topindexes;

        public int HeadIndex;
        public int WeaponIndex;
        public int RootIndex;
        public int TopRootIndex;
        public int BottomRootIndex;

        public Microsoft.Xna.Framework.Matrix HeadMatrix;
        public Microsoft.Xna.Framework.Matrix WeaponMatrix;

        public Skeleton baseskelet;
        #endregion

        public SkeletonExtended(Skeleton s)
        {
            baseskelet = s;
        }

        public SkeletonExtended()
        {
           
        }
    }
}
