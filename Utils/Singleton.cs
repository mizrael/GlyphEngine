using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace GlyphEngine.Utils
{
    public class Singleton<T> where T : class
    {
        public static T Instance
        {
            get
            {
                return SingletonInternal.instance;
            }
        }

        private class SingletonInternal
        {
            static SingletonInternal()
            {
            }

            internal static readonly T instance = (T)Activator.CreateInstance(typeof(T), true);
        }


    }

}
