using System;
using System.Collections.Generic;
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
    public class AnimatedSpriteInstance : IRenderable, IUpdatable
    {
        #region Members

        private SpriteSheet _spriteSheet = null;

        private Rectangle _sourceRect;

        private float _timer = 0f;
        private int _currentFramePosX = 0;
        private int _currentFramePosY = 0;
        private int _currentFrame = 0;
        
        private SpriteAnimation _currAnim = null;
        private Texture2D _currSpriteTex = null;

        private bool _animationEnded = false;       

        #endregion Members

        public AnimatedSpriteInstance(SpriteSheet spriteSheet)
        {
            this.SetSpriteSheet(spriteSheet);
        }

        public void Update(GameTime gameTime)
        {
            if (_currAnim == null || 1 == _currAnim.NumFrames || (_animationEnded && !_currAnim.Loop ))
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            _timer += deltaTime;

            if (_timer > _currAnim.FrameInterval )
            {
                _currentFramePosX++;
                _currentFrame++;
                if ((_animationEnded = _currentFrame > _currAnim.NumFrames - 1) && _currAnim.Loop)
                {
                    _currentFramePosX = _currAnim.StartPos.X;
                    _currentFramePosY = _currAnim.StartPos.Y;
                    _currentFrame = 0;
                    _animationEnded = false;
                }

                if (!_animationEnded)
                {
                    int x = _currentFramePosX * _currAnim.SpriteSize.X;
                    if (x >= _currSpriteTex.Width)
                    {
                        x = 0;
                        _currentFramePosX = 0;
                        _currentFramePosY++;
                    }
                    int y = _currentFramePosY * _currAnim.SpriteSize.Y;
                    _sourceRect = new Rectangle(x, y, _currAnim.SpriteSize.X, _currAnim.SpriteSize.Y);
                }
                else
                {
                    if (null != OnAnimationEnd)
                        OnAnimationEnd(this);
                }

                _timer = 0f;
            }           
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Transform tr, Color col, float depth)
        {
            this.Draw(gameTime, spriteBatch, ref tr.Position, tr.Rotation, ref tr.Scale, col, depth);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, ref Vector2 pos, float rot, ref Vector2 scale, Color col, float depth)
        {
            spriteBatch.Draw(_currSpriteTex, pos, _sourceRect, 
                             col, rot, _currAnim.SpriteCenter,
                             scale, SpriteEffects.None, depth);
        }

        #region Animation Methods

        public void SetSpriteSheet(SpriteSheet spriteSheet)
        {
            if (null == spriteSheet) throw new ArgumentNullException("SpriteSheet");
            _spriteSheet = spriteSheet;
            SetAnimation(0);
        }

        public void SetAnimation(string name)
        {
            if (null != _currAnim && 0 == string.Compare(_currAnim.Name, name))
                return;

            SpriteAnimation anim = _spriteSheet.GetAnimation(name);
            if (anim == null)
                return;
            SetAnimation(anim);
        }

        public void SetAnimation(int id)
        {
            SpriteAnimation anim = _spriteSheet.GetAnimation(id);
            if (anim == null)
                return;

            if ((_currAnim != null && anim.Name == _currAnim.Name))
                return;

            SetAnimation(anim);
        }

        private void SetAnimation(SpriteAnimation anim)
        {
            _currAnim = anim;
            _currSpriteTex = _spriteSheet.Textures[ (_currAnim != null) ? _currAnim.TextureID : 0 ];

            this.ResetAnimation();
        }

        public void ResetAnimation()
        {
            if (_currAnim == null)
                return;

            _animationEnded = false;
            _currentFramePosX = _currAnim.StartPos.X;
            _currentFramePosY = _currAnim.StartPos.Y;
            FrameInterval = _currAnim.FrameInterval;
            _currentFrame = 0;
            _timer = 0f;
            _sourceRect = new Rectangle(_currentFramePosX * _currAnim.SpriteSize.X,
                                        _currentFramePosY * _currAnim.SpriteSize.Y,
                                        _currAnim.SpriteSize.X, _currAnim.SpriteSize.Y);
            
            this.Bounds = new Rectangle(0, 0, _currAnim.SpriteSize.X, _currAnim.SpriteSize.Y);
        }

        #endregion Animation Methods

        #region Properties

        public Rectangle Bounds
        {
            get;
            private set;
        }

        public Rectangle CollisionBounds
        {
            get { return (null != _currAnim) ? _currAnim.BoundingBox : Rectangle.Empty; }
        }

        public Vector2 Origin
        {
            get { return (_currAnim != null) ? _currAnim.SpriteCenter : Vector2.Zero; }
            set { if (_currAnim != null) _currAnim.SpriteCenter = value; }
        }

        public Vector2 BaseCenter
        {
            get { return (_currAnim != null) ? _currAnim.SpriteBaseCenter : Vector2.Zero; }
            set { if (_currAnim != null) _currAnim.SpriteBaseCenter = value; }
        }

        public bool isAnimationEnded
        {
            get { return _animationEnded; }
        }

        public string CurrentAnimation
        {
            get { return (_currAnim != null) ?  _currAnim.Name : string.Empty; } 
        }
       
        public int CurrentFrame
        {
            get { return _currentFrame; }
        }
     
        public Texture2D Texture
        {
            get { return _currSpriteTex; }
        }

        public SpriteSheet SpriteSheet
        {
            get { return _spriteSheet; }            
        }       

        public float FrameInterval = 0f;        

        #endregion

        #region Event

        public delegate void AnimationEndEventHandler(AnimatedSpriteInstance sender);
        public event AnimationEndEventHandler OnAnimationEnd;

        #endregion Event
    }
}
