﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysX_test2.TheGame.LogicControllers.Parameters
{
    public enum EffectItemType
    {
        Object,
        Billboard,
        ConstrainedBillboard,
        Particles
    }

    public class EffectParameters : ControllerParameters
    {
        public class EffectItem
        {
            public double _lifeTime;
            public EffectItemType _type;
            public Dictionary<string, object> _parameters;

            public EffectItem()
            {
                _parameters = new Dictionary<string, object>();
            }
        }
        public double _lifeTime;
        public EffectItem[] _effectItems;

        public EffectParameters(int __id, string __name, object[] __data)
            : base(__id, __name)
        {
            _effectItems = new EffectItem[__data.Length];
            _lifeTime = 0;
            for (int i = 0; i < _effectItems.Length; i++)
            {
                object[] effectData = __data[i] as object[];
                EffectItem ei = new EffectItem();
                ei._lifeTime = Convert.ToDouble(effectData[1].ToString().Replace('.', ','));
                if (ei._lifeTime > _lifeTime)
                    _lifeTime = ei._lifeTime;
                string name = effectData[2].ToString();
                if (name.CompareTo("Object") == 0)
                {
                    ei._type = EffectItemType.Object;
                    ei._parameters.Add("objectName", effectData[3]);
                }
                else if (name.CompareTo("Billboard") == 0)
                {
                    ei._type = EffectItemType.Billboard;
                }
                else if (name.CompareTo("ConstrainedBillboard") == 0)
                {
                    ei._type = EffectItemType.ConstrainedBillboard;
                }
                else if (name.CompareTo("Particles") == 0)
                {
                    ei._type = EffectItemType.Particles;
                }

                _effectItems[i] = ei;
            }
        }
    }
}