using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;

using GlyphEngine.Interfaces;
using GlyphEngine.SceneGraph;
using GlyphEngine.Utils;
using GlyphEngine.Components;

namespace GlyphEngine.Services
{
    public interface ISceneGraphService : IGameComponent
    {
        void AddNode(SceneNode node, SceneNode parent);
        void RemoveNode(SceneNode node);
        SceneNode FindNodeByID(int id);
        SceneNode FindNode(ref Point point);
        SceneNode Root { get; }
    }

    public class SceneGraphService : GameComponent, ISceneGraphService
    {
        #region Members

        private SceneNode _root = null;
        private HashSet<SceneNode> _nodes = null; //used for fast lookup
        private Queue<Pair<SceneNode, SceneNode>> _addQueue = null;
        private Queue<SceneNode> _removalQueue = null;

        private int _addStep = 0;
        private int _removalStep = 0;

        #endregion Members

        public SceneGraphService(Game game)
            : base(game)
        {
        
            _addQueue = new Queue<Pair<SceneNode, SceneNode>>();
            _removalQueue = new Queue<SceneNode>();
            _nodes = new HashSet<SceneNode>();

            game.Services.AddService(typeof(ISceneGraphService), this);
        }

        #region Interface

        public override void Update(GameTime gameTime)
        {
            PerformAdd();

            if (null != _root)
                _root.Update(gameTime);

            PerformRemove();
        }

        /// <summary>
        /// adds a new node to the scenegraph
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent">if null the new node will be added as root.</param>
        public void AddNode(SceneNode node, SceneNode parent)
        {
            _addQueue.Enqueue(new Pair<SceneNode, SceneNode>(node, parent));
        }

        public void RemoveNode(SceneNode node)
        {
            if (null != node)
            {
                foreach(var child in node.Children)
                    _removalQueue.Enqueue(child);
                _removalQueue.Enqueue(node);
            }
        }

        public SceneNode FindNodeByID(int id)
        {
        //    return _nodes.Where(n => n.ID == id).FirstOrDefault();
            return FindNodeByID(id, _root);
        }

        public SceneNode FindNode(ref Point worldPoint)
        {
            return FindNode(ref worldPoint, this.Root);
        }

        #endregion Interface

        #region private Methods

        private void PerformAdd()
        {
            ++_addStep;

            if (_addStep > 10)
            {
                _addStep = 0;
                int count = _addQueue.Count;

                while (0 != count--)
                {
                    var pair = _addQueue.Dequeue();
                    var node = pair.First;
                    var parent = pair.Second;

                    if(null != node.Father && parent != node.Father)
                        node.Father.Children.Remove(node);

                    if (null == parent)
                    {
                        if (null != Root) //swap current root with the new node                        
                            node.Children.Add(Root);                       

                        _root = node;
                    }
                    else
                    {
                        parent.Children.Add(node);
                    }

                    if (!_nodes.Contains(node))
                        _nodes.Add(node);                                            

                    node.Init();
                }
            }
        }

        private void PerformRemove()
        {
            ++_removalStep;

            if (_removalStep > 33)
            {
                _removalStep = 0;

                int count = _removalQueue.Count;

                while (0 != count--)
                {
                    var node = _removalQueue.Dequeue();
                    if (null == node) 
                        continue;

                    if (_nodes.Contains(node))
                    {
                        if (null != node.Father)
                            node.Father.Children.Remove(node);
                        else if (node == _root)
                            _root = null;

                        node.Father = null;
                        node.Components.Dispose();
                        node.Active = false;
                        
                        _nodes.Remove(node);
                        node = null;
                    }
                }
            }
        }
        
        private SceneNode FindNodeByID(int id, SceneNode root)
        {
            if (null != root)
            {
                if (id == root.ID)
                    return root;     

                if (null != root.Children && 0 != root.Children.Count())
                {
                    SceneNode node = null;
                    foreach (var c in root.Children)
                    {
                        if(null != (node = FindNodeByID(id, c)))
                            return node;
                    }
                }
            }
            return null;
        }

        private SceneNode FindNode(ref Point worldPoint, SceneNode father)
        {
            if (null != father && father.Active)
            {
                var renderComp = father.Components.Get<RenderComponent>();
                if (null != renderComp && renderComp.Visible && null != renderComp.Model)
                {
                    var trasfComp = father.Components.Get<TransformComponent>();
                    if (null != trasfComp)
                    {
                        var trasfBounds = Rectangle.Empty;
                        trasfBounds.X = renderComp.Model.Bounds.X + (int)(trasfComp.World.Position.X - renderComp.Model.BaseCenter.X);
                        trasfBounds.Y = renderComp.Model.Bounds.Y + (int)(trasfComp.World.Position.Y - renderComp.Model.BaseCenter.Y);
                        trasfBounds.Width = renderComp.Model.Bounds.Width;
                        trasfBounds.Height = renderComp.Model.Bounds.Height;

                        var result = false;
                        trasfBounds.Contains(ref worldPoint, out result);
                        if (result)
                        {
                            SceneNode node = null;
                            foreach (var c in father.Children)
                            {
                                if (null != (node = FindNode(ref worldPoint, c)))
                                    return node;
                            }
                            return father;
                        }
                    }
                }
                else
                {
                    SceneNode node = null;
                    foreach (var c in father.Children)
                    {
                        if (null != (node = FindNode(ref worldPoint, c)))
                            return node;
                    }
                }
            }
            return null;
        }


        #endregion Methods

        #region Properties

        public SceneNode Root { get { return _root; } }

        #endregion Properties
    }

   
}
