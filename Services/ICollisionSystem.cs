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
using GlyphEngine.Components;
using GlyphEngine.Utils;

namespace GlyphEngine.Services
{
    public interface ICollisionService : IGameComponent
    {
        void Register(CollisionComponent a, CollisionComponent b);     
    }

    public class CollisionService : GameComponent, ICollisionService
    {
        #region Members

        private HashSet<int> _hashes = null;
        private Queue<CollisionPair> _pairs = null;

        #endregion Members

        public CollisionService(Game g) : base(g)
        {
            g.Services.AddService(typeof(ICollisionService), this);

            _hashes = new HashSet<int>();
            _pairs = new Queue<CollisionPair>();
        }

        public void Register(CollisionComponent a, CollisionComponent b)
        {
            var key = CollisionPair.GetHashCode(a, b);
            if (!_hashes.Contains(key))
            {
                _hashes.Add(key);
                _pairs.Enqueue(new CollisionPair(a, b));
            }
        }

        public override void Update(GameTime gameTime)
        {
            while (0 != _pairs.Count)
            {
                var pair = _pairs.Dequeue();
                pair.First.ResponseFunc(pair.Second.Owner);
                pair.Second.ResponseFunc(pair.First.Owner);
            }
            _hashes.Clear();
        }
    }

    internal class CollisionPair
    {
        #region Members

        private int _hash = 0;

        #endregion Members

        public CollisionPair(CollisionComponent a, CollisionComponent b)
        {
            this.First = a;
            this.Second = b;

            _hash = CollisionPair.GetHashCode(a, b);
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public static int GetHashCode(CollisionComponent a, CollisionComponent b)
        {
            if (null != a && null != b)
                return Helpers.ComputeHash(a.Owner.ID, b.Owner.ID);
            return 0;
        }

        #region Properties

        public CollisionComponent First = null;
        public CollisionComponent Second = null;

        #endregion Properties
    }
}
