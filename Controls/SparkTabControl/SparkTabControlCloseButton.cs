using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    internal class SparkTabControlCloseButton
    {
        #region Fields

        private Rectangle crossRect = Rectangle.Empty;
        private bool isMouseOver = false;
        private ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer();
        private readonly SparkTabControl _parent = null;
        #endregion

        #region Props

        public bool IsMouseOver
        {
            get { return isMouseOver; }
            set { isMouseOver = value; }
        }

        public Rectangle Bounds
        {
            get { return crossRect; }
            set { crossRect = value; }
        }

        public SparkTabControlCloseButtonTheme CloseTheme => _parent.Theme.CloseTheme;

        #endregion

        #region Ctor

        internal SparkTabControlCloseButton(SparkTabControl trtTabControl)
        {
            this._parent = trtTabControl;
        }

        #endregion

        #region Methods

        public void DrawCross(Graphics g, RectangleF crossRect, bool isCurrent = false)
        {
            //画圆，还是画矩形
            bool isCircle = true;
            //是否选中关闭按钮
            bool isSelect = isMouseOver && isCurrent;
            Color backColor;
            Color borderColor;
            Color foreColor;
            if (isCircle)
            {
                //绘制圆
                backColor = isSelect ? CloseTheme.MouseOverBackColor : CloseTheme.BackColor;
                borderColor = isSelect ? CloseTheme.MouseOverBorderColor : CloseTheme.BorderColor;
                foreColor = isSelect ? CloseTheme.MouseOverForeColor : CloseTheme.ForeColor;
                GDIHelper.DrawCircle(g, backColor, borderColor, 1, crossRect);
                GDIHelper.DrawXByCircle(g, foreColor, 2.5F, 1.6F, crossRect);
            }
            else
            {
                //绘制矩形
                backColor = isSelect ? CloseTheme.MouseOverBackColor : CloseTheme.BackColor;
                borderColor = isSelect ? CloseTheme.MouseOverBorderColor : CloseTheme.BorderColor;
                foreColor = isSelect ? CloseTheme.MouseOverForeColor : CloseTheme.ForeColor;
                GDIHelper.DrawRectangle(g, crossRect, backColor, 1);
                GDIHelper.FillRectangle(g, crossRect, borderColor);
                GDIHelper.DrawXByRectangle(g, foreColor, 2.5F, 1.6F, crossRect);
            }
        }

        #endregion
    }
}