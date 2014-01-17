using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlyphEngine.SceneGraph
{
    public interface ISceneNodeDecorator<T> where T : ISceneNode
    {
        void Decorate(T obj);
    }
}
