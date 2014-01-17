using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GlyphEngine.Core
{
    public class Transform
    {  
        #region Properties       

        public Vector2 Position = Vector2.Zero;

        public Vector2 Scale = Vector2.One;

        public Vector2 Direction = Vector2.Zero;        

        public float Rotation = 0f;

        #endregion Properties

        #region Methods

        public void Reset()
        {
            this.Position = Vector2.Zero;
            this.Scale = Vector2.One;
            this.Direction = Vector2.Zero;
            this.Rotation = 0f;
        }

        public void ToMatrix(out Matrix matrix)
        {
            matrix = Matrix.CreateScale(this.Scale.X, this.Scale.Y, 0.0f) *
                     Matrix.CreateRotationZ(this.Rotation) *
                     Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0f);
        }

        public Matrix ToMatrix()
        {
            return Matrix.CreateScale(this.Scale.X, this.Scale.Y, 0.0f) *
                   Matrix.CreateRotationZ(this.Rotation) *
                   Matrix.CreateTranslation(this.Position.X, this.Position.Y, 0f);
        }

        #endregion Methods

        public static void Inherit(ref Transform parentWorld, ref Transform childLocal, ref Transform childWorld)
        {
            Vector2.Add(ref parentWorld.Position, ref childLocal.Position, out childWorld.Position);

            childWorld.Direction = childLocal.Direction;
            childWorld.Rotation = childLocal.Rotation;
            childWorld.Scale = childLocal.Scale;
        }
    }
}
