using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Content
{
    public class LevelObjectDescription : PackContent
    {

        public string RCCMName;

        public bool IsRCCMEnabled;

        public bool IsRCCMAnimated;

        public string matname;

        public string RODName;

        public RenderObjectDescription ROD;

        public bool IsAnimated;

        public string CharacterName;

        public Vector3 RCShapeSize;
        public int RCShapeType;

        public string DefaultAnimName;

        public int BehaviourType;

        public string PhysicCollisionName;

        public float Mass;

        public int ShapeType;
            //0-physix shape
            //1-collision mesh


        public int PhysXShapeType;

        public bool IsStatic;

        public Vector3 CenterOfMass;

        public Vector3 ShapeSize;

        public Vector3 ShapeRotationAxis;

        public float ShapeRotationAngle;

        public LevelObjectDescription()
        {

        }

        public override void loadbody(byte[] array)
        {
            BinaryReader br = new BinaryReader(new MemoryStream(array));
            matname = br.ReadPackString();

            RODName = br.ReadPackString();

            IsRCCMEnabled = br.ReadBoolean();
            if (IsRCCMEnabled)
                RCCMName = br.ReadPackString();
            else
            {
                RCShapeType = br.ReadInt32();
                RCShapeSize = br.ReadVector3();
            }
            IsAnimated = br.ReadBoolean();
            if (IsAnimated)
            {
                CharacterName = br.ReadPackString();
                IsRCCMAnimated = br.ReadBoolean();
            }

            BehaviourType = br.ReadInt32();
            switch (BehaviourType)
            {
                case objectmovingbehaviourmodel:
                    { } break;
                case objectphysicbehaviourmodel:
                    {
                        ShapeType = br.ReadInt32();
                        if (ShapeType == 0)
                        {
                            PhysXShapeType = br.ReadInt32();
                            ShapeSize = new Vector3(br.ReadSingle(),
                            br.ReadSingle(),
                            br.ReadSingle());
                            //read shape type, size of shape, rotation axis and angle
                        }
                        if (ShapeType == 1)
                        {
                            PhysicCollisionName = br.ReadPackString();
                        }
                        IsStatic = br.ReadBoolean();
                        if (!IsStatic)
                        {
                            Mass = br.ReadSingle();
                            CenterOfMass.X = br.ReadSingle();
                            CenterOfMass.Y = br.ReadSingle();
                            CenterOfMass.Z = br.ReadSingle();
                        }
                    } break;
                case objectphysiccharcontrollerbehaviourmodel:
                    {
                        ShapeType = br.ReadInt32();
                        if (ShapeType == 0)
                        {
                            PhysXShapeType = br.ReadInt32(); ;
                            ShapeSize = new Vector3(br.ReadSingle(),
                             br.ReadSingle(),
                            br.ReadSingle());
                        }
                        if (ShapeType == 1)
                        {
                            PhysicCollisionName = br.ReadPackString();
                        }
                        Mass = br.ReadSingle();
                        CenterOfMass.X = br.ReadSingle();
                        CenterOfMass.Y = br.ReadSingle();
                        CenterOfMass.Z = br.ReadSingle();

                    } break;
                case objectstaticbehaviourmodel:
                    { } break;
                default: break;
            }

            br.Close();
        }
    
        public Pack Pack
        {
            get;
            set;
        }

        //движущийся объект(не по законам физики, а собств вычислениями - пули например)
        public const int objectmovingbehaviourmodel = 0;
        const string ObjectMovingBehaviourModel = "Moving behaviour model";
        //недвижимый объект, НЕ ИМЕЮЩИЙ ФИЗ МОДЕЛИ
        public const int objectstaticbehaviourmodel = 1;
        const string ObjectStaticBehaviourModel = "Static behaviour model";
        //любой объект, который обрабатывается физиксом, кроме чарактерконтроллеров
        public const int objectphysicbehaviourmodel = 2;
        const string ObjectPhysicBehaviourModel = "Physic behaviour model";
        //физические чарактерконтроллеры
        public const int objectphysiccharcontrollerbehaviourmodel = 3;
        const string ObjectPhysicCharacterControllerBehaviourModel = "Physic character controller behaviour model";
        //зависимые от парента объекты
        public const int objectRelatedBehaviourModel = 4;
        const string ObjectRelatedBehaviourModel = "Related behaviour model";
        //зависимые от кости парента объекты
        public const int objectBonerelatedbehaviourmodel = 5;
        const string ObjectBoneRelatedBehaviourModel = "Bone related behaviour model";
    }
}
