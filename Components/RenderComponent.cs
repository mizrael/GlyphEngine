using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GlyphEngine.Core;
using GlyphEngine.Components;
using GlyphEngine.Interfaces;
using GlyphEngine.SceneGraph;
using GlyphEngine.Utils;
using GlyphEngine.GameScreens;
using GlyphEngine.Services;
using GlyphEngine.Effects;

namespace GlyphEngine.Components
{
    public class RenderComponent : IComponent, IUpdatable
    {
        #region Members

        private TransformComponent _transform = null;

        private RenderService _renderServ = null;

        #endregion Members

        public RenderComponent(ISceneNode owner)
            : base(owner)
        {           
        }

        #region Methods

        public override void Init()
        {
            var currNode = base.Owner;
            while (null == _transform)
            {
                if (null == currNode)
                    throw new Exception("Unable to find TransformComponent");

                _transform = currNode.Components.Get<TransformComponent>();
                currNode = currNode.Father;                
            }

            _renderServ = ScreenManager.Instance.Game.Services.GetService<RenderService>();
            _renderServ.Add(this);
        }

        public override void Dispose()
        {            
            _renderServ.Remove(this);
        }

        public void Update(GameTime gameTime)
        {
            var updModel = this.Model as IUpdatable;
            if (null != updModel)            
                updModel.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (null != Model && Visible)            
                Model.Draw(gameTime, spriteBatch, _transform.World, Color, LayerDepth);
        }

        public void DrawBoundingBox(GraphicsDevice dev)
        {
            if (null != Model && Visible)
            {  
                var tmpRect = this.Model.CollisionBounds;
                tmpRect.X -= (int)this.Model.Origin.X;
                tmpRect.Y -= (int)this.Model.Origin.Y;                
         
                var mat = _transform.WorldMatrix * _renderServ.ViewMatrix;
                Helpers.TrasformBoundingBox(ref mat, ref tmpRect);

                TextureHelpers.DrawRectangle(tmpRect, ScreenManager.Instance.Game.GraphicsDevice);
            }
        }

        #endregion Methods

        #region Properties

        public bool Visible = true;

        public int LayerID = 0;

        public float LayerDepth = 0f;

        public Color Color = Color.White;

        public IRenderable Model = null;

        public readonly List<BaseEffect> Effects = new List<BaseEffect>();

        #endregion Properties
    }
}
