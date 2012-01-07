using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PhysX_test2.ContentNew;
using PhysX_test2.Engine;
using PhysX_test2.Engine.Logic;
using PhysX_test2.Engine.Render;
using PhysX_test2.Engine.Render.Materials;
using PhysX_test2.Engine.Animation;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.ContentLoader
{
    public abstract class NewContentLoader
    {
        private static StillDesign.PhysX.Material characterMaterial;
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
                            physicCM = PackList.Instance.GetObject(description.RCCMName, physicCM)._contentObject as CollisionMesh;

                            ObjectActorDescription.Shapes.Add(physicCM.CreatreConvexShape(scene.Core));
                        }

                        ObjectActorDescription.BodyDescription = new StillDesign.PhysX.BodyDescription(description.Mass);
                        Microsoft.Xna.Framework.Matrix MassCenterMatrix;
                        Microsoft.Xna.Framework.Matrix.CreateTranslation(ref description.CenterOfMass, out MassCenterMatrix);
                        ObjectActorDescription.BodyDescription.MassLocalPose = MassCenterMatrix.toPhysicM();
                        ObjectActorDescription.Shapes[0].Material = characterMaterial;
                        ObjectActor = scene.CreateActor(ObjectActorDescription);

                        ObjectActor.RaiseBodyFlag(StillDesign.PhysX.BodyFlag.FrozenRotation);
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
                            physicCM = PackList.Instance.GetObject(description.RCCMName, physicCM)._contentObject as CollisionMesh;

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

        private static ContentNew.Texture LoadTexture(string name, PackList packs)
        {
            ContentNew.Texture tex = new ContentNew.Texture();
            PackContentHeader pch = packs.GetObject(name, tex);
            pch.Retain(tex);
            return tex;
        }

        private static Material loadMaterial(string name, PackList packs)
        {
            PhysX_test2.ContentNew.MaterialDescription mat = new MaterialDescription();
            PackContentHeader mat_pch = packs.GetObject(name, mat);
            if (mat_pch._engineObject == null)
            {
                DiffuseMaterial.Lod[] lods = new DiffuseMaterial.Lod[mat.lodMats.Length];
                for (int i = 0; i < mat.lodMats.Length; i++)
                {
                    DiffuseMaterial.SubsetMaterial[] mats = new DiffuseMaterial.SubsetMaterial[mat.lodMats[i].mats.Length];
                    for (int j = 0; j < mat.lodMats[i].mats.Length; j++)
                    {
                        mats[j] = new DiffuseMaterial.SubsetMaterial();
                        mats[j].diffuseTexture = LoadTexture(mat.lodMats[i].mats[j].DiffuseTextureName, packs).texture;
                    }
                    lods[i] = new DiffuseMaterial.Lod(mats);

                }
                DiffuseMaterial result = new DiffuseMaterial(lods);
                mat_pch.Retain(result);

                return new DiffuseMaterial(lods);
            }
            else
            {
                DiffuseMaterial result = mat_pch._engineObject as DiffuseMaterial;
                mat_pch.Retain();

                for (int i = 0; i < mat.lodMats.Length; i++)
                    for (int j = 0; j < mat.lodMats[i].mats.Length; j++)
                        LoadTexture(mat.lodMats[i].mats[j].DiffuseTextureName, packs);
                   
                return result;
            }
        }

        private static ContentMesh[] loadMeshArray(string[] mesnames, PackList packs)
        {
            ContentMesh[] subsetmeshes = new ContentMesh[mesnames.Length];
            for (int i = 0; i < mesnames.Length; i++)
            {
                subsetmeshes[i] = new ContentMesh();
                subsetmeshes[i] = packs.GetObject(mesnames[i], subsetmeshes[i])._contentObject as ContentMesh;
            }
            return subsetmeshes;
        }

        private static ContentMeshSkinned[] loadSkinnedMeshArray(string[] mesnames, PackList packs, CharacterStatic character)
        {
            ContentMeshSkinned.skeleton = character;
            ContentMeshSkinned[] subsetmeshes = new ContentMeshSkinned[mesnames.Length];
            for (int i = 0; i < mesnames.Length; i++)
            {
                subsetmeshes[i] = new ContentMeshSkinned();
                subsetmeshes[i] = packs.GetObject(mesnames[i], subsetmeshes[i])._contentObject as ContentMeshSkinned;
            }
            return subsetmeshes;
        }

        public static Engine.Animation.CharacterController createCharacter(PackList packs, string name)
        {
            ContentNew.CharacterBase characterContent = new CharacterBase();
            PackContentHeader pch = packs.GetObject(name, characterContent);
            CharacterStatic character = null;
            if (pch._engineObject == null)
            {
                character = new CharacterStatic();
                character.CreateFromContentEntity(new ContentNew.IPackContentEntity[] { characterContent });
                pch._contentObject = characterContent;
                pch.Retain(character);
            }
            else
            {
                character = pch._engineObject as CharacterStatic;
                pch.Retain();
            }

            CharacterController result = new CharacterController(character, new string[] { "stay1\0", "stay1\0" });
            return result;
        }

        private static RenderObject loadro(LevelObjectDescription description,
           PackList packs)
        {
            RenderObjectDescription rod = new RenderObjectDescription();
            PackContentHeader pch = packs.GetObject(description.RODName, rod);

            if (pch._engineObject == null)
            {
                if (description.IsAnimated)
                {
                    Engine.Animation.CharacterController character = createCharacter(packs, description.CharacterName);

                    AnimRenderObject.Model[] models = new AnimRenderObject.Model[rod.LODs.Count];
                    for (int i = 0; i < models.Length; i++)
                    {
                        AnimRenderObject.SubSet[] modelsubsets = new AnimRenderObject.SubSet[rod.LODs[i].subsets.Count];
                        for (int j = 0; j < modelsubsets.Length; j++)
                        {
                            ContentMeshSkinned[] subsetmeshes = loadSkinnedMeshArray(rod.LODs[i].subsets[j].MeshNames, packs, character._baseCharacter);
                            EngineMeshSkinned subsetmesh = new EngineMeshSkinned();
                            subsetmesh.CreateFromContentEntity(subsetmeshes);

                            //меши могут быть по-разному сгруппированы поэтому будем их каждый раз по новой загружать
                            //TODO LOAD
                            //modelsubsets[j] = new AnimRenderObject.SubSet(subsetmesh);
                        }
                        models[i] = new AnimRenderObject.Model(modelsubsets);
                    }
                    RenderObject result = new AnimRenderObject(character, models, rod.IsShadowCaster, rod.IsShadowReceiver);
                    pch.Retain(result);
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
                            IPackContentEntity[] subsetmeshes = loadMeshArray(rod.LODs[i].subsets[j].MeshNames, packs);
                            EngineMesh subsetmesh = new EngineMesh();
                            subsetmesh.CreateFromContentEntity(subsetmeshes);

                            //меши могут быть по-разному сгруппированы поэтому будем их каждый раз по новой загружать
                            //TODO LOAD
                            //modelsubsets[j] = new SubSet(subsetmesh);
                        }
                        models[i] = new MyModel(modelsubsets);
                    }
                    RenderObject result = new UnAnimRenderObject(models, rod.IsShadowCaster, rod.IsShadowReceiver);
                    pch.Retain(result);
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
                    AnimRenderObject res = pch._engineObject as AnimRenderObject;
                    Engine.Animation.CharacterController character = createCharacter(packs, description.CharacterName);
                    result = new AnimRenderObject(character, res.LODs, rod.IsShadowCaster, rod.IsShadowReceiver);
                }
                else
                {
                    UnAnimRenderObject res = pch._engineObject as UnAnimRenderObject;
                    result = new UnAnimRenderObject(res.LODs, rod.IsShadowCaster, rod.IsShadowReceiver);
                    result.isSelfIllumination = rod.isSelfIllumination;
                    result.isTransparent = rod.isTransparent;
                }
                if (result.isanimated != description.IsAnimated)
                    throw new Exception("wrong object animation flag!!!");
                pch.Retain();
                return result;
            }
        }

        private static RaycastBoundObject loadrcbo(LevelObjectDescription description,
            PackList packs)
        {
            //клижмеш мб один на несколько объектов//его прекрасно удалит сам гк если что
            EngineCollisionMesh ObjectRCCM = null;
            //бш уникален для каждого //его тож гк удалит
            Logic.SceneGraph.OTBoundingShape bs = null;
            if (description.IsRCCMEnabled)
            {
                CollisionMesh cm = new CollisionMesh();
                PackContentHeader cm_pch = packs.GetObject(description.RCCMName, cm);
                if (cm_pch._engineObject == null)
                    ObjectRCCM = EngineCollisionMesh.FromcontentCollisionMesh(cm);
                else
                    ObjectRCCM = cm_pch._engineObject as EngineCollisionMesh;

                cm_pch.Retain(ObjectRCCM);


                //TODO LOAD
                //bs = new Logic.SceneGraph.OTBoundingShape(ObjectRCCM);
            }
            else
                bs = new Logic.SceneGraph.OTBoundingShape(description.RCShapeSize);

            //его тоже удалят
            //TODO LOAD
            //RaycastBoundObject raycastaspect = new RaycastBoundObject(bs, ObjectRCCM);
            //return raycastaspect;
            return null;
        }

        public static LevelObject LevelObjectFromDescription(
            ContentNew.PackContentHeader header,
           ContentNew.LevelObjectDescription description,
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

            if (header._engineObject == null)
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


                Logic.BehaviourModel.ObjectBehaviourModel behaviourmodel = createBehaviourModel(desc, scene, __dependType);

                //её гк удалит
                LevelObject createdobject = new LevelObject(behaviourmodel, renderaspect, material, raycastaspect);
                createdobject.bmDescription = desc;
                header.Retain(createdobject);
                createdobject.editorAspect = new EditorData(header.name, header.loadedformat);
                return createdobject;
            }
            else
            {
                LevelObject createdobject = header._engineObject as LevelObject;

                RenderObject ro = loadro(description, packs);
                Material material = loadMaterial(description.matname, packs);

                Logic.BehaviourModel.BehaviourModelDescription desc = createdobject.bmDescription;
                Logic.BehaviourModel.ObjectBehaviourModel behaviourmodel = createBehaviourModel(desc, scene, __dependType);

                RaycastBoundObject raycastaspect = loadrcbo(description, packs);

                LevelObject createdobject1 = new LevelObject(behaviourmodel, ro, material, raycastaspect);
                header.Retain();
                createdobject1.bmDescription = desc;
                createdobject1.editorAspect = new EditorData(header.name, header.loadedformat);
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
            PackContentHeader pc = PackList.Instance.FindObject(theobject.editorAspect.DescriptionName, ref p);

            LevelObjectDescription description = pc._contentObject as LevelObjectDescription;


            //unload ro
            PackContentHeader pc_rod = PackList.Instance.FindObject(description.RODName, ref p);
            pc_rod.Release();


            if (description.IsAnimated)
            {
                PackContentHeader pc_characterContent = PackList.Instance.FindObject(description.CharacterName, ref p);
                pc_characterContent.Release();
            }

            //unload material
            PackContentHeader pc_mat = PackList.Instance.FindObject(description.matname, ref p);
            MaterialDescription matd = pc_mat._contentObject as MaterialDescription;
            pc_mat.Release();

            for (int i = 0; i < matd.lodMats.Length; i++)
                for (int j = 0; j < matd.lodMats[i].mats.Length; j++)
                {
                    PackContentHeader pc_image = PackList.Instance.FindObject(matd.lodMats[i].mats[j].DiffuseTextureName, ref p);
                    pc_image.Release();
                }
            

            if (description.IsRCCMEnabled)
            {
                //unload raycast
                PackContentHeader pc_cm = PackList.Instance.FindObject(description.RCCMName, ref p);
                pc_cm.Release();
            }
            theobject._unloaded = true;
        }
    }
    
}
