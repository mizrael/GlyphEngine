using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlyphEngine.Components;

namespace GlyphEngine.Services.RenderSortingPolicies
{
    public class YRenderSortingPolicy : IComparer<RenderComponent>
    {
        public int Compare(RenderComponent x, RenderComponent y)
        {
            var layerDiff = x.LayerID.CompareTo(y.LayerID);
            if (0 == layerDiff)
            {
                var trasfX = x.Owner.Components.Get<TransformComponent>();
                var trasfY = y.Owner.Components.Get<TransformComponent>();
                if (null != trasfX && null != trasfY)
                {
                    var xVal = trasfX.World.Position.Y + x.Model.BaseCenter.Y;
                    var yVal = trasfY.World.Position.Y + y.Model.BaseCenter.Y;
                    return xVal.CompareTo(yVal);
                }
            }

            return layerDiff;
        }
    }
}
