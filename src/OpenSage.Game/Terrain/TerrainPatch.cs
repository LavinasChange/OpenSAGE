﻿using System.Numerics;
using OpenSage.Graphics.Rendering;
using OpenSage.Graphics.Shaders;
using OpenSage.Mathematics;
using Veldrid;
using Rectangle = OpenSage.Mathematics.Rectangle;

namespace OpenSage.Terrain
{
    public sealed class TerrainPatch
    {
        private readonly DeviceBuffer _vertexBuffer;
        private readonly DeviceBuffer _indexBuffer;
        private readonly uint _numIndices;

        private readonly BeforeRenderDelegate _beforeRender;

        public Rectangle Bounds { get; }

        public BoundingBox BoundingBox { get; }

        public Triangle[] Triangles { get; }

        internal TerrainPatch(
            Rectangle patchBounds,
            DeviceBuffer vertexBuffer,
            DeviceBuffer indexBuffer,
            uint numIndices,
            Triangle[] triangles,
            in BoundingBox boundingBox,
            ResourceSet materialResourceSet)
        {
            Bounds = patchBounds;

            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
            _numIndices = numIndices;

            BoundingBox = boundingBox;
            Triangles = triangles;

            _beforeRender = (cl, context) =>
            {
                cl.SetGraphicsResourceSet(4, materialResourceSet);
                cl.SetVertexBuffer(0, _vertexBuffer);
            };
        }

        internal void Intersect(
            Ray ray,
            ref float? closestIntersection)
        {
            if (!ray.Intersects(BoundingBox, out var _))
            {
                return;
            }

            for (var i = 0; i < Triangles.Length; i++)
            {
                if (ray.Intersects(Triangles[i], out var intersection))
                {
                    if (closestIntersection != null)
                    {
                        if (intersection < closestIntersection)
                        {
                            closestIntersection = intersection;
                        }
                    }
                    else
                    {
                        closestIntersection = intersection;
                    }
                }
            }
        }

        internal void BuildRenderList(
            RenderList renderList,
            ShaderSet shaderSet,
            Pipeline pipeline)
        {
            renderList.Opaque.RenderItems.Add(new RenderItem(
                shaderSet,
                pipeline,
                BoundingBox,
                Matrix4x4.Identity,
                0,
                _numIndices,
                _indexBuffer,
                _beforeRender));
        }
    }
}
