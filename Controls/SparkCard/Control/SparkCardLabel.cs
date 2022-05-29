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
    /// 文本标签控件。
    /// </summary>
    [Serializable]
    public class SparkCardLabel : SparkCardControl, IVisiblity, ITooltip, IToString
    {
        /// <summary>
        /// 获取或设置标签关联的文本。
        /// </summary>
        public StringExpression Value { get; set; }

        /// <summary>
        /// 获取或设置标签关联的提示信息。
        /// </summary>
        public StringExpression TooltipExpression { get; set; }

        /// <summary>
        /// 获取或设置标签显示的数据类型（数字、日期），用于数据格式化。
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 获取或设置标签的显示格式。
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 获取或设置标签的边框宽度。
        /// </summary>
        public int BorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置标签的边框颜色。
        /// </summary>
        public Color BorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置标签的边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置标签的背景色。
        /// </summary>
        public ColorExpression BackColor { get; set; } = new ColorExpression() { Value = Color.Transparent };

        /// <summary>
        /// 获取或设置标签的字体颜色。
        /// </summary>
        public ColorExpression ForeColor { get; set; } = new ColorExpression() { Value = Color.Black };

        /// <summary>
        /// 获取或设置标签的字体。
        /// </summary>
        public Font Font { get; set; } = Consts.DEFAULT_FONT;

        /// <summary>
        /// 获取或设置标签的对齐方式。
        /// </summary>
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.TopLeft;

        /// <summary>
        /// 获取或设置标签的可见状态。
        /// </summary>
        public BooleanExpression Visible { get; set; } = new BooleanExpression() { Value = true };

        /// <summary>
        /// 绘制的样式
        /// </summary>
        [DefaultValue(DrawShapeStyle.Rectangle)]
        public DrawShapeStyle ShapeStyle { get; set; } = DrawShapeStyle.Rectangle;

        /// <summary>
        /// 绘制控件。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="data">行数据源。</param>
        public override void Paint(Graphics g, DataRowView data)
        {
            if (!Visible[data]) { return; }
            Rectangle rectangle = Rectangle;

            if (ShapeStyle == DrawShapeStyle.Rectangle)
            {
                // 绘制边框
                if (BorderWidth > 0)
                {
                    using (Pen pen = new Pen(BorderColor, BorderWidth) { DashStyle = BorderStyle })
                    {
                        g.DrawRectangle(pen, rectangle);
                    }
                }
                // 充填背景
                g.FillRectangle(new SolidBrush(BackColor[data]), rectangle);
            }
            else
            {
                Rectangle rect = Rectangle.Empty;
                if (Rectangle.Width > Rectangle.Height)
                {
                    rect = new Rectangle((Rectangle.Width - Rectangle.Height) / 2, 0, Rectangle.Height, Rectangle.Height);
                }
                else
                {
                    rect = new Rectangle(0, (Rectangle.Height - Rectangle.Width) / 2, Rectangle.Width, Rectangle.Width);
                }
                g.FillEllipse(new SolidBrush(this.BorderColor), rect);
            }

            // 设置格式
            TextAlign.ToStringAlignment(out StringAlignment hsa, out StringAlignment vsa);
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit)
            {
                Alignment = hsa,
                LineAlignment = vsa,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                // 绘制
                g.DrawString(Value[data], Font, new SolidBrush(ForeColor[data]), rectangle, format);
            }

            // 递归
            if (Children != null && Children.Count > 0)
            {
                Children.ForEach(c => c.Paint(g, data));
            }
        }

        /// <summary>
        /// 将 <see cref="SparkCardLabel"/> 的 xml 表示形式转换为与它等效的 <see cref="SparkCardLabel"/> 实例。
        /// </summary>
        /// <param name="element">待转换的 xml 元素。</param>
        /// <returns>转换生成的 <see cref="SparkCardLabel"/> 实例。</returns>
        internal static SparkCardLabel ParseXml(XElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element), "值不能为空。");

            SparkCardLabel ctrl = new SparkCardLabel
            {
                Type = element.GetAttributeValue("type"),
            };
            ctrl.FromXml(element);

            if (GetPropertyValue<StringExpression, StringExpressionConverter>(element, "Value", out StringExpression value)) ctrl.Value = value;
            if (GetPropertyValue<StringExpression, StringExpressionConverter>(element, "Tooltip", out StringExpression tooltip)) ctrl.TooltipExpression = tooltip;
            if (GetPropertyValue<string, StringConverter>(element, "DataType", out string dataType)) ctrl.DataType = dataType;
            if (GetPropertyValue<string, StringConverter>(element, "Format", out string format)) ctrl.Format = format;
            if (GetPropertyValue<Color, ColorConverter>(element, "BorderColor", out Color borderColor)) ctrl.BorderColor = borderColor;
            if (GetPropertyValue<DashStyle, EnumConverter<DashStyle>>(element, "BorderStyle", out DashStyle borderStyle)) ctrl.BorderStyle = borderStyle;
            if (GetPropertyValue<int, Int32Converter>(element, "BorderWidth", out int borderWidth)) ctrl.BorderWidth = borderWidth;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "BackgroundColor", out ColorExpression backColor)) ctrl.BackColor = backColor;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "FontColor", out ColorExpression foreColor)) ctrl.ForeColor = foreColor;
            if (GetPropertyValue<Font, FontConverter>(element, "Font", out Font font)) ctrl.Font = font;
            if (GetPropertyValue<ContentAlignment, EnumConverter<ContentAlignment>>(element, "TextAlign", out ContentAlignment textAlign)) ctrl.TextAlign = textAlign;
            if (GetPropertyValue<BooleanExpression, BooleanExpressionConverter>(element, "Visiblity", out BooleanExpression visiblity)) ctrl.Visible = visiblity;

            if (GetPropertyValue<DrawShapeStyle, EnumConverter<DrawShapeStyle>>(element, "ShapeStyle", out DrawShapeStyle shapeStyle)) ctrl.ShapeStyle = shapeStyle;

            return ctrl;
        }

        /// <summary>
        /// 转换为等效的字符串表示形式。
        /// </summary>
        /// <param name="datasource">数据源对象。</param>
        /// <returns>对象的字符串表示形式。</returns>
        public string ToString(object datasource)
        {
            if (datasource is DataRowView drv)
            {
                return Value[drv];
            }
            return string.Empty;
        }

        public override IDrawStyle CreateStyle(DataRowView dataRowView, IDrawStyle parent)
        {
            var style = new SparkCardLabelStyle();
            style.Init(this, dataRowView, parent);
            return style;
        }
    }



    public class SparkCardLabelStyle : DrawStyle, ITooltipString, IVisiblityBool
    {
        /// <summary>
        /// 获取或设置标签关联的文本。
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 获取或设置标签关联的提示信息。
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// 获取或设置标签显示的数据类型（数字、日期），用于数据格式化。
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 获取或设置标签的显示格式。
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 获取或设置标签的边框宽度。
        /// </summary>
        public int BorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置标签的边框颜色。
        /// </summary>
        public Color BorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置标签的边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置标签的背景色。
        /// </summary>
        public Color BackColor { get; set; } = Color.Transparent;

        /// <summary>
        /// 获取或设置标签的字体颜色。
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>
        /// 获取或设置标签的字体。
        /// </summary>
        public Font Font { get; set; } = Consts.DEFAULT_FONT;

        /// <summary>
        /// 获取或设置标签的对齐方式。
        /// </summary>
        public ContentAlignment TextAlign { get; set; } = ContentAlignment.TopLeft;

        /// <summary>
        /// 绘制的样式
        /// </summary>
        [DefaultValue(DrawShapeStyle.Rectangle)]
        public DrawShapeStyle ShapeStyle { get; set; } = DrawShapeStyle.Rectangle;

        public override void Init(SparkCardControl trtCardStyle, DataRowView dataRowView, IDrawStyle parent)
        {
            this.Parent = parent;
            SparkCardLabel trtCardLabel = trtCardStyle as SparkCardLabel;
            this.Value = trtCardLabel.Value.GetValue(dataRowView);
            this.Visible = trtCardLabel.Visible.GetValue(dataRowView);
            this.BorderWidth = trtCardLabel.BorderWidth;
            this.BorderColor = trtCardLabel.BorderColor;
            this.BorderStyle = trtCardLabel.BorderStyle;
            this.BackColor = trtCardLabel.BackColor.GetValue(dataRowView);
            this.ForeColor = trtCardLabel.ForeColor.GetValue(dataRowView);
            this.Font = trtCardLabel.Font;
            this.TextAlign = trtCardLabel.TextAlign;

            this.ShapeStyle = trtCardLabel.ShapeStyle;

            this.Tooltip = trtCardLabel.TooltipExpression.GetValue(dataRowView);
            //this.DataType = trtCardLabel.DataType;
            //this.Format = trtCardLabel.Format;
            this.Size = trtCardLabel.Size;
            this.Location = trtCardLabel.Location;

        }

        public override void Paint(Graphics g)
        {
            if (!Visible) { return; }
            Rectangle rectangle = this.Rectangle;

            if (ShapeStyle == DrawShapeStyle.Rectangle)
            {
                // 绘制边框
                if (BorderWidth > 0)
                {
                    using (Pen pen = new Pen(BorderColor, BorderWidth) { DashStyle = BorderStyle })
                    {
                        g.DrawRectangle(pen, rectangle);
                    }
                }

                // 充填背景
                g.FillRectangle(new SolidBrush(BackColor), rectangle);
            }
            else
            {
                Rectangle rect = Rectangle.Empty;
                if (Rectangle.Width > Rectangle.Height)
                {
                    rect = new Rectangle(Rectangle.X + (Rectangle.Width - Rectangle.Height) / 2, Rectangle.Y, Rectangle.Height, Rectangle.Height);
                }
                else
                {
                    rect = new Rectangle(Rectangle.X, Rectangle.Y + (Rectangle.Height - Rectangle.Width) / 2, Rectangle.Width, Rectangle.Width);
                }
                g.FillEllipse(new SolidBrush(this.BorderColor), rect);
            }

            // 设置格式
            TextAlign.ToStringAlignment(out StringAlignment hsa, out StringAlignment vsa);
            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit)
            {
                Alignment = hsa,
                LineAlignment = vsa,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                // 绘制
                g.DrawString(Value, Font, new SolidBrush(ForeColor), rectangle, format);
            }

        }

        public override string ToString()
        {
            return this.Value;
        }
    }

    public enum DrawShapeStyle
    {
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle = 0,
        /// <summary>
        /// 圆形
        /// </summary>
        Circle
    }
}