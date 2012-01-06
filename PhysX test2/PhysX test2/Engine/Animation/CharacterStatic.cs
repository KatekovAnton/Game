using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace PhysX_test2.Engine.Animation
{
    public class CharacterStatic : ContentNew.IPackEngineObject                              // класс чарактера
    {
        public CharacterPart[] parts;                   // список частей
        public SkeletonExtended skeleton;

        public bool Disposed = true;

        public CharacterStatic()
        { }

        public void Dispose()
        {
            parts = null;
            skeleton = null;
            Disposed = true;
        }

        private void loadFormData(byte[] __data)
        {
            skeleton = new SkeletonExtended();
            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.MemoryStream(__data));
            skeleton.loadbody(br);
            List<CharacterPart> parts1 = new List<CharacterPart>();
            bool bottomExist = br.ReadBoolean();
            if (bottomExist)
                parts1.Add(new CharacterPart(AnimationGraph.AnimationGraphFromStream(br)));


            bool topExist = br.ReadBoolean();
            if (topExist)
                parts1.Add(new CharacterPart(AnimationGraph.AnimationGraphFromStream(br)));

            parts = parts1.ToArray();
        }

        public void CreateFromContentEntity(ContentNew.IPackContentEntity[] __contentEntities)
        {
            if (!Disposed)
                Dispose();
            ContentNew.CharacterBase character = __contentEntities[0] as ContentNew.CharacterBase;
            if (character == null)
                throw new Exception("wrong object in EngineMesh.CreateFromContentEntity");

            loadFormData(character.data);
        }

        //TODO: ЭТОТ МЕТОД ОТ СТАРОГО ЗАГРУЗЧИКА
        public void CreateFromContentEntity(Content.PackContent[] __contentEntities)
        {
            if (!Disposed)
                Dispose();
            Content.CharacterContent character = __contentEntities[0] as Content.CharacterContent;
            if (character == null)
                throw new Exception("wrong object in EngineMesh.CreateFromContentEntity");

            loadFormData(character.data);
        }
    }
}
