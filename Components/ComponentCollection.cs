using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GlyphEngine.SceneGraph;
using GlyphEngine.Interfaces;

namespace GlyphEngine.Components
{
    public class ComponentCollection : IComponent, IEnumerable<IComponent>
    {
        #region Members

        private List<IComponent> _list = new List<IComponent>();
        private Dictionary<Type, IComponent> _dict = new Dictionary<Type, IComponent>();

        #endregion Members

        public ComponentCollection(SceneNode owner)
            : base(owner)
        {
        }

        #region Methods

        public override void Init()
        {
        }

        public override void Dispose()
        {
            foreach (var comp in _list)
                comp.Dispose();
            _list.Clear();
            _dict.Clear();
        }

        internal bool Add(IComponent component)
        {
           Type cType = component.GetType();
            if (_dict.ContainsKey(cType))
                return false;

            _list.Add(component);
            _dict.Add(cType, component);
            return true;
        }

        internal void Remove<T>() where T : IComponent
        {
            var cType = typeof(T);
            IComponent component = null;
            _dict.TryGetValue(cType, out component);
            if (null != component)
            {
                _dict.Remove(cType);
                _list.Remove(component);
            }
        }

        public T Get<T>() where T : IComponent
        {
            IComponent retVal = null;

            _dict.TryGetValue(typeof(T), out retVal);

            return retVal as T;
        }

        #endregion Methods

        public IComponent ElementAt(int pos)
        {
            return _list[pos];
        }

        public IEnumerator<IComponent> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
