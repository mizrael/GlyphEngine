using System;
using Microsoft.Xna.Framework;

using GlyphEngine.Interfaces;
using GlyphEngine.SceneGraph;
using GlyphEngine.Components;
using GlyphEngine.Core;
using GlyphEngine.GameScreens;
using GlyphEngine.Services;
using GlyphEngine.Utils;

namespace GlyphEngine.Components
{
    public class CollisionComponent : IComponent, IUpdatable
    {
        #region Members

        private TransformComponent _transfComp = null;
        private RenderComponent _renderComp = null;

        private IPartitioningService _partitioning = null;
        private ICollisionService _collSyst = null;

        #endregion Members

        public CollisionComponent(ISceneNode owner, Action<ISceneNode> responseFunc)
            : base(owner)
        {
            ResponseFunc = responseFunc;
        }

        #region Methods

        public override void Init()
        {
             _partitioning = ScreenManager.Instance.Game.Services.GetService<IPartitioningService>();
            _collSyst = ScreenManager.Instance.Game.Services.GetService<ICollisionService>();

            _transfComp = base.Owner.Components.Get<TransformComponent>();
            _renderComp = base.Owner.Components.Get<RenderComponent>();
            if (null != _transfComp && null != _renderComp && null != _renderComp.Model)
            {
                var size = new Point(_renderComp.Model.Bounds.Width, _renderComp.Model.Bounds.Height);
                
                _partitioning.RegisterEntity(base.Owner, ref size);            
            }
        }

        public override void Dispose()
        {
        }

        public void Update(GameTime gameTime)
        {
            var nearby = _partitioning.GetNearby(base.Owner);
            if (null != nearby && 0 != nearby.Count)
            {
                var tmpRect = _renderComp.Model.Bounds;
                var tmpOrig = _renderComp.Model.Origin;
                Helpers.TrasformBoundingBox(ref _transfComp.World, ref tmpOrig, ref tmpRect);
                
                var tmpRect2 = Rectangle.Empty;               
                var tmpOrig2 = Vector2.Zero;
                bool tmpResult = false;

                int count = nearby.Count;
                for (int i = 0; i != count; ++i)
                {
                    var currEnt = nearby[i];
                    var nearbyCollComp = currEnt.Components.Get<CollisionComponent>();
                    if (null != nearbyCollComp && nearbyCollComp.Active)
                    {
                        if (null != CheckCollisionFunc)
                        {
                            tmpResult = CheckCollisionFunc(currEnt);
                        }
                        else
                        {
                            var nearbyTransfComp = currEnt.Components.Get<TransformComponent>();
                            var nearbyRenderComp = currEnt.Components.Get<RenderComponent>();
                            if (null != nearbyTransfComp && null != nearbyRenderComp)
                            {
                                tmpRect2 = nearbyRenderComp.Model.Bounds;
                                tmpOrig2 = nearbyRenderComp.Model.Origin;

                                Helpers.TrasformBoundingBox(ref nearbyTransfComp.World, ref tmpOrig2, ref tmpRect2);

                                tmpRect.Intersects(ref tmpRect2, out tmpResult);                               
                            }
                        }

                        if (tmpResult)
                        {
                            _collSyst.Register(this, nearbyCollComp);
                        }
                    }
                }
            }
        }        

        

        #endregion Methods

        #region Properties

        public Action<ISceneNode> ResponseFunc = null;
        public Func<ISceneNode, bool> CheckCollisionFunc = null;

        #endregion Properties
    }
}
