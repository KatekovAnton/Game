using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.Content;
using PhysX_test2.Engine;
using PhysX_test2.Engine.Logic;
using PhysX_test2.Engine.Render;
using PhysX_test2.Engine.Render.Materials;
using PhysX_test2.Engine.Animation;
using Microsoft.Xna.Framework.Graphics;


namespace PhysX_test2.Engine.ContentLoader
{
    public abstract class ContentLoader
    {
        private static StillDesign.PhysX.Material characterMaterial;
        public static Character currentCharacter;
        public static int boneToAdd;
        private static Material loadMaterial(string name, PackList packs)
        {
            PhysX_test2.Content.MaterialDescription mat = new MaterialDescription();
            mat = packs.GetObject(name , mat) as  MaterialDescription;
            if (mat.Enginereadedobject.Count == 0)
            {
                TextureMaterial.Lod[] lods = new TextureMaterial.Lod[mat.lodMats.Length];
                for (int i = 0; i < mat.lodMats.Length; i++)
                {
                    TextureMaterial.SubsetMaterial[] mats = new TextureMaterial.SubsetMaterial[mat.lodMats[i].mats.Length];
                    for (int j = 0; j < mat.lodMats[i].mats.Length; j++)
                    {
                        mats[j] = new TextureMaterial.SubsetMaterial();
                        Content.Texture inage = new Content.Texture();
                        inage = packs.GetObject(mat.lodMats[i].mats[j].DiffuseTextureName, inage) as Content.Texture;



                        inage.Enginereadedobject.Add(inage.texture);
                        mats[j].diffuseTexture = inage.texture;
                    }
                    lods[i] = new TextureMaterial.Lod(mats);
                    
                }
                TextureMaterial result = new TextureMaterial(lods);
                mat.Enginereadedobject.Add(result);

                return new TextureMaterial(lods);
            }
            else
            {
                TextureMaterial result = mat.Enginereadedobject[0] as TextureMaterial;
                mat.Enginereadedobject.Add(result);

                for (int i = 0; i < mat.lodMats.Length; i++)
                {
                    for (int j = 0; j < mat.lodMats[i].mats.Length; j++)
                    {
                        Content.Texture inage = new Content.Texture();
                        inage = packs.GetObject(mat.lodMats[i].mats[j].DiffuseTextureName, inage) as Content.Texture;
                        inage.Enginereadedobject.Add(inage.Enginereadedobject[0]);
                    }
                }


                return result;
            }
        }

        private static ContentMesh[] loadMeshArray(string[] mesnames, PackList packs)
        {
            ContentMesh[] subsetmeshes = new ContentMesh[mesnames.Length];
            for (int i = 0; i < mesnames.Length; i++)
            {
                subsetmeshes[i] = new ContentMesh();
                subsetmeshes[i] = packs.GetObject(mesnames[i], subsetmeshes[i]) as ContentMesh;
            }
            return subsetmeshes;
        }

        private static ContentSkinnedMesh[] loadSkinnedMeshArray(string[] mesnames, PackList packs, CharacterContent character)
        {
            ContentSkinnedMesh.skeleton = character;
            ContentSkinnedMesh[] subsetmeshes = new ContentSkinnedMesh[mesnames.Length];
            for (int i = 0; i < mesnames.Length; i++)
            {
                subsetmeshes[i] = new ContentSkinnedMesh();
                subsetmeshes[i] = packs.GetObject(mesnames[i], subsetmeshes[i]) as ContentSkinnedMesh;
            }
            return subsetmeshes;
        }

        public static Engine.Animation.Character createCharacter(PackList packs, string name, out CharacterContent characterContent)
        {
            characterContent = new Content.CharacterContent();
            characterContent = packs.GetObject(name, characterContent) as Content.CharacterContent;
            CharacterStatic characterResult = null;
            if (characterContent.Enginereadedobject.Count == 0)
            {
                characterResult = new CharacterStatic();
                characterResult.skeleton = new SkeletonExtended();
                System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.MemoryStream(characterContent.data));
                characterResult.skeleton.loadbody(br);
                List<CharacterPart> parts = new List<CharacterPart>();
                bool bottomExist = br.ReadBoolean();
                if (bottomExist)
                    parts.Add(new CharacterPart(AnimationGraph.AnimationGraphFromStream(br)));


                bool topExist = br.ReadBoolean();
                if (topExist)
                    parts.Add(new CharacterPart(AnimationGraph.AnimationGraphFromStream(br)));

                characterResult.parts = parts.ToArray();
                characterContent.Enginereadedobject.Add(characterResult);
            }
            else
            {
                characterResult = characterContent.Enginereadedobject[0] as CharacterStatic;
                characterContent.Enginereadedobject.Add(characterResult);
            }
            Character result = new Character(characterResult, new string[] { "stay1\0", "stay1\0" });
            return result;
        }

        private static RenderObject loadro(LevelObjectDescription description,
           PackList packs)
        {
            RenderObjectDescription rod = new RenderObjectDescription();

            rod = packs.GetObject(description.RODName, rod) as Content.RenderObjectDescription;
            description.ROD = rod;
            if (rod.Enginereadedobject.Count == 0)
            {
                if (description.IsAnimated)
                {
                    CharacterContent characterContent = null; ;
                    Engine.Animation.Character character = createCharacter(packs, description.CharacterName, out characterContent);

                    AnimRenderObject.Model[] models = new AnimRenderObject.Model[rod.LODs.Count];
                    for (int i = 0; i < models.Length; i++)
                    {
                        AnimRenderObject.SubSet[] modelsubsets = new AnimRenderObject.SubSet[rod.LODs[i].subsets.Count];
                        for (int j = 0; j < modelsubsets.Length; j++)
                        {
                            ContentSkinnedMesh[] subsetmeshes = loadSkinnedMeshArray(rod.LODs[i].subsets[j].MeshNames, packs, characterContent);
                            EngineSkinnedMesh subsetmesh = EngineSkinnedMesh.FromContentMeshes(subsetmeshes);
                            //меши могут быть по-разному сгруппированы поэтому будем их каждый раз по новой загружать
                            modelsubsets[j] = new AnimRenderObject.SubSet(subsetmesh);
                        }
                        models[i] = new AnimRenderObject.Model(modelsubsets);
                    }
                    RenderObject result = new AnimRenderObject(character, models, rod.IsShadowCaster, rod.IsShadowReceiver);
                    rod.Enginereadedobject.Add(result);
                    return result;
                }
                else
                {
                    UnAnimRenderObject.Model[] models = new UnAnimRenderObject.Model[rod.LODs.Count];
                    for (int i = 0; i < models.Length; i++)
                    {
                        UnAnimRenderObject.SubSet[] modelsubsets = new UnAnimRenderObject.SubSet[rod.LODs[i].subsets.Count];
                        for (int j = 0; j < modelsubsets.Length; j++)
                        {
                            ContentMesh[] subsetmeshes = loadMeshArray(rod.LODs[i].subsets[j].MeshNames, packs);
                            EngineMesh subsetmesh = EngineMesh.FromContentMeshes(subsetmeshes);
                            //меши могут быть по-разному сгруппированы поэтому будем их каждый раз по новой загружать
                            modelsubsets[j] = new UnAnimRenderObject.SubSet(subsetmesh);
                        }
                        models[i] = new UnAnimRenderObject.Model(modelsubsets);
                    }
                    RenderObject result = new UnAnimRenderObject(models, rod.IsShadowCaster, rod.IsShadowReceiver);
                    rod.Enginereadedobject.Add(result);
                    return result;
                }
            }
            else
            {
                RenderObject result = rod.Enginereadedobject[0] as RenderObject;
                if (result.isanimated != description.IsAnimated)
                    throw new Exception("wrong object animation flag!!!");
                rod.Enginereadedobject.Add(result);
                return result;
            }

        }

        private static RaycastBoundObject loadrcbo(Content.LevelObjectDescription description,
            PackList packs)
        {
            //клижмеш мб один на несколько объектов//его прекрасно удалит сам гк если что
            EngineCollisionMesh ObjectRCCM = null;
            //бш уникален для каждого //его тож гк удалит
            Logic.SceneGraph.OTBoundingShape bs = null;
            if (description.IsRCCMEnabled)
            {
                Content.CollisionMesh cm = new CollisionMesh();
                cm = packs.GetObject(description.RCCMName, cm) as CollisionMesh;
                if (cm.Enginereadedobject.Count == 0)
                    ObjectRCCM = EngineCollisionMesh.FromcontentCollisionMesh(cm as CollisionMesh);
                else
                    ObjectRCCM = cm.Enginereadedobject[0] as EngineCollisionMesh;

                cm.Enginereadedobject.Add(ObjectRCCM);

                bs = new Logic.SceneGraph.OTBoundingShape(ObjectRCCM);
            }
            else
                bs = new Logic.SceneGraph.OTBoundingShape(description.RCShapeSize);
           
            //его тоже удалят
            RaycastBoundObject raycastaspect = new RaycastBoundObject(bs, ObjectRCCM);
            return raycastaspect;
        }

        public static LevelObject LevelObjectFromDescription(
           Content.LevelObjectDescription description,
           PackList packs,
            StillDesign.PhysX.Scene scene)
        {
            if (characterMaterial == null)
            {
                StillDesign.PhysX.MaterialDescription md = new StillDesign.PhysX.MaterialDescription();
                md.SetToDefault();
                md.Restitution = 0.05f;
                characterMaterial = scene.CreateMaterial(md);
            }
            if (description.Enginereadedobject.Count == 0)
            {


                //рендераспект - мб один на несколько объектов
                RenderObject renderaspect = loadro(description, packs);
                Material material = loadMaterial(description.matname, packs);
                //его тоже удалят
                RaycastBoundObject raycastaspect = loadrcbo(description, packs);
                Logic.BehaviourModel.ObjectBehaviourModel behaviourmodel; 
                #region behaviour
                StillDesign.PhysX.Actor ObjectActor = null;
                switch (description.BehaviourType)
                {
                    case LevelObjectDescription.objectmovingbehaviourmodel:
                        {
                            throw new Exception("Unsupported behaviour model!");
                        } break;
                    case LevelObjectDescription.objectphysiccharcontrollerbehaviourmodel:
                        {
                            StillDesign.PhysX.ActorDescription ObjectActorDescription = new StillDesign.PhysX.ActorDescription();

                            if (description.ShapeType == 0)
                            {
                                if (description.PhysXShapeType == 0)
                                {
                                    StillDesign.PhysX.BoxShapeDescription boxshape = new StillDesign.PhysX.BoxShapeDescription(description.ShapeSize.toPhysicV3());
                                    boxshape.LocalRotation = Microsoft.Xna.Framework.Matrix.CreateRotationX(Microsoft.Xna.Framework.MathHelper.PiOver2).toPhysicM();
                                    ObjectActorDescription.Shapes.Add(boxshape);
                                }
                                else if (description.PhysXShapeType == 1)
                                {
                                    StillDesign.PhysX.CapsuleShapeDescription capsshape = new StillDesign.PhysX.CapsuleShapeDescription(description.ShapeSize.X, description.ShapeSize.Z);
                                    capsshape.LocalRotation = Microsoft.Xna.Framework.Matrix.CreateRotationX(Microsoft.Xna.Framework.MathHelper.PiOver2).toPhysicM();
                                    ObjectActorDescription.Shapes.Add(capsshape);
                                }
                            }
                            else if (description.ShapeType == 1)
                            {
                                CollisionMesh physicCM = new CollisionMesh();
                                physicCM = packs.GetObject(description.RCCMName, physicCM) as CollisionMesh;

                                ObjectActorDescription.Shapes.Add(physicCM.CreatreConvexShape(scene.Core));
                            }


                            ObjectActorDescription.BodyDescription = new StillDesign.PhysX.BodyDescription(description.Mass);
                            Microsoft.Xna.Framework.Matrix MassCenterMatrix;
                            Microsoft.Xna.Framework.Matrix.CreateTranslation(ref description.CenterOfMass, out MassCenterMatrix);
                            ObjectActorDescription.BodyDescription.MassLocalPose = MassCenterMatrix.toPhysicM();

                            ObjectActor = scene.CreateActor(ObjectActorDescription);
                            ObjectActor.RaiseBodyFlag(StillDesign.PhysX.BodyFlag.FrozenRotation);
                            foreach (var c in ObjectActor.Shapes)
                            {
                                c.Group = 31;
                            }
                            behaviourmodel = new Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel(ObjectActor);
                        } break;
                    case LevelObjectDescription.objectstaticbehaviourmodel:
                        {
                            behaviourmodel = new Logic.BehaviourModel.ObjectStaticBehaviourModel();
                        } break;
                    case LevelObjectDescription.objectphysicbehaviourmodel:
                        {
                            StillDesign.PhysX.ActorDescription ObjectActorDescription = new StillDesign.PhysX.ActorDescription();
                            if (description.ShapeType == 0)
                            {
                                if (description.PhysXShapeType == 0)
                                {
                                    StillDesign.PhysX.BoxShapeDescription boxshape = new StillDesign.PhysX.BoxShapeDescription(description.ShapeSize.toPhysicV3());
                                    Microsoft.Xna.Framework.Matrix m;
                                    Microsoft.Xna.Framework.Vector3 v = description.ShapeRotationAxis;
                                    Microsoft.Xna.Framework.Matrix.CreateFromAxisAngle(ref v, description.ShapeRotationAngle, out m);
                                    boxshape.LocalRotation = m.toPhysicM();

                                    ObjectActorDescription.Shapes.Add(boxshape);
                                }
                                else if (description.PhysXShapeType == 1)
                                {
                                    StillDesign.PhysX.CapsuleShapeDescription capsshape = new StillDesign.PhysX.CapsuleShapeDescription(description.ShapeSize.X, description.ShapeSize.Z);
                                    Microsoft.Xna.Framework.Matrix m;
                                    Microsoft.Xna.Framework.Vector3 v = description.ShapeRotationAxis;
                                    Microsoft.Xna.Framework.Matrix.CreateFromAxisAngle(ref v, description.ShapeRotationAngle, out m);
                                    capsshape.LocalRotation = m.toPhysicM();

                                    ObjectActorDescription.Shapes.Add(capsshape);
                                }
                            }
                            else if (description.ShapeType == 1)
                            {
                                CollisionMesh physicCM = new CollisionMesh();
                                physicCM = packs.GetObject(description.RCCMName, physicCM) as CollisionMesh;


                                if (description.IsStatic)
                                    ObjectActorDescription.Shapes.Add(physicCM.CreateTriangleMeshShape(scene.Core));
                                else
                                    ObjectActorDescription.Shapes.Add(physicCM.CreatreConvexShape(scene.Core));
                            }

                            if (description.IsStatic)
                            {
                                ObjectActorDescription.BodyDescription = null;
                            }
                            else
                            {
                                ObjectActorDescription.BodyDescription = new StillDesign.PhysX.BodyDescription(description.Mass);
                                Microsoft.Xna.Framework.Matrix MassCenterMatrix;
                                Microsoft.Xna.Framework.Matrix.CreateTranslation(ref description.CenterOfMass, out MassCenterMatrix);
                                ObjectActorDescription.BodyDescription.MassLocalPose = MassCenterMatrix.toPhysicM();
                            }
                            ObjectActor = scene.CreateActor(ObjectActorDescription);
                            if (description.IsStatic)
                            {
                                foreach (var c in ObjectActor.Shapes)
                                {
                                    c.Group = 1;
                                }
                            }
                            else
                            {
                                foreach (var c in ObjectActor.Shapes)
                                {
                                    c.Group = 31;
                                }
                            }
                            behaviourmodel = new Logic.BehaviourModel.ObjectPhysicBehaviourModel(ObjectActor);
                            //CONTACT REPORT DISABLED TEMPORARY
                            //ObjectActor.ContactReportFlags = StillDesign.PhysX.ContactPairFlag.All;
                        } break;
                    case LevelObjectDescription.objectBonerelatedbehaviourmodel:
                        {
                            behaviourmodel = new Engine.Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel(currentCharacter, boneToAdd);
                        }break;
                    default:
                        {
                            throw new Exception("Unsupported behaviour model!");
                        } break;
                }
                #endregion
                //её гк удалит
                LevelObject createdobject = new LevelObject(behaviourmodel, renderaspect, material, raycastaspect);
                description.Enginereadedobject.Add(createdobject);
                return createdobject;
            }
            else
            {
                LevelObject createdobject = description.Enginereadedobject[0] as LevelObject;
                
                RenderObject ro = loadro(description, packs);
                Material material = loadMaterial(description.matname, packs);



                Logic.BehaviourModel.ObjectBehaviourModel behaviourmodel;
                #region behaviour
                StillDesign.PhysX.Actor ObjectActor = null;
                switch (description.BehaviourType)
                {
                    case LevelObjectDescription.objectmovingbehaviourmodel:
                        {
                            throw new Exception("Unsupported behaviour model!");
                        } break;
                    case LevelObjectDescription.objectphysiccharcontrollerbehaviourmodel:
                        {
                            StillDesign.PhysX.ActorDescription ObjectActorDescription = new StillDesign.PhysX.ActorDescription();

                            if (description.ShapeType == 0)
                            {
                                if (description.PhysXShapeType == 0)
                                {
                                    StillDesign.PhysX.BoxShapeDescription boxshape = new StillDesign.PhysX.BoxShapeDescription(description.ShapeSize.toPhysicV3());
                                    boxshape.LocalRotation = Microsoft.Xna.Framework.Matrix.CreateRotationX(Microsoft.Xna.Framework.MathHelper.PiOver2).toPhysicM();
                                    ObjectActorDescription.Shapes.Add(boxshape);
                                }
                                else if (description.PhysXShapeType == 1)
                                {
                                    StillDesign.PhysX.CapsuleShapeDescription capsshape = new StillDesign.PhysX.CapsuleShapeDescription(description.ShapeSize.X, description.ShapeSize.Z);
                                    capsshape.LocalRotation = Microsoft.Xna.Framework.Matrix.CreateRotationX(Microsoft.Xna.Framework.MathHelper.PiOver2).toPhysicM();
                                    ObjectActorDescription.Shapes.Add(capsshape);
                                }
                            }
                            else if (description.ShapeType == 1)
                            {
                                CollisionMesh physicCM = new CollisionMesh();
                                physicCM = packs.GetObject(description.RCCMName, physicCM) as CollisionMesh;

                                ObjectActorDescription.Shapes.Add(physicCM.CreatreConvexShape(scene.Core));
                            }


                            ObjectActorDescription.BodyDescription = new StillDesign.PhysX.BodyDescription(description.Mass);
                            Microsoft.Xna.Framework.Matrix MassCenterMatrix;
                            Microsoft.Xna.Framework.Matrix.CreateTranslation(ref description.CenterOfMass, out MassCenterMatrix);
                            ObjectActorDescription.BodyDescription.MassLocalPose = MassCenterMatrix.toPhysicM();
                            ObjectActorDescription.Shapes[0].Material = characterMaterial;
                            ObjectActor = scene.CreateActor(ObjectActorDescription);
                            

                            ObjectActor.RaiseBodyFlag(StillDesign.PhysX.BodyFlag.FrozenRotation);
                            foreach (var c in ObjectActor.Shapes)
                            {
                                c.Group = 31;
                            }
                            behaviourmodel = new Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel(ObjectActor);
                        } break;
                    case LevelObjectDescription.objectstaticbehaviourmodel:
                        {
                            behaviourmodel = new Logic.BehaviourModel.ObjectStaticBehaviourModel();
                        } break;
                    case LevelObjectDescription.objectphysicbehaviourmodel:
                        {
                            StillDesign.PhysX.ActorDescription ObjectActorDescription = new StillDesign.PhysX.ActorDescription();
                            if (description.ShapeType == 0)
                            {
                                if (description.PhysXShapeType == 0)
                                {
                                    StillDesign.PhysX.BoxShapeDescription boxshape = new StillDesign.PhysX.BoxShapeDescription(description.ShapeSize.toPhysicV3());
                                    Microsoft.Xna.Framework.Matrix m;
                                    Microsoft.Xna.Framework.Vector3 v = description.ShapeRotationAxis;
                                    Microsoft.Xna.Framework.Matrix.CreateFromAxisAngle(ref v, description.ShapeRotationAngle, out m);
                                    boxshape.LocalRotation = m.toPhysicM();

                                    ObjectActorDescription.Shapes.Add(boxshape);
                                }
                                else if (description.PhysXShapeType == 1)
                                {
                                    StillDesign.PhysX.CapsuleShapeDescription capsshape = new StillDesign.PhysX.CapsuleShapeDescription(description.ShapeSize.X, description.ShapeSize.Z);
                                    Microsoft.Xna.Framework.Matrix m;
                                    Microsoft.Xna.Framework.Vector3 v = description.ShapeRotationAxis;
                                    Microsoft.Xna.Framework.Matrix.CreateFromAxisAngle(ref v, description.ShapeRotationAngle, out m);
                                    capsshape.LocalRotation = m.toPhysicM();

                                    ObjectActorDescription.Shapes.Add(capsshape);
                                }
                            }
                            else if (description.ShapeType == 1)
                            {
                                CollisionMesh physicCM = new CollisionMesh();
                                physicCM = packs.GetObject(description.RCCMName, physicCM) as CollisionMesh;


                                if (description.IsStatic)
                                    ObjectActorDescription.Shapes.Add(physicCM.CreateTriangleMeshShape(scene.Core));
                                else
                                    ObjectActorDescription.Shapes.Add(physicCM.CreatreConvexShape(scene.Core));
                            }

                            if (description.IsStatic)
                            {
                                ObjectActorDescription.BodyDescription = null;
                            }
                            else
                            {
                                ObjectActorDescription.BodyDescription = new StillDesign.PhysX.BodyDescription(description.Mass);
                                Microsoft.Xna.Framework.Matrix MassCenterMatrix;
                                Microsoft.Xna.Framework.Matrix.CreateTranslation(ref description.CenterOfMass, out MassCenterMatrix);
                                ObjectActorDescription.BodyDescription.MassLocalPose = MassCenterMatrix.toPhysicM();
                            }
                            ObjectActor = scene.CreateActor(ObjectActorDescription);
                            if (description.IsStatic)
                            {
                                foreach (var c in ObjectActor.Shapes)
                                {
                                    c.Group = 1;
                                }
                            }
                            else
                            {
                                foreach (var c in ObjectActor.Shapes)
                                {
                                    c.Group = 31;
                                }
                            }
                            behaviourmodel = new Logic.BehaviourModel.ObjectPhysicBehaviourModel(ObjectActor);
                            //CONTACT REPORT DISABLED TEMPORARY
                            //ObjectActor.ContactReportFlags = StillDesign.PhysX.ContactPairFlag.All;
                        } break;
                    case LevelObjectDescription.objectBonerelatedbehaviourmodel:
                        {
                            behaviourmodel = new Engine.Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel(currentCharacter, boneToAdd);
                        } break;
                    default:
                        {
                            throw new Exception("Unsupported behaviour model!");
                        } break;
                }
                #endregion
                

                RaycastBoundObject raycastaspect = loadrcbo(description, packs);

                LevelObject createdobject1 = new LevelObject(behaviourmodel, ro, material, raycastaspect);
                description.Enginereadedobject.Add(createdobject1);
                return createdobject1;
            }
        }

        public static void UnloadPivotObject(
            PivotObject theobject)
        {
            LevelObject gobject = theobject as LevelObject;
            if (gobject == null)
                return;
            Pack p = null;
           PackContent pc = PackList.Instance.FindObject(theobject.editorAspect.DescriptionName, ref p);

            Content.LevelObjectDescription description = pc as Content.LevelObjectDescription;

            if (description != null)
            {
                description.Enginereadedobject.RemoveAt(description.Enginereadedobject.Count - 1);

                //unload ro
                Content.RenderObjectDescription rod = PackList.Instance.FindObject(description.RODName, ref p) as Content.RenderObjectDescription;
                RenderObject obj = rod.Enginereadedobject[rod.Enginereadedobject.Count - 1] as RenderObject;
                rod.Enginereadedobject.RemoveAt(rod.Enginereadedobject.Count - 1);
                if (rod.Enginereadedobject.Count == 0)
                {
                    IDisposable i = obj;
                    i.Dispose();
                }
                if (description.IsAnimated)
                {
                    Content.CharacterContent characterContent = PackList.Instance.FindObject(description.CharacterName, ref p) as Content.CharacterContent;
                    characterContent.Enginereadedobject.RemoveAt(characterContent.Enginereadedobject.Count - 1);
                }


                //unload material
                Content.MaterialDescription matd = PackList.Instance.FindObject(description.matname, ref p) as Content.MaterialDescription;

                matd.Enginereadedobject.RemoveAt(matd.Enginereadedobject.Count - 1);

                for (int i = 0; i < matd.lodMats.Length; i++)
                {
                    for (int j = 0; j < matd.lodMats[i].mats.Length; j++)
                    {
                        Content.Texture inage = PackList.Instance.FindObject(matd.lodMats[i].mats[j].DiffuseTextureName, ref p) as Content.Texture;
                        Texture2D tex = inage.Enginereadedobject[inage.Enginereadedobject.Count - 1] as Texture2D;
                        inage.Enginereadedobject.RemoveAt(inage.Enginereadedobject.Count - 1);
                        if (inage.Enginereadedobject.Count == 0)
                        {
                            tex.Dispose();
                        }
                    }
                }

                if (description.IsRCCMEnabled)
                {
                    //unload raycast
                    Content.CollisionMesh cm = PackList.Instance.FindObject(description.RCCMName, ref p) as Content.CollisionMesh;
                    cm.Enginereadedobject.RemoveAt(cm.Enginereadedobject.Count - 1);
                }
            }
        }


    }
}
