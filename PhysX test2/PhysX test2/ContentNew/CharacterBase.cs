using System;
using System.Collections.Generic;
using System.Linq;
using BoneMap = System.Collections.Generic.Dictionary<string, int>;
using System.IO;

using Microsoft.Xna.Framework;


namespace PhysX_test2.ContentNew
{
    public class CharacterBase:PackContentEntity
    {
        public byte[] data;

        public void LoadBody(byte[] array)
        {
            data = array;
        }
    }
}
