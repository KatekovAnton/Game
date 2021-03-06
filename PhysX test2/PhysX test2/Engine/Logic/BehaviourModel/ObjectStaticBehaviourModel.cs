﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace PhysX_test2.Engine.Logic.BehaviourModel
{
    public class ObjectStaticBehaviourModel:ObjectBehaviourModel
    {
        public ObjectStaticBehaviourModel()
        {
            CurrentPosition = globalpose =  Matrix.Identity;
        }
  

        public override void SetGlobalPose(Matrix GlobalPoseMatrix, object Additionaldata, PivotObject __parent)
        {
            CurrentPosition = GlobalPoseMatrix;
        }
       
        public override void DoFrame(GameTime gametime)
        {
        }

        public override void Move(Vector3 displacement)
        {
            Matrix newposition = globalpose * Matrix.CreateTranslation(displacement);
            CurrentPosition = newposition;
        }
    }
}
