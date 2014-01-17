using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

using GlyphEngine.Core;
using GlyphEngine.Interfaces;

namespace GlyphEngine.Sprite
{
    public class SpriteInstance : IRenderable
    {
        #region Members

        private Texture2D _sprite = null;
        private Vector2 _origin = Vector2.Zero;
        private Vector2 _baseCenter = Vector2.Zero;
        private Rectangle _spriteRect = Rectangle.Empty;

        #endregion Members

        public SpriteInstance(Texture2D sprite)
        {
            Sprite = sprite;           
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Transform tr, Color col, float depth)
        {
            this.Draw(gameTime, spriteBatch, ref tr.Position, tr.Rotation, ref tr.Scale, col, depth);

          /*  spriteBatch.Draw(Sprite, tr.Position, _spriteRect, col, tr.Rotation,
                            _origin, tr.Scale, SpriteEffects.None, depth);*/
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, ref Vector2 pos, float rot, ref Vector2 scale, Color col, float depth)
        {
            spriteBatch.Draw(Sprite, pos, _spriteRect, col, rot,
                             _origin, scale, SpriteEffects.None, depth);
        }

        private void Init()
        {
            _spriteRect = new Rectangle(0, 0, _sprite.Width, _sprite.Height);
            _origin = new Vector2((float)_sprite.Width * .5f, (float)_sprite.Height * .5f);
            
            _baseCenter.X = _origin.X;
            _baseCenter.Y = _sprite.Height;
        }

        #region Properties

        public Rectangle Bounds 
        {
            get { return _spriteRect; }
        }

        public Rectangle CollisionBounds
        {
            get { return _spriteRect; }
        }

        public Texture2D Sprite
        {
            get { return _sprite; }
            set { _sprite = value; Init(); }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        public Vector2 BaseCenter
        {
            get { return _baseCenter; }
            set { _baseCenter = value; }
        }

        #endregion Properties
    }
}
