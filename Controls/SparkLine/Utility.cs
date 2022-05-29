using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SparkControls.Controls
{
    internal sealed class Utility
    {
        private static Pen m_Pen;

        private Utility()
        {
        }

        public static int CheckInteger(int num)
        {
            if (num > 0)
            {
                return Math.Min(num, 0x7fff);
            }
            return Math.Max(num, -32768);
        }

        public static void ClearPen()
        {
            if (m_Pen != null)
            {
                m_Pen.Dispose();
                m_Pen = null;
            }
        }

        public static int GetBorderWidth(Rectangle rect, int initWidth, int minWidth)
        {
            int num = Math.Min(rect.Width, rect.Height) / 2;
            num = Math.Min(num, initWidth);
            return Math.Max(minWidth, num);
        }

        public static int GetCornerRadius(Rectangle rect, int initRadius)
        {
            int num2 = Math.Min(rect.Width, rect.Height) / 2;
            return Math.Min(num2, initRadius);
        }

        public static void InitPen()
        {
            if (m_Pen == null)
            {
                m_Pen = (Pen)Pens.Black.Clone();
                m_Pen.DashStyle = DashStyle.Solid;
            }
        }

        public static bool SafeStringEquals(string string1, string string2, bool ignoreCase)
        {
            if ((string1 == null) || (string2 == null))
            {
                return false;
            }
            if (string1.Length != string2.Length)
            {
                return false;
            }
            if (ignoreCase)
            {
                return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
            }
            return string.Equals(string1, string2, StringComparison.Ordinal);
        }

        public static void WidenPath(ref GraphicsPath path, int width)
        {
            m_Pen.Width = width;
            path.Widen(m_Pen);
        }
    }
}