using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StillDesign.PhysX;
using Microsoft.Xna.Framework;
namespace PhysX_test2
{
    public delegate void ContactCallback(Actor a, Actor b, ContactPairFlag events, Vector3 FrictionForce, Vector3 NormalForce);

    public class ContactReport : UserContactReport
    {
        private MyGame _demo;

        private List<ContactReportPair> _contactPairs;

        public ContactReport(MyGame demo)
        {
            _demo = demo;

            // Associate the pairs with a function
            _contactPairs = new List<ContactReportPair>();
           // addnewunit(demo._engine.TestSideActor, demo._engine.BoxActor, new ContactCallback(BoxAndGroundPlaneContact));

        }
        public void addnewunit(Actor a, Actor b, ContactCallback ccb)
        {
            _contactPairs.Add(new ContactReportPair(a, b, ccb));
        }
        // PhysX calls OnContactNotify is the base class which you then provide the implementation for
        public override void OnContactNotify(ContactPair contactInformation, ContactPairFlag events)
        {
            Actor a = contactInformation.ActorA;
            Actor b = contactInformation.ActorB;
           
            ContactStreamIterator iter = new ContactStreamIterator(contactInformation.ContactStream);

            while (iter.GoToNextPair())
            {
                while (iter.GoToNextPatch())
                {
                    while (iter.GoToNextPoint())
                    {
                        // Test each of the available 'information' functions/properties
                        int numberOfPairs = iter.GetNumberOfPairs();
                        //какие конкретно части столкнулись???? видимо. полезно.
                        Shape shapeA = iter.GetShapeA();
                        Shape shapeB = iter.GetShapeB();
                        bool isShapeADeleted = iter.IsDeletedShapeA();
                        bool isShapeBDeleted = iter.IsDeletedShapeB();
                        ShapeFlag shapeFlags = iter.GetShapeFlags();
                        int numberOfPatches = iter.GetNumberOfPatches();
                        int numberOfPatchesRemaining = iter.GetNumberOfPatchesRemaining();
                        StillDesign.PhysX.MathPrimitives.Vector3 patchNormal = iter.GetPatchNormal();
                        int numberOfPoints = iter.GetNumberOfPoints();
                        int numberOfPointsRemaining = iter.GetNumberOfPointsRemaining();
                        StillDesign.PhysX.MathPrimitives.Vector3 point = iter.GetPoint();
                        float separation = iter.GetSeperation();
                        int featureIndex0 = iter.GetFeatureIndex0();
                        int featureIndex1 = iter.GetFeatureIndex1();
                        float pointNormalForce = iter.GetPointNormalForce();
                    }
                }
            }

            iter.Dispose();
            foreach (ContactReportPair pair in _contactPairs)
            {
                if ((pair.A == a || pair.A == b) && (pair.B == a || pair.B == b))
                {
                    pair.Callback(a, b, events, contactInformation.FrictionForce.toXNAV3(), contactInformation.NormalForce.toXNAV3());
                    break;
                }
            }
        }

        private void BoxAndGroundPlaneContact(Actor a, Actor b, ContactPairFlag events, Vector3 FrictionForce, Vector3 NormalForce)
        {
            


            if (!FrictionForce.NewNear(Vector3.Zero,10) || !NormalForce.NewNear(Vector3.Zero,1000))
            {
                float ffl = FrictionForce.Length();
                float nfl = NormalForce.Length();
                if(Engine.LogProvider.NeedTraceContactReportMessages)
                    Engine.LogProvider.TraceMessage("Contact BoxActor/GroundPlane: Contact type:" + events.ToString() + ";\t---> Contact friction force=" + FrictionForce.ToString() + "; module=" + ffl.ToString() + ";\tContact normal force:" + NormalForce.ToString() + "; module =" + nfl.ToString() + ";");
            }
            else
                if (Engine.LogProvider.NeedTraceContactReportMessages)
                    Engine.LogProvider.TraceMessage("Contact BoxActor/GroundPlane: Contact type:" + events.ToString() + ";\tContact forces - ZERO.   (FrFL = " + FrictionForce.Length().ToString() + ",\tNormFL = " + NormalForce.Length().ToString()+ ");");
        }
    }
    public class ContactReportPair
    {
        private Actor _a, _b;
        private ContactCallback _callback;
       
        public ContactReportPair(Actor a, Actor b, ContactCallback callback)
        {
            _a = a;
            _b = b;
            _callback = callback;
        }

        public Actor A
        {
            get
            {
                return _a;
            }
        }
        public Actor B
        {
            get
            {
                return _b;
            }
        }
        public ContactCallback Callback
        {
            get
            {
                return _callback;
            }
        }
    }
}
