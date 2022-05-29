using System.Drawing;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表示标题栏的项。
    /// </summary>
    public class SparkTitleBarItem
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkTitleBarItem()
        {
            TitleAction = TitleItemAction.Custom;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="titleAction"></param>
        /// <param name="contextMenu"></param>
        internal SparkTitleBarItem(TitleItemAction titleAction, SparkContextMenuStrip contextMenu = null)
        {
            Key = titleAction.ToString();
            TitleAction = titleAction;
            ContextMenu = contextMenu;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="image"></param>
        /// <param name="contextMenu"></param>
        public SparkTitleBarItem(object key, Image image, SparkContextMenuStrip contextMenu = null)
            : this(TitleItemAction.Custom, contextMenu)
        {
            Key = key;
            Image = image;
        }

        /// <summary>
        /// 图片
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// 绘制的范围
        /// </summary>
        internal Rectangle Bounds { get; set; } = Rectangle.Empty;

        /// <summary>
        /// 菜单类型
        /// </summary>
        public TitleItemAction TitleAction { get; internal set; } = TitleItemAction.None;

        /// <summary>
        /// 是否显示，true显示，false隐藏
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// 按钮是否有效
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 点击弹出菜单
        /// </summary>
        public SparkContextMenuStrip ContextMenu { get; set; }

        /// <summary>
        /// 按钮的颜色，比如X，最大化，最小化
        /// </summary>
        public Color ItemColor { get; set; } = Color.White;

        /// <summary>
        /// Key值，用于单击事件传出参数
        /// </summary>
        public object Key { get; set; }

        internal void Draw(Pen pen, Graphics g, SparkTitleBarDraw titleBar, SolidBrush fillSolidBrush)
        {
            switch (this.TitleAction)
            {
                case TitleItemAction.Max:
                    DrawMaxButton(pen, g, titleBar, fillSolidBrush);
                    break;
                case TitleItemAction.Min:
                    DrawMinButton(pen, g);
                    break;
                case TitleItemAction.Close:
                    DrawCloseButton(pen, g);
                    break;
                case TitleItemAction.Custom:
                    DrawCustomButton(pen, g);
                    break;
                default:
                    break;
            }
        }

        private void DrawCustomButton(Pen pen, Graphics g)
        {
            if (this.Image != null)
            {
                //按图片的大小绘制 默认24*24
                Rectangle rect = new Rectangle(this.Bounds.Location, this.Bounds.Size);
                if (this.Image.Width < this.Bounds.Width)
                {
                    rect.Width = this.Image.Width;
                    rect.X += (this.Bounds.Width - this.Image.Width) / 2;
                }
                if (this.Image.Height < this.Bounds.Width)
                {
                    rect.Height = this.Image.Height;
                    rect.Y += (this.Bounds.Height - this.Image.Height) / 2;
                }
                g.DrawImage(this.Image, rect);
            }
        }

        private void DrawCloseButton(Pen pen, Graphics g)
        {
            // Close button
            g.DrawLine(
                pen,
                this.Bounds.X + (int)(this.Bounds.Width * 0.33),
                this.Bounds.Y + (int)(this.Bounds.Height * 0.33),
                this.Bounds.X + (int)(this.Bounds.Width * 0.66),
                this.Bounds.Y + (int)(this.Bounds.Height * 0.66)
           );

            g.DrawLine(
                pen,
                this.Bounds.X + (int)(this.Bounds.Width * 0.66),
                this.Bounds.Y + (int)(this.Bounds.Height * 0.33),
                this.Bounds.X + (int)(this.Bounds.Width * 0.33),
                this.Bounds.Y + (int)(this.Bounds.Height * 0.66));
        }

        private void DrawMaxButton(Pen pen, Graphics g, SparkTitleBarDraw titleBar, SolidBrush fillSolidBrush)
        {
            if (fillSolidBrush == null)
            {
                fillSolidBrush = new SolidBrush(titleBar.Theme.BackColor);
            }
            if (titleBar.Maximized)
            {
                g.DrawRectangle(
                    pen,
                    this.Bounds.X + (int)(this.Bounds.Width * 0.38),
                    this.Bounds.Y + (int)(this.Bounds.Height * 0.34),
                    (int)(this.Bounds.Width * 0.31),
                    (int)(this.Bounds.Height * 0.31)
                );

                g.FillRectangle(fillSolidBrush,
                   this.Bounds.X + (int)(this.Bounds.Width * 0.38) + 1,
                   this.Bounds.Y + (int)(this.Bounds.Height * 0.34) + 1,
                   (int)(this.Bounds.Width * 0.31) - 2,
                   (int)(this.Bounds.Height * 0.31) - 2
                );


                g.DrawRectangle(
                    pen,
                    this.Bounds.X + (int)(this.Bounds.Width * 0.38) - 3,
                    this.Bounds.Y + (int)(this.Bounds.Height * 0.34) + 3,
                    (int)(this.Bounds.Width * 0.31),
                    (int)(this.Bounds.Height * 0.31)
                );


                g.FillRectangle(fillSolidBrush,
                    this.Bounds.X + (int)(this.Bounds.Width * 0.38) - 3 + 1,
                    this.Bounds.Y + (int)(this.Bounds.Height * 0.34) + 3 + 1,
                    (int)(this.Bounds.Width * 0.31) - 2,
                    (int)(this.Bounds.Height * 0.31 - 2)
                );
            }
            else
            {
                g.DrawRectangle(
                    pen,
                    this.Bounds.X + (int)(this.Bounds.Width * 0.33),
                    this.Bounds.Y + (int)(this.Bounds.Height * 0.36),
                    (int)(this.Bounds.Width * 0.39),
                    (int)(this.Bounds.Height * 0.31)
                );

                g.FillRectangle(fillSolidBrush,
                     this.Bounds.X + (int)(this.Bounds.Width * 0.33) + 1,
                    this.Bounds.Y + (int)(this.Bounds.Height * 0.36) + 1,
                    (int)(this.Bounds.Width * 0.39) - 2,
                    (int)(this.Bounds.Height * 0.31) - 2
                );
            }
        }

        private void DrawMinButton(Pen pen, Graphics g)
        {
            int x = this.Bounds.X;
            int y = this.Bounds.Y;

            g.DrawLine(
                pen,
                x + (int)(this.Bounds.Width * 0.33),
                y + (int)(this.Bounds.Height * 0.66),
                x + (int)(this.Bounds.Width * 0.66),
                y + (int)(this.Bounds.Height * 0.66)
           );
        }
    }

    /// <summary>
    /// 按钮类型枚举
    /// </summary>
    public enum TitleItemAction
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 最大化
        /// </summary>
        Max = 1,
        /// <summary>
        /// 最小化
        /// </summary>
        Min = 2,
        /// <summary>
        /// 关闭
        /// </summary>
        Close = 3,
        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 99
    }
}