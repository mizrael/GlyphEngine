using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GlyphEngine.Components;
using GlyphEngine.Core;
using GlyphEngine.GameScreens;
using GlyphEngine.Utils;
using GlyphEngine.SceneGraph;

namespace GlyphEngine.Services.NodeSelectionPolicies
{
    public class MouseNodeSelectionPolicy : INodeSelectionPolicy
    {
        #region Members

        private RenderService _renderServ = null;        

        #endregion Members

        public void FrameInit(GameTime gameTime)
        {
             _renderServ = _renderServ ?? ScreenManager.Instance.Game.Services.GetService<RenderService>();
        }

        public bool Check(ISceneNode node)
        {
            var renderComp = node.Components.Get<RenderComponent>();
            if (null != renderComp && renderComp.Visible && null != renderComp.Model)
            {
                var trasfComp = node.Components.Get<TransformComponent>();
                if (null != trasfComp)
                {
                    var tmpRect = renderComp.Model.CollisionBounds;
                    tmpRect.X -= (int)renderComp.Model.Origin.X;
                    tmpRect.Y -= (int)renderComp.Model.Origin.Y;

                    var mat = trasfComp.WorldMatrix * _renderServ.ViewMatrix;
                    Helpers.TrasformBoundingBox(ref mat, ref tmpRect);

                    return tmpRect.Contains((int)InputManager.MousePositionVec.X, (int)InputManager.MousePositionVec.Y);
                }
            }
            return false;
        }
    }

    public class ColorHighlightPolicy : INodeSelectionHighlightPolicy
    {
        public void Hightlight(ISceneNode node)
        {
            var renderComp = node.Components.Get<RenderComponent>();
            if (null != renderComp)
                renderComp.Color = Color.Red;    
        }

        public void Undo(ISceneNode node)
        {
            var renderComp = node.Components.Get<RenderComponent>();
            if (null != renderComp)
                renderComp.Color = Color.White;
        }
    }
}
