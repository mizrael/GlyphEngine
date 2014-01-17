using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GlyphEngine.Core
{
    public static class CurveFactory
    {
        private static Dictionary<string, Curve> _loadedCurves = new Dictionary<string, Curve>();

        public static Curve Load(string asset)
        {
            Curve retVal = null;
            if (!_loadedCurves.TryGetValue(asset, out retVal))
            {
                var xDoc = XDocument.Load(asset);
                var xCurves = xDoc.Root.Elements("Asset").Where(x => x.Attribute("Type").Value.Equals("Framework:Curve", StringComparison.InvariantCultureIgnoreCase));
                if(null != xCurves && 0 != xCurves.Count())
                {
                    var xCurve = xCurves.ElementAt(0);
                    retVal = new Curve()
                    {
                         PreLoop = (CurveLoopType)Enum.Parse(typeof(CurveLoopType), xCurve.Element("PreLoop").Value),
                         PostLoop = (CurveLoopType)Enum.Parse(typeof(CurveLoopType), xCurve.Element("PostLoop").Value)                         
                    };

                    var strKeys = xCurve.Element("Keys").Value;
                    var strKeysSplit = strKeys.Split(new char[] {' '},  StringSplitOptions.RemoveEmptyEntries);
                    if(null != strKeysSplit && 0 != strKeysSplit.Length)
                    {
                        for(int i=0;i!=strKeysSplit.Length;i+=5)
                        {
                            retVal.Keys.Add(new CurveKey(float.Parse(strKeysSplit[i]), float.Parse(strKeysSplit[i + 1]), float.Parse(strKeysSplit[i + 2]), float.Parse(strKeysSplit[i + 3]),
                                                         (CurveContinuity)Enum.Parse(typeof(CurveContinuity), strKeysSplit[4])));
                        }
                    }

                    retVal.ComputeTangents(CurveTangent.Smooth);
                    _loadedCurves.Add(asset, retVal);
                }

            }

            return retVal;
        }      
    }
}
