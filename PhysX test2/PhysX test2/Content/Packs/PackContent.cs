using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace PhysX_test2.Content
{


    public class ElementType
    {
        public const int MissingObject = -1;
        public const int MeshOptimazedForStore = 0;
        public const int MeshOptimazedForLoading = 1;
        public const int PNGTexture = 2;


        public const int MeshList = 10;
        public const int StringList = 11;
        public const int WorldObjectDescription = 12;
        public const int GameObjectDescription = 13;
        public const int RenderObjectDescription = 14;



        public const int CollisionMesh = 20;
        public const int ConvexMesh = 21;

        public const int Skeleton = 30;
        public const int SkeletonWithAddInfo = 31;
        public const int BaseAnimation = 40;
        public const int Character = 50;
        public const int TextureList = 60;
        public const int Material = 61;




        public static string ReturnString(int format)
        {
            switch (format)
            {
                case ElementType.MeshOptimazedForLoading:
                    return "Mesh New";//"New format (recomendated)";
                case ElementType.MeshOptimazedForStore:
                    return "Old format (not recomendated)";
                case ElementType.PNGTexture:
                    return "Texture (PNG)";
                case ElementType.Skeleton:
                    return "Skeleton";
                case ElementType.BaseAnimation:
                    return "Base Animation";
                case ElementType.Character:
                    return "Character description";
                case ElementType.MeshList:
                    return "Mesh List";
                case ElementType.SkeletonWithAddInfo:
                    return "Skeleton with additional information";
                case ElementType.TextureList:
                    return "Texture list";
                case ElementType.CollisionMesh:
                    return "Collision mesh";
                case ElementType.ConvexMesh:
                    return "Convex mesh";
                case ElementType.StringList:
                    return "String list";
                case ElementType.WorldObjectDescription:
                    return "World object description";
                case ElementType.GameObjectDescription:
                    return "Game object description";
                case ElementType.RenderObjectDescription:
                    return "Render object description";
                case ElementType.Material:
                    return "Material";
                default:
                    return "wrong format";
            }

        }
        public static int ReturnFormat(string format)
        {
            switch (format)
            {
                case "Mesh New"://"New format (recomendated)":
                    return ElementType.MeshOptimazedForLoading;
                case "Old format (not recomendated)":
                    return ElementType.MeshOptimazedForStore;
                case "Texture (PNG)":
                    return ElementType.PNGTexture;
                case "Skeleton":
                    return ElementType.Skeleton;
                case "Base Animation":
                    return ElementType.BaseAnimation;
                case "Character description":
                    return ElementType.Character;
                case "Mesh List":
                    return ElementType.MeshList;
                case "Skeleton with additional information":
                    return ElementType.SkeletonWithAddInfo;
                case "Texture list":
                    return ElementType.TextureList;
                case "Collision mesh":
                    return ElementType.CollisionMesh;
                case "Convex mesh":
                    return ElementType.ConvexMesh;
                case "String list":
                    return ElementType.StringList;
                case "World object description":
                    return ElementType.WorldObjectDescription;
                case "Game object description":
                    return ElementType.GameObjectDescription;
                case "Render object description":
                    return ElementType.RenderObjectDescription;
                case "Material":
                    return ElementType.Material;
                default:
                    return -1;
            }
        }
    }

    public class HeaderInfo
    {
        public int offset;
        public int size;
        public int headersize;
        public int loadedformat;
        public string name;
    }
   
    public class MeshContentadditionalheader
    {
        public int ismaxdetalized;
        public int isstatic;
        public int skinsize;
        public string[] lods;
        public void load(BinaryReader br, int loadedformat)
        {
            switch (loadedformat)
            {
                case ElementType.MeshOptimazedForStore:
                    {
                        ismaxdetalized = br.ReadInt32();
                        skinsize = br.ReadInt32();
                        lods = new string[br.ReadInt32()];
                        int length;
                        for (int j = 0; j < lods.Length; j++)
                        {
                            length = br.ReadInt32();
                            lods[j] = new string(br.ReadChars(length + 1));
                        }
                    } break;
                case ElementType.MeshOptimazedForLoading:
                    {
                        ismaxdetalized = br.ReadInt32();
                        isstatic = br.ReadInt32();
                        lods = new string[br.ReadInt32()];
                        int length;
                        for (int j = 0; j < lods.Length; j++)
                        {
                            length = br.ReadInt32();
                            lods[j] = new string(br.ReadChars(length + 1));
                        }
                    } break;
                default:
                    { } break;
            }
        }
    }
   
    public class PackContent
    {
        public List<object> Enginereadedobject;
        public PackContent ReadedContentObject; 


        public bool objectReaded;
        public int number;
        public int offset;
        public int size;
        public int headersize;
        public int loadedformat;
        public string name;
        public MeshContentadditionalheader mh = null;
        public PackContent()
        {
            Enginereadedobject = new List<object>();
        }
        public PackContent(System.IO.BinaryReader br, int _number)//16+имя
        {
            this.number = _number;
            int length = br.ReadInt32();
            name = new string(br.ReadChars(length + 1));
            offset = br.ReadInt32();
            loadedformat = br.ReadInt32();
            headersize = br.ReadInt32();
            
            if (loadedformat == ElementType.MeshOptimazedForStore || loadedformat == ElementType.MeshOptimazedForLoading)
            {
                mh = new MeshContentadditionalheader();
                mh.load(br, loadedformat);
            }
            else
            {
                size = br.ReadInt32();
            }
            Enginereadedobject = new List<object>();
        }
        public virtual void loadbody(byte[] array)
        {}
        public virtual void Unload()
        {
            objectReaded = false;
            ReadedContentObject = null;
        }
    }
}
