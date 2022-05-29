using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Win32;

namespace SparkControls.Controls
{
    /// <summary>
    /// GDI绘制辅助类
    /// </summary>
    public static class GDIHelper
    {
        #region InitializeGraphics

        /// <summary>
        /// 初始化 Graphics 对象为高质量的绘制。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        public static void InitializeGraphics(Graphics g)
        {
            if (g != null)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            }
        }

        #endregion

        #region DrawImage

        /// <summary>
        /// 在指定区域绘制图片(平铺绘制)。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="img">要绘制的图片。</param>
        /// <param name="opacity">透明度。</param>
        public static void DrawImage(Graphics g, Rectangle rect, Image img, float opacity)
        {
            if (opacity <= 0)
            {
                return;
            }

            using (ImageAttributes imgAttributes = new ImageAttributes())
            {
                GDIHelper.SetImageOpacity(imgAttributes, opacity >= 1 ? 1 : opacity);
                Rectangle imageRect = new Rectangle(rect.X, rect.Y + rect.Height / 2 - img.Size.Height / 2, img.Size.Width, img.Size.Height);
                g.DrawImage(img, rect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
            }
        }

        /// <summary>
        /// 在指定区域绘制图片(平铺绘制)。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="img">要绘制的图片。</param>
        public static void DrawImage(Graphics g, Rectangle rect, Image img)
        {
            g.DrawImage(img, rect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// 按照指定区域绘制指定尺寸的图片。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="img">要绘制的图片。</param>
        /// <param name="imgSize">图片尺寸。.</param>
        public static void DrawImage(Graphics g, Rectangle rect, Image img, Size imgSize)
        {
            if (g != null && img != null)
            {
                int x = rect.X + rect.Width / 2 - imgSize.Width / 2;
                int y = rect.Y + rect.Height / 2 - imgSize.Height / 2;
                Rectangle imageRect = new Rectangle(x, y, imgSize.Width, imgSize.Height);
                g.DrawImage(img, imageRect);
            }
        }

        #endregion

        #region Draw image and string

        /// <summary>
        /// 在指定的区域绘制绘制图像和文字
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect"></param>
        /// <param name="image">The image.</param>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="forceColor">Color of the force.</param>
        /// User:K.Anding  CreateTime:2011-7-24 22:07.
        public static void DrawImageAndString(Graphics g, Rectangle rect, Image image, Size imageSize, string text, Font font, Color forceColor)
        {
            int x = rect.X, y = rect.Y;
            Size size = MeasureString(g, text, font).ToSize();
            int len = Convert.ToInt32(size.Width);
            x += rect.Width / 2 - len / 2;
            if (image != null)
            {
                x -= imageSize.Width / 2;
                Rectangle imageRect = new Rectangle(x, y + rect.Height / 2 - imageSize.Height / 2, imageSize.Width, imageSize.Height);
                g.DrawImage(image, imageRect);
                x += imageSize.Width + 2;
            }

            //g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit; 设置这个属性，表格界面会闪烁
            using (SolidBrush brush = new SolidBrush(forceColor))
            {
                g.DrawString(text, font, brush, x, y + (rect.Height - size.Height) / 2);
            }
        }

        /// <summary>
        /// 在指定的区域绘制绘制文字
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="rect"></param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="forceColor">Color of the force.</param>
        /// <param name="align">对齐方式</param>
        /// User:K.Anding  CreateTime:2011-7-24 22:07.
        public static void DrawString(Graphics g, RectangleF rect, string text, Font font, Color forceColor, StringAlignment align = StringAlignment.Center)
        {
            using (SolidBrush brush = new SolidBrush(forceColor))
            {
                var format = new StringFormat()
                {
                    Trimming = StringTrimming.EllipsisCharacter,
                    Alignment = align,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, font, brush, rect, format);
            }
        }

        /// <summary>
        /// 在指定的区域绘制绘制文字
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="rect">区域</param>
        /// <param name="text">绘制的文字</param>
        /// <param name="font">文字的字体</param>
        /// <param name="foreColor">字体的颜色</param>
        /// <param name="stringFormat">绘制格式</param>
        public static void DrawString(Graphics g, RectangleF rect, string text, Font font, Color foreColor, StringFormat stringFormat)
        {
            using (SolidBrush brush = new SolidBrush(foreColor))
            {
                g.DrawString(text, font, brush, rect, stringFormat);
            }
        }
        #endregion

        #region FillRectangle

        /// <summary>
        /// 使用渐变色渲染一个矩形区域。
        /// Fills the rectangle.
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">填充区域。.</param>
        /// <param name="color">填充色。</param>
        public static void FillRectangle(Graphics g, Rectangle rect, GradientColor color)
        {
            if (g != null && rect.Width > 0 && rect.Height > 0)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, color.First, color.Second, LinearGradientMode.Vertical))
                {
                    brush.Blend.Factors = color.Factors;
                    brush.Blend.Positions = color.Positions;
                    g.FillRectangle(brush, rect);
                }
            }
        }

        /// <summary>
        /// 使用纯色渲染一个矩形区域。
        /// Fills the rectangle.
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">填充区域。.</param>
        /// <param name="color">填充色。</param>
        public static void FillRectangle(Graphics g, Rectangle rect, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || g == null)
            {
                return;
            }

            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// 使用纯色渲染一个矩形区域。
        /// Fills the rectangle.
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">填充区域。.</param>
        /// <param name="color">填充色。</param>
        public static void FillRectangle(Graphics g, RectangleF rect, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0 || g == null)
            {
                return;
            }

            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// 使用渐变色渲染一个圆角矩形区域。
        /// Fills the rectangle.
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="roundRect">填充区域。.</param>
        /// <param name="color">填充色。</param>
        public static void FillRectangle(Graphics g, RoundRectangle roundRect, GradientColor color)
        {
            if (roundRect.Rect.Width <= 0 || roundRect.Rect.Height <= 0)
            {
                return;
            }

            using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(roundRect.Rect, color.First, color.Second, LinearGradientMode.Vertical))
                {
                    brush.Blend.Factors = color.Factors;
                    brush.Blend.Positions = color.Positions;
                    g.FillPath(brush, path);
                }
            }
        }

        /// <summary>
        /// 使用纯色渲染一个圆角矩形区域。
        /// Fills the rectangle.
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="roundRect">填充区域。.</param>
        /// <param name="color">填充色。</param>
        public static void FillRectangle(Graphics g, RoundRectangle roundRect, Color color)
        {
            if (roundRect.Rect.Width <= 0 || roundRect.Rect.Height <= 0)
            {
                return;
            }

            using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
            {
                using (Brush brush = new SolidBrush(color))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        #endregion

        #region FillPath

        /// <summary>
        /// 绘制一个多边形区域。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="path">区域路径。</param>
        /// <param name="color">边框颜色。</param>
        public static void FillPath(Graphics g, GraphicsPath path, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            {
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// 使用渐变色渲染一个图形区域。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="path">渲染路径。</param>
        /// <param name="rect">填充区域。.</param>
        /// <param name="color">填充色。</param>
        public static void FillPath(Graphics g, GraphicsPath path, Rectangle rect, GradientColor color)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, color.First, color.Second, LinearGradientMode.Vertical))
            {
                brush.Blend.Factors = color.Factors;
                brush.Blend.Positions = color.Positions;
                g.FillPath(brush, path);
            }
        }

        /// <summary>
        /// 使用渐变色渲染一个圆角区域。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="color1">填充色1。</param>
        /// <param name="color2">填充色2。</param>
        /// <param name="blend">色彩混合方案。</param>
        public static void FillPath(Graphics g, RoundRectangle roundRect, Color color1, Color color2, Blend blend)
        {
            FillRectangle(g, roundRect, new GradientColor(color1, color2, blend.Factors, blend.Positions));
        }

        /// <summary>
        /// 使用渐变色渲染一个圆角区域。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="color1">填充色1。</param>
        /// <param name="color2">填充色2。</param>
        public static void FillPath(Graphics g, RoundRectangle roundRect, Color color1, Color color2)
        {
            if (roundRect.Rect.Width > 0 && roundRect.Rect.Height > 0)
            {
                using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        roundRect.Rect, color1, color2, LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        /// <summary>
        /// 使用渐变色渲染一个圆角区域。
        /// </summary>
        /// <param name="g"></param>
        /// <param name="roundRect"></param>
        /// <param name="colorBlend"></param>
        public static void FillPath(Graphics g, RoundRectangle roundRect, ColorBlend colorBlend)
        {
            if (roundRect.Rect.Width > 0 && roundRect.Rect.Height > 0)
            {
                using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
                {
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.InterpolationColors = colorBlend;
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        /// <summary>
        /// 使用纯色渲染一个圆角区域。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="color">填充色。</param>
        public static void FillPath(Graphics g, RoundRectangle roundRect, Color color)
        {
            FillPath(g, roundRect, color, color);
        }

        #endregion

        #region DrawPathBorder

        /// <summary>
        /// 绘制控件非工作区域边框
        /// </summary>
        /// <param name="control">绘制控件</param>
        /// <param name="borderColor">边框颜色</param>
        /// <param name="drawOther">绘制其他内容</param>
        public static void DrawNonWorkAreaBorder(Control control, Color borderColor, Action<Graphics> drawOther = null)
        {
            if (control == null || !control.IsHandleCreated) return;
            IntPtr hDC = NativeMethods.GetWindowDC(control.Handle);
            if (hDC.ToInt32() == 0) return;
            Graphics g = Graphics.FromHdc(hDC);
            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
            ControlPaint.DrawBorder(g, rect, borderColor, ButtonBorderStyle.Solid);
            drawOther?.Invoke(g);
            g.Dispose();
            NativeMethods.ReleaseDC(control.Handle, hDC);
        }

        /// <summary>
        /// 绘制一个多边形区域。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="path">区域路径。</param>
        /// <param name="color">边框颜色。</param>
        public static void DrawRectangle(Graphics g, Rectangle path, Color color)
        {
            using (Pen pen = new Pen(color, 1))
            {
                g.DrawRectangle(pen, path);
            }
        }

        /// <summary>
        /// 绘制一个多边形区域。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="path">区域路径。</param>
        /// <param name="color">边框颜色。</param>
        /// <param name="borderWidth">线宽。</param>
        public static void DrawRectangle(Graphics g, RectangleF path, Color color, float borderWidth = 1)
        {
            using (Pen pen = new Pen(color, borderWidth))
            {
                g.DrawRectangle(pen, path.X, path.Y, path.Width, path.Height);
            }
        }

        /// <summary>
        /// 绘制一个多边形区域。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="path">区域路径。</param>
        /// <param name="color">边框颜色。</param>
        public static void DrawPathBorder(Graphics g, GraphicsPath path, Color color)
        {
            using (Pen pen = new Pen(color, 1))
            {
                g.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// 绘制一个多边形区域。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="borderColor">边框颜色。</param>
        public static void DrawPathBorder(Graphics g, RoundRectangle roundRect, Color borderColor)
        {
            DrawPathBorder(g, roundRect, borderColor, 1);
        }

        /// <summary>
        /// 绘制指定区域的边框。<br/>
        /// 注意:若要设置边框宽度，需要自己调整区域，它是向外绘制的
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="borderColor">边框颜色。</param>
        /// <param name="borderWidth">边框宽度。</param>
        public static void DrawPathBorder(Graphics g, RoundRectangle roundRect, Color borderColor, float borderWidth)
        {
            using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
            {
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// 绘制指定区域的内边框。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="borderColor">边框颜色。</param>
        public static void DrawPathInnerBorder(Graphics g, RoundRectangle roundRect, Color borderColor)
        {
            RectangleF rect = roundRect.Rect;
            rect.X++; rect.Y++; rect.Width -= 2; rect.Height -= 2;
            DrawPathBorder(g, new RoundRectangle(rect, roundRect.CornerRadius), borderColor);
        }

        /// <summary>
        /// 绘制指定区域的内边框。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="borderColor">边框颜色。</param>
        /// <param name="borderWidth">边框宽度。</param>
        public static void DrawPathInnerBorder(Graphics g, RoundRectangle roundRect, Color borderColor, int borderWidth)
        {
            RectangleF rect = roundRect.Rect;
            rect.X++; rect.Y++; rect.Width -= 2; rect.Height -= 2;
            DrawPathBorder(g, new RoundRectangle(rect, roundRect.CornerRadius), borderColor, borderWidth);
        }

        /// <summary>
        /// 绘制指定区域的外边框。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="borderColor">边框颜色。</param>
        public static void DrawPathOuterBorder(Graphics g, RoundRectangle roundRect, Color borderColor)
        {
            RectangleF rect = roundRect.Rect;
            rect.X--; rect.Y--; rect.Width += 2; rect.Height += 2;
            DrawPathBorder(g, new RoundRectangle(rect, roundRect.CornerRadius), borderColor);
        }

        /// <summary>
        /// 绘制指定区域的外边框。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="roundRect">圆角区域。</param>
        /// <param name="borderColor">边框颜色。</param>
        /// <param name="borderWidth">边框宽度。</param>
        public static void DrawPathOuterBorder(Graphics g, RoundRectangle roundRect, Color borderColor, int borderWidth)
        {
            RectangleF rect = roundRect.Rect;
            rect.X--; rect.Y--; rect.Width += 2; rect.Height += 2;
            DrawPathBorder(g, new RoundRectangle(rect, roundRect.CornerRadius), borderColor, borderWidth);
        }

        #endregion

        #region DrawGradientLine

        /// <summary>
        /// 绘制阶梯渐变的线条，可以在参数Blend对象中设置色彩混合规则。
        /// </summary>
        public static void DrawGradientLine(Graphics g, Color lineColor, Blend blend, int angle, int lineWidth, int x1, int y1, int x2, int y2)
        {
            Color c1 = lineColor;
            Color c2 = Color.FromArgb(10, c1);
            Rectangle rect = new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, c1, c2, angle))
            {
                brush.Blend = blend;
                using (Pen pen = new Pen(brush, lineWidth))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }

        /// <summary>
        /// 绘制向两边阶梯渐变的线条。
        /// </summary>
        public static void DrawGradientLine(Graphics g, Color lineColor, int angle, int x1, int y1, int x2, int y2)
        {
            Blend blend = new Blend
            {
                Positions = new float[] { 0f, .15f, .5f, .85f, 1f },
                Factors = new float[] { 1f, .4f, 0f, .4f, 1f }
            };
            DrawGradientLine(g, lineColor, blend, angle, 1, x1, y1, x2, y2);
        }

        #endregion

        #region SetImageOpacity

        /// <summary>
        /// 设置图片透明度。
        /// </summary>
        /// <param name="imgAttributes">图片信息。</param>
        /// <param name="opacity">透明度，0：完全透明，1：不透明。</param>
        public static void SetImageOpacity(ImageAttributes imgAttributes, float opacity)
        {
            float[][] nArray ={
                new float[] { 1, 0, 0, 0, 0 },
                new float[] { 0, 1, 0, 0, 0 },
                new float[] { 0, 0, 1, 0, 0 },
                new float[] { 0, 0, 0, opacity, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            };
            imgAttributes.SetColorMatrix(new ColorMatrix(nArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        #endregion

        #region DrawArrow

        /// <summary>
        /// 绘制指针（方向箭头）,默认指针颜色为（55, 63, 78）
        /// </summary>
        /// <param name="g">The Graphics.</param>
        /// <param name="direction">指针的方向.</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="arrowSize">箭头尺寸。</param>
        public static void DrawArrow(Graphics g, ArrowDirection direction, Rectangle rect, Size arrowSize)
        {
            DrawArrow(g, direction, rect, arrowSize, 1.8f, Color.FromArgb(55, 63, 78));
        }

        /// <summary>
        /// 绘制箭头
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="direction">箭头方向.</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="arrowSize">箭头尺寸。</param>
        /// <param name="offset">凹进偏移。</param>
        /// <param name="color">箭头颜色。</param>
        public static void DrawArrow(Graphics g, ArrowDirection direction, RectangleF rect, Size arrowSize, float offset, Color color)
        {
            PointF center = new PointF(rect.X + rect.Width / 2.0F, rect.Y + rect.Height / 2.0F);
            GraphicsPath path = new GraphicsPath();

            PointF[] points = null;
            switch (direction)
            {
                case ArrowDirection.Down:
                    points = new PointF[]
                    {
                            new PointF(center.X, center.Y + arrowSize.Height / 2.0F),
                            new PointF(center.X - arrowSize.Width / 2.0F,center.Y - arrowSize.Height / 2.0F),
                            new PointF(center.X, center.Y - arrowSize.Height / 2 + offset),
                            new PointF(center.X + arrowSize.Width / 2.0F,center.Y - arrowSize.Height / 2.0F)
                    };
                    break;
                case ArrowDirection.Up:
                    points = new PointF[]
                    {
                            new PointF(center.X, center.Y - arrowSize.Height / 2.0F),
                            new PointF(center.X - arrowSize.Width / 2.0F,center.Y + arrowSize.Height / 2.0F),
                            new PointF(center.X, center.Y + arrowSize.Height / 2.0F - offset),
                            new PointF(center.X + arrowSize.Width / 2.0F,center.Y + arrowSize.Height / 2.0F)
                    };
                    break;
                case ArrowDirection.Left:
                    points = new PointF[]
                    {
                            new PointF(center.X - arrowSize.Width / 2.0F, center.Y),
                            new PointF(center.X + arrowSize.Width / 2.0F, center.Y - arrowSize.Height / 2.0F),
                            new PointF(center.X + arrowSize.Width / 2.0F - offset, center.Y),
                            new PointF(center.X + arrowSize.Width / 2.0F, center.Y + arrowSize.Height / 2.0F)
                    };
                    break;
                case ArrowDirection.Right:
                    points = new PointF[]
                    {
                            new PointF(center.X + arrowSize.Width / 2.0F,center.Y),
                            new PointF(center.X - arrowSize.Width / 2.0F,center.Y - arrowSize.Height / 2.0F),
                            new PointF(center.X - arrowSize.Width / 2.0F+offset,center.Y),
                            new PointF(center.X - arrowSize.Width / 2.0F,center.Y + arrowSize.Height / 2.0F)
                    };
                    break;
            }

            path.AddLines(points);
            using (Brush brush = new SolidBrush(color))
            {
                g.FillPath(brush, path);
            }
        }

        #endregion

        #region DrawCrystalButton

        /// <summary>
        /// 绘制水晶按钮。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="surroundColor">外围色。</param>
        /// <param name="centerColor">中心色。</param>
        /// <param name="lightColor">浅色。</param>
        /// <param name="blend">渐变因子。</param>
        public static void DrawCrystalButton(Graphics g, Rectangle rect, Color surroundColor, Color centerColor, Color lightColor, Blend blend)
        {
            int sweep, start;
            Point p1, p2, p3;
            Rectangle rinner = rect;
            rinner.Inflate(-1, -1);
            using (GraphicsPath p = new GraphicsPath())
            {
                p.AddEllipse(rect);

                using (PathGradientBrush gradient = new PathGradientBrush(p))
                {
                    gradient.WrapMode = WrapMode.Clamp;
                    gradient.CenterPoint = new PointF(Convert.ToSingle(rect.Left + rect.Width / 2), Convert.ToSingle(rect.Bottom));
                    gradient.CenterColor = centerColor;
                    gradient.SurroundColors = new Color[] { surroundColor };
                    gradient.Blend = blend;
                    g.FillPath(gradient, p);
                }
            }

            // Bottom round shine
            Rectangle bshine = new Rectangle(0, 0, rect.Width / 2, rect.Height / 2);
            bshine.X = rect.X + (rect.Width - bshine.Width) / 2;
            bshine.Y = rect.Y + rect.Height / 2;

            using (GraphicsPath p = new GraphicsPath())
            {
                p.AddEllipse(bshine);

                using (PathGradientBrush gradient = new PathGradientBrush(p))
                {
                    gradient.WrapMode = WrapMode.Clamp;
                    gradient.CenterPoint = new PointF(Convert.ToSingle(rect.Left + rect.Width / 2), Convert.ToSingle(rect.Bottom));
                    gradient.CenterColor = Color.White;
                    gradient.SurroundColors = new Color[] { Color.Transparent };

                    g.FillPath(gradient, p);
                }
            }

            // Upper Glossy
            using (GraphicsPath p = new GraphicsPath())
            {
                sweep = 160;
                start = 180 + (180 - sweep) / 2;
                p.AddArc(rinner, start, sweep);

                p1 = Point.Round(p.PathData.Points[0]);
                p2 = Point.Round(p.PathData.Points[p.PathData.Points.Length - 1]);
                p3 = new Point(rinner.Left + rinner.Width / 2, p2.Y - 3);
                p.AddCurve(new Point[] { p2, p3, p1 });

                using (PathGradientBrush gradient = new PathGradientBrush(p))
                {
                    gradient.WrapMode = WrapMode.Clamp;
                    gradient.CenterPoint = p3;
                    gradient.CenterColor = Color.Transparent;
                    gradient.SurroundColors = new Color[] { lightColor };

                    blend = new Blend(3);
                    blend.Factors = new float[] { .3f, .8f, 1f };
                    blend.Positions = new float[] { 0, 0.50f, 1f };
                    gradient.Blend = blend;

                    g.FillPath(gradient, p);
                }

                using (LinearGradientBrush b = new LinearGradientBrush(new Point(rect.Left, rect.Top), new Point(rect.Left, p1.Y), Color.White, Color.Transparent))
                {
                    blend = new Blend(4)
                    {
                        Factors = new float[] { 0f, .4f, .8f, 1f },
                        Positions = new float[] { 0f, .3f, .4f, 1f }
                    };
                    b.Blend = blend;
                    g.FillPath(b, p);
                }
            }

            // Upper shine
            using (GraphicsPath p = new GraphicsPath())
            {
                sweep = 160;
                start = 180 + (180 - sweep) / 2;
                p.AddArc(rinner, start, sweep);

                using (Pen pen = new Pen(Color.White))
                {
                    g.DrawPath(pen, p);
                }
            }

            // Lower Shine
            using (GraphicsPath p = new GraphicsPath())
            {
                sweep = 160;
                start = (180 - sweep) / 2;
                p.AddArc(rinner, start, sweep);
                Point pt = Point.Round(p.PathData.Points[0]);

                Rectangle rrinner = rinner; rrinner.Inflate(-1, -1);
                sweep = 160;
                start = (180 - sweep) / 2;
                p.AddArc(rrinner, start, sweep);

                using (LinearGradientBrush b = new LinearGradientBrush(
                    new Point(rinner.Left, rinner.Bottom),
                    new Point(rinner.Left, pt.Y - 1),
                    lightColor, Color.FromArgb(50, lightColor)))
                {
                    g.FillPath(b, p);
                }
            }
        }

        #endregion

        #region EllipseRender

        /// <summary>
        /// 绘制一个圆形。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="borderColor">边框颜色。</param>
        /// <param name="borderWidth">边框宽度。</param>
        public static void DrawEllipse(Graphics g, Rectangle rect, Color borderColor, int borderWidth)
        {
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                g.DrawEllipse(pen, rect);
            }
        }

        /// <summary>
        /// 渲染一个圆形区域(纯色)。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="color">填充色。</param>
        public static void FillEllipse(Graphics g, Rectangle rect, Color color)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillEllipse(brush, rect);
            }
        }

        /// <summary>
        /// 渲染一个圆形区域(渐变)。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="rect">绘制区域。</param>
        /// <param name="color1">填充色1。</param>
        /// <param name="color2">填充色2。</param>
        public static void FillEllipse(Graphics g, Rectangle rect, Color color1, Color color2)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = color1;
                    brush.SurroundColors = new Color[] { color2 };
                    brush.Blend = new Blend
                    {
                        Factors = new float[] { 0f, 0.8f, 1f },
                        Positions = new float[] { 0f, 0.5f, 1f }
                    };
                    g.FillPath(brush, path);
                }
            }
        }

        #endregion

        #region DrawCheckBox

        ///// <summary>
        ///// 绘制选择状态(勾选状态)
        ///// 手动绘制勾勾
        ///// </summary>
        ///// User:K.Anding  CreateTime:2011-7-24 21:59.
        //public static void DrawCheckedState(Graphics g, Rectangle rect, Color color)
        //{
        //	PointF[] points = new PointF[3];
        //	points[0] = new PointF(
        //		rect.X + rect.Width / 5f,
        //		rect.Y + rect.Height / 2.5f);
        //	points[1] = new PointF(
        //		rect.X + rect.Width / 2.5f,
        //		rect.Bottom - rect.Height / 3.6f);
        //	points[2] = new PointF(
        //		rect.Right - rect.Width / 5.0f,
        //		rect.Y + rect.Height / 7.0f);
        //	using (Pen pen = new Pen(color, 2))
        //	{
        //		g.SmoothingMode = SmoothingMode.AntiAlias;
        //		g.CompositingQuality = CompositingQuality.HighQuality;
        //		g.DrawLines(pen, points);
        //	}
        //}

        /// <summary>
        /// 绘制选择状态(勾选状态)
        /// 设计好的图片勾
        /// </summary>
        /// User:K.Anding  CreateTime:2011-7-24 21:59.
        public static void DrawCheckedStateByImage(Graphics g, Rectangle rect)
        {
            rect.Inflate(-1, -1);
            g.DrawImage(Properties.Resources.Check, rect);
        }

        ///// <summary>
        ///// 绘制选择状态
        ///// </summary>
        ///// User:K.Anding  CreateTime:2011-7-24 21:59.
        //public static void DrawCheckedState(Graphics g, Rectangle rect)
        //{
        //	DrawCheckedState(g, rect, Color.Green);
        //}

        /// <summary>
        /// 绘制checkbox的边框和背景
        /// </summary>
        /// <param name="g"></param>
        /// <param name="roundRect"></param>
        /// <param name="baseColor"></param>
        /// <param name="borderColor"></param>
        public static void DrawCheckBox(Graphics g, RoundRectangle roundRect, Color baseColor, Color borderColor)
        {
            DrawCheckBox(g, roundRect, baseColor, borderColor, 1);

            //using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
            //{
            //    using (PathGradientBrush brush = new PathGradientBrush(path))
            //    {
            //        brush.CenterColor = baseColor;
            //        brush.SurroundColors = new Color[] { borderColor };
            //        Blend blend = new Blend();
            //        blend.Positions = new float[] { 0f, 0.18f, 1f };
            //        blend.Factors = new float[] { 0f, 0.89f, 1f };
            //        brush.Blend = blend;
            //        g.FillPath(brush, path);
            //    }

            //    DrawPathBorder(g, roundRect, borderColor);
            //}
        }

        #endregion

        #region GetOppositeColor

        /// <summary>
        /// 获取指定颜色的相反颜色。
        /// </summary>
        /// <param name="sourceColor">源色。</param>
        /// <returns>反色。</returns>
        public static Color GetOppositeColor(Color sourceColor)
        {
            return Color.FromArgb(255 - sourceColor.A, 255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
        }

        #endregion

        #region GetOppositeColor

        /// <summary>
        /// 测量用指定字体绘制的指定字符串。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="text">要绘制的字符串。</param>
        /// <param name="font">字符串的字体格式。</param>
        /// <returns>绘制字符串的尺寸。</returns>
        public static SizeF MeasureString(Graphics g, string text, Font font)
        {
            return MeasureString(g, text, font, new StringFormat(default(StringFormatFlags)) { Trimming = StringTrimming.None });
        }

        /// <summary>
        /// 测量用指定字体绘制的指定字符串。
        /// </summary>
        /// <param name="g">GDI 绘图图面。</param>
        /// <param name="text">要绘制的字符串。</param>
        /// <param name="font">字符串的字体格式。</param>
        /// <param name="format">字符串的格式化信息。</param>
        /// <returns>绘制字符串的尺寸。</returns>
        public static SizeF MeasureString(Graphics g, string text, Font font, StringFormat format)
        {
            if (text.IsNullOrEmpty()) { return SizeF.Empty; }
            if (g == null) { throw new ArgumentNullException(nameof(g), "值不能为空。"); }
            if (font == null) { throw new ArgumentNullException(nameof(font), "值不能为空。"); }
            if (format == null) { format = new StringFormat(default(StringFormatFlags)) { Trimming = StringTrimming.None }; }

            CharacterRange[] charRanges;
            if (text.Length > 32)
            {
                // more than 32 ranges not allowed for SetMeasurableCharacterRanges
                charRanges = new[] { new CharacterRange(0, text.Length) };
            }
            else
            {
                charRanges = new CharacterRange[text.Length];
                for (var i = 0; i < text.Length; i++)
                {
                    charRanges[i] = new CharacterRange(i, 1);
                }
            }
            format.SetMeasurableCharacterRanges(charRanges);

            var ranges = g.MeasureCharacterRanges(text, font, new RectangleF(0, 0, float.MaxValue, float.MaxValue), format);
            return new SizeF(ranges.Sum(r => r.GetBounds(g).Width), ranges.Max(r => r.GetBounds(g).Height));
        }

        #endregion

        #region DrawCircle

        /// <summary>
        ///绘制矩形的同心圆
        /// </summary>
        /// <param name="g"></param>
        /// <param name="backColor"></param>
        /// <param name="borderColor"></param>
        /// <param name="borderWidth"></param> 
        /// <param name="rect"></param>
        public static void DrawCircle(Graphics g, Color backColor, Color borderColor, float borderWidth, RectangleF rect)
        {
            //先算圆的直径
            float diameter = Math.Min(rect.Height, rect.Width);
            //圆心点
            //PointF dimcenterPoint = new PointF(rect.X + rect.Width / 2.0F, rect.Y + rect.Height / 2.0F);
            g.FillEllipse(new SolidBrush(backColor), rect.X, rect.Y, diameter, diameter);
            g.DrawEllipse(new Pen(borderColor, borderWidth), rect.X, rect.Y, diameter, diameter);
        }

        #endregion

        #region DrawX

        /// <summary>
        ///绘制矩形同心圆的X
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineColor">线条颜色</param>
        /// <param name="margin">边距</param>
        /// <param name="borderWidth">线宽</param> 
        /// <param name="rect">矩形</param>
        public static void DrawXByCircle(Graphics g, Color lineColor, float margin, float borderWidth, RectangleF rect)
        {
            var points = GetXByCircle(rect, margin);
            using (Pen pen = new Pen(lineColor, borderWidth))
            {
                g.DrawLine(pen, points[0], points[1]);
                g.DrawLine(pen, points[2], points[3]);
            }
        }

        /// <summary>
        ///绘制矩形中的X
        /// </summary>
        /// <param name="g"></param>
        /// <param name="lineColor">线条颜色</param>
        /// <param name="margin">边距</param>
        /// <param name="borderWidth">线宽</param> 
        /// <param name="rect">矩形</param>
        public static void DrawXByRectangle(Graphics g, Color lineColor, float margin, float borderWidth, RectangleF rect)
        {
            var points = GetXByRectangle(rect, margin);
            using (Pen pen = new Pen(lineColor, borderWidth))
            {
                g.DrawLine(pen, points[0], points[1]);
                g.DrawLine(pen, points[2], points[3]);
            }
        }

        #endregion

        #region DrawCheckBox

        public static void DrawCheckBox(Graphics g, RoundRectangle roundRect, Color backColor, Color borderColor, float borderWidth = 1)
        {

            using (SolidBrush brush = new SolidBrush(backColor))
            {
                //brush.CenterColor = backColor;
                //brush.SurroundColors = new Color[] { backColor };
                //brush.Blend = new Blend
                //{
                //    Positions = new float[] { 1f, 1f, 1f },
                //    Factors = new float[] { 1f, 1f, 1f }
                //};
                g.FillRectangle(brush, roundRect.Rect);
            }
            GDIHelper.DrawPathBorder(g, roundRect, borderColor, borderWidth);
            //using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
            //{
            //    using (PathGradientBrush brush = new PathGradientBrush(path))
            //    {
            //        brush.CenterColor = backColor;
            //        brush.SurroundColors = new Color[] { backColor };
            //        brush.Blend = new Blend
            //        {
            //            Positions = new float[] { 1f, 1f, 1f },
            //            Factors = new float[] { 1f, 1f, 1f }
            //        };
            //        g.FillPath(brush, path);
            //    }
            //    GDIHelper.DrawPathBorder(g, roundRect, borderColor, borderWidth);
            //}
        }

        public static void DrawCheckTick(Graphics g, RectangleF rect, Color color)
        {
            PointF[] points = new PointF[3];
            points[0] = new PointF(
                rect.X + rect.Width / 5f,
                rect.Y + rect.Height / 2.5f
            );
            points[1] = new PointF(
                rect.X + rect.Width / 2.5f,
                rect.Bottom - rect.Height / 4f
            );
            points[2] = new PointF(
                rect.Right - rect.Width / 5.0f,
                rect.Y + rect.Height / 7.0f
            );
            using (Pen pen = new Pen(color, 2))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawLines(pen, points);
            }
        }

        #endregion

        #region Calc

        /// <summary>
        /// 根据direction获取矩形的边线的坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Point[] GetLineByRectangle(Rectangle rect, LineDrawDirection direction)
        {
            Point[] pointFs = null;
            if (rect == Rectangle.Empty) return pointFs;
            pointFs = new Point[2];

            switch (direction)
            {
                case LineDrawDirection.Left:
                    pointFs[0] = new Point(rect.X, rect.Y);
                    pointFs[1] = new Point(rect.X, rect.Bottom);
                    break;
                case LineDrawDirection.Top:
                    pointFs[0] = new Point(rect.X, rect.Y);
                    pointFs[1] = new Point(rect.Right, rect.Y);
                    break;
                case LineDrawDirection.Right:
                    pointFs[0] = new Point(rect.Right, rect.Y);
                    pointFs[1] = new Point(rect.Right, rect.Bottom);
                    break;
                case LineDrawDirection.Bottom:
                    pointFs[0] = new Point(rect.X, rect.Bottom);
                    pointFs[1] = new Point(rect.Right, rect.Bottom);
                    break;
            }
            return pointFs;
        }

        /// <summary>
        /// 根据direction获取矩形的边线的坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static PointF[] GetLineByRectangle(RectangleF rect, LineDrawDirection direction)
        {
            return GetLineByRectangle(rect, direction, 0, 0, 0, 0);
        }

        /// <summary>
        /// 根据direction获取矩形的边线的坐标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="direction"></param>
        /// <param name="minusHead">减去头部</param>
        /// <param name="minusTail">减去尾部</param>
        /// <param name="xOffset">x偏移值</param>
        /// <param name="yOffset">y偏移值</param>
        /// <returns></returns>
        public static PointF[] GetLineByRectangle(RectangleF rect, LineDrawDirection direction, float minusHead, float minusTail, float xOffset, float yOffset)
        {
            PointF[] pointFs = null;
            if (rect == RectangleF.Empty) return pointFs;
            pointFs = new PointF[2];

            switch (direction)
            {
                case LineDrawDirection.Left:
                    pointFs[0] = new PointF(rect.X + xOffset, rect.Y + minusHead + yOffset);
                    pointFs[1] = new PointF(rect.X + xOffset, rect.Bottom - minusTail + yOffset);
                    break;
                case LineDrawDirection.Top:
                    pointFs[0] = new PointF(rect.X + minusHead + xOffset, rect.Y + yOffset);
                    pointFs[1] = new PointF(rect.Right - minusTail + xOffset, rect.Y + yOffset);
                    break;
                case LineDrawDirection.Right:
                    pointFs[0] = new PointF(rect.Right + xOffset, rect.Y + minusHead + yOffset);
                    pointFs[1] = new PointF(rect.Right + xOffset, rect.Bottom - minusTail + yOffset);
                    break;
                case LineDrawDirection.Bottom:
                    pointFs[0] = new PointF(rect.X + minusHead + xOffset, rect.Bottom + yOffset);
                    pointFs[1] = new PointF(rect.Right - minusTail + xOffset, rect.Bottom + yOffset);
                    break;
            }
            return pointFs;
        }


        /// <summary>
        /// 获取矩形中间X的两个线条的坐标
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="margin">距离矩形边框的边距</param>
        /// <returns></returns>
        public static PointF[] GetXByRectangle(RectangleF rect, float margin)
        {
            PointF[] pointF = new PointF[4];
            pointF[0] = new PointF(rect.X + margin, rect.Y + margin);
            pointF[1] = new PointF(rect.Right - margin, rect.Bottom - margin);

            pointF[2] = new PointF(rect.X + margin, rect.Bottom - margin);
            pointF[3] = new PointF(rect.Right - margin, rect.Y + margin);

            return pointF;
        }

        /// <summary>
        /// 根据矩形的同心圆，获取同心圆的X的两个线条的坐标
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="margin">线条到同心圆的边距</param>
        /// <returns></returns>
        public static PointF[] GetXByCircle(RectangleF rect, float margin)
        {
            /*圆点坐标：(x0,y0) 
            半径：r 
            角度：a0 
            则圆上任一点为：（x1,y1） 
            x1   =   x0   +   r   *   cos(ao   *   3.14   /180   ) 
            y1   =   y0   +   r   *   sin(ao   *   3.14   /180   ) 
            */
            //先算圆的半径
            float radius = Math.Min(rect.Height, rect.Width) / 2.0F;
            //圆心点
            PointF dimcenterPoint = new PointF(rect.X + rect.Width / 2.0F, rect.Y + rect.Height / 2.0F);

            //线条位置
            //03
            //21
            //根据角度，算出直径的两个点的坐标
            PointF[] pointF = new PointF[4];
            int angle = 225;
            pointF[0] = new PointF(dimcenterPoint.X + radius * (float)Math.Cos(angle * Math.PI / 180D),
                                   dimcenterPoint.Y + radius * (float)Math.Sin(angle * Math.PI / 180D));
            angle = 45;
            pointF[1] = new PointF(dimcenterPoint.X + radius * (float)Math.Cos(angle * Math.PI / 180D),
                                   dimcenterPoint.Y + radius * (float)Math.Sin(angle * Math.PI / 180D));
            angle = 135;
            pointF[2] = new PointF(dimcenterPoint.X + radius * (float)Math.Cos(angle * Math.PI / 180D),
                                   dimcenterPoint.Y + radius * (float)Math.Sin(angle * Math.PI / 180D));
            angle = 315;
            pointF[3] = new PointF(dimcenterPoint.X + radius * (float)Math.Cos(angle * Math.PI / 180D),
                                   dimcenterPoint.Y + radius * (float)Math.Sin(angle * Math.PI / 180D));
            //根据边距，计算减少的宽度和高度
            //弧度=PI / 180 * 角度
            float width = margin * (float)Math.Cos(45 * Math.PI / 180);
            float hight = margin * (float)Math.Sin(45 * Math.PI / 180);
            pointF[0].X += width;
            pointF[0].Y += hight;

            pointF[1].X -= width;
            pointF[1].Y -= hight;

            pointF[2].X += width;
            pointF[2].Y -= hight;

            pointF[3].X -= width;
            pointF[3].Y += hight;

            return pointF;
        }

        #endregion
    }

    /// <summary>
    /// 线的绘制方向枚举
    /// </summary>
    [Flags]
    public enum LineDrawDirection
    {
        /// <summary>
        /// 左
        /// </summary>
        Left = 1,
        /// <summary>
        /// 上
        /// </summary>
        Top = 2,
        /// <summary>
        /// 右
        /// </summary>
        Right = 4,
        /// <summary>
        /// 下
        /// </summary>
        Bottom = 8
    }
}