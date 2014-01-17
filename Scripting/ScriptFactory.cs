using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlyphEngine.Scripting
{
    public static class ScriptFactory
    {
        #region Members

        private static Dictionary<string, BaseScript> _scriptsDict = new Dictionary<string, BaseScript>();

        #endregion Members

        #region Methods

        public static T Get<T>(string scriptName) where T: BaseScript, new()
        {
            BaseScript retVal = null;

            if (!_scriptsDict.TryGetValue(scriptName, out retVal) || null == retVal as T)
            {
                retVal = new T();
                retVal.Load(scriptName);

                _scriptsDict.Remove(scriptName);
                _scriptsDict.Add(scriptName, retVal);
            }

            return retVal as T;
        }

        public static void ReloadAll()
        {
            foreach (var s in _scriptsDict.Values)
                s.Load(s.Filename);
        }

        #endregion Methods
    }
}
