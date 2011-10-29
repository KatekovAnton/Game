using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysX_test2.BaseExtensions.Graph;

namespace PhysX_test2.TheGame.Objects.StateGraphs
{
    public class CharacterGraphNode:ObjectGraphNode
    {
        /// <summary>
        /// can it to receive controller events - move, rotate, etc
        /// </summary>
        public bool _canReceiveControl;

        /// <summary>
        /// is it alive and can it to to carry out of a problem (if dead - cannot)
        /// if true - enemies will attack him
        /// </summary>
        public bool _isOperable;

        public override void OnActivate(IGraphUser __object)
        {
            base.OnActivate(__object);
        }

        public override void OnDeactivate(IGraphUser __object)
        {
            base.OnDeactivate(__object);
        }

        public override void OnUpdate(IGraphUser __object)
        {
            base.OnUpdate(__object);
        }
    }
}
