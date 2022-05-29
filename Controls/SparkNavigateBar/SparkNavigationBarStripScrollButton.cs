using System;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 按钮超出容器的边界，上下或左右箭头按钮。
    /// </summary>
    [System.ComponentModel.DesignerCategory("Code")]
    internal sealed class SparkNavigationBarStripScrollButton : SparkNavigationBarStripButton
    {
        private readonly TabStripScrollDirection _direction;

        public SparkNavigationBarStripScrollButton(TabStripScrollDirection direction)
        {
            if (direction != TabStripScrollDirection.Left &&
                direction != TabStripScrollDirection.Right)
            {
                throw new ArgumentOutOfRangeException("direction");
            }
            this._direction = direction;
        }

        public TabStripScrollDirection ScrollDirection => this._direction;

        public override Size GetPreferredSize(Size constrainingSize)
        {
            return new Size(17, constrainingSize.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.Owner != null)
            {
                switch (this.Owner.Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        //▲▼◀▶
                        if (this._direction == TabStripScrollDirection.Left)
                        {
                            this.Text = "◀";
                        }
                        else
                        {
                            this.Text = "▶";
                        }
                        break;
                    case DockStyle.Left:
                    case DockStyle.Right:
                        if (this._direction == TabStripScrollDirection.Left)
                        {
                            this.Text = "▲";
                        }
                        else
                        {
                            this.Text = "▼";
                        }
                        break;
                }
            }
            //if (Renderer != null)
            //{
            //    var chevronRect = new Rectangle(
            //        new Point(Padding.Left, Padding.Top),
            //        new Size(Width - Padding.Horizontal, Height - Padding.Vertical));

            //    var args = new TabStripScrollButtonRenderEventArgs(e.Graphics, this, chevronRect);

            //    Renderer.DrawTabScrollChevron(args);
            //}
        }
    }
}