using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SparkControls.Controls
{
    /// <summary>
    /// 直线控件。
    /// </summary>
    [Serializable]
    public class SparkCardLine : SparkCardControl
    {
        /// <summary>
        /// 获取或设置线段的颜色。
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>
        /// 获取或设置线段的宽度。
        /// </summary>
        public int LineWidth { get; set; } = 1;

        /// <summary>
        /// 获取或设置线段的长度。
        /// </summary>
        public int LineLong { get; set; } = 64;

        /// <summary>
        /// 获取或设置线段的宽度。
        /// </summary>
        public DashStyle LineStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置线段的起点。
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// 获取或设置线段的终点。
        /// </summary>
        public Point EndPoint { get; set; }

        /// <summary>
        /// 绘制控件。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="data">行数据源。</param>
        public override void Paint(Graphics g, DataRowView data)
        {
            using (Pen pen = new Pen(ForeColor, LineWidth) { DashStyle = LineStyle })
            {
                g.DrawLine(pen, StartPoint, EndPoint);
            }
        }

        /// <summary>
        /// 将 <see cref="SparkCardLine"/> 的 xml 表示形式转换为与它等效的 <see cref="SparkCardLine"/> 实例。
        /// </summary>
        /// <param name="element">待转换的 xml 元素。</param>
        /// <returns>转换生成的 <see cref="SparkCardLine"/> 实例。</returns>
        internal static SparkCardLine ParseXml(XElement element)
        {
            SparkCardLine ctrl = new SparkCardLine
            {
                Type = element.GetAttributeValue("type"),
            };
            ctrl.FromXml(element);

            if (GetPropertyValue<Color, ColorConverter>(element, "BorderColor", out Color borderColor)) ctrl.ForeColor = borderColor;
            if (GetPropertyValue<DashStyle, EnumConverter<DashStyle>>(element, "BorderStyle", out DashStyle borderStyle)) ctrl.LineStyle = borderStyle;
            if (GetPropertyValue<int, Int32Converter>(element, "BorderWidth", out int borderWidth)) ctrl.LineWidth = borderWidth;
            if (GetPropertyValue<Point, PointConverter>(element, "StartPoint", out Point startPoint)) ctrl.StartPoint = startPoint;
            if (GetPropertyValue<Point, PointConverter>(element, "EndPoint", out Point endPoint)) ctrl.EndPoint = endPoint;

            return ctrl;
        }

        public override IDrawStyle CreateStyle(DataRowView dataRowView, IDrawStyle parent)
        {
            var style = new SparkCardLineStyle();
            style.Init(this, dataRowView, parent);
            return style;
        }
    }


    public class SparkCardLineStyle : DrawStyle
    {
        /// <summary>
        /// 获取或设置线段的颜色。
        /// </summary>
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>
        /// 获取或设置线段的宽度。
        /// </summary>
        public int LineWidth { get; set; } = 1;

        /// <summary>
        /// 获取或设置线段的宽度。
        /// </summary>
        public DashStyle LineStyle { get; set; } = DashStyle.Solid;

        /// <summary>
        /// 获取或设置线段的起点。
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// 获取或设置线段的终点。
        /// </summary>
        public Point EndPoint { get; set; }

        public override void Init(SparkCardControl trtCardControl, DataRowView dataRowView, IDrawStyle parent)
        {
            this.Parent = parent;
            var trtCardLine = trtCardControl as SparkCardLine;
            this.ForeColor = trtCardLine.ForeColor;
            this.LineWidth = trtCardLine.LineWidth;
            this.LineStyle = trtCardLine.LineStyle;
            this.StartPoint = trtCardLine.StartPoint;
            this.EndPoint = trtCardLine.EndPoint;

        }

        public override void Paint(Graphics g)
        {
            using (Pen pen = new Pen(ForeColor, LineWidth) { DashStyle = LineStyle })
            {
                g.DrawLine(pen, StartPoint, EndPoint);
            }
        }
    }
}
