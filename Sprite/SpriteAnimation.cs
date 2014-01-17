using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;


namespace GlyphEngine.Sprite
{
    public class SpriteAnimation
    {         
         private Point _spriteSize = Point.Zero;

         #region Properties

         public string Name = string.Empty;
       
         public int NumFrames  = 0;
       
         public Point StartPos = Point.Zero;       

         public int TextureID  = 0;        

         public float FrameInterval = 0f;

         public bool Loop = false;       

         public Vector2 SpriteCenter = Vector2.Zero;

         public Vector2 SpriteBaseCenter = Vector2.Zero;      

         public Point SpriteSize
         {
             get { return _spriteSize; }
             set { _spriteSize = value; SpriteCenter = new Vector2(_spriteSize.X / 2, _spriteSize.Y / 2); }
         }         

         public Rectangle BoundingBox = Rectangle.Empty;       

         #endregion Properties

         public override string ToString()
         {
             return Name;
         }

         public static SpriteAnimation Read(XmlElement elem)
         {
             var anim = new SpriteAnimation();

             anim.Name = elem.Attributes["name"].InnerText;
             anim.NumFrames = int.Parse(elem.Attributes["frameCount"].InnerText);
             anim.FrameInterval = float.Parse(elem.Attributes["frameInterval"].InnerText);

             int X = int.Parse(elem.Attributes["x"].InnerText);
             int Y = int.Parse(elem.Attributes["y"].InnerText);
             anim.StartPos = new Point(X, Y);

             anim.TextureID = int.Parse(elem.Attributes["texID"].InnerText);

             X = int.Parse(elem.Attributes["sizeX"].InnerText);
             Y = int.Parse(elem.Attributes["sizeY"].InnerText);
             anim._spriteSize = new Point(X, Y);

             anim.BoundingBox.Width = X;
             anim.BoundingBox.Height = Y;
                          
             anim.SpriteCenter = new Vector2((float)anim._spriteSize.X * .5f, (float)anim._spriteSize.Y * .5f);

             if (null != elem.Attributes["boundingBox"])
             {
                 var bboxSize = elem.Attributes["boundingBox"].InnerText;
                 if (!string.IsNullOrEmpty(bboxSize))
                 {
                     var splitted = bboxSize.Split(',');
                     if (null != splitted && 4 == splitted.Length)
                     {
                         anim.BoundingBox.X = int.Parse(splitted[0]);
                         anim.BoundingBox.Y = int.Parse(splitted[1]);
                         anim.BoundingBox.Width = int.Parse(splitted[2]);
                         anim.BoundingBox.Height = int.Parse(splitted[3]);
                     }
                 }                 
             }

             try {
                 anim.SpriteCenter = new Vector2(float.Parse(elem.Attributes["baseCenterX"].InnerText), float.Parse(elem.Attributes["baseCenterY"].InnerText));
             }
             catch{ 
                 anim.SpriteBaseCenter = anim.SpriteCenter;
             }

             anim.Loop = bool.Parse(elem.Attributes["loop"].InnerText);

             return anim;
         }
    }
}
