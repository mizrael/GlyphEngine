using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GlyphEngine.Core
{
    public static class Helpers
    {
        private static Random random = new Random();
       
        public static Random Random
        {
            get { return random; }
        }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public static int RandomBetween(int min, int max)
        {
            return random.Next(min, max);
        }

        /// <summary>
        /// Returns a random value in [-eps, +eps]
        /// </summary>
        /// <param name="eps"></param>
        /// <returns></returns>
        public static float ComputeRandomFloat(float eps)
        {
            float val = (float)(random.NextDouble() * 2 - 1) * eps;
            return val;
        }

        public static T Clamp<T>(T value, T max, T min)   where T : System.IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        } 
       
        public static void Vector2Truncate(ref Vector2 vec, float max)
        {
            if (vec.Length() > max)
            {
                vec = Vector2.Normalize(vec) * max;
            } 
        }

        public static float Vector2Lenght(Vector2 vec)
        {
            return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        }

        public static float Vector2LenghtSquared(Vector2 vec)
        {
            return vec.X * vec.X + vec.Y * vec.Y;
        }
    }


    public static class ServiceExtender
    {
        public static T GetService<T>(this GameServiceContainer container) where T : class
        {
            return container.GetService(typeof(T)) as T;
        }
    }
}
