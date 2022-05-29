using System;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 按钮
    /// </summary>
    public class SparkNavigationBarStripButton : ToolStripButton
    {
        public SparkNavigationBarStripButton()
        {
        }

        public SparkNavigationBarStripButton(string text)
            : base(text)
        {
        }

        public SparkNavigationBarStripButton(Image image)
            : base(image)
        {
        }

        public SparkNavigationBarStripButton(string text, Image image)
            : base(text, image)
        {
        }

        public SparkNavigationBarStripButton(string text, Image image, EventHandler onClick)
            : base(text, image, onClick)
        {
        }

        public SparkNavigationBarStripButton(string text, Image image, EventHandler onClick, string name)
            : base(text, image, onClick, name)
        {
        }

        public new SparkNavigateToolStrip Owner => base.Owner as SparkNavigateToolStrip;

        protected override Padding DefaultPadding => new Padding(4, 4, 4, 4);

        protected override Padding DefaultMargin => new Padding(0, 0, 0, 0);
    }
}