using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SparkControls.Foundation;


namespace SparkControls.Controls
{
    [ToolboxBitmap(typeof(Button))]
    [ToolboxItem(true)]
    public partial class SparkDropDownButton : SparkButton
    {
        private bool _select = false;
        private int _splitWight = 15; //按钮区域宽带
        private Color _splitLineColor = SystemColors.ButtonShadow;

        /// <summary>
        /// 菜单项点击事件
        /// </summary>
        [Browsable(true)]
        public event ToolStripItemClickedEventHandler DropDownItemClicked;

        [Browsable(false)]
        public override string[] Items { get; set; }

        /// <summary>
        /// 是否画分割线
        /// </summary>
        [Category("Spark"), Description("是否画分割线。")]
        [DefaultValue(true)]
        public bool IsSplitLine { get; set; } = true;

        /// <summary>
        /// 分割线的颜色
        /// </summary>
        [Category("Spark"), Description("分割线的颜色。")]
        [DefaultValue(typeof(SystemColors), "ButtonShadow")]
        public Color SplitLineColor { get; set; } = SystemColors.ButtonShadow;

        private void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (DropDownItemClicked != null)
            {
                DropDownItemClicked(sender, e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            //画线
            if (IsSplitLine && this.Width > _splitWight)
            {
                //下拉按钮15单位
                //XXXXXX|XX
                float height = Math.Max(this.Height * 4 / 5, 1);//防止负数
                g.DrawLine(new Pen(SplitLineColor), this.Width - _splitWight, (this.Height - height) / 2F, this.Width - _splitWight, (this.Height - height) / 2F + height);
            }

            //画三角形
            int len = 6; //三角形的边长
            len = _select ? 8 : 6;
            float sh = (float)Math.Sqrt(Math.Pow(len, 2) - Math.Pow(1.0 * len / 2, 2));//三角形的高
            var dropDownRect = new RectangleF(ClientRectangle.Width - _splitWight, 0, _splitWight - 1, this.Height - 1);
            var points = new PointF[]
            {
                new PointF(dropDownRect.X + (_splitWight - len) / 2F,       dropDownRect.Y + (dropDownRect.Height - sh) / 2.0F),
                new PointF(dropDownRect.X + (_splitWight - len) / 2F + len, dropDownRect.Y + (dropDownRect.Height - sh) / 2.0F),
                new PointF(dropDownRect.X + (_splitWight - len) / 2F + len / 2.0F, dropDownRect.Y + (dropDownRect.Height - sh) / 2.0F + sh)
            };
            g.FillPolygon(Brushes.Black, points);
        }

        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);
            _select = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _select = false;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (this.ContextMenuStrip != null)
            {
                this.ContextMenuStrip.ItemClicked += ContextMenuStrip_ItemClicked;
            }
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                if (this.ContextMenuStrip != null && this.ContextMenuStrip.Items.Count > 0)
                {

                    this.ContextMenuStrip.Show(this, new Point(0, Height));
                }
            }
            base.OnMouseDown(mevent);
        }

    }
}
