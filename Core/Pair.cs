using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GlyphEngine.Core
{
    public struct Pair<T1, T2>
    {
        
        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public T1 First;
        public T2 Second;
    } 
}
