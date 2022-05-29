using System;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 工具栏绘制的渲染器Renderer
    /// </summary>
    public class SparkNavigateBarToolStripRenderer : SparkToolStripRenderer
    {
        public SparkNavigateBarToolStripRenderer(SparkToolStripTheme theme) : base(theme)
        {

        }

        /// <summary>
        /// 绘制每个按钮的Text文字及颜色
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            //修正ToolStripDropDown的子项文本不居中的问题
            if (e.ToolStrip is ToolStripOverflow)
            {
                //Overflow不需要修正
            }
            else if (e.ToolStrip is ToolStripDropDown || e.ToolStrip is ContextMenuStrip)
            {
                e.TextRectangle = new Rectangle(new Point(e.TextRectangle.X, e.TextRectangle.Y + e.Item.Padding.Vertical / 2), e.TextRectangle.Size);
            }
            else
            {
                if (e.Item.Text.Contains("\\r\\n"))
                {
                    e.Item.Text = e.Item.Text.Replace("\\r\\n", Environment.NewLine);
                }
                e.TextFormat = TextFormatFlags.Default;
                if (e.Item.Selected || e.Item.Pressed)
                {
                    e.Item.ForeColor = this.Theme.SelectedForeColor;
                }
                else
                {
                    e.Item.ForeColor = this.Theme.ForeColor;
                }
            }

            if (e.Item is ToolStripMenuItem && (e.Item.Selected || e.Item.Pressed))
            {
                e.TextColor = e.Item.ForeColor;
            }
            base.OnBaseRenderItemText(e);
        }

        /// <summary>
        /// 绘制按钮的背景色
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderButtonBackground(e);
            if (e.Item is ToolStripButton button && button.CheckState == CheckState.Checked)
            {
                RoundRectangle roundRect = new RoundRectangle(new Rectangle(1, 1, button.Width - 4, button.Height - 3), this.ItemCornerRadius);
                //checked为true状态下
                GDIHelper.FillRectangle(e.Graphics, roundRect, Theme.HighlightColor);
                e.Item.ForeColor = this.Theme.SelectedForeColor;
            }
        }
    }
}