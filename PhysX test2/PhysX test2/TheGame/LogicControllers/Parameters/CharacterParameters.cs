using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public class AnimationInfo
    {
        public int _weaponType;
        public string _stayTopAnim;
        public string _stayBotAnim;
        public string _fireAniml;
        public string _actionAnim;

        public AnimationInfo(int __type,
            string __sta,
            string __sba,
            string __fa,
            string __aa)
        {
            _fireAniml = __fa;
            if (!_fireAniml.EndsWith("\0"))
                _fireAniml += "\0";
            _stayTopAnim = __sta;
            if (!_stayTopAnim.EndsWith("\0"))
                _stayTopAnim += "\0";
            _stayBotAnim = __sba;
            if (!_stayBotAnim.EndsWith("\0"))
                _stayBotAnim += "\0";
            _weaponType = __type;
            _actionAnim = __aa;
            if (!_actionAnim.EndsWith("\0"))
                _actionAnim += "\0";
        }
    }


    public class CharacterParameters : MotralParameters
    {
        public float _walkSpeed;

        public string _levelObjectName;
        public string _defaultDeadStartAnimName;
        public string _defaultDeadIdleAnimName;
        public string _rifleStayAnimName;

        public string _headObjectName;
        public bool _isUniq;

        public Dictionary<int, AnimationInfo> _animations;

        public CharacterParameters(int __dbID, 
                                    string __displayName,
                                    string __levelObjectName,
                                    string __defaultDeadStartAnimName,
                                    string __defaultDeadIdleAnimName,
                                    string __rifleStayAnimName,
                                    string __headObjectName,
                                    bool __isUniq,
                                    float __speed,
                                    Dictionary<int, AnimationInfo> __anims)
            : base(__dbID, __displayName)
        {
            _animations = __anims;
            _walkSpeed = __speed;
            _levelObjectName = __levelObjectName;
            _rifleStayAnimName = __rifleStayAnimName;
            _defaultDeadIdleAnimName = __defaultDeadIdleAnimName;
            _defaultDeadStartAnimName = __defaultDeadStartAnimName;

            _headObjectName = __headObjectName;
            _isUniq = __isUniq;


            if (!_levelObjectName.EndsWith("\0"))
                _levelObjectName += "\0";
            if (!_rifleStayAnimName.EndsWith("\0"))
                _rifleStayAnimName += "\0";
            if (!_defaultDeadIdleAnimName.EndsWith("\0"))
                _defaultDeadIdleAnimName += "\0";
            if (!_defaultDeadStartAnimName.EndsWith("\0"))
                _defaultDeadStartAnimName += "\0";
            if (!_headObjectName.EndsWith("\0"))
                _headObjectName += "\0";
        }
    }

    public class CharacterDynamicParameters : DynamicParameters
    { }
}
