using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2
{
    public class MyRandom
    {

        private static Random _randomze;
        public static Random Instance
        {
            get
            {
                
                if (_randomze == null)
                    _randomze = new Random(DateTime.Now.Millisecond);
                return _randomze;
            }
        }
    }
}
