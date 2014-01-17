
namespace GlyphEngine.Extensions
{
    public static class MathExtensions
    {
        public static bool IsEven(this int number) {
            return 0 == (number & 1);
        }
    }
}
