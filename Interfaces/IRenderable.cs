using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GlyphEngine.Core;
using GlyphEngine.Components;

namespace GlyphEngine.Interfaces
{
    public interface IRenderable
    {
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, Transform tr, Color col, float depth);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch, ref Vector2 pos, float rot, ref Vector2 scale, Color col, float depth);

        Rectangle Bounds { get; }
        Rectangle CollisionBounds { get; }
        Vector2 Origin { get; set; }
        Vector2 BaseCenter { get; set; }
    }
}
