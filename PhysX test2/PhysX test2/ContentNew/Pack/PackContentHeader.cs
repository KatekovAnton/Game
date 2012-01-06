using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace PhysX_test2.ContentNew
{
    public class PackContentHeader
    {
        public int _userCount;
        public IPackEngineObject _engineObject;
        public IPackContentEntity _contentObject;



        public int number;
        public int offset;
        public int size;
        public int headersize;
        public int loadedformat;
        public string name;
        public MeshContentadditionalheader mh = null;


        public int _disposeCount = 0;

        public PackContentHeader()
        {
            _userCount = 0;
        }

        public PackContentHeader(System.IO.BinaryReader br, int _number)//16+имя
        {
            this.number = _number;
            int length = br.ReadInt32();
            name = new string(br.ReadChars(length + 1));
            offset = br.ReadInt32();
            loadedformat = br.ReadInt32();
            headersize = br.ReadInt32();

            //когдато я бы л молодой и глупый и теперь пишу этот кастыль
            if (loadedformat == ElementType.MeshOptimazedForStore || loadedformat == ElementType.MeshOptimazedForLoading)
            {
                mh = new MeshContentadditionalheader();
                mh.load(br, loadedformat);
            }
            else
            {
                size = br.ReadInt32();
            }
        }

        public void Retain(IPackEngineObject __newObject = null)
        {
            //TODO - check logic
            if (__newObject == null && _engineObject == null)
                throw new Exception();
            _userCount++;
            if (__newObject == null)
                return;
            if (_engineObject == null)
                _engineObject = __newObject;
        }

        public void Release()
        {
            _userCount--;
            if (_userCount == 0)
            {
                _engineObject.Dispose();
                _engineObject = null;

                _disposeCount++;
            }
        }

        public virtual void Unload()
        {
            _contentObject = null;
        }
    }

    /// <summary>
    /// Interface for content objects
    /// </summary>
    public interface IPackContentEntity
    {
        void LoadBody(byte[] __data);
        int GetContentType();
    }

    /// <summary>
    /// Interface for engine objects
    /// </summary>
    public interface IPackEngineObject : IDisposable
    {
        void CreateFromContentEntity(IPackContentEntity[] __contentEntities);
       // void CreateFormEngineObject(IPackEngineObject __object);
    }
}
