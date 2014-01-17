using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlyphEngine.Components;
using System.Timers;

namespace GlyphEngine.Effects
{
    public abstract class BaseEffect
    {
        #region Members
                
        private Timer _updateTimer = null;

        #endregion Members

        public BaseEffect()
        {
            _updateTimer = new Timer(1000.0 / 33);
            _updateTimer.Elapsed += new ElapsedEventHandler(_updateTimer_Elapsed);            
        }

        #region Methods

        public void Reset()
        {           
            onReset();
        }   

        public void Begin(RenderComponent rc)
        {
            RenderComponent = rc;
            
            _updateTimer.Start();

            onBegin();
        }

        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            onUpdate(0.001);
        }

        protected void Completed()
        {           
            if (-1 == this.RepeatCount)
            {
                this.Reset();
            }
            else            
            {
                RepeatCount--;
            }

            if (0 == this.RepeatCount)
            {
                _updateTimer.Stop();
            }

            if (null != this.OnComplete)
                this.OnComplete(this);
            
        }

        #endregion Methods

        #region Interface

        protected abstract void onReset();
        protected abstract void onBegin();
        protected abstract void onUpdate(double elapsed);

        #endregion Interface

        #region Events

        public delegate void CompleteEventHandler(BaseEffect sender);
        public event CompleteEventHandler OnComplete;

        #endregion Events

        #region Properties

        public int RepeatCount = 1; // -1 = unlimited

        public RenderComponent RenderComponent { get; private set; }
    
        #endregion Properties
    }

    
}
