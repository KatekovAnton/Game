using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.Engine.Animation
{
    public class CharacterStaticInfo //: PackContent//int SkeletonWithAddInfo = 31;
    {
        #region packcontent methods
       
      /*  public override int loadbody(BinaryReader br, System.Windows.Forms.ToolStripProgressBar toolStripProgressBar)
        {
            long a = br.BaseStream.Position;
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


            bool bottomExist = br.ReadBoolean();
            if (bottomExist)
                BottomGraph = AnimationGraph.AnimationGraphFromStream(br);

            bool topExist = br.ReadBoolean();
            if (topExist)
                TopGraph = AnimationGraph.AnimationGraphFromStream(br);

            return Convert.ToInt32(br.BaseStream.Position - a);
        }*/
        #endregion

        #region data
        public int[] botomindexes;

        public int[] topindexes;

        public int HeadIndex;
        public int WeaponIndex;
        public int RootIndex;
        public int TopRootIndex;
        public int BottomRootIndex;

        public Skeleton baseskelet
        {
            get;
            private set;
        }
        #endregion

      //  public AnimationGraph BottomGraph;
     //   public AnimationGraph TopGraph;

        public CharacterStaticInfo(Skeleton s)
        {
            baseskelet = s;
        }

        public CharacterStaticInfo()
        {
           
        }
    }
}
