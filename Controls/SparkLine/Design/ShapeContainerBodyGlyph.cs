using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal class ShapeContainerBodyGlyph : ControlBodyGlyph
    {
        private Point m_Offset;
        private ShapeContainer m_ShapeContainer;

        public ShapeContainerBodyGlyph(Rectangle bounds, Cursor cursor, IComponent relatedComponent, ControlDesigner designer, Point offset) : base(bounds, cursor, relatedComponent, designer)
        {
            this.m_Offset = offset;
            this.m_ShapeContainer = (ShapeContainer)relatedComponent;
        }

        public override Cursor GetHitTest(Point p)
        {
            if (this.Bounds.Contains(p))
            {
                Point pt = p;
                pt.Offset(this.m_Offset);
                if (this.m_ShapeContainer.GetChildAtPoint(pt) != null)
                {
                    return Cursors.SizeAll;
                }
            }
            return base.GetHitTest(p);
        }
    }
}