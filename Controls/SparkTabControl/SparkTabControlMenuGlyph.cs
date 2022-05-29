using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	internal class SparkTabControlMenuGlyph
    {
        #region Fields

        private Rectangle glyphRect = Rectangle.Empty;
        private bool isMouseOver = false;
        private ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer();

        #endregion

        #region Props

        public bool IsMouseOver
        {
            get { return isMouseOver; }
            set { isMouseOver = value; }
        }

        public Rectangle Bounds
        {
            get { return glyphRect; }
            set { glyphRect = value; }
        }

        #endregion

        #region Ctor

        internal SparkTabControlMenuGlyph()
        {

        }

        #endregion

        #region Methods

        public void DrawGlyph(Graphics g)
        {
            if (isMouseOver)
            {
                Color fill = renderer.ColorTable.ButtonSelectedHighlight; //Color.FromArgb(35, SystemColors.Highlight);
                g.FillRectangle(new SolidBrush(fill), glyphRect);
                Rectangle borderRect = glyphRect;

                borderRect.Width--;
                borderRect.Height--;

                g.DrawRectangle(SystemPens.Highlight, borderRect);
            }

            SmoothingMode bak = g.SmoothingMode;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int gap = 2;
            using (Pen pen = new Pen(Color.Black))
            {
                pen.Width = 2;

                g.DrawLine(pen,
                    new Point(glyphRect.Left + gap, glyphRect.Y + gap),
                    new Point(glyphRect.Right - gap - 1, glyphRect.Y + gap));
            }

            g.FillPolygon(Brushes.Black, new PointF[]{
                new PointF(glyphRect.Left + gap, glyphRect.Y + gap + gap + gap),
                new PointF(glyphRect.Right - gap -1, glyphRect.Y + gap + gap + gap),
                new PointF(1.0F*glyphRect.Left + glyphRect.Width / 2.0F - 0.5F, glyphRect.Bottom - gap)});

            g.SmoothingMode = bak;
        }

        #endregion
    }
}