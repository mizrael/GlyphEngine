using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using GlyphEngine.Core;
using GlyphEngine.Interfaces;
using GlyphEngine.Utils;
using GlyphEngine.SceneGraph;

namespace GlyphEngine.Components
{
    public class RigidBodyComponent : IComponent, IUpdatable
    {
        #region Members

        private TransformComponent _ownerTransformComp = null;

        private Vector2 _fDrag = Vector2.Zero;

        private Queue<Vector2> _impulseQueue = new Queue<Vector2>();
        
        #endregion Members

        public RigidBodyComponent(ISceneNode owner) : base(owner)
        {            
        }

        public override void Init()
        {
            _ownerTransformComp = base.Owner.Components.Get<TransformComponent>();
            if (null == _ownerTransformComp)
                throw new Exception("TransformComponent needed on owner container");   
        }

        public override void Dispose()
        {
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 Ftrac = Vector2.Zero;
            
            while (0 != _impulseQueue.Count)
                Ftrac += _impulseQueue.Dequeue();

            Vector2 Fdrag = Velocity * Drag * Speed;
            Vector2 F = Ftrac - Fdrag;

            Acceleration = F / Mass;
            Velocity += Acceleration * dt;

            Speed = Velocity.Length();

            _ownerTransformComp.Local.Position += Velocity * dt;

            if(Velocity.LengthSquared() > 0)
                Vector2.Normalize(ref Velocity, out _ownerTransformComp.Local.Direction);
        }

        public void ResetForces()
        {
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            Speed = 0f;
            _impulseQueue.Clear();
        }

        public void AddImpulse(ref Vector2 force)
        {
            _impulseQueue.Enqueue(force);
        }

        #region Properties
        
        public Vector2 Velocity = Vector2.Zero;
        public Vector2 Acceleration = Vector2.Zero;

        public float Speed = 0f;

        public float Drag = 1f;
        public float EnginePower = 10000.0f;
        public float Mass = 1f;

        #endregion Properties

    }
}
