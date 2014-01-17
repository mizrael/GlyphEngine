using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GlyphEngine.SceneGraph;

namespace GlyphEngine.AI.Evaluators
{
    public interface IAIGoalEvaluator
    {
        float Evaluate(ISceneNode node);
        void SetGoal(ISceneNode node);
    }
}
