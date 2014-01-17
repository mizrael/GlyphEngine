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
    public class SceneNodeCollection : IEnumerable<SceneNode>
    {
        private List<SceneNode> _nodes = new List<SceneNode>();
        private SceneNode _owner = null;

        public SceneNodeCollection(SceneNode owner)
        {
            if (null == owner)
                throw new ArgumentNullException();
            _owner = owner;
        }

        public IEnumerator<SceneNode> GetEnumerator()
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
        internal void Add(SceneNode node)
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
        internal bool Remove(SceneNode node)
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
