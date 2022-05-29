using System.Drawing;
using System.Drawing.Drawing2D;

namespace SparkControls.Controls
{
	/// <summary>
	/// 表示圆角的矩形。
	/// </summary>
	public class RoundRectangle
    {
        #region Properties

        /// <summary>
        /// 获取或设置矩形区域
        /// </summary>
        public RectangleF Rect { get; set; }

        /// <summary>
        /// 获取或设置圆角半径
        /// </summary>
        public CornerRadius CornerRadius { get; set; }

        #endregion

        #region Initializes

        /// <summary>
        /// 初始 <see cref="RoundRectangle"/> 类型的新实例。
        /// </summary>
        /// <param name="rect">矩形区域</param>
        /// <param name="radius">圆角半径。</param>
        public RoundRectangle(RectangleF rect, int radius) : this(rect, new CornerRadius(radius))
        {
        }

        /// <summary>
        /// 初始 <see cref="RoundRectangle"/> 类型的新实例。
        /// </summary>
        /// <param name="rect">矩形区域。</param>
        /// <param name="cornerRadius">圆角半径。</param>
        public RoundRectangle(RectangleF rect, CornerRadius cornerRadius)
        {
            this.Rect = rect;
            this.CornerRadius = cornerRadius;
        }

        /// <summary>
        /// 初始 <see cref="RoundRectangle"/> 类型的新实例。
        /// </summary>
        /// <param name="point">位置。</param>
        /// <param name="size">尺寸</param>
        /// <param name="cornerRadius">圆角半径。</param>
        public RoundRectangle(PointF point, SizeF size, CornerRadius cornerRadius)
        {
            this.Rect = new RectangleF(point, size);
            this.CornerRadius = cornerRadius;
        }

        /// <summary>
        /// 初始 <see cref="RoundRectangle"/> 类型的新实例。
        /// </summary>
        /// <param name="x">横坐标。</param>
        /// <param name="y">纵坐标。</param>
        /// <param name="width">宽度。</param>
        /// <param name="height">高度。</param>
        /// <param name="cornerRadius">圆角半径。</param>
        public RoundRectangle(float x, float y, float width, float height, CornerRadius cornerRadius)
        {
            this.Rect = new RectangleF(x, y, width, height);
            this.CornerRadius = cornerRadius;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// 获取该圆角矩形的 <see cref="GraphicsPath"/> 对象(圆角使用 Bezier 曲线实现)。
        /// </summary>
        /// <returns><see cref="GraphicsPath"/>类型的实例。</returns>
        public GraphicsPath ToGraphicsBezierPath()
        {
            float x = this.Rect.X;
            float y = this.Rect.Y;
            float w = this.Rect.Width;
            float h = this.Rect.Height;

            GraphicsPath path = new GraphicsPath();
            path.AddBezier(x, y + this.CornerRadius.TopLeft, x, y, x + this.CornerRadius.TopLeft, y, x + this.CornerRadius.TopLeft, y);
            path.AddLine(x + this.CornerRadius.TopLeft, y, x + w - this.CornerRadius.TopRight, y);
            path.AddBezier(x + w - this.CornerRadius.TopRight, y, x + w, y, x + w, y + this.CornerRadius.TopRight, x + w, y + this.CornerRadius.TopRight);
            path.AddLine(x + w, y + this.CornerRadius.TopRight, x + w, y + h - this.CornerRadius.BottomRight);
            path.AddBezier(x + w, y + h - this.CornerRadius.BottomRight, x + w, y + h, x + w - this.CornerRadius.BottomRight, y + h, x + w - this.CornerRadius.BottomRight, y + h);
            path.AddLine(x + w - this.CornerRadius.BottomRight, y + h, x + this.CornerRadius.BottomLeft, y + h);
            path.AddBezier(x + this.CornerRadius.BottomLeft, y + h, x, y + h, x, y + h - this.CornerRadius.BottomLeft, x, y + h - this.CornerRadius.BottomLeft);
            path.AddLine(x, y + h - this.CornerRadius.BottomLeft, x, y + this.CornerRadius.TopLeft);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// 获取该圆角矩形的 <see cref="GraphicsPath"/> 对象(圆角使用矩形圆弧曲线实现)。
        /// </summary>
        /// <returns><see cref="GraphicsPath"/>类型的实例。</returns>
        public GraphicsPath ToGraphicsArcPath()
        {
            float x = this.Rect.X;
            float y = this.Rect.Y;
            float w = this.Rect.Width;
            float h = this.Rect.Height;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(x, y, this.CornerRadius.TopLeft, this.CornerRadius.TopLeft, 180, 90);
            path.AddArc(x + w - this.CornerRadius.TopRight, y, this.CornerRadius.TopRight, this.CornerRadius.TopRight, 270, 90);
            path.AddArc(x + w - this.CornerRadius.BottomRight, y + h - this.CornerRadius.BottomRight, this.CornerRadius.BottomRight, this.CornerRadius.BottomRight, 0, 90);
            path.AddArc(x, y + h - this.CornerRadius.BottomLeft, this.CornerRadius.BottomLeft, this.CornerRadius.BottomLeft, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// 获取该圆角矩形的 <see cref="GraphicsPath"/> 对象(天使之翼的区域样式，主要用于 Tabcontrol 的标签样式)。
        /// </summary>
        /// <returns><see cref="GraphicsPath"/>类型的实例。</returns>
        public GraphicsPath ToGraphicsAnglesWingPath()
        {
            float x = this.Rect.X;
            float y = this.Rect.Y;
            float w = this.Rect.Width;
            float h = this.Rect.Height;

            GraphicsPath path = new GraphicsPath();
            path.AddBezier(x, y + this.CornerRadius.TopLeft, x, y, x + this.CornerRadius.TopLeft, y, x + this.CornerRadius.TopLeft, y);
            path.AddLine(x + this.CornerRadius.TopLeft, y, x + w - this.CornerRadius.TopRight, y);
            path.AddBezier(x + w - this.CornerRadius.TopRight, y, x + w, y, x + w, y + this.CornerRadius.TopRight, x + w, y + this.CornerRadius.TopRight);
            path.AddLine(x + w, y + this.CornerRadius.TopRight, x + w, y + h - this.CornerRadius.BottomRight);
            path.AddBezier(x + w, y + h - this.CornerRadius.BottomRight, x + w, y + h, x + w + this.CornerRadius.BottomRight, y + h, x + w + this.CornerRadius.BottomRight, y + h);
            path.AddLine(x + w + this.CornerRadius.BottomRight, y + h, x - this.CornerRadius.BottomLeft, y + h);
            path.AddBezier(x - this.CornerRadius.BottomLeft, y + h, x, y + h, x, y + h - this.CornerRadius.BottomLeft, x, y + h - this.CornerRadius.BottomLeft);
            path.AddLine(x, y + h - this.CornerRadius.BottomLeft, x, y + this.CornerRadius.TopLeft);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}