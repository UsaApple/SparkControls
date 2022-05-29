using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

using SparkControls.Win32;

namespace SparkControls.Controls
{
	[TypeConverter(typeof(ShapeContainerConverter)), Browsable(false),
        DesignTimeVisible(false), ToolboxItem(false), Designer(typeof(ShapeContainerDesigner))]
    public sealed class ShapeContainer : UserControl
    {
        private const int HTTRANSPARENT = -1;
        private AccessibleRole m_AccessibleRole = AccessibleRole.Client;
        private Cursor m_Cursor;
        private bool m_Disposed;
        private Shape m_FocusedShape;
        private Graphics m_Graphics;
        private Shape m_HitShape;
        private bool m_IsControlAddHandled;
        private bool m_IsControlRemoveHandled;
        private bool m_IsPaintHandled;
        private Shape m_LastMouseActiveShape;
        private Control m_LastParent;
        private MdiClient m_MdiClient;
        private ShapeContainerMouseHoverTimer m_MouseHoverTimer;
        private Shape m_SelectedShape;
        private static int m_ShapeContainerInstanceCount;
        private ShapeCollection m_Shapes;
        private const int WM_NCHITTEST = 0x84;
        private const int WS_EX_TRANSPARENT = 0x20;

        public ShapeContainer()
        {
            this.Dock = DockStyle.Fill;
            Padding padding = new Padding(0);
            this.Margin = padding;
            this.TabStop = false;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.m_Shapes = new ShapeCollection(this);
            if (m_ShapeContainerInstanceCount == 0)
            {
                Utility.InitPen();
            }
            m_ShapeContainerInstanceCount++;
        }

        private void Clear()
        {
            if ((this.m_LastParent != null) && !this.m_LastParent.Equals(this.Parent))
            {
                if (this.m_IsControlAddHandled)
                {
                    this.m_LastParent.ControlAdded -= new ControlEventHandler(this.OnParentControlAdded);
                    this.m_LastParent.BackgroundImageLayoutChanged -= new EventHandler(this.OnParentImageLayoutChanged);
                    this.m_IsControlAddHandled = false;
                }
                if (this.m_IsControlRemoveHandled)
                {
                    this.m_LastParent.ControlRemoved -= new ControlEventHandler(this.OnParentControlRemoved);
                    this.m_IsControlRemoveHandled = false;
                }
                if (this.m_IsPaintHandled)
                {
                    this.m_LastParent.Paint -= new PaintEventHandler(this.OnParentPaint);
                    this.m_IsPaintHandled = false;
                }
                if (this.m_MdiClient != null)
                {
                    this.m_MdiClient.Paint -= new PaintEventHandler(this.OnMdiClientPaint);
                    this.m_MdiClient = null;
                }
                this.m_LastParent = null;
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ShapeContainerAccessibleObject(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.m_Disposed && disposing)
            {
                if (this.m_Graphics != null)
                {
                    this.m_Graphics.Dispose();
                }
                m_ShapeContainerInstanceCount--;
                if (m_ShapeContainerInstanceCount == 0)
                {
                    Utility.ClearPen();
                }
                if (this.m_MouseHoverTimer != null)
                {
                    this.m_MouseHoverTimer.Dispose();
                }
                this.Clear();
            }
            this.m_Disposed = true;
            base.Dispose(disposing);
        }

        public Shape GetChildAtPoint(Point pt)
        {
            pt = this.PointToScreen(pt);
            return this.GetChildAtPointInternal(pt);
        }

        internal Shape GetChildAtPointInternal(Point pt)
        {
            return this.GetHitTest(pt.X, pt.Y);
        }

        private Shape GetHitTest(int x, int y)
        {
            Point location = new Point(0, 0);
            if ((!this.DesignMode && (this.Parent != null)) && typeof(Form).IsAssignableFrom(this.Parent.GetType()))
            {
                Form parent = (Form)this.Parent;
                foreach (Form form2 in parent.MdiChildren)
                {
                    if ((form2.Parent != null) && form2.Parent.RectangleToScreen(form2.Bounds).Contains(x, y))
                    {
                        return null;
                    }
                }
                if (parent.IsMdiContainer && (parent.Controls.Count > 0))
                {
                    for (int j = parent.Controls.Count; j >= 1; j += -1)
                    {
                        Control control = parent.Controls[j - 1];
                        if (typeof(MdiClient).IsAssignableFrom(control.GetType()))
                        {
                            location = control.Location;
                            break;
                        }
                    }
                }
            }
            int num4 = this.Shapes.Count - 1;
            for (int i = 0; i <= num4; i++)
            {
                Shape shape2 = (Shape)this.Shapes[i];
                x -= location.X;
                y -= location.Y;
                if (shape2.HitTest(x, y))
                {
                    return shape2;
                }
            }
            return null;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Shape GetNextShape(Shape shape, bool forward)
        {
            return this.GetNextShapeInternal(shape, forward, false);
        }

        private Shape GetNextShapeInternal(Shape shape, bool forward, bool wrap)
        {
            if (shape == null)
            {
                return null;
            }
            int index = this.Shapes.IndexOf(shape);
            if (index < 0)
            {
                return null;
            }
            int num2 = 0;
            if (forward)
            {
                num2 = index + 1;
            }
            else
            {
                num2 = index - 1;
            }
            if (wrap)
            {
                if (num2 < 0)
                {
                    num2 += this.Shapes.Count;
                }
                else if (num2 >= this.Shapes.Count)
                {
                    num2 -= this.Shapes.Count;
                }
            }
            else if ((num2 < 0) || (num2 >= this.Shapes.Count))
            {
                return null;
            }
            return (Shape)this.Shapes[num2];
        }

        protected override void OnClick(EventArgs e)
        {
            if ((!this.DesignMode && (e != null)) && typeof(MouseEventArgs).IsAssignableFrom(e.GetType()))
            {
                MouseEventArgs args = (MouseEventArgs)e;
                if (args != null)
                {
                    Point p = this.PointToScreen(args.Location);
                    if (this.m_HitShape != null)
                    {
                        p = this.m_HitShape.PointToClient(p);
                        this.m_HitShape.OnClick(new MouseEventArgs(args.Button, args.Clicks, p.X, p.Y, args.Delta));
                    }
                }
            }
            base.OnClick(e);
        }

        private void OnContextMenuStripClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ContextMenuStrip strip = (ContextMenuStrip)sender;
            strip.Closing -= new ToolStripDropDownClosingEventHandler(this.OnContextMenuStripClosing);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if ((!this.DesignMode && (e != null)) && typeof(MouseEventArgs).IsAssignableFrom(e.GetType()))
            {
                MouseEventArgs args = (MouseEventArgs)e;
                if (args != null)
                {
                    Point p = this.PointToScreen(args.Location);
                    if (this.m_HitShape != null)
                    {
                        p = this.m_HitShape.PointToClient(p);
                        this.m_HitShape.OnDoubleClick(new MouseEventArgs(args.Button, args.Clicks, p.X, p.Y, args.Delta));
                    }
                }
            }
            base.OnDoubleClick(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            if (!this.DesignMode && (this.SelectedShape != null))
            {
                this.SelectedShape.OnEnter(e);
            }
            base.OnEnter(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (this.SelectedShape != null)
            {
                this.SelectedShape.OnGotFocus(e);
                this.FocusedShape = this.SelectedShape;
            }
            base.OnGotFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!this.DesignMode && (this.FocusedShape != null))
            {
                this.FocusedShape.OnKeyDown(e);
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!this.DesignMode && (this.FocusedShape != null))
            {
                this.FocusedShape.OnKeyPress(e);
            }
            base.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!this.DesignMode && (this.FocusedShape != null))
            {
                this.FocusedShape.OnKeyUp(e);
            }
            base.OnKeyUp(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            if (!this.DesignMode && (this.SelectedShape != null))
            {
                this.SelectedShape.OnLeave(e);
                this.SelectedShape = null;
            }
            base.OnLeave(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!this.DesignMode && (this.FocusedShape != null))
            {
                this.FocusedShape.OnLostFocus(e);
                this.FocusedShape = null;
            }
            base.OnLostFocus(e);
        }

        private void OnMdiClientPaint(object sender, PaintEventArgs e)
        {
            this.OnPaintInternal(e);
            if ((this.Parent != null) && typeof(Form).IsAssignableFrom(this.Parent.GetType()))
            {
                Form parent = (Form)this.Parent;
                foreach (Form form2 in parent.MdiChildren)
                {
                    form2.Invalidate();
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!this.DesignMode && (e != null))
            {
                Point p = this.PointToScreen(e.Location);
                if (this.m_HitShape != null)
                {
                    p = this.m_HitShape.PointToClient(p);
                    this.m_HitShape.OnMouseClick(new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta));
                }
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (!this.DesignMode && (e != null))
            {
                Point p = this.PointToScreen(e.Location);
                if (this.m_HitShape != null)
                {
                    p = this.m_HitShape.PointToClient(p);
                    this.m_HitShape.OnMouseDoubleClick(new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta));
                }
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.DesignMode && (e != null))
            {
                Point p = this.PointToScreen(e.Location);
                if (this.m_HitShape != null)
                {
                    p = this.m_HitShape.PointToClient(p);
                    this.m_HitShape.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta));
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this.DesignMode && (e != null))
            {
                Point pt = this.PointToScreen(e.Location);
                Shape childAtPointInternal = this.GetChildAtPointInternal(pt);
                if (this.m_LastMouseActiveShape == null)
                {
                    if (childAtPointInternal != null)
                    {
                        childAtPointInternal.OnMouseEnter(EventArgs.Empty);
                        this.m_LastMouseActiveShape = childAtPointInternal;
                    }
                }
                else if (childAtPointInternal == null)
                {
                    this.m_LastMouseActiveShape.OnMouseLeave(EventArgs.Empty);
                    this.m_LastMouseActiveShape = childAtPointInternal;
                }
                else if (childAtPointInternal.Equals(this.m_LastMouseActiveShape))
                {
                    pt = childAtPointInternal.PointToClient(pt);
                    childAtPointInternal.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta));
                }
                else
                {
                    this.m_LastMouseActiveShape.OnMouseLeave(EventArgs.Empty);
                    childAtPointInternal.OnMouseEnter(EventArgs.Empty);
                    this.m_LastMouseActiveShape = childAtPointInternal;
                }
            }
            if (!this.DesignMode)
            {
                this.MouseHoverTimer.Start(this.m_LastMouseActiveShape);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                if (!this.DesignMode && (e != null))
                {
                    Point p = this.PointToScreen(e.Location);
                    if (this.m_HitShape != null)
                    {
                        p = this.m_HitShape.PointToClient(p);
                        this.m_HitShape.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta));
                    }
                }
                base.OnMouseUp(e);
            }
            catch (ObjectDisposedException exception1)
            {
                //ProjectData.SetProjectError(exception1);
                ObjectDisposedException exception = exception1;
                //ProjectData.ClearProjectError();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if ((!this.DesignMode && (e != null)) && (this.Focused && (this.m_FocusedShape != null)))
            {
                Point p = this.PointToScreen(e.Location);
                p = this.m_FocusedShape.PointToClient(p);
                this.m_FocusedShape.OnMouseWheel(new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta));
            }
            base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.DesignMode || (this.m_MdiClient == null))
            {
                this.OnPaintInternal(e);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        private void OnPaintInternal(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!this.DesignMode)
            {
                this.PaintAll(e.Graphics);
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            this.Clear();
            if (this.Parent != null)
            {
                this.Parent.ControlAdded += new ControlEventHandler(this.OnParentControlAdded);
                this.Parent.Invalidated += Parent_Invalidated;
                this.Parent.BackgroundImageLayoutChanged += new EventHandler(this.OnParentImageLayoutChanged);
                this.m_IsControlAddHandled = true;
                if (typeof(SplitterPanel).IsAssignableFrom(this.Parent.GetType()))
                {
                    this.Parent.Paint += new PaintEventHandler(this.OnParentPaint);
                    this.m_IsPaintHandled = true;
                }
                //levy 注释掉DataRepeaterItem部分
                //if (typeof(DataRepeaterItem).IsAssignableFrom(this.Parent.GetType()))
                //{
                //    this.Parent.Paint += new PaintEventHandler(this.OnParentPaint);
                //    this.m_IsPaintHandled = true;
                //}
                this.m_LastParent = this.Parent;
                if (typeof(Form).IsAssignableFrom(this.Parent.GetType()) && (this.Parent.Controls.Count > 0))
                {
                    this.Parent.ControlRemoved += new ControlEventHandler(this.OnParentControlRemoved);
                    this.m_IsControlRemoveHandled = true;
                    Control control = this.Parent.Controls[this.Parent.Controls.Count - 1];
                    if (typeof(MdiClient).IsAssignableFrom(control.GetType()))
                    {
                        this.m_MdiClient = (MdiClient)control;
                        this.m_MdiClient.Paint += new PaintEventHandler(this.OnMdiClientPaint);
                    }
                }
            }
            base.OnParentChanged(e);
        }

        private void Parent_Invalidated(object sender, InvalidateEventArgs e)
        {
            this.Invalidate();
        }

        private void OnParentControlAdded(object sender, ControlEventArgs e)
        {
            if ((e.Control != null) && typeof(MdiClient).IsAssignableFrom(e.Control.GetType()))
            {
                this.m_MdiClient = (MdiClient)e.Control;
                e.Control.Paint += new PaintEventHandler(this.OnMdiClientPaint);
                if (this.DesignMode)
                {
                    this.Invalidate();
                }
            }
            if (!this.DesignMode)
            {
                this.SendToBackInternal();
            }
        }

        private void OnParentControlRemoved(object sender, ControlEventArgs e)
        {
            if ((e.Control != null) && typeof(MdiClient).IsAssignableFrom(e.Control.GetType()))
            {
                if (this.m_MdiClient != null)
                {
                    this.m_MdiClient.Paint -= new PaintEventHandler(this.OnMdiClientPaint);
                    this.m_MdiClient = null;
                }
                if (this.DesignMode)
                {
                    this.Invalidate();
                }
            }
        }

        private void OnParentImageLayoutChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void OnParentPaint(object sender, PaintEventArgs e)
        {
            this.OnPaint(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (!this.DesignMode && (this.FocusedShape != null))
            {
                this.FocusedShape.OnPreviewKeyDown(e);
            }
            base.OnPreviewKeyDown(e);
        }

        private void PaintAll(Graphics g)
        {
            if (this.Visible)
            {
                for (int i = this.m_Shapes.Count; i >= 1; i += -1)
                {
                    Shape shape = (Shape)this.m_Shapes[i - 1];
                    if (shape.Visible)
                    {
                        //levy 下面的矩形设置为空,不知道有没有问题??
                        Rectangle rectangle = Rectangle.Empty;
                        shape.OnPaint(new PaintEventArgs(g, rectangle));
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool SelectNextShape(Shape shape, bool forward, bool wrap)
        {
            Shape shape2 = this.GetNextShapeInternal(shape, forward, wrap);
            return ((shape2 != null) && shape2.Focus());
        }

        internal void SendToBackInternal()
        {
            if ((this.Parent != null) && typeof(Form).IsAssignableFrom(this.Parent.GetType()))
            {
                int count = this.Parent.Controls.Count;
                int index = this.Parent.Controls.IndexOf(this);
                if ((count > 1) && (index >= 0))
                {
                    Control control = this.Parent.Controls[count - 1];
                    if (typeof(MdiClient).IsAssignableFrom(control.GetType()))
                    {
                        if (index != (count - 2))
                        {
                            this.Parent.Controls.SetChildIndex(this, count - 2);
                        }
                    }
                    else if (index != (count - 1))
                    {
                        this.SendToBack();
                    }
                }
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Rectangle bounds = this.Bounds;
            base.SetBoundsCore(x, y, width, height, specified);
            bool flag = false;
            if ((this.Parent != null) && (this.m_Shapes != null))
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = this.Shapes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Shape current = (Shape)enumerator.Current;
                        if (current.Anchor != (AnchorStyles.Left | AnchorStyles.Top))
                        {
                            flag = true;
                            goto Label_0070;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
            }
        Label_0070:
            if (flag && !bounds.IsEmpty)
            {
                Rectangle rectangle2 = this.Bounds;
                if ((rectangle2.Height > 0) && (rectangle2.Width > 0))
                {
                    int deltaWidth = rectangle2.Width - bounds.Width;
                    int deltaHeight = rectangle2.Height - bounds.Height;
                    if ((deltaWidth != 0) || (deltaHeight != 0))
                    {
                        this.SuspendLayout();
                        try
                        {
                            IEnumerator enumerator2 = null;
                            try
                            {
                                enumerator2 = this.Shapes.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    Shape shp = (Shape)enumerator2.Current;
                                    if (shp.Anchor != (AnchorStyles.Left | AnchorStyles.Top))
                                    {
                                        this.SetBoundsForShape(shp, deltaWidth, deltaHeight);
                                    }
                                }
                            }
                            finally
                            {
                                if (enumerator2 is IDisposable)
                                {
                                    (enumerator2 as IDisposable).Dispose();
                                }
                            }
                        }
                        finally
                        {
                            this.ResumeLayout(true);
                        }
                    }
                }
            }
        }

        private void SetBoundsForShape(Shape shp, int deltaWidth, int deltaHeight)
        {
            Rectangle virtualBounds = shp.VirtualBounds;
            if (deltaWidth != 0)
            {
                if (((shp.Anchor & AnchorStyles.Left) == AnchorStyles.Left) && ((shp.Anchor & AnchorStyles.Right) == AnchorStyles.Right))
                {
                    virtualBounds.Width += deltaWidth;
                }
                else if ((shp.Anchor & AnchorStyles.Left) != AnchorStyles.Left)
                {
                    if ((shp.Anchor & AnchorStyles.Right) == AnchorStyles.Right)
                    {
                        virtualBounds.X += deltaWidth;
                    }
                    else
                    {
                        virtualBounds.X += (int)Math.Round((double)(((double)deltaWidth) / 2.0));
                    }
                }
            }
            if (deltaHeight != 0)
            {
                if (((shp.Anchor & AnchorStyles.Top) == AnchorStyles.Top) && ((shp.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom))
                {
                    virtualBounds.Height += deltaHeight;
                }
                else if ((shp.Anchor & AnchorStyles.Top) != AnchorStyles.Top)
                {
                    if ((shp.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
                    {
                        virtualBounds.Y += deltaHeight;
                    }
                    else
                    {
                        virtualBounds.Y += (int)Math.Round((double)(((double)deltaHeight) / 2.0));
                    }
                }
            }
            shp.VirtualBounds = virtualBounds;
        }

        internal void ShowContextMenu(Shape sh, Point pos)
        {
            ContextMenu contextMenu = sh.ContextMenu;
            ContextMenuStrip contextMenuStrip = sh.ContextMenuStrip;
            if (contextMenu != null)
            {
                contextMenu.Show(this, pos);
            }
            else if (contextMenuStrip != null)
            {
                contextMenuStrip.Closing += new ToolStripDropDownClosingEventHandler(this.OnContextMenuStripClosing);
                contextMenuStrip.Show(this, pos);
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (!this.DesignMode && (m.Msg == 0x84))
            {
                IntPtr lParam = m.LParam;
                Point point = new Point(lParam.ToInt32());
                this.m_HitShape = this.GetHitTest(point.X, point.Y);
                if (this.m_HitShape == null)
                {
                    if (this.m_LastMouseActiveShape != null)
                    {
                        this.m_LastMouseActiveShape.OnMouseLeave(EventArgs.Empty);
                        this.m_LastMouseActiveShape = null;
                        this.m_MouseHoverTimer.Cancel();
                    }
                    lParam = new IntPtr(-1);
                    m.Result = lParam;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        [DefaultValue(typeof(AccessibleRole), "Client")]
        public AccessibleRole AccessibleRole
        {
            get
            {
                return this.m_AccessibleRole;
            }
            set
            {
                if (!Enum.IsDefined(typeof(AccessibleRole), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(AccessibleRole));
                }
                this.m_AccessibleRole = value;
            }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= (int)WindowExStyles.WS_EX_TRANSPARENT;
                return createParams;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Cursor Cursor
        {
            get
            {
                if (!this.UseWaitCursor && (this.m_Cursor != null))
                {
                    return this.m_Cursor;
                }
                return base.Cursor;
            }
            set
            {
                this.m_Cursor = value;
            }
        }

        [DefaultValue(typeof(DockStyle), "Fill"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                if (!Enum.IsDefined(typeof(DockStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(DockStyle));
                }
                base.Dock = DockStyle.Fill;
            }
        }

        internal Shape FocusedShape
        {
            get
            {
                return this.m_FocusedShape;
            }
            set
            {
                this.m_FocusedShape = value;
            }
        }

        private ShapeContainerMouseHoverTimer MouseHoverTimer
        {
            get
            {
                if (this.m_MouseHoverTimer == null)
                {
                    this.m_MouseHoverTimer = new ShapeContainerMouseHoverTimer();
                }
                return this.m_MouseHoverTimer;
            }
        }

        internal Shape SelectedShape
        {
            get
            {
                return this.m_SelectedShape;
            }
            set
            {
                this.m_SelectedShape = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            Description("Gets the collection of shapes that are contained in the ShapeContainer."), Browsable(false)]
        public ShapeCollection Shapes
        {
            get
            {
                return this.m_Shapes;
            }
        }

        internal sealed class ShapeContainerConverter : TypeConverter
        {
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, System.Attribute[] attributes)
            {
                return null;
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }
        }

        private sealed class ShapeContainerMouseHoverTimer : IDisposable
        {
            private Shape m_CurrentShape;
            private bool m_Disposed;
            private Timer m_MouseHoverTimer;
            private const int SPI_GETMOUSEHOVERTIME_WIN9X = 400;

            public ShapeContainerMouseHoverTimer()
            {
                int mouseHoverTime = SystemInformation.MouseHoverTime;
                if (mouseHoverTime == 0)
                {
                    mouseHoverTime = 400;
                }
                this.m_MouseHoverTimer = new Timer();
                this.m_MouseHoverTimer.Interval = mouseHoverTime;
                this.m_MouseHoverTimer.Tick += new EventHandler(this.OnTick);
            }

            public void Cancel()
            {
                this.m_MouseHoverTimer.Enabled = false;
                this.m_CurrentShape = null;
            }

            public void Cancel(Shape shape)
            {
                if (object.ReferenceEquals(shape, this.m_CurrentShape))
                {
                    this.Cancel();
                }
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if ((!this.m_Disposed && disposing) && (this.m_MouseHoverTimer != null))
                {
                    this.Cancel();
                    this.m_MouseHoverTimer.Dispose();
                    this.m_MouseHoverTimer = null;
                }
                this.m_Disposed = true;
            }

            private void OnTick(object sender, EventArgs e)
            {
                this.m_MouseHoverTimer.Enabled = false;
                if ((this.m_CurrentShape != null) && !this.m_CurrentShape.IsDisposed)
                {
                    this.m_CurrentShape.OnMouseHover(EventArgs.Empty);
                }
            }

            public void Start(Shape shape)
            {
                if (!object.ReferenceEquals(shape, this.m_CurrentShape))
                {
                    this.Cancel(this.m_CurrentShape);
                }
                this.m_CurrentShape = shape;
                if (this.m_CurrentShape != null)
                {
                    this.m_MouseHoverTimer.Enabled = true;
                }
            }
        }
    }
}