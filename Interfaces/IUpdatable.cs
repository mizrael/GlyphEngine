using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GlyphEngine.Components;

namespace GlyphEngine.Interfaces
{
    public interface IUpdatable
    {
        void Update(GameTime gameTime);
    }
}
