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
    /// 图片框控件。
    /// </summary>
    [Serializable]
    public class SparkCardPicture : SparkCardControl, IVisiblity, ITooltip
    {
        /// <summary>
        /// 获取或设置图片内容。
        /// </summary>
        public ImageExpression Image { get; set; }

        /// <summary>
        /// 获取或设置标签关联的提示信息。
        /// </summary>
        public StringExpression TooltipExpression { get; set; }

        /// <summary>
        /// 获取或设置图片的边框宽度。
        /// </summary>
        public int BorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置图片的边框颜色。
        /// </summary>
        public Color BorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置图片的边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置标签的可见状态。
        /// </summary>
        public BooleanExpression Visible { get; set; } = new BooleanExpression() { Value = true };

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

            // 绘制图片
            Image image = Image[data];
            if (image != null)
            {
                g.DrawImage(image, rectangle);
            }

            // 递归
            if (Children != null && Children.Count > 0)
            {
                Children.ForEach(c => c.Paint(g, data));
            }
        }

        /// <summary>
        /// 将 <see cref="SparkCardPicture"/> 的 xml 表示形式转换为与它等效的 <see cref="SparkCardPicture"/> 实例。
        /// </summary>
        /// <param name="element">待转换的 xml 元素。</param>
        /// <returns>转换生成的 <see cref="SparkCardPicture"/> 实例。</returns>
        internal static SparkCardPicture ParseXml(XElement element)
        {
            SparkCardPicture ctrl = new SparkCardPicture
            {
                Type = element.GetAttributeValue("type"),
            };
            ctrl.FromXml(element);

            if (GetPropertyValue<ImageExpression, ImageExpressionConverter>(element, "Image", out ImageExpression image)) ctrl.Image = image;
            if (GetPropertyValue<StringExpression, StringExpressionConverter>(element, "Tooltip", out StringExpression tooltip)) ctrl.TooltipExpression = tooltip;
            if (GetPropertyValue<Color, ColorConverter>(element, "BorderColor", out Color borderColor)) ctrl.BorderColor = borderColor;
            if (GetPropertyValue<DashStyle, EnumConverter<DashStyle>>(element, "BorderStyle", out DashStyle borderStyle)) ctrl.BorderStyle = borderStyle;
            if (GetPropertyValue<int, Int32Converter>(element, "BorderWidth", out int borderWidth)) ctrl.BorderWidth = borderWidth;
            if (GetPropertyValue<BooleanExpression, BooleanExpressionConverter>(element, "Visiblity", out BooleanExpression visiblity)) ctrl.Visible = visiblity;

            return ctrl;
        }

        public override IDrawStyle CreateStyle(DataRowView dataRowView, IDrawStyle parent)
        {
            var style = new SparkCardPictureStyle();
            style.Init(this, dataRowView, parent);
            return style;
        }
    }


    public class SparkCardPictureStyle : DrawStyle, IVisiblityBool
    {
        /// <summary>
        /// 获取或设置图片内容。
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// 获取或设置标签关联的提示信息。
        /// </summary>
        public string TooltipExpression { get; set; }

        /// <summary>
        /// 获取或设置图片的边框宽度。
        /// </summary>
        public int BorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置图片的边框颜色。
        /// </summary>
        public Color BorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置图片的边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        public override void Init(SparkCardControl trtCardControl, DataRowView dataRowView, IDrawStyle parent)
        {
            this.Parent = parent;
            var trtCardPicture = trtCardControl as SparkCardPicture;
            this.Visible = trtCardPicture.Visible.GetValue(dataRowView);
            this.BorderWidth = trtCardPicture.BorderWidth;
            this.BorderStyle = trtCardPicture.BorderStyle;
            this.Image = trtCardPicture.Image.GetValue(dataRowView);
            this.BorderColor = trtCardPicture.BorderColor;
            this.Size = trtCardPicture.Size;
            this.Location = trtCardPicture.Location;
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

            // 绘制图片
            Image image = Image;
            if (image != null)
            {
                g.DrawImage(image, rectangle);
            }

            // 递归
            if (Children?.Any() == true)
            {
                Children.ForEach(c => c.Paint(g));
            }
        }
    }

}