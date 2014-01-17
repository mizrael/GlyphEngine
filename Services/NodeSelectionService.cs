using System.Collections.Generic;
using System.Linq;
using GlyphEngine.Components;
using GlyphEngine.Extensions;
using GlyphEngine.SceneGraph;
using Microsoft.Xna.Framework;

namespace GlyphEngine.Services
{
    public class NodeSelectionService : GameComponent
    {
        #region Members

        private ISceneGraphService _sceneGraph = null;

        #endregion Members

        public NodeSelectionService(Game g)
            : base(g)
        {
            g.Services.AddService(typeof(NodeSelectionService), this);
            g.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (null == _sceneGraph)
                _sceneGraph = this.Game.Services.GetService<ISceneGraphService>();

            if (null != this.SelectionHighlightPolicy && this.SelectedNodes.Any())
                this.SelectedNodes.ForEach(n => this.SelectionHighlightPolicy.Undo(n));

            this.SelectedNodes.Clear();

            if (this.SelectionPolicies.Any())
            {
                this.SelectionPolicies.ForEach(p => p.FrameInit(gameTime));
                RecurseCheck(_sceneGraph.Root);
            }

            if (null != this.SelectionHighlightPolicy && this.SelectedNodes.Any())
                this.SelectedNodes.ForEach(n => this.SelectionHighlightPolicy.Highlight(n));
        }

        private void AddNode(SceneNode node)
        {
            if (0 == this.SelectedNodes.Count || this.AllowMultipleSelection)
                this.SelectedNodes.Add(node);

            if (null != this.SelectionHighlightPolicy)
                this.SelectionHighlightPolicy.Highlight(node);
        }

        private bool RecurseCheck(SceneNode node)
        {
            if (null != node && node.Active)
            {
                var check = this.CheckNode(node);

                foreach (var c in node.Children)
                    if (RecurseCheck(c)) return true;

                if (check)
                    AddNode(node);

                return check;                
            }
            return false;
        }

        private bool CheckNode(SceneNode node)
        {
            return null != node.Components.Get<SelectionComponent>() && this.SelectionPolicies.TrueForAll(p => p.Check(node));
        }

        #region Properties

        public readonly List<INodeSelectionPolicy> SelectionPolicies = new List<INodeSelectionPolicy>();
        public INodeSelectionHighlightPolicy SelectionHighlightPolicy = null;

        public readonly List<SceneNode> SelectedNodes = new List<SceneNode>();
        public bool AllowMultipleSelection = false;

        #endregion Properties
    }

    public interface INodeSelectionPolicy
    {
        void FrameInit(GameTime gameTime);

        bool Check(SceneNode node);
    }

    public interface INodeSelectionHighlightPolicy
    {
        void Highlight(SceneNode node);
        void Undo(SceneNode node);
    }
}
