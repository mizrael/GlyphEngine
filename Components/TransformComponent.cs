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
    public class TransformComponent : IComponent, IUpdatable
    {
        #region Members

        private TransformComponent _fatherTrasfComp = null;
        private bool _needCheckFatherTrasfComp = true;

        #endregion Members

        public TransformComponent(SceneNode owner)
            : base(owner)
        {       
        }

        public override void Init()
        {
        }

        public override void Dispose()
        {
            _fatherTrasfComp = null;
            _needCheckFatherTrasfComp = true;

            this.Local.Reset();
            this.World.Reset();

            this.WorldMatrix = Matrix.Identity;
        }        

        #region IUpdatable Members

        public void Update(GameTime gameTime)
        {
            if (_needCheckFatherTrasfComp && null != base.Owner.Father)
            {
                var currNode = base.Owner.Father;
                while (true)
                {
                    _fatherTrasfComp = currNode.Components.Get<TransformComponent>();
                    currNode = currNode.Father;
                    if (null != _fatherTrasfComp || null == currNode)
                        break;
                }
                _needCheckFatherTrasfComp = false;
            }

            if (null != _fatherTrasfComp)
                Transform.Inherit(ref _fatherTrasfComp.World, ref Local, ref World);
            else
                World = Local;

            this.World.ToMatrix(out this.WorldMatrix);
        }

        #endregion        

        #region Properties

        public Transform Local = new Transform();

        public Transform World = new Transform();

        public Matrix WorldMatrix = Matrix.Identity;

        #endregion Properties
    }
}
