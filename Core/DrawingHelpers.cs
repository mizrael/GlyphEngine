using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GlyphEngine.Core
{
    public class DrawingHelpers
    {

        const int DEFAULT_INNER_TRANSITION_WIDTH = 1;
        const int DEFAULT_OUTER_TRANSITION_WIDTH = 0;
        const int DEFAULT_BORDER_WIDTH = 1;


        // Lines
        public static Texture2D CreateLineTexture(GraphicsDevice graphicsDevice, int lineThickness, Color color)
        {
            Texture2D texture2D = new Texture2D(graphicsDevice, 2, lineThickness + 2, 1, TextureUsage.None, SurfaceFormat.Color);

            //Texture2D texture2D = new Texture2D(graphicsDevice, 2, lineWidth + 2);
            int count = 2 * (lineThickness + 2);
            Color[] colorArray = new Color[count];
            colorArray[0] = Color.TransparentWhite;
            colorArray[1] = Color.TransparentWhite;

            for (int i = 0; i < count; i++)
            {
                int y = i / 2;
                colorArray[i] = color;
            }

            colorArray[count - 2] = Color.TransparentWhite;
            colorArray[count - 1] = Color.TransparentWhite;
            texture2D.SetData<Color>(colorArray);
            return texture2D;
        }

        #region Circles

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color)
        {
            return CreateCircleTexture(graphicsDevice, radius, 0, 0, 2, color, color);
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color, Color borderColor)
        {
            return CreateCircleTexture(graphicsDevice, radius, 1, 1, 1, color, borderColor);
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, int borderWidth, Color color, Color borderColor)
        {
            return CreateCircleTexture(graphicsDevice, radius, borderWidth, 1, 2, color, borderColor);
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, int borderWidth, int borderInnerTransitionWidth, int borderOuterTransitionWidth, Color color, Color borderColor)
        {
            int x = 0;
            int y = -1;
            int diameter = (radius + 2) * 2;
            //Vector2 center = new Vector2(0, 0);
            Vector2 center = new Vector2((diameter - 1) / 2f, (diameter - 1) / 2f);

            Texture2D circle = new Texture2D(graphicsDevice, diameter, diameter, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] colors = new Color[diameter * diameter];

            for (int i = 0; i < colors.Length; i++)
            {
                if (i % diameter == 0) { y += 1; }
                x = i % diameter;

                Vector2 distance = new Vector2(Math.Abs(center.X - x), Math.Abs(center.Y - y));

                Vector2 diff = new Vector2(x, y) - center;
                float length = diff.Length();// distance.Length();

                if (length > radius)
                {
                    colors[i] = Color.TransparentBlack;
                }
                else if (length >= radius - borderOuterTransitionWidth)
                {

                    float transitionAmount = (length - (radius - borderOuterTransitionWidth)) / borderOuterTransitionWidth;
                    transitionAmount = 255 * (1 - transitionAmount);
                    colors[i] = new Color(borderColor.R, borderColor.G, borderColor.B, (byte)transitionAmount);
                }
                else if (length > radius - (borderWidth + borderOuterTransitionWidth))
                {
                    colors[i] = borderColor;
                }
                else if (length >= radius - (borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth))
                {
                    float transitionAmount = (length - (radius - (borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth))) / (borderInnerTransitionWidth + 1);
                    colors[i] = new Color((byte)MathHelper.Lerp(color.R, borderColor.R, transitionAmount), (byte)MathHelper.Lerp(color.G, borderColor.G, transitionAmount), (byte)MathHelper.Lerp(color.B, borderColor.B, transitionAmount));
                }
                else
                {
                    colors[i] = color;
                }
            }
            circle.SetData<Color>(colors);
            return circle;
        }

        #endregion
        
        #region Rectangles

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            return CreateRectangleTexture(graphicsDevice, width, height, DEFAULT_BORDER_WIDTH, DEFAULT_INNER_TRANSITION_WIDTH, DEFAULT_OUTER_TRANSITION_WIDTH, color, color);
        }

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, Color color, Color borderColor)
        {
            return CreateRectangleTexture(graphicsDevice, width, height, DEFAULT_BORDER_WIDTH, DEFAULT_INNER_TRANSITION_WIDTH, DEFAULT_OUTER_TRANSITION_WIDTH, color, borderColor);
        }

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, int borderWidth, Color color, Color borderColor)
        {
            return CreateRectangleTexture(graphicsDevice, width, height, borderWidth, DEFAULT_INNER_TRANSITION_WIDTH, DEFAULT_OUTER_TRANSITION_WIDTH, color, borderColor);
        }

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, int borderWidth, int borderInnerTransitionWidth, int borderOuterTransitionWidth, Color color, Color borderColor)
        {
            Texture2D texture2D = new Texture2D(graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);

            //Texture2D texture2D = new Texture2D(graphicsDevice, 2, lineWidth + 2);
            int x;
            int y = -1;
            int j = 0;
            int count = width * height;
            Color[] colorArray = new Color[count];
            Color[] shellColor = new Color[borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth];
            float transitionAmount = 0;

            for (j = 0; j < borderOuterTransitionWidth; j++)
            {
                transitionAmount = (float)(j) / (float)(borderOuterTransitionWidth);
                shellColor[j] = new Color(borderColor.R, borderColor.G, borderColor.B, (byte)(255 * transitionAmount));
                //shellColor[j] = Color.Red;
            }
            for (j = borderOuterTransitionWidth; j < borderWidth + borderOuterTransitionWidth; j++)
            {
                shellColor[j] = borderColor;
                //shellColor[j] = Color.Green;
            }
            for (j = borderWidth + borderOuterTransitionWidth; j < borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth; j++)
            {
                transitionAmount = 1 - (float)(j - (borderWidth + borderOuterTransitionWidth) + 1) / (float)(borderInnerTransitionWidth + 1);
                shellColor[j] = new Color((byte)MathHelper.Lerp(color.R, borderColor.R, transitionAmount), (byte)MathHelper.Lerp(color.G, borderColor.G, transitionAmount), (byte)MathHelper.Lerp(color.B, borderColor.B, transitionAmount));
            }


            for (int i = 0; i < count; i++)
            {
                if (i % width == 0) { y += 1; }
                x = i % width;

                //check if pixel is in one of the rectangular border shells
                bool isInShell = false;
                for (int k = 0; k < shellColor.Length; k++)
                {
                    if (InShell(x, y, width, height, k))
                    {
                        colorArray[i] = shellColor[k];
                        isInShell = true;
                        break;
                    }
                }
                //pixel is not in shell so it is in the center
                if (!isInShell)
                {
                    colorArray[i] = color;
                }
            }

            texture2D.SetData<Color>(colorArray);
            return texture2D;
        }

        private static bool InShell(int x, int y, int width, int height, int shell)
        {
            //check each line of rectangle.
            if ((x == shell && IsBetween(y, shell, height - 1 - shell)) || (x == width - 1 - shell && IsBetween(y, shell, height - 1 - shell)))
            {
                return true;
            }
            else if ((y == shell && IsBetween(x, shell, width - 1 - shell)) || (y == height - 1 - shell && IsBetween(x, shell, width - 1 - shell)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsBetween(float value, float min, float max)
        {
            if (value >= min && value <= max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void DrawRectangle(Rectangle rect, GraphicsDevice dev)
        {
            VertexDeclaration _vertexDecl = new VertexDeclaration(dev, VertexPositionColor.VertexElements);

            VertexPositionColor[] verts = new VertexPositionColor[8];

            verts[0].Position = new Vector3(rect.X, rect.Y, 0f);
            verts[0].Color = Color.White;
            verts[1].Position = new Vector3(rect.X + rect.Width, rect.Y, 0f);
            verts[1].Color = Color.White;

            verts[2].Position = new Vector3(rect.X, rect.Y + rect.Height, 0f);
            verts[2].Color = Color.White;
            verts[3].Position = new Vector3(rect.X + rect.Width, rect.Y + rect.Height, 0f);
            verts[3].Color = Color.White;

            verts[4].Position = new Vector3(rect.X, rect.Y, 0f);
            verts[4].Color = Color.White;
            verts[5].Position = new Vector3(rect.X, rect.Y + rect.Height, 0f);
            verts[5].Color = Color.White;

            verts[6].Position = new Vector3(rect.X + rect.Width, rect.Y, 0f);
            verts[6].Color = Color.White;
            verts[7].Position = new Vector3(rect.X + rect.Width, rect.Y + rect.Height, 0f);
            verts[7].Color = Color.White;

            VertexBuffer _vb = new VertexBuffer(dev, typeof(VertexPositionColor), 8, BufferUsage.WriteOnly);
            _vb.SetData<VertexPositionColor>(verts);

            BasicEffect eff = new BasicEffect(dev, null);

            dev.VertexDeclaration = _vertexDecl;
            dev.Vertices[0].SetSource(_vb, 0, VertexPositionColor.SizeInBytes);

            eff.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);
            eff.World = Matrix.Identity;
            eff.Projection = Matrix.CreateOrthographicOffCenter(0, dev.Viewport.Width,
                                                                dev.Viewport.Height, 0, 0, 1f);

            eff.Begin();
            eff.CurrentTechnique.Passes[0].Begin();
            dev.DrawPrimitives(PrimitiveType.LineList, 0, 4);
            eff.CurrentTechnique.Passes[0].End();
            eff.End();
        }

        #endregion

        #region Textures

        public static void Texture2DCopy(Texture2D src, ref Texture2D dest)
        {
            try
            {
                if (src == null)
                    return;

                GraphicsDevice dev = src.GraphicsDevice;
                Texture2D tmpsrc = src;

                if (dest == null)
                {
                    dest = new Texture2D(dev, src.Width, src.Height, 1, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Color);
                }

                if (src.Width != dest.Width || src.Height != dest.Height)
                {
                    int w = src.Width;
                    int h = src.Height;

                    SpriteBatch sp = new SpriteBatch(dev);
                    RenderToTexture rtt = new RenderToTexture(dev, w, h, SurfaceFormat.Color, DepthFormat.Depth16);

                    rtt.Begin();
                    dev.Clear(new Color(255, 255, 255, 0));
                    sp.Begin(SpriteBlendMode.AlphaBlend);
                    sp.Draw(src, Vector2.Zero, Color.White);
                    sp.End();
                    rtt.End();

                    tmpsrc = rtt.Texture;
                }

                byte[] srcData = new byte[tmpsrc.Width * tmpsrc.Height * 4];

                tmpsrc.GetData<byte>(srcData);
                dest.SetData<byte>(srcData);

                tmpsrc = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Texture2D SetTextureAlpha(Texture2D tex, Color alpha)
        {
            if (tex == null)
                return null;

            int size = tex.Width * tex.Height;

            Texture2D tmpTex = new Texture2D(tex.GraphicsDevice, tex.Width, tex.Height);

            Color[] data = new Color[size];

            tex.GetData<Color>(data);
            for (int i = 0; i != size; ++i)
            {
                if (data[i].R == alpha.R && data[i].G == alpha.G && data[i].B == alpha.B)
                {
                    data[i].A = 0;
                }
            }

            tmpTex.SetData<Color>(data);

            return tmpTex;
        }

        public static Texture2D Texture2DRotate(Texture2D src, float angle)
        {
            try
            {
                if (src == null)
                    return null;

                int w = src.Width;
                int h = src.Height;

                int halfw = src.Width >> 1;
                int halfh = src.Height >> 1;

                Vector2 halfSize=new Vector2(halfw, halfh);

                GraphicsDevice dev = src.GraphicsDevice;

                SpriteBatch sp = new SpriteBatch(dev);
                RenderToTexture rtt = new RenderToTexture(dev, w, h, SurfaceFormat.Color, DepthFormat.Depth16);

                rtt.Begin();
                dev.Clear(new Color(255, 255, 255, 0));
                sp.Begin(SpriteBlendMode.AlphaBlend);
                sp.Draw(src, halfSize, null, Color.White, angle, halfSize,
                        1f, SpriteEffects.None, 0f);
                sp.End();
                rtt.End();

                byte[] srcData = new byte[rtt.Texture.Width * rtt.Texture.Height * 4];

                rtt.Texture.GetData<byte>(srcData);

                Texture2D tmpTex = new Texture2D(dev, w,h);
                tmpTex.SetData<byte>(srcData);
                return tmpTex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
