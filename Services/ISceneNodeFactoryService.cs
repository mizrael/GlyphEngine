using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GlyphEngine.SceneGraph;

namespace GlyphEngine.Services
{
    public interface ISceneNodeFactoryService
    {
        bool RegisterFactory(string name, ISceneNodeFactory factory);
        ISceneNodeFactory GetFactory(string name);
    }

    public class SceneNodeFactoryService : ISceneNodeFactoryService      
    {
        #region Members

        private Dictionary<string, ISceneNodeFactory> _factories = null;

        #endregion Members

        public SceneNodeFactoryService(Game game)
        {
            _factories = new Dictionary<string, ISceneNodeFactory>();

            game.Services.AddService(typeof(ISceneNodeFactoryService), this);
        }

        #region Methods

        public bool RegisterFactory(string name, ISceneNodeFactory factory)
        {
            if (!_factories.ContainsKey(name))
            {
                _factories.Add(name, factory);
                return true;
            }
            return false;
        }

        public ISceneNodeFactory GetFactory(string name)
        {
            ISceneNodeFactory retVal = null;
            _factories.TryGetValue(name, out retVal);
            return retVal;
        }

        #endregion Methods
    }
}
