using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GlyphEngine.Core
{
    public enum TextAlignment
    {
        Bottom = 0,
        BottomLeft,
        BottomRight,
        Left,
        Right,
        Middle,
        TopLeft,
        TopRight,
        Top
    }

    public static class FontHelpers
    {

        #region Print

        public static void Print(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos, float scale, Color col, bool center)
        {
            Vector2 origin = (center) ? font.MeasureString(text) * 0.5f : Vector2.Zero;
            Vector2 s = Vector2.One * scale;

            Print(spriteBatch, font, text, pos, origin, s, col);
        }

        public static void Print(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos, Vector2 origin, Vector2 scale, Color col)
        {
            spriteBatch.DrawString(font, text, pos, col, 0, origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws a string. 
        /// </summary>
        /// <param name="sb">A reference to a SpriteBatch object that will draw the text.</param>
        /// <param name="fnt">A reference to a SpriteFont object.</param>
        /// <param name="text">The text to be drawn. <remarks>If the text contains \n it
        /// will be treated as a new line marker and the text will drawn acordingy.</remarks></param>
        /// <param name="r">The screen rectangle that the rext should be drawn inside of.</param>
        /// <param name="col">The color of the text that will be drawn.</param>
        /// <param name="align">Specified the alignment within the specified screen rectangle.</param>
        /// <param name="performWordWrap">If true the words within the text will be aranged to rey and
        /// fit within the bounds of the specified screen rectangle.</param>
        /// <param name="offsett">Draws the text at a specified offset relative to the screen
        /// rectangles top left position. </param>
        /// <param name="textBounds">Returns a rectangle representing the size of the bounds of
        /// the text that was drawn.</param>
        public static void Print(SpriteBatch sb, SpriteFont font, string text, Rectangle r, Color col, TextAlignment align,
                                 bool performWordWrap, Vector2 offsett, out Rectangle textBounds)
        {
            // check if there is text to draw
            textBounds = r;
            if (text == null) return;
            if (text == string.Empty) return;

            System.Collections.Specialized.StringCollection lines = new System.Collections.Specialized.StringCollection();
            lines.AddRange(text.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries));

            // calc the size of the rect for all the text
            Rectangle tmprect = ProcessLines(font, r, performWordWrap, lines);

            // setup the position where drawing will start
            Vector2 pos = new Vector2(r.X, r.Y);
            int aStyle = 0;

            switch (align)
            {
                case TextAlignment.Bottom:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 1;
                    break;
                case TextAlignment.BottomLeft:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 0;
                    break;
                case TextAlignment.BottomRight:
                    pos.Y = r.Bottom - tmprect.Height;
                    aStyle = 2;
                    break;
                case TextAlignment.Left:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 0;
                    break;
                case TextAlignment.Middle:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 1;
                    break;
                case TextAlignment.Right:
                    pos.Y = r.Y + ((r.Height / 2) - (tmprect.Height / 2));
                    aStyle = 2;
                    break;
                case TextAlignment.Top:
                    aStyle = 1;
                    break;
                case TextAlignment.TopLeft:
                    aStyle = 0;
                    break;
                case TextAlignment.TopRight:
                    aStyle = 2;
                    break;
            }

            // draw text
            for (int idx = 0; idx < lines.Count; idx++)
            {
                string txt = lines[idx];
                Vector2 size = font.MeasureString(txt);
                switch (aStyle)
                {
                    case 0:
                        pos.X = r.X;
                        break;
                    case 1:
                        pos.X = r.X + ((r.Width / 2) - (size.X / 2));
                        break;
                    case 2:
                        pos.X = r.Right - size.X;
                        break;
                }
                // draw the line of text
                sb.DrawString(font, txt, pos + offsett, col);
                pos.Y += font.LineSpacing;
            }

            textBounds = tmprect;
        }

        internal static Rectangle ProcessLines(SpriteFont font, Rectangle r, bool performWordWrap,
                                        System.Collections.Specialized.StringCollection lines)
        {
            // llop through each line in the collection
            Rectangle bounds = r;
            bounds.Width = 0;
            bounds.Height = 0;
            int index = 0;
            float Width = 0;
            bool lineInserted = false;
            while (index < lines.Count)
            {
                // get a line of text
                string linetext = lines[index];
                //measure the line of text
                Vector2 size = font.MeasureString(linetext);

                // check if the line of text is geater then then the rect we want to draw it inside of
                if (performWordWrap && size.X > r.Width)
                {
                    // find last space character in line
                    string endspace = string.Empty;
                    // deal with trailing spaces
                    if (linetext.EndsWith(" "))
                    {
                        endspace = " ";
                        linetext = linetext.TrimEnd();
                    }

                    // get the index of the last space character
                    int i = linetext.LastIndexOf(" ");
                    if (i != -1)
                    {
                        // if there was a space grab the last word in the line
                        string lastword = linetext.Substring(i + 1);
                        // move word to next line
                        if (index == lines.Count - 1)
                        {
                            lines.Add(lastword);
                            lineInserted = true;
                        }
                        else
                        {
                            // prepend last word to begining of next line
                            if (lineInserted)
                            {
                                lines[index + 1] = lastword + endspace + lines[index + 1];
                            }
                            else
                            {
                                lines.Insert(index + 1, lastword);
                                lineInserted = true;
                            }
                        }

                        // crop last word from the line that is being processed
                        lines[index] = linetext.Substring(0, i + 1);

                    }
                    else
                    {
                        // there appear to be no space characters on this line s move to the next line
                        lineInserted = false;
                        size = font.MeasureString(lines[index]);
                        if (size.X > bounds.Width) Width = size.X;
                        bounds.Height += font.LineSpacing;// size.Y - 1;
                        index++;
                    }
                }
                else
                {
                    // this line will fit so we can skip to the next line
                    lineInserted = false;
                    size = font.MeasureString(lines[index]);
                    if (size.X > bounds.Width) bounds.Width = (int)size.X;
                    bounds.Height += font.LineSpacing;//size.Y - 1;
                    index++;
                }
            }

            // returns the size of the text
            return bounds;
        }

        #endregion Print

       
    } 
}
