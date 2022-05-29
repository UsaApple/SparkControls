using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal sealed class SelectionBehavior : Behavior
    {
        private Rectangle m_MouseDragRect = Rectangle.Empty;
        private SelectionManager m_SelectionManager;

        public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
        {
            if (this.SelectionManager.IsHit())
            {
                Size dragSize = SystemInformation.DragSize;
                Point location = new Point(mouseLoc.X - ((int)Math.Round((double)(((double)dragSize.Width) / 2.0))), mouseLoc.Y - ((int)Math.Round((double)(((double)dragSize.Height) / 2.0))));
                this.m_MouseDragRect = new Rectangle(location, dragSize);
            }
            else
            {
                this.m_MouseDragRect = Rectangle.Empty;
            }
            return base.OnMouseDown(g, button, mouseLoc);
        }

        public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
        {
            if ((button == MouseButtons.Left) && this.SelectionManager.IsHit())
            {
                this.SelectionManager.StartResize(mouseLoc);
                return true;
            }
            return base.OnMouseMove(g, button, mouseLoc);
        }

        internal SelectionManager SelectionManager
        {
            get
            {
                return this.m_SelectionManager;
            }
            set
            {
                this.m_SelectionManager = value;
            }
        }
    }
}