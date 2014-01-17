using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GlyphEngine.Components;
using GlyphEngine.SceneGraph;

namespace GlyphEngine.Interfaces
{
    public abstract class IComponent
    {
        public IComponent(SceneNode owner)
        {
            if (null == (Owner = owner))
                throw new ArgumentNullException();
            
            if(null != owner.Components)
                owner.Components.Add(this);            
        }

        /// <summary>
        /// Non va invocato nel costruttore, ma possibilmente in ISceneNode.Init()
        /// </summary>
        public abstract void Init();

        public abstract void Dispose();

        public readonly SceneNode Owner = null;

        public bool Active = true;

    }

    
}
