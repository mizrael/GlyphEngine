using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GlyphEngine.Utils;
using GlyphEngine.Components;
using GlyphEngine.GameScreens;
using GlyphEngine.SceneGraph;
using GlyphEngine.Sprite;
using GlyphEngine.Services;

namespace GlyphEngine.SceneGraph
{
    public interface ISceneNodeFactory
    {
        ISceneNode Get(string templateName, ISceneNode parent);

        void Load(ContentManager cm, string asset);
    }

    public abstract class SceneNodeFactoryBase<NodeT, TemplT> : ISceneNodeFactory
        where NodeT : ISceneNode //, new()
        where TemplT : class, ISceneNodeDecorator<NodeT>//, new()
    {
        #region Members

        private ISceneGraphService _sceneGraph = null;
        
        private Pool<NodeT> _pool = null;

        #endregion Members

        public SceneNodeFactoryBase(ISceneGraphService sceneGraph)
        {
            _sceneGraph = sceneGraph;           

            Templates = new Dictionary<string, TemplT>();

            _pool = new Pool<NodeT>(20, p => (p.Active));
        }

        #region Methods

        public ISceneNode Get(string templateName, ISceneNode parent)
        {
            NodeT retVal = null;
            TemplT template = null;

            if (Templates.TryGetValue(templateName, out template))
            {
                retVal = _pool.New();
                if (null != retVal)
                {
                    template.Decorate(retVal);

                    retVal.Active = true;

                    _sceneGraph.AddNode(retVal, parent);                        
                }
            }

            return retVal;
        }

        #endregion Methods

        #region Interface

        public abstract void Load(ContentManager cm, string asset);

        #endregion Interface

        #region Properties

        protected Dictionary<string, TemplT> Templates { get; private set; }

        #endregion Properties
    }
}
