using System.Linq;

using Microsoft.Xna.Framework;

using GlyphEngine.Interfaces;
using GlyphEngine.Components;

namespace GlyphEngine.SceneGraph
{
    public abstract class ISceneNode 
    {
        #region Members

        private static int _lastID = 0;

        #endregion Members

        public ISceneNode()
        {
            this.ID = ++_lastID;

            this.Children = new SceneNodeCollection(this);
            this.Components = new ComponentCollection(this);
        }

        #region Methods

        internal void Init() 
        {
            foreach (var c in this.Components)
                c.Init();

            OnInit();
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            BeforeUpdate(gameTime);

            if (!Active)
                return;
            
            int count = Components.Count();
            for (int i = 0; i != count; ++i)
            {
                var comp = Components.ElementAt(i);
                IUpdatable upd = comp  as IUpdatable;
                if (null != upd && comp.Active)
                    upd.Update(gameTime);
            }

            this.Children.UpdateNodes(gameTime);

            AfterUpdate(gameTime);
        }

        public override int GetHashCode()
        {
            return this.ID;
        }

        public override bool Equals(object obj)
        {
            var node = obj as ISceneNode;
            return null != node ? this.ID.Equals(node.ID) : false;
        }

        #endregion Methods

        #region Interface

        protected virtual void OnInit() { }       

        protected virtual void BeforeUpdate(GameTime gameTime) { }
        protected virtual void AfterUpdate(GameTime gameTime) { }

        #endregion Interface

        #region Properties

        public readonly int ID;

        public ISceneNode Father { get; internal set; }

        public readonly SceneNodeCollection Children = null;

        public readonly ComponentCollection Components = null;

        public bool Active = true;
      
        #endregion Properties
    }

   
}
