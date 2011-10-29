using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using PhysX_test2.TheGame.Level;
using PhysX_test2.TheGame.Objects;
using PhysX_test2.TheGame.Objects.StateGraphs;


namespace PhysX_test2.TheGame
{
    public class StaticObjects
    {
        //single object
        private static StaticObjects instance;
        public static StaticObjects Instance()
        {
            if (instance == null)
                instance = new StaticObjects();
            return instance;
        }


        public const string _characterNodeIdle = "idle";
        public const string _characterNodeDead = "dead";
        public const string _characterEdgeDeadToIdle = "setalive\0";
        public const string _characterEdgeIdleToDead = "setdead\0";

        public const string _levelgeometryNodeIdle = "idle";


        public ObjectGraphStatic _graphCharacter;
        public ObjectGraphStatic _graphLevelGeometry;


        private StaticObjects()
        { 
            //init all static objects here
            InitCharacterGraph();
            InitLevelGeometryGraph();
        }

        private void InitCharacterGraph()
        {
            CharacterGraphNode nodeIdle = new CharacterGraphNode();
            nodeIdle._name = _characterNodeIdle;
            nodeIdle._isOperable = true;
            nodeIdle._canReceiveControl = true;

            CharacterGraphNode nodeDead = new CharacterGraphNode();
            nodeDead._name = _characterNodeDead;
            nodeDead._isOperable = false;
            nodeDead._canReceiveControl = false;

            CharacterGraphEdge deadToAlive = new CharacterGraphEdge();
            deadToAlive._chance = 1.0f;
            deadToAlive._eventName = _characterEdgeDeadToIdle;
            deadToAlive._nodeFrom = nodeDead;
            deadToAlive._nodeTo = nodeIdle;

            CharacterGraphEdge aliveToDead = new CharacterGraphEdge();
            deadToAlive._chance = 1.0f;
            deadToAlive._eventName = _characterEdgeIdleToDead;
            deadToAlive._nodeFrom = nodeIdle;
            deadToAlive._nodeTo = nodeDead;

            _graphCharacter = new ObjectGraphStatic(
                new ObjectGraphNode[] { nodeIdle, nodeDead }, 
                new ObjectGraphEdge[] { deadToAlive, aliveToDead }
                );
        }

        private void InitLevelGeometryGraph()
        {
            ObjectGraphNode idleNode = new ObjectGraphNode();
            idleNode._name = _levelgeometryNodeIdle;
            idleNode._outEdges = new BaseExtensions.Graph.GraphEdge[] { };

            _graphLevelGeometry = new ObjectGraphStatic(new ObjectGraphNode[] { idleNode }, new ObjectGraphEdge[] { });
        }
    }
}
