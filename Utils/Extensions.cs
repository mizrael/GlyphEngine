
using Microsoft.Xna.Framework;

namespace GlyphEngine.Utils
{
    public static class ServiceExtender
    {
        public static T GetService<T>(this GameServiceContainer container) where T : class
        {
            return container.GetService(typeof(T)) as T;
        }
    }
}
