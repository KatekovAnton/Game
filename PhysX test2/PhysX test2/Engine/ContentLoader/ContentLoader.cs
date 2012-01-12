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
      //  public static PivotObject currentParentObject;
      //  public static int boneToAdd;

        public static Logic.BehaviourModel.ObjectBehaviourModel createBehaviourModel(
            Logic.BehaviourModel.BehaviourModelDescription description,
            StillDesign.PhysX.Scene scene,
            PivotObjectDependType __dependType)
        {
            Logic.BehaviourModel.ObjectBehaviourModel behaviourmodel;
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
                            physicCM = PackList.Instance.GetObject(description.RCCMName, physicCM) as CollisionMesh;

                            ObjectActorDescription.Shapes.Add(physicCM.CreatreConvexShape(scene.Core));
                        }

                        ObjectActorDescription.BodyDescription = new StillDesign.PhysX.BodyDescription(description.Mass);
                        Microsoft.Xna.Framework.Matrix MassCenterMatrix;
                        Microsoft.Xna.Framework.Matrix.CreateTranslation(ref description.CenterOfMass, out MassCenterMatrix);
                        ObjectActorDescription.BodyDescription.MassLocalPose = MassCenterMatrix.toPhysicM();
                        ObjectActorDescription.Shapes[0].Material = characterMaterial;
                        ObjectActor = scene.CreateActor(ObjectActorDescription);

                        ObjectActor.RaiseBodyFlag(StillDesign.PhysX.BodyFlag.FrozenRotation);
                      /*  foreach (var c in ObjectActor.Shapes)
                        {
                            c.Group = 31;
                        }*/
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

                            physicCM = PackList.Instance.GetObject(description.RCCMName, physicCM) as CollisionMesh;


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
                       /* if (description.IsStatic)
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
                        }*/
                        behaviourmodel = new Logic.BehaviourModel.ObjectPhysicBehaviourModel(ObjectActor);
                        //CONTACT REPORT DISABLED TEMPORARY
                        //ObjectActor.ContactReportFlags = StillDesign.PhysX.ContactPairFlag.All;
                    } break;
                case LevelObjectDescription.objectBonerelatedbehaviourmodel:
                    {
                        Engine.Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel _behaviourmodel = new Engine.Logic.BehaviourModel.ObjectBoneRelatedBehaviourModel();
                        _behaviourmodel._dependType = __dependType;
                        behaviourmodel = _behaviourmodel;
                    } break;
                case LevelObjectDescription.objectRelatedBehaviourModel:
                    {
                        behaviourmodel = new Engine.Logic.BehaviourModel.ObjectRelatedBehaviourModel();
                    } break;
                default:
                    {
                        throw new Exception("Unsupported behaviour model!");
                    } break;
            }
            behaviourmodel.Disable();
            return behaviourmodel;
        }

        private static Material loadMaterial(string name, PackList packs)
        {
            PhysX_test2.Content.MaterialDescription mat = new MaterialDescription();
            mat = packs.GetObject(name , mat) as  MaterialDescription;
            if (mat._userCount == 0)
            {
                DiffuseMaterial.Lod[] lods = new DiffuseMaterial.Lod[mat.lodMats.Length];
                for (int i = 0; i < mat.lodMats.Length; i++)
                {
                    DiffuseMaterial.SubsetMaterial[] mats = new DiffuseMaterial.SubsetMaterial[mat.lodMats[i].mats.Length];
                    for (int j = 0; j < mat.lodMats[i].mats.Length; j++)
                    {
                        mats[j] = new DiffuseMaterial.SubsetMaterial();
                        Content.Texture inage = new Content.Texture();
                        inage = packs.GetObject(mat.lodMats[i].mats[j].DiffuseTextureName, inage) as Content.Texture;



                        inage.Retain(inage.texture);
                        mats[j].diffuseTexture = inage.texture;
                    }
                    lods[i] = new DiffuseMaterial.Lod(mats);
                    
                }
                DiffuseMaterial result = new DiffuseMaterial(lods);
                mat.Retain(result);

                return new DiffuseMaterial(lods);
            }
            else
            {
                DiffuseMaterial result = mat._engineSampleObject as DiffuseMaterial;
                mat.Retain();

                for (int i = 0; i < mat.lodMats.Length; i++)
                {
                    for (int j = 0; j < mat.lodMats[i].mats.Length; j++)
                    {
                        Content.Texture inage = new Content.Texture();
                        inage = packs.GetObject(mat.lodMats[i].mats[j].DiffuseTextureName, inage) as Content.Texture;
                        inage.Retain();
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

        public static Engine.Animation.CharacterController createCharacter(PackList packs, string name, out CharacterContent characterContent)
        {
            characterContent = new Content.CharacterContent();
            characterContent = packs.GetObject(name, characterContent) as Content.CharacterContent;
            CharacterStatic characterResult = null;
            if (characterContent._userCount == 0)
            {
                characterResult = new CharacterStatic();
                characterResult.CreateFromContentEntity(new PackContent[] { characterContent });
                characterContent.Retain(characterResult);
            }
            else
            {
                characterResult = characterContent._engineSampleObject as CharacterStatic;
                characterContent.Retain();
            }
            CharacterController result = new CharacterController(characterResult, new string[] { "stay1\0", "stay1\0" });
            return result;
        }

        private static RenderObject loadro(LevelObjectDescription description,
           PackList packs)
        {
            RenderObjectDescription rod = new RenderObjectDescription();

            rod = packs.GetObject(description.RODName, rod) as Content.RenderObjectDescription;
            if (rod._userCount == 0)
            {
                if (description.IsAnimated)
                {
                    CharacterContent characterContent = null;
                    Engine.Animation.CharacterController character = createCharacter(packs, description.CharacterName, out characterContent);

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
                    rod.Retain(result);
                    return result;
                }
                else
                {
                    MyModel[] models = new MyModel[rod.LODs.Count];
                    for (int i = 0; i < models.Length; i++)
                    {
                        SubSet[] modelsubsets = new SubSet[rod.LODs[i].subsets.Count];
                        for (int j = 0; j < modelsubsets.Length; j++)
                        {
                            ContentMesh[] subsetmeshes = loadMeshArray(rod.LODs[i].subsets[j].MeshNames, packs);
                            EngineMesh subsetmesh = EngineMesh.FromContentMeshes(subsetmeshes);
                            //меши могут быть по-разному сгруппированы поэтому будем их каждый раз по новой загружать
                            modelsubsets[j] = new SubSet(subsetmesh);
                        }
                        models[i] = new MyModel(modelsubsets);
                    }
                    RenderObject result = new UnAnimRenderObject(models, rod.IsShadowCaster, rod.IsShadowReceiver);
                    rod.Retain(result);
                    result.isSelfIllumination = rod.isSelfIllumination;
                    result.isTransparent = rod.isTransparent;
                    return result;
                }
            }
            else
            {
                RenderObject result;
                if (description.IsAnimated)
                {
                    AnimRenderObject res = rod._engineSampleObject as AnimRenderObject;
                    CharacterContent characterContent = null; ;
                    Engine.Animation.CharacterController character = createCharacter(packs, description.CharacterName, out characterContent);
                    result = new AnimRenderObject(character, res.LODs, rod.IsShadowCaster, rod.IsShadowReceiver);
                }
                else
                {
                    UnAnimRenderObject res = rod._engineSampleObject as UnAnimRenderObject;
                    result = new UnAnimRenderObject(res.LODs, rod.IsShadowCaster, rod.IsShadowReceiver);
                    result.isSelfIllumination = rod.isSelfIllumination;
                    result.isTransparent = rod.isTransparent;
                }
                if (result.isanimated != description.IsAnimated)
                    throw new Exception("wrong object animation flag!!!");
                rod.Retain();
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
                if (cm._userCount == 0)
                    ObjectRCCM = EngineCollisionMesh.FromcontentCollisionMesh(cm as CollisionMesh);
                else
                    ObjectRCCM = cm._engineSampleObject as EngineCollisionMesh;

                cm.Retain(ObjectRCCM);

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
            StillDesign.PhysX.Scene scene,
            PivotObjectDependType __dependType)
        {


            if (characterMaterial == null)
            {
                StillDesign.PhysX.MaterialDescription md = new StillDesign.PhysX.MaterialDescription();
                md.SetToDefault();
                md.Restitution = 0.05f;
                characterMaterial = scene.CreateMaterial(md);
            }

            if (description._userCount == 0)
            {
                //рендераспект - мб один на несколько объектов
                RenderObject renderaspect = loadro(description, packs);
                Material material = loadMaterial(description.matname, packs);
                //его тоже удалят
                RaycastBoundObject raycastaspect = loadrcbo(description, packs);
                Logic.BehaviourModel.BehaviourModelDescription desc = new Logic.BehaviourModel.BehaviourModelDescription();
                desc.BehaviourType = description.BehaviourType;
                desc.CenterOfMass = description.CenterOfMass;
                desc.IsStatic = description.IsStatic;
                desc.Mass = description.Mass;
                desc.PhysXShapeType = description.PhysXShapeType;
                desc.RCCMName = description.RCCMName;
                desc.ShapeRotationAngle = description.ShapeRotationAngle;
                desc.ShapeRotationAxis = description.ShapeRotationAxis;
                desc.ShapeSize = description.ShapeSize;
                desc.ShapeType = description.ShapeType;


                Logic.BehaviourModel.ObjectBehaviourModel behaviourmodel = createBehaviourModel(desc, scene,__dependType); 
              
                //её гк удалит
                LevelObject createdobject = new LevelObject(behaviourmodel, renderaspect, material, raycastaspect);
                createdobject.bmDescription = desc;
                description.Retain(createdobject);
                createdobject.editorAspect = new EditorData(description.name, description.loadedformat);
                return createdobject;
            }
            else
            {
                LevelObject createdobject = description._engineSampleObject as LevelObject;
                
                RenderObject ro = loadro(description, packs);
                Material material = loadMaterial(description.matname, packs);

                Logic.BehaviourModel.BehaviourModelDescription desc = createdobject.bmDescription;
                Logic.BehaviourModel.ObjectBehaviourModel behaviourmodel = createBehaviourModel(desc, scene,__dependType); 
                
                RaycastBoundObject raycastaspect = loadrcbo(description, packs);

                LevelObject createdobject1 = new LevelObject(behaviourmodel, ro, material, raycastaspect);
                description.Retain();
                createdobject1.bmDescription = desc;
                createdobject1.editorAspect = new EditorData(description.name, description.loadedformat);
                return createdobject1;
            }
        }

        public static void UnloadPivotObject(
            PivotObject theobject)
        {
            if (theobject._unloaded)
                return;
            LevelObject gobject = theobject as LevelObject;
            if (gobject == null)
                return;
            Pack p = null;
            PackContent pc = PackList.Instance.FindObject(theobject.editorAspect.DescriptionName, ref p);

            Content.LevelObjectDescription description = pc.ReadedContentObject as Content.LevelObjectDescription;

            if (description != null)
            {

                description.Release();
                if (description._userCount == 0)
                {
                    pc.objectReaded = false;
                    pc.ReadedContentObject = null;
                }
                //unload ro
                PackContent pc_rod = PackList.Instance.FindObject(description.RODName, ref p);
                Content.RenderObjectDescription rod = pc_rod.ReadedContentObject as Content.RenderObjectDescription;
                RenderObject obj = rod._engineSampleObject as RenderObject;
                rod.Release();
                if (rod._userCount == 0)
                {
                    pc_rod.objectReaded = false;
                    pc_rod.ReadedContentObject = null;
                }

                //unload character
                if (description.IsAnimated)
                {
                    PackContent pc_characterContent = PackList.Instance.FindObject(description.CharacterName, ref p);
                    Content.CharacterContent characterContent = pc_characterContent.ReadedContentObject as Content.CharacterContent;
                    characterContent.Release();
                    if (characterContent._userCount == 0)
                    {
                        pc_characterContent.objectReaded = false;
                        pc_characterContent.ReadedContentObject = null;
                    }
                }

                //unload material
                PackContent pc_mat = PackList.Instance.FindObject(description.matname, ref p);
                Content.MaterialDescription matd = pc_mat.ReadedContentObject as Content.MaterialDescription;
                matd.Release();
                if (matd._userCount == 0)
                {
                    pc_mat.objectReaded = false;
                    pc_mat.ReadedContentObject = null;
                }

                for (int i = 0; i < matd.lodMats.Length; i++)
                {
                    for (int j = 0; j < matd.lodMats[i].mats.Length; j++)
                    {
                        PackContent pc_image =  PackList.Instance.FindObject(matd.lodMats[i].mats[j].DiffuseTextureName, ref p);
                        Content.Texture inage = pc_image.ReadedContentObject as Content.Texture;
                        inage.Release();
                        if (inage._userCount == 0)
                        {
                            pc_image.objectReaded = false;
                            pc_image.ReadedContentObject = null;
                        }
                    }
                }

                if (description.IsRCCMEnabled)
                {
                    //unload raycast
                    PackContent pc_cm = PackList.Instance.FindObject(description.RCCMName, ref p);
                    Content.CollisionMesh cm = pc_cm.ReadedContentObject as Content.CollisionMesh;
                    cm.Release();
                    if (cm._userCount == 0)
                    {
                        pc_cm.objectReaded = false;
                        pc_cm.ReadedContentObject = null;
                    }
                }


                //unload behaviourmodel
                gobject.behaviourmodel.Disable();
                switch (description.BehaviourType)
                {
                    case LevelObjectDescription.objectmovingbehaviourmodel:
                        {
                            throw new Exception("Unsupported behaviour model!");
                        } break;
                    case LevelObjectDescription.objectphysiccharcontrollerbehaviourmodel:
                        {
                            Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel cm = gobject.behaviourmodel as Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel;
                            cm._actor.Dispose();
                        } break;
                    case LevelObjectDescription.objectstaticbehaviourmodel:
                        {
                            //nothing; gc 
                        } break;
                    case LevelObjectDescription.objectphysicbehaviourmodel:
                        {
                            Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel cm = gobject.behaviourmodel as Logic.BehaviourModel.ObjectPhysicControllerBehaviourModel;
                            cm._actor.Dispose();
                        } break;
                    case LevelObjectDescription.objectBonerelatedbehaviourmodel:
                        {
                            //nothing; gc
                        } break;
                    case LevelObjectDescription.objectRelatedBehaviourModel:
                        {
                            //nothing; gc
                        } break;
                    default:
                        {
                            throw new Exception("Unsupported behaviour model!");
                        } break;
                }
                gobject.behaviourmodel = null;
                theobject._unloaded = true;
            }
        }
    }
}
