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

using GlyphEngine.Core;
using GlyphEngine.Interfaces;

namespace GlyphEngine.Sprite
{
    public class SpriteSheet
    {
        #region Members

        private Dictionary<string, SpriteAnimation> _animations = null;

        #endregion Members   
   
        private SpriteSheet() { }

        public SpriteAnimation GetAnimation(string name)
        {
            SpriteAnimation anim = null;
            _animations.TryGetValue(name, out anim);
            return anim;            
        }

        public SpriteAnimation GetAnimation(int id)
        {
            if (_animations.Count > id)
            {
                int curr = 0;
                string key = string.Empty;
                foreach (string k in _animations.Keys)
                {
                    if (curr == id)
                    {
                        return _animations[k];
                    }
                    ++curr;
                }
            }

            return null;
        }

        public bool CreateAnimation(string name, out SpriteAnimation anim)
        {
            anim = null;
            if (_animations.ContainsKey(name))
                return false;

            anim = new SpriteAnimation();
            anim.Name = name;
            _animations.Add(name, anim);
            
            return true;
        }

        #region Properties

        public string[] Assets { get; private set; }

        public Texture2D[] Textures { get; private set; }

        #endregion Properties

        #region Factory

        private static Dictionary<string, SpriteSheet> _dict = new Dictionary<string, SpriteSheet>();

        public static SpriteSheet Load(ContentManager cm, string asset)
        {
            SpriteSheet retVal = null;
            if (!_dict.TryGetValue(asset, out retVal))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(asset);

                XmlElement elem = null;

                XmlNodeList nodeList = doc.GetElementsByTagName("Asset");
                List<Texture2D> texList = new List<Texture2D>();
                foreach (XmlNode node in nodeList)
                {
                    elem = (XmlElement)node;
                    if (!elem.HasAttributes)
                        continue;

                    string name = elem.Attributes["name"].InnerText;

                    Texture2D tex = cm.Load<Texture2D>(name);
                    texList.Add(tex);
                }

                retVal = new SpriteSheet();
                retVal.Textures = texList.ToArray();

                nodeList = doc.GetElementsByTagName("Animation");
                retVal._animations = new Dictionary<string, SpriteAnimation>();
                if (null != nodeList)
                {
                    foreach (XmlNode node in nodeList)
                    {
                        elem = (XmlElement)node;
                        if (!elem.HasAttributes)
                            continue;

                        SpriteAnimation anim = SpriteAnimation.Read(elem);
                        if (!retVal._animations.ContainsKey(anim.Name))
                            retVal._animations.Add(anim.Name, anim);
                    }
                }

                retVal.Assets = new string[1];
                retVal.Assets[0] = asset;
            }

            return retVal;
        }

        #endregion Factory
    }

}
