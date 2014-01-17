using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GlyphEngine.Utils
{
    public static class Helpers
    {
        #region Members

        private static Random random = new Random();

        private const float COS_FRONT_8 = 0.923879532511f;
        private const float COS_SIDE_8 = 0.382683432365f;

        #endregion Members

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

        public static int ComputeHash(int key)
        {
            return (key + (key << 3)) + (key << 12);
        }

        public static int ComputeHash(int x, int y)
        {           
            return ComputeHash(x) ^ ComputeHash(y);
        }

        public static T Clamp<T>(T value, T min, T max) where T : System.IComparable<T>
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

        public static float TurnToFace(Vector2 position, Vector2 faceThis,
                                       float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = MathHelper.WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return MathHelper.WrapAngle(currentAngle + difference);
        }

        public static OrientationEnum CalculateOrientation(ref Vector2 direction)
        {
            float leftDot = Vector2.Dot(direction, Vector2.UnitY);

            if (leftDot > COS_FRONT_8)
                return OrientationEnum.s;

            if (leftDot > COS_SIDE_8)
            {
                if (direction.X < 0.0f)
                    return OrientationEnum.so;
                else
                    return OrientationEnum.se;
            }
            else if (leftDot > -COS_SIDE_8)
            {
                if (direction.X < 0.0f)
                    return OrientationEnum.o;
                else
                    return OrientationEnum.e;
            }
            else if (leftDot > -COS_FRONT_8)
            {
                if (direction.X < 0.0f)
                    return OrientationEnum.no;
                else
                    return OrientationEnum.ne;
            }

            return OrientationEnum.n;
        }


        public static void TrasformBoundingBox(ref GlyphEngine.Core.Transform world, ref Vector2 center, ref Rectangle boundingBox)
        {
            Matrix matTrans =
                    Matrix.CreateTranslation(new Vector3(-center, 0.0f)) *
                    Matrix.CreateScale(new Vector3(world.Scale, 0.0f)) *
                    Matrix.CreateRotationZ(world.Rotation) *
                    Matrix.CreateTranslation(new Vector3(world.Position, 0.0f));

            Helpers.TrasformBoundingBox(ref matTrans, ref boundingBox);
        }

        public static void TrasformBoundingBox(ref Matrix matTrans, ref Rectangle boundingBox)
        {
            Vector2 leftTop = new Vector2(boundingBox.Left, boundingBox.Top);
            Vector2 rightTop = new Vector2(boundingBox.Right, boundingBox.Top);
            Vector2 leftBottom = new Vector2(boundingBox.Left, boundingBox.Bottom);
            Vector2 rightBottom = new Vector2(boundingBox.Right, boundingBox.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref matTrans, out leftTop);
            Vector2.Transform(ref rightTop, ref matTrans, out rightTop);
            Vector2.Transform(ref leftBottom, ref matTrans, out leftBottom);
            Vector2.Transform(ref rightBottom, ref matTrans, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));
            boundingBox.X = (int)min.X;
            boundingBox.Y = (int)min.Y;
            boundingBox.Width = (int)(max.X - min.X);
            boundingBox.Height = (int)(max.Y - min.Y);
        }
    }

}
