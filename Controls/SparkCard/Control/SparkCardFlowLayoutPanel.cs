using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

using SparkControls.Controls;

namespace SparkControls.Controls
{
    /// <summary>
    /// 流布局控件。
    /// </summary>
    [Serializable]
    public class SparkCardFlowLayoutPanel : SparkCardControl, IVisiblity
    {
        /// <summary>
        /// 获取或设置面板的边框宽度。
        /// </summary>
        public int BorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置面板的边框颜色。
        /// </summary>
        public Color BorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置面板的边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置面板的背景色。
        /// </summary>
        public ColorExpression BackColor { get; set; } = new ColorExpression() { Value = Color.Transparent };

        /// <summary>
        /// 获取或设置面板的字体颜色。
        /// </summary>
        public ColorExpression ForeColor { get; set; } = new ColorExpression() { Value = Color.Black };

        /// <summary>
        /// 获取或设置面板的可见状态。
        /// </summary>
        public BooleanExpression Visible { get; set; } = new BooleanExpression() { Value = true };

        /// <summary>
        /// 获取或设置面板的布局方向。
        /// </summary>
        public FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;

        /// <summary>
        /// 获取或设置一个值，该值指示应当对控件内容换行还是裁剪。
        /// </summary>
        public bool WrapContents { get; set; } = false;

        /// <summary>
        /// 绘制控件。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="data">行数据源。</param>
        public override void Paint(Graphics g, DataRowView data)
        {
            if (!Visible[data]) { return; }
            Rectangle rectangle = Rectangle;

            // 绘制边框
            if (BorderWidth > 0)
            {
                using (Pen pen = new Pen(BorderColor, BorderWidth) { DashStyle = BorderStyle })
                {
                    g.DrawRectangle(pen, rectangle);
                }
            }
            // 填充背景
            g.FillRectangle(new SolidBrush(BackColor[data]), rectangle);

            // 递归
            if (Children != null && Children.Count > 0)
            {
                int maxWidth = 0;
                int maxHeight = 0;

                int x = FlowDirection == FlowDirection.RightToLeft ? this.Location.X + this.Size.Width : 0;
                int y = FlowDirection == FlowDirection.BottomUp ? this.Location.Y + this.Size.Height : 0;
                Children.ForEach(c =>
                {
                    if (c is IVisiblity v && v.Visible[data] == true || c is IVisiblity == false)
                    {
                        // 一行控件的最大宽度和最大高度
                        if (maxWidth < c.Size.Width) { maxWidth = c.Size.Width; }
                        if (maxHeight < c.Size.Height) { maxHeight = c.Size.Height; }

                        // 计算坐标
                        switch (FlowDirection)
                        {
                            case FlowDirection.LeftToRight:
                                x += c.Margin.Left;
                                if (x > this.Location.X + this.Size.Width)
                                {
                                    x = c.Margin.Left;
                                    y += maxHeight;
                                    maxHeight = 0;
                                }
                                break;
                            case FlowDirection.RightToLeft:
                                x = x - c.Margin.Right - c.Size.Width;
                                if (x < 0)
                                {
                                    x = this.Location.X + this.Size.Width - c.Margin.Right - c.Size.Width;
                                    y += maxHeight;
                                    maxHeight = 0;
                                }
                                break;
                            case FlowDirection.TopDown:
                                y += c.Margin.Top;
                                if (y > this.Location.Y + this.Size.Height)
                                {
                                    y = c.Margin.Top;
                                    x += maxWidth;
                                    maxWidth = 0;
                                }
                                break;
                            case FlowDirection.BottomUp:
                                y = y - c.Margin.Bottom - c.Size.Height;
                                if (y < 0)
                                {
                                    y = this.Location.Y + this.Size.Height - c.Margin.Bottom - c.Size.Height;
                                    x += maxWidth;
                                    maxWidth = 0;
                                }
                                break;
                        }

                        // 绘制子控件
                        c.Location = new Point(x, y);
                        c.Paint(g, data);

                        // 坐标递增
                        switch (FlowDirection)
                        {
                            case FlowDirection.LeftToRight:
                                x += c.Size.Width;
                                break;
                            case FlowDirection.RightToLeft:
                                x -= c.Size.Width;
                                break;
                            case FlowDirection.TopDown:
                                y += c.Size.Height;
                                break;
                            case FlowDirection.BottomUp:
                                y -= c.Size.Height;
                                break;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 将 <see cref="SparkCardFlowLayoutPanel"/> 的 xml 表示形式转换为与它等效的 <see cref="SparkCardFlowLayoutPanel"/> 实例。
        /// </summary>
        /// <param name="element">待转换的 xml 元素。</param>
        /// <returns>转换生成的 <see cref="SparkCardFlowLayoutPanel"/> 实例。</returns>
        internal static SparkCardFlowLayoutPanel ParseXml(XElement element)
        {
            SparkCardFlowLayoutPanel ctrl = new SparkCardFlowLayoutPanel
            {
                Type = element.GetAttributeValue("type"),
            };
            ctrl.FromXml(element);

            if (GetPropertyValue<Color, ColorConverter>(element, "BorderColor", out Color borderColor)) ctrl.BorderColor = borderColor;
            if (GetPropertyValue<DashStyle, EnumConverter<DashStyle>>(element, "BorderStyle", out DashStyle borderStyle)) ctrl.BorderStyle = borderStyle;
            if (GetPropertyValue<int, Int32Converter>(element, "BorderWidth", out int borderWidth)) ctrl.BorderWidth = borderWidth;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "BackgroundColor", out ColorExpression backColor)) ctrl.BackColor = backColor;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "FontColor", out ColorExpression foreColor)) ctrl.ForeColor = foreColor;
            if (GetPropertyValue<BooleanExpression, BooleanExpressionConverter>(element, "Visiblity", out BooleanExpression visiblity)) ctrl.Visible = visiblity;
            if (GetPropertyValue<FlowDirection, EnumConverter<FlowDirection>>(element, "FlowDirection", out FlowDirection flowDirection)) ctrl.FlowDirection = flowDirection;
            if (GetPropertyValue<bool, BooleanConverter>(element, "Visiblity", out bool wrapContents)) ctrl.WrapContents = wrapContents;

            return ctrl;
        }

        public override IDrawStyle CreateStyle(DataRowView dataRowView, IDrawStyle parent)
        {
            var style = new SparkCardFlowLayoutPanelStyle();
            style.Init(this, dataRowView, parent);
            return style;
        }
    }

    public class SparkCardFlowLayoutPanelStyle : DrawStyle
    {
        /// <summary>
        /// 获取或设置面板的边框宽度。
        /// </summary>
        public int BorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置面板的边框颜色。
        /// </summary>
        public Color BorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置面板的边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置面板的背景色。
        /// </summary>
        public Color BackColor { get; set; } = Color.Transparent;

        /// <summary>
        /// 获取或设置面板的布局方向。
        /// </summary>
        public FlowDirection FlowDirection { get; set; } = FlowDirection.LeftToRight;

        public override void Init(SparkCardControl trtCardControl, DataRowView dataRowView, IDrawStyle parent)
        {
            this.Parent = parent;
            var trtCardFlowLayoutPanel = trtCardControl as SparkCardFlowLayoutPanel;
            this.Size = trtCardFlowLayoutPanel.Size;
            this.Location = trtCardFlowLayoutPanel.Location;
            this.Visible = trtCardFlowLayoutPanel.Visible.GetValue(dataRowView);
            this.BorderWidth = trtCardFlowLayoutPanel.BorderWidth;
            this.BorderColor = trtCardFlowLayoutPanel.BorderColor;
            this.BorderStyle = trtCardFlowLayoutPanel.BorderStyle;
            this.BackColor = trtCardFlowLayoutPanel.BackColor.GetValue(dataRowView);
            this.FlowDirection = trtCardFlowLayoutPanel.FlowDirection;
            this.Margin = trtCardFlowLayoutPanel.Margin;

            if (trtCardFlowLayoutPanel.Children?.Any() == true)
            {
                Children = trtCardFlowLayoutPanel.Children.Select(a => a.CreateStyle(dataRowView, this));
            }
        }

        public override void Paint(Graphics g)
        {
            if (!Visible) { return; }
            Rectangle rectangle = this.Rectangle;

            // 绘制边框
            if (BorderWidth > 0)
            {
                using (Pen pen = new Pen(BorderColor, BorderWidth) { DashStyle = BorderStyle })
                {
                    g.DrawRectangle(pen, rectangle);
                }
            }
            // 填充背景
            g.FillRectangle(new SolidBrush(BackColor), rectangle);

            // 递归
            if (Children?.Any() == true)
            {
                int maxWidth = 0;
                int maxHeight = 0;

                int x = FlowDirection == FlowDirection.RightToLeft ? this.Location.X + this.Size.Width : 0;
                int y = FlowDirection == FlowDirection.BottomUp ? this.Location.Y + this.Size.Height : 0;
                Children.ForEach(c =>
                {
                    if (c is IVisiblityBool visiblity && visiblity.Visible || !(c is IVisiblityBool))
                    {
                        // 一行控件的最大宽度和最大高度
                        if (maxWidth < c.Size.Width) { maxWidth = c.Size.Width; }
                        if (maxHeight < c.Size.Height) { maxHeight = c.Size.Height; }

                        // 计算坐标
                        switch (FlowDirection)
                        {
                            case FlowDirection.LeftToRight:
                                x += c.Margin.Left;
                                if (x > this.Location.X + this.Size.Width)
                                {
                                    x = c.Margin.Left;
                                    y += maxHeight;
                                    maxHeight = 0;
                                }
                                break;
                            case FlowDirection.RightToLeft:
                                x = x - c.Margin.Right - c.Size.Width;
                                if (x < 0)
                                {
                                    x = this.Location.X + this.Size.Width - c.Margin.Right - c.Size.Width;
                                    y += maxHeight;
                                    maxHeight = 0;
                                }
                                break;
                            case FlowDirection.TopDown:
                                y += c.Margin.Top;
                                if (y > this.Location.Y + this.Size.Height)
                                {
                                    y = c.Margin.Top;
                                    x += maxWidth;
                                    maxWidth = 0;
                                }
                                break;
                            case FlowDirection.BottomUp:
                                y = y - c.Margin.Bottom - c.Size.Height;
                                if (y < 0)
                                {
                                    y = this.Location.Y + this.Size.Height - c.Margin.Bottom - c.Size.Height;
                                    x += maxWidth;
                                    maxWidth = 0;
                                }
                                break;
                        }

                        // 绘制子控件
                        c.Location = new Point(x, y);
                        c.Paint(g);

                        // 坐标递增
                        switch (FlowDirection)
                        {
                            case FlowDirection.LeftToRight:
                                x += c.Size.Width;
                                break;
                            case FlowDirection.RightToLeft:
                                x -= c.Size.Width;
                                break;
                            case FlowDirection.TopDown:
                                y += c.Size.Height;
                                break;
                            case FlowDirection.BottomUp:
                                y -= c.Size.Height;
                                break;
                        }
                    }
                });
            }
        }
    }

}
