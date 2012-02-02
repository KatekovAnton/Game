using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.TheGame.Level
{
    public class GameLevelDescription
    {
        public class ObjectElement
        {
            public uint id;
            public uint group_id;
            public uint type;
            public string descriptionName;
            public Matrix objectMatrix;
            //сюда надо добавить логическую инфу - мб это моб? 
            //а может левел_геометри? а мб источник света/звука/..? 

            public ObjectElement()
            { }

            public ObjectElement(uint id, uint group_id, uint type, string descriptionName, Matrix objectMatrix)
            {
                this.id = id; ;
                this.group_id = group_id;
                this.type = type;
                this.descriptionName = descriptionName;
                this.objectMatrix = objectMatrix;
            }

            public ObjectElement(System.IO.BinaryReader br)
            {
                FromStream(br);
            }

            public void ToStream(System.IO.BinaryWriter bw)
            {
                bw.Write(id);
                bw.Write(group_id);
                bw.Write(type);
                bw.WritePackString(descriptionName);
                bw.WriteMatrixFull(objectMatrix);
            }

            public void FromStream(System.IO.BinaryReader br)
            {
                id = br.ReadUInt32();
                group_id = br.ReadUInt32();
                type = br.ReadUInt32();
                descriptionName = br.ReadPackString();
                objectMatrix = br.ReadMatrixFull();
            }
        }
        public class CameraInfo
        {
            public float _cameraPitch;
            public float _cameraYaw;
            public Matrix _view;

            public CameraInfo() { }

            public void toStream(System.IO.BinaryWriter bw)
            {
                bw.Write(_cameraPitch);
                bw.Write(_cameraYaw);
                bw.WriteMatrixFull(_view);
            }

            public void fromStream(System.IO.BinaryReader br)
            {
                _cameraPitch = br.ReadSingle();
                _cameraYaw = br.ReadSingle();
                _view = br.ReadMatrixFull();
            }
        }
        public MyContainer<ObjectElement> objectInformation;
        public CameraInfo camera;

        //сюда надо ещё добавить кучу ины по самому уровню -
        //уровень гравитации, ид персонажа игрока(чтоб знать к чему камеру крепить)
        //и прочее прочее прочее

        public uint generator;
        public GameLevelDescription()
        {
            objectInformation = new MyContainer<ObjectElement>(100, 2);
        }

        public void AddNewObject(uint id, uint group_id, uint type, string descriptionName, Matrix objectMatrix)
        {
            objectInformation.Add(new ObjectElement(id, group_id, type, descriptionName, objectMatrix));
        }

        public void DeleteObject(int id)
        {
            for (int i = 0; i < objectInformation.Count; i++)
                if (objectInformation[i].id == id)
                {
                    objectInformation.RemoveAt(i);
                    return;
                }
        }

        #region PackContent methods
        public int loadbody(System.IO.BinaryReader br)
        {
            long pos = br.BaseStream.Position;
            generator = br.ReadUInt32();
            int elementcount = br.ReadInt32();
            //objectInformation = new ObjectElement[br.ReadInt32()];
            for (int i = 0; i < elementcount; i++)
            {
                objectInformation.Add(new ObjectElement(br));
            }
            camera = new CameraInfo();
            camera.fromStream(br);
            return Convert.ToInt32(br.BaseStream.Position - pos);
        }

        public void savebody(System.IO.BinaryWriter bw)
        {
            bw.Write(generator);
            bw.Write(objectInformation.Count);
            for (int i = 0; i < objectInformation.Count; i++)
            {
                objectInformation[i].ToStream(bw);
            }
            camera.toStream(bw);
        }

        public void calcbodysize()
        {
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.MemoryStream());
            bw.Write(generator);
            bw.Write(objectInformation.Count);
            for (int i = 0; i < objectInformation.Count; i++)
            {
                objectInformation[i].ToStream(bw);
            }
            camera.toStream(bw);

        }

        
        #endregion
    }
}
