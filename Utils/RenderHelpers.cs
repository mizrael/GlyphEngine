using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GlyphEngine.Utils
{
    public static class RenderHelpers
    {
        private static VertexPositionTexture[] UnitQuad = new VertexPositionTexture[]
        {
            new VertexPositionTexture()
            {
                Position = new Vector3(-1, 1, 0),
                TextureCoordinate = new Vector2(0, 0),
            },
            new VertexPositionTexture()
            {
                Position = new Vector3(1, 1, 0),
                TextureCoordinate = new Vector2(1, 0),
            },
            new VertexPositionTexture()
            {
                Position =new Vector3(-1, -1, 0),
                TextureCoordinate = new Vector2(0, 1),
            },
            new VertexPositionTexture()
            {
                Position = new Vector3(1, -1, 0),
                TextureCoordinate = new Vector2(1, 1),
            },
        };

        public static void DrawUnitQuad(GraphicsDevice dev)
        {
            dev.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, UnitQuad, 0, 2);
        }

        public static void DrawSquareQuad(GraphicsDevice dev, Vector2 position, float rotation, float size, Color color)
        {
            size = size * .5f;

            size = (float)Math.Sqrt(Math.Pow(size, 2) + Math.Pow(size, 2));

            rotation += (float)Math.PI * .25f;

            var cos = (float)Math.Cos(rotation) * size;
            var sin = (float)Math.Sin(rotation) * size;

            var v1 = new Vector3(+cos, +sin, 0) + new Vector3(position, 0);
            var v2 = new Vector3(-sin, +cos, 0) + new Vector3(position, 0);
            var v3 = new Vector3(-cos, -sin, 0) + new Vector3(position, 0);
            var v4 = new Vector3(+sin, -cos, 0) + new Vector3(position, 0);

            DrawSquareQuad(dev, v1, v2, v3, v4, color);
        }

        public static void DrawSquareQuad(GraphicsDevice dev, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color)
        {
            var quad = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture()
                {
                    Position = v2,
                    Color = color,
                    TextureCoordinate = new Vector2(0,0),
                },
                new VertexPositionColorTexture()
                {
                    Position = v1,
                    Color = color,
                    TextureCoordinate = new Vector2(1,0),
                },
                new VertexPositionColorTexture()
                {
                    Position = v3,
                    Color = color,
                    TextureCoordinate = new Vector2(0,1),
                },
                new VertexPositionColorTexture()
                {
                    Position = v4,
                    Color = color,
                    TextureCoordinate = new Vector2(1,1),
                },
            };

            dev.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, quad, 0, 2);
        }
    }
}
