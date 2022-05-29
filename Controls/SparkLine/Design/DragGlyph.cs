using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
namespace SparkControls.Controls
{
    internal sealed class DragGlyph : Glyph
    {
        private Adorner m_Adorner;
        private BehaviorService m_BehaviorService;
        private Control m_Control;

        public DragGlyph(BehaviorService behaviorSvc, Control control) : base(new DragBehavior())
        {
            this.m_BehaviorService = behaviorSvc;
            this.m_Control = control;
            this.DragBehavior.Glyph = this;
            this.DragBehavior.BehaviorService = this.BehaviorService;
            this.DragBehavior.DesignerHost = (IDesignerHost)control.Site.GetService(typeof(IDesignerHost));
        }

        public Point FromScreenToAdorner(Point p)
        {
            Point point2 = this.BehaviorService.AdornerWindowToScreen();
            p.Offset(0 - point2.X, 0 - point2.Y);
            return p;
        }

        public override Cursor GetHitTest(Point p)
        {
            return Cursors.SizeAll;
        }

        public override void Paint(PaintEventArgs pe)
        {
        }

        public Adorner Adorenr
        {
            get
            {
                return this.m_Adorner;
            }
            set
            {
                this.m_Adorner = value;
            }
        }

        public BehaviorService BehaviorService
        {
            get
            {
                return this.m_BehaviorService;
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, this.m_Control.Width, this.m_Control.Height);
            }
        }

        public DragBehavior DragBehavior
        {
            get
            {
                return (DragBehavior)this.Behavior;
            }
        }

        public Graphics Graphics
        {
            get
            {
                return this.BehaviorService.AdornerWindowGraphics;
            }
        }
    }
}