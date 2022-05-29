using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SparkControls.Controls
{
    public class SparkCardTemplate : SparkCardControl
    {
        /// <summary>
        /// 获取或设置控件位置。
        /// </summary>
        public override Point Location
        {
            get => base.Location;
            set
            {
                base.Location = value;
            }
        }

        /// <summary>
        /// 获取或设置面板背景色。
        /// </summary>
        public ColorExpression BackColor { get; set; } = new ColorExpression() { Value = Color.FromKnownColor(KnownColor.Control) };

        /// <summary>
        /// 获取或设置面板背景图片。
        /// </summary>
        public ImageExpression BackgroundImage { get; set; }

        /// <summary>
        /// 获取或设置面板边框宽度。
        /// </summary>
        public int BorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置面板边框颜色。
        /// </summary>
        public Color BorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置面板边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置面板激活状态的背景色。
        /// </summary>
        public ColorExpression ActivedBackColor { get; set; } = new ColorExpression() { Value = Color.LightGray };

        /// <summary>
        /// 获取或设置面板激活状态的背景图片。
        /// </summary>
        public ImageExpression ActivedBackgroundImage { get; set; }

        /// <summary>
        /// 获取或设置面板激活状态的边框宽度。
        /// </summary>
        public int ActivedBorderWidth { get; set; } = 0;

        /// <summary>
        /// 获取或设置面板激活状态的边框颜色。
        /// </summary>
        public Color ActivedBorderColor { get; set; } = Color.LightGray;

        /// <summary>
        /// 获取或设置面板激活状态的边框样式。
        /// </summary>
        public DashStyle ActivedBorderStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置面板的字体。
        /// </summary>
        public Font Font { get; set; } = Consts.DEFAULT_FONT;

        /// <summary>
        /// 获取或设置角带显示标志。
        /// </summary>
        public bool ShowBand { get; set; } = true;

        /// <summary>
        /// 获取或设置角带起始位置。
        /// </summary>
        public int BandStart { get; set; } = 30;

        /// <summary>
        /// 获取或设置角带宽带。
        /// </summary>
        public int BandWidth { get; set; } = 28;

        /// <summary>
        /// 获取或设置角带背景色。
        /// </summary>
        public ColorExpression BandBackColor { get; set; } = new ColorExpression() { Value = ColorTranslator.FromHtml("#3894FB") };

        /// <summary>
        /// 获取或设置角带前景色。
        /// </summary>
        public ColorExpression BandForeColor { get; set; } = new ColorExpression() { Value = Color.White };

        /// <summary>
        /// 获取或设置角带文本。
        /// </summary>
        public StringExpression BandText { get; set; } = new StringExpression();

        /// <summary>
        /// 将 <see cref="SparkCard"/> 的 xml 表示形式转换为与它等效的 <see cref="SparkCard"/> 实例。
        /// </summary>
        /// <param name="element">待转换的 xml 元素。</param>
        /// <returns>转换生成的 <see cref="SparkCard"/> 实例。</returns>
        internal static SparkCardTemplate ParseXml(XElement element)
        {
            var props = element.Elements("Property");
            SparkCardTemplate ctrl = new SparkCardTemplate
            {
                Type = element.GetAttributeValue("type"),
                Name = props.FirstOrDefault(p => p.GetAttributeValue("name") == "Name")?.Value
            };

            if (GetPropertyValue<Size, SizeConverter>(element, "Size", out Size size)) ctrl.Size = size;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "BackgroundColor", out ColorExpression backColor)) ctrl.BackColor = backColor;
            if (GetPropertyValue<ImageExpression, ImageExpressionConverter>(element, "BackgroundImage", out ImageExpression backgroundImage)) ctrl.BackgroundImage = backgroundImage;
            if (GetPropertyValue<Color, ColorConverter>(element, "BorderColor", out Color borderColor)) ctrl.BorderColor = borderColor;
            if (GetPropertyValue<DashStyle, EnumConverter<DashStyle>>(element, "BorderStyle", out DashStyle borderStyle)) ctrl.BorderStyle = borderStyle;
            if (GetPropertyValue<int, Int32Converter>(element, "BorderWidth", out int borderWidth)) ctrl.BorderWidth = borderWidth;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "ActivedBackgroundColor", out ColorExpression activedbackColor)) ctrl.ActivedBackColor = activedbackColor;
            if (GetPropertyValue<ImageExpression, ImageExpressionConverter>(element, "ActivedBackgroundImage", out ImageExpression activedBackgroundImage)) ctrl.ActivedBackgroundImage = activedBackgroundImage;
            if (GetPropertyValue<Color, ColorConverter>(element, "ActivedBorderColor", out Color activedBorderColor)) ctrl.ActivedBorderColor = activedBorderColor;
            if (GetPropertyValue<DashStyle, EnumConverter<DashStyle>>(element, "ActivedBorderStyle", out DashStyle activedBorderStyle)) ctrl.ActivedBorderStyle = activedBorderStyle;
            if (GetPropertyValue<int, Int32Converter>(element, "ActivedBorderWidth", out int activedBorderWidth)) ctrl.ActivedBorderWidth = activedBorderWidth;
            if (GetPropertyValue<Font, FontConverter>(element, "Font", out Font font)) ctrl.Font = font;
            if (GetPropertyValue<bool, BooleanConverter>(element, "ShowBand", out bool propValue)) ctrl.ShowBand = propValue;
            if (GetPropertyValue<int, Int32Converter>(element, "BandStart", out int bandStart)) ctrl.BandStart = bandStart;
            if (GetPropertyValue<int, Int32Converter>(element, "BandWidth", out int bandWidth)) ctrl.BandWidth = bandWidth;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "BandBackColor", out ColorExpression bandBackColor)) ctrl.BandBackColor = bandBackColor;
            if (GetPropertyValue<ColorExpression, ColorExpressionConverter>(element, "BandForeColor", out ColorExpression bandForeColor)) ctrl.BandForeColor = bandForeColor;
            if (GetPropertyValue<StringExpression, StringExpressionConverter>(element, "BandText", out StringExpression bandText)) ctrl.BandText = bandText;

            return ctrl;
        }

        /// <summary>
        /// 从指定的 xml 元素创建 <see cref="SparkCard"/> 模板。
        /// </summary>
        /// <param name="template">xml 模板元素。</param>
        /// <returns><see cref="SparkCard"/> 实例。</returns>
        public static SparkCardTemplate FromXml(XElement template)
        {
            return (SparkCardTemplate)GetTemplateControl(template);
        }

        /// <summary>
        /// 从指定的 xml 串创建 <see cref="SparkCard"/> 模板。
        /// </summary>
        /// <param name="template">xml 模板串。</param>
        /// <returns><see cref="SparkCard"/> 实例。</returns>
        public static SparkCardTemplate FromXml(string template)
        {
            return FromXml(XElement.Parse(template));
        }

        /// <summary>
        /// 生成模板控件。
        /// </summary>
        /// <param name="template">xml 模板元素。</param>
        /// <param name="tooltips">提示信息集合。</param>
        private static SparkCardControl GetTemplateControl(XElement template)
        {
            SparkCardControl ctrl = null;
            if (template != null && template.Name == "Object")
            {
                string type = template.GetAttributeValue("type")?.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault();
                switch (type)
                {
                    case SparkCardConsts.OBJECT_TYPE_CONTROL:
                        ctrl = SparkCardTemplate.ParseXml(template);
                        break;
                    case SparkCardConsts.OBJECT_TYPE_LABEL:
                        ctrl = SparkCardLabel.ParseXml(template);
                        break;
                    case SparkCardConsts.OBJECT_TYPE_PICTURE:
                        ctrl = SparkCardPicture.ParseXml(template);
                        break;
                    case SparkCardConsts.OBJECT_TYPE_LINE:
                        ctrl = SparkCardLine.ParseXml(template);
                        break;
                    case SparkCardConsts.OBJECT_TYPE_FLOWLAYOUT:
                        ctrl = SparkCardFlowLayoutPanel.ParseXml(template);
                        break;
                }

                // 递归
                if (ctrl != null)
                {
                    var children = template.Elements("Object");
                    if (children.Any())
                    {
                        foreach (var child in children)
                        {
                            ctrl.Children.Add(GetTemplateControl(child));
                        }
                    }
                }
            }

            return ctrl;
        }

        public override void Paint(Graphics g, DataRowView data)
        {
        }

        public override IDrawStyle CreateStyle(DataRowView dataRowView, IDrawStyle parent)
        {
            var style = new SparkCardTemplateStyle();
            style.Init(this, dataRowView, parent);
            return style;
        }
    }

    public abstract class DrawStyle : IDrawStyle
    {
        public virtual Rectangle Rectangle => new Rectangle(new Point(this.Location.X + this.Parent.Location.X,
            this.Location.Y + this.Parent.Location.Y),
            this.Size);

        public virtual Padding Margin { get; set; }
        public virtual bool Visible { get; set; } = true;
        public virtual Size Size { get; set; }
        public virtual Point Location { get; set; }
        public virtual IEnumerable<IDrawStyle> Children { get; protected set; }

        public virtual IDrawStyle Parent { get; protected set; } = null;

        public virtual void Init(SparkCardControl trtCardControl, DataRowView dataRowView, IDrawStyle parent)
        {
            throw new NotImplementedException();
        }

        public virtual void Paint(Graphics g)
        {
            throw new NotImplementedException();
        }
    }


    public class SparkCardTemplateStyle : DrawStyle
    {
        public override Rectangle Rectangle => new Rectangle(new Point(this.Location.X, this.Location.Y), this.Size);

        /// <summary>
        /// 获取或设置面板是否处于激活状态。
        /// </summary>
        public bool IsActived { get; set; }

        /// <summary>
        /// 获取或设置面板背景色。
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        /// 获取或设置面板背景图片。
        /// </summary>
        public Image BackgroundImage { get; set; }

        /// <summary>
        /// 获取或设置面板边框宽度。
        /// </summary>
        public int BorderWidth { get; set; }

        /// <summary>
        /// 获取或设置面板边框颜色。
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// 获取或设置面板边框样式。
        /// </summary>
        public DashStyle BorderStyle { get; set; }

        /// <summary>
        /// 获取或设置面板激活状态的背景色。
        /// </summary>
        public Color ActivedBackColor { get; set; }

        /// <summary>
        /// 获取或设置面板激活状态的背景图片。
        /// </summary>
        public Image ActivedBackgroundImage { get; set; }

        /// <summary>
        /// 获取或设置面板激活状态的边框宽度。
        /// </summary>
        public int ActivedBorderWidth { get; set; }

        /// <summary>
        /// 获取或设置面板激活状态的边框颜色。
        /// </summary>
        public Color ActivedBorderColor { get; set; }

        /// <summary>
        /// 获取或设置面板激活状态的边框样式。
        /// </summary>
        public DashStyle ActivedBorderStyle { get; set; }

        /// <summary>
        /// 获取或设置面板的字体。
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// 获取或设置角带显示标志。
        /// </summary>
        public bool ShowBand { get; set; }

        /// <summary>
        /// 获取或设置角带起始位置。
        /// </summary>
        public int BandStart { get; set; }

        /// <summary>
        /// 获取或设置角带宽带。
        /// </summary>
        public int BandWidth { get; set; }

        /// <summary>
        /// 获取或设置角带背景色。
        /// </summary>
        public Color BandBackColor { get; set; }

        /// <summary>
        /// 获取或设置角带前景色。
        /// </summary>
        public Color BandForeColor { get; set; }

        /// <summary>
        /// 获取或设置角带文本。
        /// </summary>
        public string BandText { get; set; }


        public override void Init(SparkCardControl trtCardControl, DataRowView dataRowView, IDrawStyle Parent)
        {
            var trtCardStyle = trtCardControl as SparkCardTemplate;
            this.BackColor = trtCardStyle.BackColor.GetValue(dataRowView);
            this.BackgroundImage = trtCardStyle.BackgroundImage?.GetValue(dataRowView);
            this.ActivedBackColor = trtCardStyle.ActivedBackColor.GetValue(dataRowView);
            this.ActivedBackgroundImage = trtCardStyle.ActivedBackgroundImage?.GetValue(dataRowView);
            this.BandBackColor = trtCardStyle.BandBackColor.GetValue(dataRowView);
            this.BandForeColor = trtCardStyle.BandForeColor.GetValue(dataRowView);
            this.BandText = trtCardStyle.BandText.GetValue(dataRowView);
            this.BorderWidth = trtCardStyle.BorderWidth;
            this.BorderColor = trtCardStyle.BorderColor;
            this.BorderStyle = trtCardStyle.BorderStyle;
            this.ActivedBorderWidth = trtCardStyle.ActivedBorderWidth;
            this.ActivedBorderColor = trtCardStyle.ActivedBorderColor;
            this.ActivedBorderStyle = trtCardStyle.ActivedBorderStyle;
            this.Font = trtCardStyle.Font;
            this.ShowBand = trtCardStyle.ShowBand;
            this.BandStart = trtCardStyle.BandStart;
            this.BandWidth = trtCardStyle.BandWidth;
            this.Size = trtCardStyle.Size;
            this.Location = trtCardStyle.Location;
            this.Visible = true;

            if (trtCardStyle.Children?.Any() == true)
            {
                Children = trtCardStyle.Children.Select(a => a.CreateStyle(dataRowView, this));
            }
        }

        /// <summary>
        /// 绘制控件。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        public override void Paint(Graphics g)
        {
            //绘制背景
            if (IsActived)
            {
                if (ActivedBackgroundImage != null)
                {
                    g.DrawImage(ActivedBackgroundImage, this.Rectangle);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(ActivedBackColor), this.Rectangle);
                }
            }
            else
            {
                if (BackgroundImage != null)
                {
                    g.DrawImage(BackgroundImage, this.Rectangle);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(BackColor), this.Rectangle);
                }
            }

            //绘制边框
            if (IsActived && ActivedBorderWidth > 0)
            {
                using (Pen pen = new Pen(ActivedBorderColor, ActivedBorderWidth) { DashStyle = ActivedBorderStyle })
                {
                    g.DrawRectangle(pen, this.Rectangle);
                }
            }
            else if (BorderWidth > 0)
            {
                using (Pen pen = new Pen(BorderColor, BorderWidth) { DashStyle = BorderStyle })
                {
                    g.DrawRectangle(pen, this.Rectangle);
                }
            }

            // 绘制角带
            if (ShowBand)
            {
                using (Brush brush = new SolidBrush(BandBackColor))
                {
                    Point[] points = {
                        new Point(Location.X + BandStart, Location.Y),
                        new Point(Location.X + BandStart + BandWidth, Location.Y),
                        new Point(Location.X, Location.Y + BandStart + BandWidth),
                        new Point(Location.X, Location.Y + BandStart)
                    };
                    g.FillPolygon(brush, points);

                    using (Pen pen = new Pen(brush, 2))
                    {
                        g.DrawLine(pen, new Point(Location.X + BandStart + BandWidth - 2, Location.Y + 1), new Point(Location.X + BandStart + BandWidth + 2, Location.Y + 1));
                        g.DrawLine(pen, new Point(Location.X + 1, Location.Y + BandStart + BandWidth - 2), new Point(Location.X + 1, Location.Y + BandStart + BandWidth + 2));
                    }
                }

                string text = BandText;
                if (!string.IsNullOrEmpty(text))
                {
                    RectangleF rectangle = new RectangleF(
                        this.Rectangle.X - 1,
                        this.Rectangle.Y + BandStart - 1,
                        (float)(BandStart / Math.Sin(Math.PI / 4)),
                        (float)(BandWidth * Math.Sin(Math.PI / 4))
                    );

                    Matrix mtxSave = g.Transform;

                    Matrix mtxRotate = g.Transform;
                    using (Brush brush = new SolidBrush(BandForeColor))
                    {
                        mtxRotate.RotateAt(315, new PointF(rectangle.X, rectangle.Y));
                        g.Transform = mtxRotate;
                        using (StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone())
                        {
                            format.Alignment = StringAlignment.Center;
                            format.LineAlignment = StringAlignment.Center;
                            format.FormatFlags = StringFormatFlags.LineLimit;
                            g.DrawString(text, this.Font, brush, rectangle, format);
                        }
                    }

                    g.Transform = mtxSave;
                }
            }


            if (Children?.Any() == true)
            {
                Children.ToList().ForEach(c => c.Paint(g));
            }
        }
    }
}
