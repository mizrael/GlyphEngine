using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlyphEngine.Effects
{
    public class Fade : BaseEffect
    {
        #region Members

        #endregion Members

        #region Methods

        protected override void onReset()
        {
            RenderComponent.Color.A = 255;
        }
        
        protected override void onBegin()
        {
            onReset();
        }

        protected override void onUpdate(double elapsed)
        {
            RenderComponent.Color.A -= Math.Min(this.Step, RenderComponent.Color.A);
            if (0 == RenderComponent.Color.A)
                Completed();
        }

        #endregion Methods

        #region Properties

        public byte Step = 4;

        #endregion Properties
    }
}
