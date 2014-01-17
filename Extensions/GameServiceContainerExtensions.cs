
using Microsoft.Xna.Framework;

namespace GlyphEngine.Extensions
{
    public static class GameServiceContainerExtensions
    {
        public static T GetService<T>(this GameServiceContainer container) where T : class
        {
            return container.GetService(typeof(T)) as T;
        }
    }
}
