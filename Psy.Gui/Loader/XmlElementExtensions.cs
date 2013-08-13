using System;
using System.Xml;
using Psy.Gui.Components;
using SlimMath;
using Psy.Core;

namespace Psy.Gui.Loader
{
    public static class XmlElementExtensions
    {
        public static string GetString(this XmlElement element, string attributeName)
        {
            return element.GetAttribute(attributeName);
        }

        public static AutoSize ReadAutosizeAttribute(this XmlElement xmlElement)
        {
            var attributeString = xmlElement.GetString("autoSize");

            var autosize = AutoSize.None;

            if (attributeString.Contains("height"))
            {
                autosize = autosize | AutoSize.Height;
            }
            if (attributeString.Contains("width"))
            {
                autosize = autosize | AutoSize.Width;
            }

            return autosize;
        }

        public static bool ReadBooleanAttribute(this XmlElement xmlElement, string attributeName, bool defaultValue = true)
        {
            var attributeString = xmlElement.GetString(attributeName);
            if (attributeString == "")
            {
                return defaultValue;
            }
            return attributeString.ToUpper() == "TRUE";
        }

        public static TextListDirection ReadDirection(this XmlElement xmlElement)
        {
            var attributeString = xmlElement.GetString("direction").ToLower();

            return attributeString.Contains("topdown") ? TextListDirection.TopDown : TextListDirection.BottomUp;
        }

        public static Anchor ReadAnchorAttribute(this XmlElement xmlElement)
        {
            var attributeString = xmlElement.GetString("anchor").ToLower();

            var anchor = Anchor.None;

            if (attributeString.Contains("left"))
            {
                anchor |= Anchor.Left;
            }
            if (attributeString.Contains("right"))
            {
                anchor |= Anchor.Right;
            }
            if (attributeString.Contains("top"))
            {
                anchor |= Anchor.Top;
            }
            if (attributeString.Contains("bottom"))
            {
                anchor |= Anchor.Bottom;
            }
            if (attributeString.Contains("horizontalmiddle"))
            {
                anchor |= Anchor.HorizontalMiddle;
            }
            if (attributeString.Contains("verticalmiddle"))
            {
                anchor |= Anchor.VerticalMiddle;
            }

            return anchor;
        }

        public static int ReadInteger(this XmlElement xmlElement, string attributeName, int defaultValue)
        {
            var attributeString = xmlElement.GetString(attributeName);

            if (attributeString == "")
                return 0;

            int value;
            return !int.TryParse(attributeString, out value) ? 0 : value;
        }

        public static Color4 ReadColor4(this XmlElement xmlElement, string attributeName, Color4 defaultValue)
        {
            if (!xmlElement.HasAttribute(attributeName))
            {
                return defaultValue;
            }

            var attributeString = xmlElement.GetString(attributeName);

            return attributeString.ParseColor4();
        }

        public static float ReadFloat(this XmlElement xmlElement, string attributeName, float defaultValue)
        {
            var attributeString = xmlElement.GetString(attributeName);

            if (attributeString == "")
                return defaultValue;

            float value;
            return !float.TryParse(attributeString, out value) ? 0 : value;
        }

        public static Vector2 ReadVector(this XmlElement xmlElement, string attributeName, Vector2 defaultValue = new Vector2())
        {
            var attributeString = xmlElement.GetString(attributeName);

            if (attributeString == "")
                return new Vector2();

            var parts = attributeString.Split(',');
            float x;
            float y;

            if (!float.TryParse(parts[0], out x))
            {
                throw new GuiException(string.Format("Invalid X component of vector. Value = `{0}`", parts[0]));
            }
            if (!float.TryParse(parts[1], out y))
            {
                throw new GuiException(string.Format("Invalid Y component of vector. Value = `{0}`", parts[1]));
            }

            return new Vector2(x, y);
        }

        public static PaddingRectangle ReadPadding(this XmlElement xmlElement, string attributeName)
        {
            var str = xmlElement.GetString(attributeName);
            return PaddingRectangle.Parse(str);
        }

        public static string ReadChildDataElement(this XmlElement xmlElement, string childElementName)
        {
            foreach (XmlElement element in xmlElement)
            {
                if (element.Name.Equals(childElementName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return element.InnerText;
                }
            }
            return "";
        }
    }
}