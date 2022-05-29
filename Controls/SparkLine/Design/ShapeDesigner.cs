using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    internal abstract class ShapeDesigner : ComponentDesigner
    {
        private bool m_OnDragging;

        public abstract bool CanDrag(HitFlag hitFlag);
        protected override void Dispose(bool disposing)
        {
            IComponentChangeService service = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
            if (service != null)
            {
                service.ComponentRename -= new ComponentRenameEventHandler(this.OnComponentRename);
            }
            base.Dispose(disposing);
        }

        public abstract void Drag(HitFlag hitFlag, int x, int y);
        public abstract SelectObject DrawAdornments(Graphics g, bool primary, Point screenOffset, Color backColor);
        public abstract void DrawFeedback(HitFlag hitFlag, Point offset, ref Graphics g, StatusBarCommand cmd, Control newContainer);
        public abstract HitFlag GetAdornmentsHitTest(Point p, bool testMove);
        public abstract Cursor GetCursor(HitFlag hitFlag);
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            IComponentChangeService service = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
            if (service != null)
            {
                service.ComponentRename += new ComponentRenameEventHandler(this.OnComponentRename);
            }
        }

        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            if (object.ReferenceEquals(RuntimeHelpers.GetObjectValue(e.Component), this.Component))
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this.Component)["Name"];
                if (descriptor != null)
                {
                    descriptor.SetValue(this.Component, e.NewName);
                }
            }
        }

        public abstract void SetLocation(Point p);

        public bool OnDragging
        {
            get
            {
                return this.m_OnDragging;
            }
            set
            {
                this.m_OnDragging = value;
            }
        }
    }
}