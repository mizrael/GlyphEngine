using GlyphEngine.Components;
using GlyphEngine.Core;
using GlyphEngine.Extensions;
using GlyphEngine.GameScreens;
using GlyphEngine.SceneGraph;
using GlyphEngine.Utils;
using Microsoft.Xna.Framework;

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

        public bool Check(SceneNode node)
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
        public void Highlight(SceneNode node)
        {
            var renderComp = node.Components.Get<RenderComponent>();
            if (null != renderComp)
                renderComp.Color = this.HighlightColor;    
        }

        public void Undo(SceneNode node)
        {
            var renderComp = node.Components.Get<RenderComponent>();
            if (null != renderComp)
                renderComp.Color = this.BaseColor;
        }

        public Color HighlightColor = Color.Red;
        public Color BaseColor = Color.White;
    }
}
