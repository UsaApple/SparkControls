using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 日期时间图片标签
	/// </summary>
	[ToolboxItem(false)]
    internal class DateTimeImgLabel : Label
    {
        private readonly bool _drawRightBorder = true;

        private static bool? isVistaOrLater;
        /// <summary>
        /// 判断是否是Vista or later
        /// </summary>
        public static bool IsVistaOrLater
        {
            get
            {
                if (!isVistaOrLater.HasValue)
                    isVistaOrLater = Environment.OSVersion.Version.Major >= 6;
                return isVistaOrLater.Value;
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public DateTimeImgLabel(bool drawRightBorder = true)
        {
            _drawRightBorder = drawRightBorder;

            SetStyle(ControlStyles.ResizeRedraw |          //调整大小时重绘
               ControlStyles.DoubleBuffer |                //双缓冲
               ControlStyles.OptimizedDoubleBuffer |       //双缓冲
               ControlStyles.AllPaintingInWmPaint |        //禁止擦除背景
               ControlStyles.SupportsTransparentBackColor |//透明
               ControlStyles.UserPaint, true
            );
        }

        /// <summary>
        /// 重新绘制
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsVistaOrLater)
            {
                if (ForeColor == Color.Transparent)
                {
                    ForeColor = Color.FromArgb(122, 122, 122);
                }
                ControlPaint.DrawBorder(e.Graphics, new Rectangle(Point.Empty, Size),
                    Color.Empty, 0, ButtonBorderStyle.None,
                    ForeColor, 1, ButtonBorderStyle.Solid,
                    ForeColor, _drawRightBorder ? 1 : 0, ButtonBorderStyle.Solid,
                    ForeColor, 1, ButtonBorderStyle.Solid
                );
            }
            base.OnPaint(e);
        }
    }
}