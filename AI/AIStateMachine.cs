#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion

using GlyphEngine.Core;
using GlyphEngine.SceneGraph;
using GlyphEngine.Utils;

namespace GlyphEngine.AI
{
    
    public interface AIStateBase
    {
        void Enter(AIStateMachine sm);

        void Update(AIStateMachine sm, GameTime gameTime);

        void Exit(AIStateMachine sm);
    }


    public class AIStateMachine
    {
        public AIStateMachine(SceneNode owner)
        {
            this.Owner = owner;
        }

        public void Update(GameTime gameTime)
        {
            if (!this.Paused && null != this.CurrentState)
                this.CurrentState.Update(this, gameTime);
        }

        public void SetCurrentState<T>() where T : class, AIStateBase
        {
            if (null != this.CurrentState)
            {
                this.CurrentState.Exit(this);
                this.LastState = this.CurrentState;
            }

            this.CurrentState = AIStateMachine.Get<T>();
            this.CurrentState.Enter(this);
        }

        public void RestoreLastState()
        {
            if (this.LastState != null)
            {
                this.CurrentState = this.LastState;
                this.CurrentState.Enter(this);
                this.LastState = null;
            }
        }

        #region Properties

        public readonly SceneNode Owner = null;

        public AIStateBase CurrentState
        {
            get;
            private set;
        }

        public AIStateBase LastState
        {
            get;
            private set;
        }

        public bool Paused = false;

        #endregion Properties

        /********************************************************/

        private static Dictionary<Type, Type> _states = new Dictionary<Type, Type>();

        public static AIStateBase Get<T>() where T : class, AIStateBase
        {
            Type oT = typeof(T);
            if (!_states.ContainsKey(oT))
                _states.Add(oT, oT);

            return Singleton<T>.Instance;
        }
        
    }
}
