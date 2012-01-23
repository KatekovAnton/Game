using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysX_test2.Engine.Render.DebugRender
{
    public class DebugRenderer
    {
        GraphicsDevice device;
        BasicEffect visualisationEffect;
        public static readonly short[] BBIndices = new short[] { 0, 1, 2, 3, 0, 2, 1, 3, 4, 5, 6, 7, 4, 6, 5, 7, 0, 4, 5, 1, 7, 3, 6, 2 };
        public static readonly short[] BRindices = new short[] { 0, 1, 2, 3, 0, 2, 1, 3 };

        public static Color AABBColor = Color.Black;
        public static Color AABRColor = Color.Red;
        public static Color BBColor = Color.Yellow;

        public VertexPositionColor[] AABBPointList = new VertexPositionColor[]
        { 
            new VertexPositionColor(new Vector3(),AABBColor),
            new VertexPositionColor(new Vector3(),AABBColor),
            new VertexPositionColor(new Vector3(),AABBColor),
            new VertexPositionColor(new Vector3(),AABBColor),
            new VertexPositionColor(new Vector3(),AABBColor),
            new VertexPositionColor(new Vector3(),AABBColor),
            new VertexPositionColor(new Vector3(),AABBColor),
            new VertexPositionColor(new Vector3(),AABBColor)
        };

        public VertexPositionColor[] BBPointList = new VertexPositionColor[]
        { 
            new VertexPositionColor(new Vector3(),BBColor),
            new VertexPositionColor(new Vector3(),BBColor),
            new VertexPositionColor(new Vector3(),BBColor),
            new VertexPositionColor(new Vector3(),BBColor),
            new VertexPositionColor(new Vector3(),BBColor),
            new VertexPositionColor(new Vector3(),BBColor),
            new VertexPositionColor(new Vector3(),BBColor),
            new VertexPositionColor(new Vector3(),BBColor)
        };

        public VertexPositionColor[] BRPointList = new VertexPositionColor[]
        { 
            new VertexPositionColor(new Vector3(),AABRColor),
            new VertexPositionColor(new Vector3(),AABRColor),
            new VertexPositionColor(new Vector3(),AABRColor),
            new VertexPositionColor(new Vector3(),AABRColor)
        };

        public DebugRenderer(GraphicsDevice _device, BasicEffect _visualizationEffect)
        {
            visualisationEffect = _visualizationEffect;
            device = _device;
        }
        public void RenderAABB(Logic.SceneGraph.OTBoundingShape boundingShape)
        {
            {
                AABBPointList[0].Position.X = boundingShape.aabb.XNAbb.Max.X; AABBPointList[0].Position.Y = boundingShape.aabb.XNAbb.Max.Y; AABBPointList[0].Position.Z = boundingShape.aabb.XNAbb.Max.Z;
                AABBPointList[1].Position.X = boundingShape.aabb.XNAbb.Max.X; AABBPointList[1].Position.Y = boundingShape.aabb.XNAbb.Max.Y; AABBPointList[1].Position.Z = boundingShape.aabb.XNAbb.Min.Z;
                AABBPointList[2].Position.X = boundingShape.aabb.XNAbb.Min.X; AABBPointList[2].Position.Y = boundingShape.aabb.XNAbb.Max.Y; AABBPointList[2].Position.Z = boundingShape.aabb.XNAbb.Max.Z;
                AABBPointList[3].Position.X = boundingShape.aabb.XNAbb.Min.X; AABBPointList[3].Position.Y = boundingShape.aabb.XNAbb.Max.Y; AABBPointList[3].Position.Z = boundingShape.aabb.XNAbb.Min.Z;
                AABBPointList[4].Position.X = boundingShape.aabb.XNAbb.Max.X; AABBPointList[4].Position.Y = boundingShape.aabb.XNAbb.Min.Y; AABBPointList[4].Position.Z = boundingShape.aabb.XNAbb.Max.Z;
                AABBPointList[5].Position.X = boundingShape.aabb.XNAbb.Max.X; AABBPointList[5].Position.Y = boundingShape.aabb.XNAbb.Min.Y; AABBPointList[5].Position.Z = boundingShape.aabb.XNAbb.Min.Z;
                AABBPointList[6].Position.X = boundingShape.aabb.XNAbb.Min.X; AABBPointList[6].Position.Y = boundingShape.aabb.XNAbb.Min.Y; AABBPointList[6].Position.Z = boundingShape.aabb.XNAbb.Max.Z;
                AABBPointList[7].Position.X = boundingShape.aabb.XNAbb.Min.X; AABBPointList[7].Position.Y = boundingShape.aabb.XNAbb.Min.Y; AABBPointList[7].Position.Z = boundingShape.aabb.XNAbb.Min.Z;
            }
            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList, AABBPointList, 0, AABBPointList.Length, BBIndices, 0, BBIndices.Length / 2);

        }
        public void RenderTransformedBB(Logic.SceneGraph.OTBoundingShape boundingShape)
        {
            {
                BBPointList[0].Position = boundingShape.bb.TransformedPoints[0];
                BBPointList[1].Position = boundingShape.bb.TransformedPoints[1];
                BBPointList[2].Position = boundingShape.bb.TransformedPoints[2];
                BBPointList[3].Position = boundingShape.bb.TransformedPoints[3];
                BBPointList[4].Position = boundingShape.bb.TransformedPoints[4];
                BBPointList[5].Position = boundingShape.bb.TransformedPoints[5];
                BBPointList[6].Position = boundingShape.bb.TransformedPoints[6];
                BBPointList[7].Position = boundingShape.bb.TransformedPoints[7];
            }
            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList, BBPointList, 0, BBPointList.Length, BBIndices, 0, BBIndices.Length / 2);

        }
        public void RenderAABR(Logic.SceneGraph.OTBoundingShape boundingShape)
        {
            { 
                BRPointList[0].Position = boundingShape.aabr.points[0];
                BRPointList[1].Position = boundingShape.aabr.points[1];
                BRPointList[2].Position = boundingShape.aabr.points[2];
                BRPointList[3].Position = boundingShape.aabr.points[3];
            };
            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList, BRPointList, 0, BRPointList.Length, BRindices, 0, BRindices.Length / 2);

        }
        public void RenderSGNodeBR(Logic.SceneGraph.SGNode node)
        {
            {
                AABBPointList[0].Position.X = node.boundingBox.Max.X; AABBPointList[0].Position.Y = node.boundingBox.Max.Y; AABBPointList[0].Position.Z = node.boundingBox.Max.Z;
                AABBPointList[1].Position.X = node.boundingBox.Max.X; AABBPointList[1].Position.Y = node.boundingBox.Max.Y; AABBPointList[1].Position.Z = node.boundingBox.Min.Z;
                AABBPointList[2].Position.X = node.boundingBox.Min.X; AABBPointList[2].Position.Y = node.boundingBox.Max.Y; AABBPointList[2].Position.Z = node.boundingBox.Max.Z;
                AABBPointList[3].Position.X = node.boundingBox.Min.X; AABBPointList[3].Position.Y = node.boundingBox.Max.Y; AABBPointList[3].Position.Z = node.boundingBox.Min.Z;
                AABBPointList[4].Position.X = node.boundingBox.Max.X; AABBPointList[4].Position.Y = node.boundingBox.Min.Y; AABBPointList[4].Position.Z = node.boundingBox.Max.Z;
                AABBPointList[5].Position.X = node.boundingBox.Max.X; AABBPointList[5].Position.Y = node.boundingBox.Min.Y; AABBPointList[5].Position.Z = node.boundingBox.Min.Z;
                AABBPointList[6].Position.X = node.boundingBox.Min.X; AABBPointList[6].Position.Y = node.boundingBox.Min.Y; AABBPointList[6].Position.Z = node.boundingBox.Max.Z;
                AABBPointList[7].Position.X = node.boundingBox.Min.X; AABBPointList[7].Position.Y = node.boundingBox.Min.Y; AABBPointList[7].Position.Z = node.boundingBox.Min.Z;
            }
            device.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineList, AABBPointList, 0, AABBPointList.Length, BBIndices, 0, BBIndices.Length / 2);

        }

    }
}
