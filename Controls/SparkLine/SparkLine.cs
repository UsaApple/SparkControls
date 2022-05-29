using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 线控件
    /// </summary>
    [ToolboxBitmap(typeof(SparkControls.Controls.SparkToolBoxBitmap.Line), "TcLine.bmp")]
    [Description("一个显示为水平、垂直或对角线的图形化控件"),
     ToolboxItemFilter("System.Windows.Forms"),
     ToolboxItem(typeof(LineShapeToolboxItem)),
     DesignTimeVisible(true), Designer(typeof(LineShapeDesigner)),
    ]
    public class SparkLine : Shape, ISparkTheme<SparkLineTheme>
    {
        #region 变量
        private Point m_P1BeforeSetVirtualBounds;
        private Point m_P2BeforeSetVirtualBounds;
        private int m_X1;
        private int m_X2;
        private int m_Y1;
        private int m_Y2;
        #endregion

        #region 事件
        /// <summary>
        /// 终点改变事件
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always), Description("终点改变事件"), Category("PropertyChanged"), Browsable(true)]
        public event EventHandler EndPointChanged;

        /// <summary>
        /// 起始点改变事件
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Always), Description("起始点改变事件"), Category("PropertyChanged"), Browsable(true)]
        public event EventHandler StartPointChanged;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkLine()
        {
            this.m_P1BeforeSetVirtualBounds = Point.Empty;
            this.m_P2BeforeSetVirtualBounds = Point.Empty;

            InitTheme();
        }

        /// <summary>
        /// 有参构造方法
        /// </summary>
        public SparkLine(ShapeContainer parent) : base(parent)
        {
            this.m_P1BeforeSetVirtualBounds = Point.Empty;
            this.m_P2BeforeSetVirtualBounds = Point.Empty;

            InitTheme();
        }

        /// <summary>
        /// 有参构造方法
        /// </summary>
        public SparkLine(int x1, int y1, int x2, int y2)
        {
            this.m_P1BeforeSetVirtualBounds = Point.Empty;
            this.m_P2BeforeSetVirtualBounds = Point.Empty;
            this.m_X1 = Utility.CheckInteger(x1);
            this.m_Y1 = Utility.CheckInteger(y1);
            this.m_X2 = Utility.CheckInteger(x2);
            this.m_Y2 = Utility.CheckInteger(y2);

            InitTheme();
        }
        #endregion

        private void InitTheme()
        {
            this.Theme = new SparkLineTheme(null);
            Theme.PropertyChanged += (sender, e) =>
            {
                this.Invalidate();
            };
        }

        /// <summary>
        /// 创建AccessibleObject
        /// </summary>
        /// <returns></returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new LineShapeAccessibleObject(this);
        }

        internal void DrawInternal(Graphics g, int x1, int y1, int x2, int y2)
        {
            if ((g != null) && ((x1 != x2) || (y1 != y2)))
            {
                SmoothingMode smoothingMode = g.SmoothingMode;
                try
                {
                    if (this.BorderStyle != DashStyle.Custom)
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        this.Pen.Width = this.BorderWidth;
                        this.Pen.Color = this.Theme.BorderColor;
                        g.DrawLine(this.Pen, x1, y1, x2, y2);
                    }
                    if ((!this.DesignMode && this.Enabled) && this.GetState(0x800))
                    {
                        using (Region region = this.FocusRegion)
                        {
                            region.Translate(this.BoundRect.X, this.BoundRect.Y);
                            using (Brush brush = new HatchBrush(HatchStyle.Percent50, this.SelectionColor, Color.Transparent))
                            {
                                g.FillRegion(brush, region);
                            }
                        }
                    }
                }
                catch (ArgumentNullException exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    ArgumentNullException exception = exception1;
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    g.SmoothingMode = smoothingMode;
                }
            }
        }

        /// <summary>
        /// DrawToBitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="targetBounds"></param>
        public override void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }
            if (targetBounds.IsEmpty)
            {
                throw new ArgumentNullException("targetBounds");
            }
            if (((targetBounds.Width <= 0) || (targetBounds.Height <= 0)) || ((targetBounds.X < 0) || (targetBounds.Y < 0)))
            {
                throw new ArgumentException("targetBounds区域无效!");
            }
            if ((targetBounds.X <= bitmap.Width) && (targetBounds.Y <= bitmap.Height))
            {
                int dx = targetBounds.Location.X - Math.Min(this.X1, this.X2);
                int dy = targetBounds.Location.Y - Math.Min(this.Y1, this.Y2);
                Point startPoint = this.StartPoint;
                Point endPoint = this.EndPoint;
                startPoint.Offset(dx, dy);
                endPoint.Offset(dx, dy);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SetClip(targetBounds);
                    this.DrawInternal(graphics, startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                }
            }
        }

        private Region GetRegionFocus()
        {
            if (!this.Enabled)
            {
                return null;
            }
            Region region2 = null;
            int width = 2;
            if (this.BorderWidth > 2)
            {
                width = 2;
            }
            int num6 = (int)Math.Round(Math.Sqrt(((this.X2 - this.X1) * (this.X2 - this.X1)) + ((this.Y2 - this.Y1) * (this.Y2 - this.Y1))));
            Rectangle rect = new Rectangle(0, 0, num6, this.BorderWidth);
            double num2 = this.BorderWidth / 2.0;
            double a = Math.Atan2(this.Y2 - this.Y1, this.X2 - this.X1);
            int dx = this.X1 - Math.Min(this.X1, this.X2);
            int dy = this.Y1 - Math.Min(this.Y1, this.Y2);
            dx = (int)Math.Round((double)(dx + (num2 * Math.Sin(a))));
            dy = (int)Math.Round((double)(dy - (num2 * Math.Cos(a))));
            if (this.X1 == this.X2)
            {
                int num7 = this.BorderWidth % 4;
                switch (num7)
                {
                    case 1:
                        dx++;
                        goto Label_0153;

                    case 2:
                        dx++;
                        rect.Height++;
                        goto Label_0153;
                }
                if (num7 != 3)
                {
                    dx++;
                    rect.Height++;
                }
            }
        Label_0153:
            if (this.Y1 == this.Y2)
            {
                switch ((this.BorderWidth % 4))
                {
                    case 1:
                        goto Label_01A2;

                    case 2:
                        rect.Height++;
                        goto Label_01A2;

                    case 3:
                        dy++;
                        goto Label_01A2;
                }
                rect.Height++;
            }
        Label_01A2:
            rect.Inflate(width, width);
            region2 = new Region(rect);
            rect.Inflate(-1, -1);
            region2.Exclude(rect);
            Matrix matrix = new Matrix();
            matrix.Rotate((int)Math.Round((double)((a * 180.0) / 3.1415926535897931)));
            region2.Transform(matrix);
            region2.Translate(dx, dy);
            return region2;
        }

        internal override Region GetRegionInternal(Shape.RegionType type)
        {
            if ((this.X1 != this.X2) || (this.Y1 != this.Y2))
            {
                switch (type)
                {
                    case Shape.RegionType.HitTest:
                        return this.GetRegionLine(false);

                    case Shape.RegionType.Invalidate:
                        return this.GetRegionLine(true);

                    case Shape.RegionType.FocusInvalidate:
                        return this.GetRegionFocus();
                }
            }
            return null;
        }

        private Region GetRegionLine(bool flag)
        {
            Region region2 = null;
            using (GraphicsPath path = new GraphicsPath())
            {
                GraphicsPath path2;
                Point startPoint = this.StartPoint;
                Point endPoint = this.EndPoint;
                if (flag)
                {
                    if (startPoint.X < endPoint.X)
                    {
                        startPoint.X--;
                        endPoint.X++;
                    }
                    else if (startPoint.X > endPoint.X)
                    {
                        startPoint.X++;
                        endPoint.X--;
                    }
                    if (startPoint.Y < endPoint.Y)
                    {
                        startPoint.Y--;
                        endPoint.Y++;
                    }
                    else if (startPoint.Y > endPoint.Y)
                    {
                        startPoint.Y++;
                        endPoint.Y--;
                    }
                    path.AddLine(startPoint, endPoint);
                    if (this.BorderWidth < 4)
                    {
                        path2 = path;
                        Utility.WidenPath(ref path2, this.BorderWidth + 4);
                    }
                    else
                    {
                        path2 = path;
                        Utility.WidenPath(ref path2, this.BorderWidth + 2);
                    }
                }
                else
                {
                    path.AddLine(startPoint, endPoint);
                    path2 = path;
                    Utility.WidenPath(ref path2, Math.Max(this.BorderWidth, 3));
                }
                region2 = new Region(path);
                region2.Translate(0 - this.BoundRect.X, 0 - this.BoundRect.Y);
                return region2;
            }
        }

        public override bool HitTest(int x, int y)
        {
            if (this.Parent == null)
            {
                return false;
            }
            if (!this.DesignMode && (!this.Visible || !this.Enabled))
            {
                return false;
            }
            Point p = new Point(x, y);
            Point point = this.Parent.PointToClient(p);
            if (point.Equals(this.StartPoint) || point.Equals(this.EndPoint))
            {
                return true;
            }
            p = new Point(x, y);
            point = this.PointToClient(p);
            Region hitTestRegion = this.HitTestRegion;
            if (hitTestRegion == null)
            {
                return false;
            }
            if (this.Region != null)
            {
                hitTestRegion.Intersect(this.Region);
            }
            return hitTestRegion.IsVisible(point);
        }

        protected virtual void OnEndPointChanged(EventArgs e)
        {
            this.ClearVirtualBounds();
            if (!this.InSetVirtulBounds)
            {
                this.m_P1BeforeSetVirtualBounds = Point.Empty;
                this.m_P2BeforeSetVirtualBounds = Point.Empty;
            }
            EndPointChanged?.Invoke(this, e);
            this.OnMove(e);
        }

        protected internal override void OnPaint(PaintEventArgs e)
        {
            if (this.Region != null)
            {
                using (Region region = this.Region.Clone())
                {
                    region.Translate(this.BoundRect.X, this.BoundRect.Y);
                    e.Graphics.SetClip(region, CombineMode.Intersect);
                    this.DrawInternal(e.Graphics, this.X1, this.Y1, this.X2, this.Y2);
                    e.Graphics.ResetClip();
                }
            }
            else
            {
                this.DrawInternal(e.Graphics, this.X1, this.Y1, this.X2, this.Y2);
            }
            base.OnPaint(e);
        }

        protected virtual void OnStartPointChanged(EventArgs e)
        {
            this.ClearVirtualBounds();
            if (!this.InSetVirtulBounds)
            {
                this.m_P1BeforeSetVirtualBounds = Point.Empty;
                this.m_P2BeforeSetVirtualBounds = Point.Empty;
            }
            StartPointChanged?.Invoke(this, e);
            this.OnMove(e);
        }

        public override void Scale(SizeF factor)
        {
            SizeF objB = new SizeF(0f, 0f);
            if (!object.Equals(factor, objB))
            {
                int x = Utility.CheckInteger((int)Math.Round(this.X1 * factor.Width));
                int y = Utility.CheckInteger((int)Math.Round(this.Y1 * factor.Height));
                int num2 = Utility.CheckInteger((int)Math.Round(this.X2 * factor.Width));
                int num4 = Utility.CheckInteger((int)Math.Round(this.Y2 * factor.Height));
                if ((!x.Equals(this.X1) || !y.Equals(this.Y1)) || (!num2.Equals(this.X2) || !num4.Equals(this.Y2)))
                {
                    if (!this.CanInvalidate)
                    {
                        this.SetStartPoint(x, y, false);
                        this.SetEndPoint(num2, num4, false);
                    }
                    else
                    {
                        Region invalidateRegion = this.GetInvalidateRegion();
                        Point location = this.BoundRect.Location;
                        this.SetStartPoint(x, y, false);
                        this.SetEndPoint(num2, num4, false);
                        Region newRegion = this.GetInvalidateRegion();
                        if (invalidateRegion != null)
                        {
                            invalidateRegion.Translate(location.X - this.BoundRect.X, location.Y - this.BoundRect.Y);
                        }
                        this.InvalidateInternal(invalidateRegion, newRegion, true);
                    }
                }
            }
        }

        private void SetEndPoint(int x, int y)
        {
            this.SetEndPoint(x, y, true);
        }

        private void SetEndPoint(int x, int y, bool invalidate)
        {
            x = Utility.CheckInteger(x);
            y = Utility.CheckInteger(y);
            if (!x.Equals(this.m_X2) || !y.Equals(this.m_Y2))
            {
                this.ClearCachedRegion();
                if (!invalidate || !this.CanInvalidate)
                {
                    this.m_X2 = x;
                    this.m_Y2 = y;
                }
                else
                {
                    Region invalidateRegion = this.GetInvalidateRegion();
                    Point location = this.BoundRect.Location;
                    this.m_X2 = x;
                    this.m_Y2 = y;
                    Region newRegion = this.GetInvalidateRegion();
                    if (invalidateRegion != null)
                    {
                        invalidateRegion.Translate(location.X - this.BoundRect.X, location.Y - this.BoundRect.Y);
                    }
                    this.InvalidateInternal(invalidateRegion, newRegion, true);
                }
                this.OnEndPointChanged(EventArgs.Empty);
            }
        }

        internal void SetLocation(Point p)
        {
            Point startPoint = this.StartPoint;
            Point endPoint = this.EndPoint;
            int num = Math.Abs(this.X2 - this.X1);
            if (this.X1 < this.X2)
            {
                startPoint.X = p.X;
                endPoint.X = p.X + num;
            }
            else
            {
                endPoint.X = p.X;
                startPoint.X = p.X + num;
            }
            num = Math.Abs(this.Y2 - this.Y1);
            if (this.Y1 < this.Y2)
            {
                startPoint.Y = p.Y;
                endPoint.Y = p.Y + num;
            }
            else
            {
                endPoint.Y = p.Y;
                startPoint.Y = p.Y + num;
            }
            this.SetStartPoint(startPoint.X, startPoint.Y);
            this.SetEndPoint(endPoint.X, endPoint.Y);
        }

        internal void SetSize(Size value)
        {
            Point startPoint = this.StartPoint;
            Point endPoint = this.EndPoint;
            int num2 = value.Width - Math.Abs(this.X2 - this.X1);
            if (num2 != 0)
            {
                if (this.X1 < this.X2)
                {
                    endPoint.X += num2;
                }
                else if (this.X1 == this.X2)
                {
                    if ((!this.m_P1BeforeSetVirtualBounds.IsEmpty && !this.m_P2BeforeSetVirtualBounds.IsEmpty) && (this.m_P1BeforeSetVirtualBounds.X < this.m_P2BeforeSetVirtualBounds.X))
                    {
                        endPoint.X += num2;
                    }
                    else
                    {
                        startPoint.X += num2;
                    }
                }
                else
                {
                    startPoint.X += num2;
                }
            }
            int num = value.Height - Math.Abs(this.Y2 - this.Y1);
            if (num != 0)
            {
                if (this.Y1 < this.Y2)
                {
                    endPoint.Y += num;
                }
                else if (this.Y1 == this.Y2)
                {
                    if ((!this.m_P1BeforeSetVirtualBounds.IsEmpty && !this.m_P2BeforeSetVirtualBounds.IsEmpty) && (this.m_P1BeforeSetVirtualBounds.Y < this.m_P2BeforeSetVirtualBounds.Y))
                    {
                        endPoint.Y += num;
                    }
                    else
                    {
                        startPoint.Y += num;
                    }
                }
                else
                {
                    startPoint.Y += num;
                }
            }
            this.SetStartPoint(startPoint.X, startPoint.Y);
            this.SetEndPoint(endPoint.X, endPoint.Y);
        }

        private void SetStartPoint(int x, int y)
        {
            this.SetStartPoint(x, y, true);
        }

        private void SetStartPoint(int x, int y, bool invalidate)
        {
            x = Utility.CheckInteger(x);
            y = Utility.CheckInteger(y);
            if (!x.Equals(this.m_X1) || !y.Equals(this.m_Y1))
            {
                this.ClearCachedRegion();
                if (invalidate && this.CanInvalidate)
                {
                    Region invalidateRegion = this.GetInvalidateRegion();
                    Point location = this.BoundRect.Location;
                    this.m_X1 = x;
                    this.m_Y1 = y;
                    Region newRegion = this.GetInvalidateRegion();
                    if (invalidateRegion != null)
                    {
                        invalidateRegion.Translate(location.X - this.BoundRect.X, location.Y - this.BoundRect.Y);
                    }
                    this.InvalidateInternal(invalidateRegion, newRegion, true);
                }
                else
                {
                    this.m_X1 = x;
                    this.m_Y1 = y;
                }
                this.OnStartPointChanged(EventArgs.Empty);
            }
        }

        internal override Rectangle BoundRect
        {
            get
            {
                return new Rectangle(Math.Min(this.X1, this.X2), Math.Min(this.Y1, this.Y2), Math.Abs(this.X1 - this.X2), Math.Abs(this.Y1 - this.Y2));
            }
            set
            {
                this.Location = value.Location;
                this.Size = value.Size;
            }
        }

        [Browsable(false), Category("Layout"), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point EndPoint
        {
            get
            {
                return new Point(this.X2, this.Y2);
            }
            set
            {
                this.SetEndPoint(value.X, value.Y);
            }
        }

        internal override Rectangle ExtentBounds
        {
            get
            {
                Rectangle boundRect = this.BoundRect;
                int width = ((int)Math.Round((double)(this.BorderWidth / 2.0))) + 1;
                boundRect.Inflate(width, width);
                return boundRect;
            }
        }

        internal Point Location
        {
            get
            {
                return new Point(Math.Min(this.X1, this.X2), Math.Min(this.Y1, this.Y2));
            }
            set
            {
                this.SetLocation(value);
            }
        }

        internal Size Size
        {
            get
            {
                return new Size(Math.Abs(this.X1 - this.X2), Math.Abs(this.Y1 - this.Y2));
            }
            set
            {
                this.SetSize(value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Point StartPoint
        {
            get
            {
                return new Point(this.X1, this.Y1);
            }
            set
            {
                this.SetStartPoint(value.X, value.Y);
            }
        }

        internal override Rectangle VirtualBounds
        {
            get
            {
                return base.VirtualBounds;
            }
            set
            {
                if (this.m_P1BeforeSetVirtualBounds.IsEmpty || this.m_P2BeforeSetVirtualBounds.IsEmpty)
                {
                    this.m_P1BeforeSetVirtualBounds = this.StartPoint;
                    this.m_P2BeforeSetVirtualBounds = this.EndPoint;
                }
                base.VirtualBounds = value;
            }
        }

        /// <summary>
        /// 起始点的X坐标
        /// </summary>
        [Category("Spark"), Description("起始点的X坐标"), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), Localizable(true)]
        public int X1
        {
            get
            {
                return this.m_X1;
            }
            set
            {
                this.SetStartPoint(value, this.Y1);
            }
        }

        /// <summary>
        /// 终点的X坐标
        /// </summary>
        [Browsable(true), Localizable(true), Description("终点的X坐标"), Category("Spark"),
            EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int X2
        {
            get
            {
                return this.m_X2;
            }
            set
            {
                this.SetEndPoint(value, this.Y2);
            }
        }

        /// <summary>
        /// 起始点的Y坐标
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            Browsable(true), Localizable(true), Description("起始点的Y坐标"), Category("Spark")]
        public int Y1
        {
            get
            {
                return this.m_Y1;
            }
            set
            {
                this.SetStartPoint(this.X1, value);
            }
        }

        /// <summary>
        /// 终点的Y坐标
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            Description("终点的Y坐标"), Category("Spark"), Localizable(true), EditorBrowsable(EditorBrowsableState.Advanced)]
        public int Y2
        {
            get
            {
                return this.m_Y2;
            }
            set
            {
                this.SetEndPoint(this.X2, value);
            }
        }

        /// <summary>
        /// 线颜色
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Description("线条的颜色"), Category("Spark"), Localizable(true), EditorBrowsable(EditorBrowsableState.Advanced)]
        [DefaultValue(typeof(Color), SparkThemeConsts.LineBorderColorString)]
        public override Color BorderColor
        {
            get
            {
                return this.Theme.BorderColor;
            }
            set
            {
                if (this.Theme.BorderColor != value) this.Theme.BorderColor = base.BorderColor = value;
            }
        }

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkLineTheme Theme { get; private set; }

        #endregion
    }
}