using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    [ToolboxBitmap(typeof(SparkControls.Controls.SparkToolBoxBitmap.Line), "TcLineV.bmp")]
    public class SparkLineV : Control, ISparkTheme<SparkLineTheme>
    {
        public SparkLineV() : base()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |            // 调整大小时重绘
                ControlStyles.DoubleBuffer |                      // 双缓冲
                ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
                ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                ControlStyles.SupportsTransparentBackColor |      // 模拟透明度
                ControlStyles.UserPaint, true                     // 控件绘制代替系统绘制
            );
            this.Theme = new SparkLineTheme(this);
            base.AutoSize = false;
            base.Height = 128;
            base.Width = 1;
            base.Text = string.Empty;
            base.BackColor = this.Theme.BorderColor;

            this.Theme.PropertyChanged += (sender, e) =>
            {
                this.Invalidate();
            };
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            //int boundX = x < this.Location.X ? this.Location.X : x;
            base.SetBoundsCore(x, y, this.LineWidth, height, specified);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen p = new Pen(this.Theme.BorderColor, this.LineWidth))
            {
                e.Graphics.DrawLine(p, this.Location, new Point(this.Location.X, this.Location.Y + this.LineLong));
            }
        }

        private int mLineWidth = 1;
        [Description("线的宽度。"), Category("Spark"), Browsable(true)]
        [DefaultValue(1)]
        public int LineWidth
        {
            get => this.mLineWidth;
            set
            {
                base.Width = this.mLineWidth = value;
                base.Invalidate();
            }
        }

        private int mLineLong = 128;
        [Description("线的长度。"), Category("Spark"), Browsable(true)]
        [DefaultValue(128)]
        public int LineLong
        {
            get => this.mLineLong;
            set
            {
                base.Height = this.mLineLong = value;
                base.Invalidate();
            }
        }

        [Description("线的颜色。"), Category("Spark"), Browsable(true)]
        [DefaultValue(typeof(Color), SparkThemeConsts.LineBorderColorString)]
        public Color LineColor
        {
            get
            {
                return this.Theme.BorderColor;
            }
            set
            {
                if (this.Theme.BorderColor != value) base.BackColor = this.Theme.BorderColor = value;
            }
        }

        [Browsable(false)]
        public override bool AutoSize => base.AutoSize;

        [Browsable(false)]
        public override Color BackColor => base.BackColor;

        [Browsable(false)]
        public override Image BackgroundImage => base.BackgroundImage;

        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout => base.BackgroundImageLayout;

        [Browsable(false)]
        public override Color ForeColor => base.ForeColor;

        [Browsable(false)]
        public override string Text => base.Text;

        [Browsable(false)]
        public new int Width => base.Width;

        [Browsable(false)]
        public new int Height => base.Height;

        [Browsable(false)]
        public new Size Size
        {
            get => base.Size;
            set { base.Width = LineWidth; base.Height = value.Height; }
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