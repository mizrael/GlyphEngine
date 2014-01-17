using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;

using GlyphEngine.Interfaces;
using GlyphEngine.SceneGraph;
using GlyphEngine.Components;
using GlyphEngine.Services.RenderSortingPolicies;

namespace GlyphEngine.Services
{
    public class RenderService : DrawableGameComponent, IGameComponent
    {
        #region Members

        private List<RenderComponent> _objects = null;

        private HashSet<RenderComponent> _removableObjects = null;
        private Queue<RenderComponent> _removalQueue = null;

        private SpriteBatch _spriteBatch = null;

        private int _currRemovalStep = 0;
        private int _removalFrameCount = 20;

        private int _currSortStep = 0;
        private int _sortFrameCount = 10;

        private Vector2 _cameraPos = Vector2.Zero;

        #endregion Members

        public RenderService(Game game)
            : base(game)
        {
            _objects = new List<RenderComponent>();
            _removalQueue = new Queue<RenderComponent>();
            _removableObjects = new HashSet<RenderComponent>();

            _spriteBatch = new SpriteBatch(game.GraphicsDevice);

            game.Services.AddService(typeof(RenderService), this);
        }

        #region Methods

        public override void Update(GameTime gameTime)
        {
            Matrix.CreateTranslation(-this.CameraPosition.X, -this.CameraPosition.Y, 0f, out this.ViewMatrix);
            Matrix.Invert(ref this.ViewMatrix, out this.InverseViewMatrix);

            PerformRemove();

            SortObjects();
        }

        public override void Draw(GameTime gameTime)
        {            
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, null, null, null, ViewMatrix);

            int count = _objects.Count;
            for (int i = 0; i != count; ++i)
            {
                var obj = _objects[i];
                if (!obj.Owner.Active)
                {
                    if (!_removableObjects.Contains(obj))
                        _removalQueue.Enqueue(obj);
                    continue;
                }
                obj.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();

#if DEBUG
            for (int i = 0; i != count; ++i)
            {
                _objects[i].DrawBoundingBox(this.GraphicsDevice);
            }
#endif
      
        }

        public void Add(RenderComponent obj)
        {
            if(!_objects.Contains(obj))
                _objects.Add(obj);
        }

        public void Remove(RenderComponent obj)
        {
            if (!_removableObjects.Contains(obj))
                _removalQueue.Enqueue(obj);
        }
       
        private void PerformRemove()
        {
            if (0 != _removalQueue.Count && ++_currRemovalStep == _removalFrameCount)
            {
                _currRemovalStep = 0;
                while (0 != _removalQueue.Count)
                {
                    _objects.Remove(_removalQueue.Dequeue());
                }
                _removableObjects.Clear();
            }            
        }

        private void SortObjects()
        {
            if (++_currSortStep == _sortFrameCount)
            {
                _currSortStep = 0;
                _objects.Sort(this.SortingPolicy);
            }
        }

        #endregion Methods

        #region Properties

        public Matrix ViewMatrix = Matrix.Identity;

        public Matrix InverseViewMatrix = Matrix.Identity;

        public Vector2 CameraPosition = Vector2.Zero;

        public IComparer<RenderComponent> SortingPolicy = new YRenderSortingPolicy();

        #endregion Properties
    }

}
