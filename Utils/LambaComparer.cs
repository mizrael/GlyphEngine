using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlyphEngine.Utils
{
    public class LambaComparer<T> : IEqualityComparer<T>
    {
        private Func<T, T, bool> _func = null;
        private Func<T, int> _hashFunc = null;

        public LambaComparer(Func<T, T, bool> f)
        {
            if (null == f)
                throw new ArgumentNullException("Comparing function cannot be null");
            _func = f;
        }

        public LambaComparer(Func<T, T, bool> f, Func<T,int> h) : this(f)
        {
            if (null == h)
                throw new ArgumentNullException("Hash function cannot be null");
            _hashFunc = h;
        }

        public bool Equals(T x, T y)
        {
            if (null != _func)
                return _func(x, y);
            return false;
        }

        public int GetHashCode(T obj)
        {
            if (null != _hashFunc)
                return _hashFunc(obj);
            return 0;
        }

    } 
}
