using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GlyphEngine.Interfaces;
using GlyphEngine.Components;


namespace GlyphEngine.SceneGraph
{  
    public class SceneNodeCollection : IEnumerable<ISceneNode>
    {
        private List<ISceneNode> _nodes = new List<ISceneNode>();
        private ISceneNode _owner = null;

        public SceneNodeCollection(ISceneNode owner)
        {
            if (null == owner)
                throw new ArgumentNullException();
            _owner = owner;
        }

        public IEnumerator<ISceneNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        /// <summary>
        /// adds a node to the collection and sets parent->child relation
        /// </summary>
        /// <param name="node"></param>
        internal void Add(ISceneNode node)
        {
            if (null != node.Father && _owner != node.Father)
                this.Remove(node);

            node.Father = _owner;
            _nodes.Add(node);
        }

        /// <summary>
        /// removes a node from the collection and unsets parent->child relation
        /// </summary>
        /// <param name="node"></param>
        internal bool Remove(ISceneNode node)
        {
            if (_nodes.Remove(node))
            {
                node.Father = null;
                return true;
            }
            return false;
        }

        internal void UpdateNodes(GameTime gameTime)
        {
            int count = _nodes.Count;
            for (int i = 0; i != count; ++i)
                _nodes[i].Update(gameTime);
        }
    }
}
