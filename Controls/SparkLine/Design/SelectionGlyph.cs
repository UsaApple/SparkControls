using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal sealed class SelectionGlyph : Glyph
    {
        private Control m_Control;
        private SelectionManager m_SelectionManager;

        public SelectionGlyph(Control control, SelectionManager selMgr) : base(new SelectionBehavior())
        {
            this.m_Control = control;
            this.m_SelectionManager = selMgr;
            ((SelectionBehavior)this.Behavior).SelectionManager = selMgr;
        }

        public override Cursor GetHitTest(Point p)
        {
            return this.SelectionManager.GetHitTest(p);
        }

        public override void Paint(PaintEventArgs pe)
        {
            this.SelectionManager.DrawAdornments();
        }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, this.m_Control.Width, this.m_Control.Height);
            }
        }

        private SelectionManager SelectionManager
        {
            get
            {
                return this.m_SelectionManager;
            }
        }
    }
}