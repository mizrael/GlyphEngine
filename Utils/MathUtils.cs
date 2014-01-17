
namespace GlyphEngine.Utils
{
    public static class MathUtils
    {
        public static bool IsEven(this int number) {
            return 0 == (number & 1);
        }
    }
}
