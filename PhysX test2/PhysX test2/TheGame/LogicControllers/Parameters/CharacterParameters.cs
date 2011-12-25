using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class CharacterParameters : MotralParameters
    {
        public float _walkSpeed;

        public string _levelObjectName;
        public string _defaultDeadStartAnimName;
        public string _defaultDeadIdleAnimName;
        public string _defaultAliveAnimName;

        public string _headObjectName;
        public bool _isUniq;

        public CharacterParameters(int __dbID, 
                                    string __displayName,
                                    string __levelObjectName,
                                    string __defaultDeadStartAnimName,
                                    string __defaultDeadIdleAnimName,
                                    string __defaultAliveAnimName,
                                    string __headObjectName,
                                    bool __isUniq,
                                    float __speed)
            : base(__dbID, __displayName)
        {
            _walkSpeed = __speed;
            _levelObjectName = __levelObjectName;
            _defaultAliveAnimName = __defaultAliveAnimName;
            _defaultDeadIdleAnimName = __defaultDeadIdleAnimName;
            _defaultDeadStartAnimName = __defaultDeadStartAnimName;

            _headObjectName = __headObjectName;
            _isUniq = __isUniq;


            if (!_levelObjectName.EndsWith("\0"))
                _levelObjectName += "\0";
            if (!_defaultAliveAnimName.EndsWith("\0"))
                _defaultAliveAnimName += "\0";
            if (!_defaultDeadIdleAnimName.EndsWith("\0"))
                _defaultDeadIdleAnimName += "\0";
            if (!_defaultDeadStartAnimName.EndsWith("\0"))
                _defaultDeadStartAnimName += "\0";
            if (!_headObjectName.EndsWith("\0"))
                _headObjectName += "\0";
        }
    }
}
