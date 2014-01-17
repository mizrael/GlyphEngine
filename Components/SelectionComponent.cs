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

namespace GlyphEngine.Components
{
    public class SelectionComponent : IComponent
    {
        public SelectionComponent(ISceneNode owner)
            :base(owner)
        {            
        }

        public override void Init()
        {
        }

        public override void Dispose()
        {
        }
    }
}
