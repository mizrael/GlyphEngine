using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GlyphEngine.SceneGraph;
using Microsoft.Xna.Framework;
using GlyphEngine.Utils;

namespace GlyphEngine.AI
{
    public enum AIGoalStatus
    {
        Inactive,
        Running,
        Completed,
        Failed
    }

    public abstract class AIGoal
    {
        #region Methods

        public void Activate(SceneNode owner)
        {
            this.Owner = owner;
            this.Status = AIGoalStatus.Running;

            if (null != this.OnActivate)
                this.OnActivate();
        }

        public abstract AIGoalStatus Process(GameTime gameTime);        

        public void Terminate()
        {
            if (null != this.OnTerminate)
                this.OnTerminate();                       
            
            this.Owner = null;

            this.Status = AIGoalStatus.Completed;
        }

        #endregion Methods

        #region Interface        

        public delegate void GoalEventHandler();

        public event GoalEventHandler OnActivate;

        public event GoalEventHandler OnTerminate;        

        #endregion Interface

        #region Properties

        public SceneNode Owner { get; private set; }

        public AIGoalStatus Status { get; protected set; }

        #endregion Properties        
    }

    public abstract class AICompositeGoal : AIGoal
    {
        #region Members

        private Queue<AIGoal> _subGoals = new Queue<AIGoal>();
        private AIGoal _currGoal = null;

        #endregion Members

        public AICompositeGoal()
        {
            base.OnTerminate += new GoalEventHandler(AICompositeGoal_OnTerminate);
        }

        #region Methods

        public void AddSubGoal<T>(T goal) where T : AIGoal
        {
            if (null != goal)
                _subGoals.Enqueue(goal);            
        }

        public void RemoveAllSubgoals()
        {
            if (null != _currGoal)
                _currGoal.Terminate();
            _currGoal = null;

            while (_subGoals.Any())
                _subGoals.Dequeue().Terminate();
            _subGoals.Clear();
        }

        protected AIGoalStatus ProcessSubgoals(GameTime gameTime)
        {
            while (_subGoals.Any() && (null == _currGoal || AIGoalStatus.Completed == _currGoal.Status || AIGoalStatus.Failed == _currGoal.Status))
            {
                if (null != _currGoal)
                {
                    _currGoal.Terminate();
                    _currGoal = null;
                }
                _currGoal = _subGoals.Dequeue();
                _currGoal.Activate(base.Owner);                
            }

            if (null != _currGoal)
            {
                var subStatus = _currGoal.Process(gameTime);
                if (AIGoalStatus.Completed == subStatus && _subGoals.Any())
                {                    
                    return AIGoalStatus.Running;
                }

                return subStatus;
            }

            return AIGoalStatus.Completed;
        }
        
        private void AICompositeGoal_OnTerminate()
        {
            this.RemoveAllSubgoals();
        }

        #endregion Methods

        #region Properties        

        public AIGoal CurrentGoal { get { return _currGoal; } }

        public bool HasSubgoals { get { return (null != _currGoal && AIGoalStatus.Running == _currGoal.Status) || _subGoals.Any(); } }

        #endregion Properties

    }

    public static class AIGoalFactory<T> where T : AIGoal, new()
    {
        private static Pool<T> _pool = new Pool<T>(g => null != g.Owner);

        public static T Get()
        {
         //  if (typeof(T).FullName.Contains("AIGoal_MoveToPosition"))
            return new T();

         //   return _pool.New();             
        }
    }
}
