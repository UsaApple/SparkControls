using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal sealed class ShapeContainerDesigner : ParentControlDesigner
    {
        private IComponentChangeService m_ChangeService;
        private bool m_CreationMode;
        private bool m_Disposed;
        private Shape m_DragShape;
        private ShapeContainerDesignerHelper m_Helper;
        private static Dictionary<IComponent, ShapeContainerDesignerHelper> m_HelperCollection;
        private Shape m_HitShape;
        private IDesignerHost m_Host;
        private bool m_InCreateToolCore;
        private bool m_InSelectionChange;
        private Point m_MouseDragEndPoint = Point.Empty;
        private bool m_MouseDragMoved;
        private Rectangle m_MouseDragRect = Rectangle.Empty;
        private Region m_MouseDragRegion;
        private Point m_MouseDragStartPoint = Point.Empty;
        private static PasteHandler m_PasteHandler;
        private ISelectionService m_SelectionSvc;
        private IToolboxService m_ToolboxSvc;

        public void CancelMouseDrag()
        {
            this.m_Helper.ShapeContainerCommandSet.DragShapeConDesigner = null;
            this.m_MouseDragStartPoint = Point.Empty;
            this.Control.Capture = false;
            this.m_MouseDragMoved = false;
            this.m_MouseDragStartPoint = Point.Empty;
            this.m_MouseDragEndPoint = Point.Empty;
            this.m_MouseDragRect = Rectangle.Empty;
            this.ClearDragRegion();
        }

        private void ClearDragRegion()
        {
            if (this.m_MouseDragRegion != null)
            {
                this.BehaviorService.Invalidate(this.m_MouseDragRegion);
                this.m_MouseDragRegion.Dispose();
                this.m_MouseDragRegion = null;
            }
        }

        protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
        {
            if (tool == null)
            {
                return null;
            }
            IDesignerHost designerHost = this.DesignerHost;
            if (designerHost == null)
            {
                return null;
            }
            IComponent[] components = null;
            System.Type c = tool.GetType(designerHost);
            if (!typeof(Shape).IsAssignableFrom(c))
            {
                this.m_InCreateToolCore = true;
                try
                {
                    components = base.CreateToolCore(tool, x, y, width, height, hasLocation, hasSize);
                }
                finally
                {
                    this.m_InCreateToolCore = false;
                }
                return components;
            }
            components = base.CreateToolCore(tool, x, y, width, height, hasLocation, hasSize);
            if (((components != null) && (components.Length > 0)) && typeof(Shape).IsAssignableFrom(((Component)components[0]).GetType()))
            {
                IDesigner designer = designerHost.GetDesigner(components[0]);
                if (designer == null)
                {
                    return components;
                }
                if (!typeof(ShapeDesigner).IsAssignableFrom(designer.GetType()))
                {
                    return components;
                }
                ISelectionService selectionService = this.SelectionService;
                if (selectionService != null)
                {
                    selectionService.SetSelectedComponents(components, SelectionTypes.Replace);
                }
            }
            return components;
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.m_Disposed)
            {
                if (disposing)
                {
                    IComponentChangeService changeService = this.ChangeService;
                    this.ShapeContainer.Paint -= new PaintEventHandler(this.OnPaintShapeContainer);
                    ISelectionService selectionService = this.SelectionService;
                    if (selectionService != null)
                    {
                        selectionService.SelectionChanged -= new EventHandler(this.OnSelectionChanged);
                    }
                    if (this.m_Helper.ShapeContainerCounter > 1)
                    {
                        ShapeContainerDesignerHelper helper = this.m_Helper;
                        helper.ShapeContainerCounter--;
                        this.m_Helper = null;
                    }
                    else if (m_HelperCollection != null)
                    {
                        IDesignerHost designerHost = this.DesignerHost;
                        if (((designerHost != null) && (designerHost.RootComponent != null)) && m_HelperCollection.Remove(this.DesignerHost.RootComponent))
                        {
                            if (this.m_Helper != null)
                            {
                                this.m_Helper.Dispose();
                            }
                            this.m_Helper = null;
                        }
                        if (m_HelperCollection.Count == 0)
                        {
                            DesignerUtility.ClearBrushes();
                            m_HelperCollection = null;
                        }
                    }
                }
                if (this.m_MouseDragRegion != null)
                {
                    this.m_MouseDragRegion.Dispose();
                    this.m_MouseDragRegion = null;
                }
                base.Dispose(disposing);
            }
            this.m_Disposed = true;
        }

        public override void DoDefaultAction()
        {
            IEventBindingService service = (IEventBindingService)this.GetService(typeof(IEventBindingService));
            if (service != null)
            {
                ISelectionService selectionService = this.SelectionService;
                if (selectionService != null)
                {
                    ICollection selectedComponents = selectionService.GetSelectedComponents();
                    EventDescriptor e = null;
                    string str = null;
                    IComponent component = null;
                    IDesignerHost designerHost = this.DesignerHost;
                    DesignerTransaction transaction = null;
                    try
                    {
                        IEnumerator enumerator = null;
                        try
                        {
                            enumerator = selectedComponents.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                if (!typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                                {
                                    continue;
                                }
                                EventDescriptor defaultEvent = TypeDescriptor.GetDefaultEvent(RuntimeHelpers.GetObjectValue(objectValue));
                                PropertyDescriptor eventProperty = null;
                                string str2 = null;
                                bool flag = false;
                                if (defaultEvent != null)
                                {
                                    eventProperty = service.GetEventProperty(defaultEvent);
                                }
                                if ((eventProperty == null) | eventProperty.IsReadOnly)
                                {
                                    return;
                                }
                                try
                                {
                                    if ((designerHost != null) && (transaction == null))
                                    {
                                        transaction = designerHost.CreateTransaction("CommandSetAddEvent");
                                    }
                                }
                                catch (CheckoutException exception1)
                                {
                                    //ProjectData.SetProjectError(exception1);
                                    if (exception1 != CheckoutException.Canceled)
                                    {
                                        throw;
                                    }
                                    //ProjectData.ClearProjectError();
                                    return;
                                }
                                str2 = Convert.ToString(eventProperty.GetValue(RuntimeHelpers.GetObjectValue(objectValue)));
                                IComponent component2 = (IComponent)objectValue;
                                if (str2 == null)
                                {
                                    flag = true;
                                    str2 = service.CreateUniqueMethodName(component2, defaultEvent);
                                }
                                else
                                {
                                    IEnumerator enumerator2 = null;
                                    flag = true;
                                    try
                                    {
                                        enumerator2 = service.GetCompatibleMethods(defaultEvent).GetEnumerator();
                                        while (enumerator2.MoveNext())
                                        {
                                            string str3 = Convert.ToString(enumerator2.Current);
                                            if (str2 == str3)
                                            {
                                                flag = false;
                                                goto Label_0164;
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
                            Label_0164:
                                if (flag && (eventProperty != null))
                                {
                                    eventProperty.SetValue(RuntimeHelpers.GetObjectValue(objectValue), str2);
                                }
                                if (objectValue.Equals(RuntimeHelpers.GetObjectValue(selectionService.PrimarySelection)))
                                {
                                    e = defaultEvent;
                                    str = str2;
                                    component = component2;
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
                    catch (InvalidOperationException exception3)
                    {
                        //ProjectData.SetProjectError(exception3);
                        InvalidOperationException exception2 = exception3;
                        if (transaction != null)
                        {
                            transaction.Cancel();
                            transaction = null;
                        }
                        //ProjectData.ClearProjectError();
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                    if ((str != null) & (e != null))
                    {
                        service.ShowCode(component, e);
                    }
                }
            }
        }

        protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
        {
            this.OnSetCursor();
            Rectangle b = this.BehaviorService.ControlRectInAdornerWindow(this.Control);
            Control parent = this.Control.Parent;
            IDesignerHost designerHost = this.DesignerHost;
            Point offset = this.BehaviorService.AdornerWindowToScreen();
            if (((parent != null) && (designerHost != null)) && !object.ReferenceEquals(designerHost.RootComponent, this.Component))
            {
                Rectangle a = this.BehaviorService.ControlRectInAdornerWindow(parent);
                Rectangle bounds = Rectangle.Intersect(a, b);
                if (selectionType.Equals(GlyphSelectionType.NotSelected))
                {
                    if (!bounds.IsEmpty && !a.Contains(b))
                    {
                        return new ShapeContainerBodyGlyph(bounds, Cursor.Current, this.ShapeContainer, this, offset);
                    }
                    if (bounds.IsEmpty)
                    {
                        return null;
                    }
                }
            }
            return new ShapeContainerBodyGlyph(b, Cursor.Current, this.ShapeContainer, this, offset);
        }

        protected override bool GetHitTest(Point point)
        {
            if (this.ToolboxItem != null)
            {
                return base.GetHitTest(point);
            }
            this.m_HitShape = this.ShapeContainer.GetChildAtPointInternal(point);
            return ((this.m_HitShape != null) || base.GetHitTest(point));
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            this.ShapeContainer.Paint += new PaintEventHandler(this.OnPaintShapeContainer);
            ISelectionService selectionService = this.SelectionService;
            if (selectionService != null)
            {
                selectionService.SelectionChanged += new EventHandler(this.OnSelectionChanged);
            }
            if (m_HelperCollection == null)
            {
                m_HelperCollection = new Dictionary<IComponent, ShapeContainerDesignerHelper>();
                DesignerUtility.InitBrushes();
            }
            IDesignerHost designerHost = this.DesignerHost;
            if ((designerHost != null) && (m_PasteHandler == null))
            {
                IDesignerEventService service = (IDesignerEventService)designerHost.GetService(typeof(IDesignerEventService));
                if (service != null)
                {
                    m_PasteHandler = new PasteHandler(service);
                }
            }
            if ((designerHost != null) && (designerHost.RootComponent != null))
            {
                if (m_HelperCollection.TryGetValue(designerHost.RootComponent, out this.m_Helper))
                {
                    ShapeContainerDesignerHelper helper = this.m_Helper;
                    helper.ShapeContainerCounter++;
                }
                else
                {
                    ShapeContainerCommandSet cmdSet = new ShapeContainerCommandSet(this.ShapeContainer.Site);
                    DragManager dragMgr = new DragManager(this.ShapeContainer.Site);
                    SelectionManager selMgr = new SelectionManager(this.ShapeContainer.Site);
                    this.m_Helper = new ShapeContainerDesignerHelper(designerHost.RootComponent);
                    this.m_Helper.Initialize(cmdSet, dragMgr, selMgr);
                    m_HelperCollection.Add(designerHost.RootComponent, this.m_Helper);
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);
            if (Control.ModifierKeys == Keys.Control)
            {
                this.Reparent();
            }
        }

        private void OnLeftButtonDoubleClick(Message m)
        {
            if (this.m_HitShape != null)
            {
                this.DoDefaultAction();
            }
            else
            {
                DesignerUtility.SendMessage(((Control)this.ParentComponent).Handle, m.Msg, m.WParam, m.LParam);
            }
        }

        private void OnLeftButtonDown(Message m)
        {
            IEnumerator enumerator = null;
            Point p = new Point(m.LParam.ToInt32());
            this.m_MouseDragStartPoint = this.ShapeContainer.PointToScreen(p);
            this.m_DragShape = this.m_HitShape;
            ISelectionService selectionService = this.SelectionService;
            if (selectionService == null)
            {
                return;
            }
            if (selectionService.SelectionCount == 0)
            {
                if (this.m_HitShape == null)
                {
                    selectionService.SetSelectedComponents(new IComponent[] { this.ParentComponent }, SelectionTypes.Replace);
                    return;
                }
                selectionService.SetSelectedComponents(new IComponent[] { this.m_HitShape }, SelectionTypes.Replace);
                return;
            }
            bool mKeyPressed = false;
            if ((Control.ModifierKeys == Keys.Control) || (Control.ModifierKeys == Keys.Shift))
            {
                mKeyPressed = true;
            }
            bool flag = false;
            try
            {
                enumerator = selectionService.GetSelectedComponents().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                    if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                    {
                        flag = true;
                    }
                    goto Label_0101;
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        Label_0101:
            if (this.m_HitShape == null)
            {
                this.SelectComponent(selectionService, this.ParentComponent, !flag, mKeyPressed);
            }
            else
            {
                this.SelectComponent(selectionService, this.m_HitShape, flag, mKeyPressed);
            }
        }

        private void OnLeftButtonUp(Message m)
        {
            this.m_MouseDragStartPoint = Point.Empty;
            this.Control.Capture = false;
            if (this.m_MouseDragMoved)
            {
                if (this.m_DragShape == null)
                {
                    this.m_Helper.InDrag = false;
                }
                if (!this.m_MouseDragRect.IsEmpty)
                {
                    ISelectionService selectionService = this.SelectionService;
                    if (selectionService != null)
                    {
                        Rectangle mouseDragRect = this.m_MouseDragRect;
                        mouseDragRect = this.ShapeContainer.RectangleToClient(mouseDragRect);
                        //levy 将Collection换成ArrayList
                        //Collection components = new Collection();
                        ArrayList components = new ArrayList();
                        using (Graphics graphics = this.ShapeContainer.CreateGraphics())
                        {
                            IEnumerator enumerator = null;
                            try
                            {
                                enumerator = this.ShapeContainer.Shapes.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    Shape current = (Shape)enumerator.Current;
                                    bool flag = false;
                                    if (typeof(SparkLine).IsAssignableFrom(current.GetType()))
                                    {
                                        flag = true;
                                    }
                                    //levy 去掉OvalShape和RectangleShape的分支
                                    //else if (typeof(OvalShape).IsAssignableFrom(current.GetType()))
                                    //{
                                    //    flag = true;
                                    //}
                                    //else if (typeof(RectangleShape).IsAssignableFrom(current.GetType()))
                                    //{
                                    //    RectangleShape shape2 = (RectangleShape)current;
                                    //    if (shape2.CornerRadius != 0)
                                    //    {
                                    //        flag = true;
                                    //    }
                                    //}
                                    if (flag)
                                    {
                                        if (current.HitTestRegion == null)
                                        {
                                            if (mouseDragRect.IntersectsWith(current.ExtentBounds))
                                            {
                                                components.Add(current);
                                            }
                                        }
                                        else
                                        {
                                            using (Region region = current.HitTestRegion.Clone())
                                            {
                                                Rectangle rect = current.RectangleToClient(this.m_MouseDragRect);
                                                region.Intersect(rect);
                                                if (!region.IsEmpty(graphics))
                                                {
                                                    components.Add(current);
                                                }
                                            }
                                        }
                                    }
                                    else if (mouseDragRect.IntersectsWith(current.ExtentBounds))
                                    {
                                        components.Add(current);
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
                        if (components.Count == 0)
                        {
                            Control parentComponent = (Control)this.ParentComponent;
                            if (parentComponent != null)
                            {
                                IEnumerator enumerator2 = null;
                                mouseDragRect = this.m_MouseDragRect;
                                mouseDragRect = parentComponent.RectangleToClient(mouseDragRect);
                                try
                                {
                                    enumerator2 = parentComponent.Controls.GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        Control item = (Control)enumerator2.Current;
                                        if (!item.Equals(this.ShapeContainer) && mouseDragRect.IntersectsWith(item.Bounds))
                                        {
                                            components.Add(item);
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
                        }
                        if (components.Count > 0)
                        {
                            if ((Control.ModifierKeys == Keys.Control) || (Control.ModifierKeys == Keys.Shift))
                            {
                                selectionService.SetSelectedComponents(components, SelectionTypes.Toggle);
                            }
                            else
                            {
                                selectionService.SetSelectedComponents(components, SelectionTypes.Replace);
                            }
                        }
                        if (selectionService.SelectionCount == 0)
                        {
                            selectionService.SetSelectedComponents(new IComponent[] { this.ParentComponent }, SelectionTypes.Click);
                        }
                    }
                }
                this.m_MouseDragMoved = false;
                this.ClearDragRegion();
            }
        }

        private void OnMouseMove(Message m)
        {
            Point p = new Point(m.LParam.ToInt32());
            p = this.ShapeContainer.PointToScreen(p);
            if (!this.m_MouseDragMoved)
            {
                if (this.m_MouseDragStartPoint.IsEmpty)
                {
                    return;
                }
                Size minDragSize = DesignerUtility.GetMinDragSize();
                if ((Math.Abs((int)(p.X - this.m_MouseDragStartPoint.X)) < minDragSize.Width) && (Math.Abs((int)(p.Y - this.m_MouseDragStartPoint.Y)) < minDragSize.Height))
                {
                    return;
                }
                this.m_MouseDragMoved = true;
                if (this.m_DragShape == null)
                {
                    if (!this.m_Helper.InDrag)
                    {
                        this.m_Helper.InDrag = true;
                    }
                    this.Control.Capture = true;
                    this.m_Helper.ShapeContainerCommandSet.DragShapeConDesigner = this;
                }
            }
            if (this.m_DragShape != null)
            {
                ISelectionService selectionService = this.SelectionService;
                if ((selectionService != null) & (selectionService.SelectionCount > 0))
                {
                    IEnumerator enumerator = null;
                    List<Shape> list2 = new List<Shape>();
                    try
                    {
                        enumerator = selectionService.GetSelectedComponents().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                            if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                            {
                                Shape item = (Shape)objectValue;
                                if (item.Parent.Equals(this.ShapeContainer))
                                {
                                    list2.Add(item);
                                }
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
                    object[] array = new object[(list2.Count - 1) + 1];
                    int num2 = list2.Count - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        array[i] = list2[i];
                    }
                    Array.Sort(array, new ShapeContainerCommandSet.ComponentZOrderCompare());
                    List<Shape> comps = new List<Shape>();
                    foreach (Shape shape2 in array)
                    {
                        comps.Add(shape2);
                    }
                    if (comps.Contains(this.m_DragShape))
                    {
                        this.m_Helper.DragManager.DoDragDrop(this, comps, HitFlag.HitMove, p, false);
                    }
                    else
                    {
                        comps.Add(this.m_DragShape);
                        this.m_Helper.DragManager.DoDragDrop(this, comps, HitFlag.HitMove, p, true);
                    }
                    if (this.m_Disposed)
                    {
                        return;
                    }
                }
                this.m_MouseDragStartPoint = Point.Empty;
                this.m_MouseDragMoved = false;
                this.m_DragShape = null;
            }
            else
            {
                this.m_MouseDragEndPoint = p;
                if ((this.ShapeContainer.GetChildAtPoint(p) != null) && !Cursor.Current.Equals(Cursors.SizeAll))
                {
                    Cursor.Current = Cursors.SizeAll;
                }
                int x = Math.Min(this.m_MouseDragStartPoint.X, this.m_MouseDragEndPoint.X);
                int y = Math.Min(this.m_MouseDragStartPoint.Y, this.m_MouseDragEndPoint.Y);
                int width = Math.Abs((int)(this.m_MouseDragEndPoint.X - this.m_MouseDragStartPoint.X));
                Rectangle r = new Rectangle(x, y, width, Math.Abs((int)(this.m_MouseDragEndPoint.Y - this.m_MouseDragStartPoint.Y)));
                this.m_MouseDragRect = r;
                Control parentComponent = (Control)this.ParentComponent;
                if (parentComponent != null)
                {
                    this.m_Helper.ShapeContainerCommandSet.StatusBarCommand.SetStatusBarInfo(parentComponent.RectangleToClient(r));
                }
                Point point2 = this.BehaviorService.AdornerWindowToScreen();
                r.Offset(0 - point2.X, 0 - point2.Y);
                r.Inflate(1, 1);
                Region region = new Region(r);
                r.Inflate(-1, -1);
                region.Exclude(r);
                if (this.m_MouseDragRegion != null)
                {
                    this.m_MouseDragRegion.Exclude(region);
                    this.BehaviorService.Invalidate(this.m_MouseDragRegion);
                    this.m_MouseDragRegion.Dispose();
                    this.m_MouseDragRegion = null;
                }
                this.m_MouseDragRegion = region;
                using (Graphics graphics = this.BehaviorService.AdornerWindowGraphics)
                {
                    Rectangle rect = new Rectangle(0, 0, r.Right + 2, r.Bottom + 2);
                    graphics.SetClip(rect);
                    DesignerUtility.DrawReversibleFrame(graphics, ((Control)this.ParentComponent).BackColor, this.m_MouseDragRegion);
                }
            }
        }

        private void OnPaintShapeContainer(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            IDesignerHost designerHost = this.DesignerHost;
            ISelectionService selectionService = this.SelectionService;
            int num = this.ShapeContainer.Shapes.Count - 1;
            if (num >= 0)
            {
                for (int i = num; i >= 0; i += -1)
                {
                    Shape component = (Shape)this.ShapeContainer.Shapes[i];
                    ShapeDesigner designer = (ShapeDesigner)designerHost.GetDesigner(component);
                    if ((designer != null) && !designer.OnDragging)
                    {
                        //levy 这里直接用Rectangle.Empty有没有问题??
                        component.OnPaint(new PaintEventArgs(e.Graphics, Rectangle.Empty));
                    }
                }
            }
            this.ShapeContainer.SendToBackInternal();
        }

        private void OnRightButtonDown(Message m)
        {
            if (this.m_HitShape != null)
            {
                ISelectionService selectionService = this.SelectionService;
                if (selectionService != null)
                {
                    if (!selectionService.GetComponentSelected(this.m_HitShape))
                    {
                        selectionService.SetSelectedComponents(new IComponent[] { this.m_HitShape }, SelectionTypes.Auto);
                    }
                    else if (!object.ReferenceEquals(this.m_HitShape, RuntimeHelpers.GetObjectValue(selectionService.PrimarySelection)))
                    {
                        selectionService.SetSelectedComponents(new IComponent[] { this.m_HitShape }, SelectionTypes.Click);
                    }
                }
            }
            else
            {
                DesignerUtility.SendMessage(((Control)this.ParentComponent).Handle, m.Msg, m.WParam, m.LParam);
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!this.m_InSelectionChange)
            {
                try
                {
                    this.m_InSelectionChange = true;
                    ISelectionService selectionService = this.SelectionService;
                    if ((selectionService != null) && (this.CreationMode && (selectionService.SelectionCount == 0)))
                    {
                        selectionService.SetSelectedComponents(new IComponent[] { (IComponent)this.ShapeContainer.Shapes[0] }, SelectionTypes.Replace);
                        this.CreationMode = false;
                        this.m_InSelectionChange = false;
                    }
                }
                finally
                {
                    this.m_InSelectionChange = false;
                }
            }
        }

        private void Reparent()
        {
            if (this.ShapeContainer.Parent != null)
            {
                for (int i = this.ShapeContainer.Controls.Count; i >= 1; i += -1)
                {
                    Control control = this.ShapeContainer.Controls[i - 1];
                    this.ShapeContainer.Controls.Remove(control);
                    Point location = control.Location;
                    Control parent = this.ShapeContainer.Parent;
                    location = this.ShapeContainer.PointToScreen(location);
                    location = parent.PointToClient(location);
                    control.Location = location;
                    parent.Controls.Add(control);
                    parent.Controls.SetChildIndex(control, 0);
                }
            }
        }

        private void SelectComponent(ISelectionService ss, IComponent selComp, bool flag, bool mKeyPressed)
        {
            if (flag)
            {
                if (ss.GetComponentSelected(selComp))
                {
                    if (mKeyPressed)
                    {
                        ss.SetSelectedComponents(new IComponent[] { selComp }, SelectionTypes.Auto);
                    }
                    else
                    {
                        ss.SetSelectedComponents(new IComponent[] { selComp }, SelectionTypes.Click);
                    }
                }
                else if (mKeyPressed)
                {
                    ss.SetSelectedComponents(new IComponent[] { selComp }, SelectionTypes.Add);
                }
                else
                {
                    ss.SetSelectedComponents(new IComponent[] { selComp }, SelectionTypes.Replace);
                }
            }
            else if (!mKeyPressed)
            {
                ss.SetSelectedComponents(new IComponent[] { selComp }, SelectionTypes.Replace);
            }
        }

        private void SelectParent()
        {
            ISelectionService selectionService = this.SelectionService;
            if ((selectionService != null) && (this.ShapeContainer.Parent != null))
            {
                selectionService.SetSelectedComponents(new IComponent[] { this.ShapeContainer.Parent }, SelectionTypes.Replace);
            }
        }

        private void ShowContextMenu(ref Message m)
        {
            if (this.m_HitShape == null)
            {
                DesignerUtility.SendMessage(((Control)this.ParentComponent).Handle, m.Msg, m.WParam, m.LParam);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x7b)
            {
                this.ShowContextMenu(ref m);
            }
            else if ((m.Msg == 0x204) || (m.Msg == 0xa4))
            {
                this.OnRightButtonDown(m);
            }
            else
            {
                if (this.ToolboxItem == null)
                {
                    if (m.Msg == 0x20)
                    {
                        if (this.m_HitShape != null)
                        {
                            if (!Cursor.Current.Equals(Cursors.SizeAll))
                            {
                                Cursor.Current = Cursors.SizeAll;
                                return;
                            }
                            return;
                        }
                    }
                    else
                    {
                        if ((m.Msg == 0x201) || (m.Msg == 0xa1))
                        {
                            this.OnLeftButtonDown(m);
                            this.BaseWndProc(ref m);
                            return;
                        }
                        if ((m.Msg == 0x202) || (m.Msg == 0xa2))
                        {
                            this.OnLeftButtonUp(m);
                            this.BaseWndProc(ref m);
                            return;
                        }
                        if ((m.Msg == 0x203) || (m.Msg == 0xa3))
                        {
                            this.OnLeftButtonDoubleClick(m);
                            this.BaseWndProc(ref m);
                            return;
                        }
                        if (((m.Msg == 0x200) || (m.Msg == 160)) && ((m.WParam.ToInt32() & 1) != 0))
                        {
                            this.OnMouseMove(m);
                            if (m.Msg == 0x200)
                            {
                                this.BaseWndProc(ref m);
                                return;
                            }
                            return;
                        }
                    }
                }
                base.WndProc(ref m);
            }
        }

        public override ICollection AssociatedComponents
        {
            get
            {
                IEnumerator enumerator = null;
                ArrayList list = new ArrayList();
                try
                {
                    enumerator = this.ShapeContainer.Shapes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Shape current = (Shape)enumerator.Current;
                        list.Add(current);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                return list;
            }
        }

        internal IComponentChangeService ChangeService
        {
            get
            {
                if (this.m_ChangeService == null)
                {
                    this.m_ChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
                }
                return this.m_ChangeService;
            }
        }

        public override Control Control
        {
            get
            {
                if (this.m_InCreateToolCore)
                {
                    this.m_InCreateToolCore = false;
                    return (Control)this.ParentComponent;
                }
                return base.Control;
            }
        }

        internal bool CreationMode
        {
            get
            {
                return this.m_CreationMode;
            }
            set
            {
                this.m_CreationMode = value;
            }
        }

        internal IDesignerHost DesignerHost
        {
            get
            {
                if (this.m_Host == null)
                {
                    this.m_Host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                }
                return this.m_Host;
            }
        }

        internal DragManager DragManager
        {
            get
            {
                return this.m_Helper.DragManager;
            }
        }

        internal SelectionManager SelectionManager
        {
            get
            {
                return this.m_Helper.SelectionManager;
            }
        }

        internal ISelectionService SelectionService
        {
            get
            {
                if (this.m_SelectionSvc == null)
                {
                    this.m_SelectionSvc = (ISelectionService)this.GetService(typeof(ISelectionService));
                }
                return this.m_SelectionSvc;
            }
        }

        public ShapeContainer ShapeContainer
        {
            get
            {
                return (ShapeContainer)this.Component;
            }
        }

        public override IList SnapLines
        {
            get
            {
                IEnumerator enumerator = null;
                IList snapLines = base.SnapLines;
                try
                {
                    enumerator = this.ShapeContainer.Shapes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                        if (objectValue != null)
                        {
                            //levy 去掉SimpleShape的分支
                            //if (typeof(SimpleShape).IsAssignableFrom(objectValue.GetType()))
                            //{
                            //    SimpleShape shape = (SimpleShape)objectValue;
                            //    snapLines.Add(new SnapLine(SnapLineType.Top, shape.Top));
                            //    snapLines.Add(new SnapLine(SnapLineType.Bottom, shape.Bottom));
                            //    snapLines.Add(new SnapLine(SnapLineType.Left, shape.Left));
                            //    snapLines.Add(new SnapLine(SnapLineType.Right, shape.Right));
                            //}
                            //else if (typeof(LineShape).IsAssignableFrom(objectValue.GetType()))
                            if (typeof(SparkLine).IsAssignableFrom(objectValue.GetType()))
                            {
                                SparkLine shape2 = (SparkLine)objectValue;
                                snapLines.Add(new SnapLine(SnapLineType.Top, shape2.BoundRect.Top));
                                snapLines.Add(new SnapLine(SnapLineType.Bottom, shape2.BoundRect.Bottom));
                                snapLines.Add(new SnapLine(SnapLineType.Left, shape2.BoundRect.Left));
                                snapLines.Add(new SnapLine(SnapLineType.Right, shape2.BoundRect.Right));
                            }
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
                return snapLines;
            }
        }

        private ToolboxItem ToolboxItem
        {
            get
            {
                IToolboxService toolboxService = this.ToolboxService;
                ToolboxItem selectedToolboxItem = null;
                if (toolboxService != null)
                {
                    selectedToolboxItem = toolboxService.GetSelectedToolboxItem(this.DesignerHost);
                }
                return selectedToolboxItem;
            }
        }

        private IToolboxService ToolboxService
        {
            get
            {
                if (this.m_ToolboxSvc == null)
                {
                    this.m_ToolboxSvc = (IToolboxService)this.GetService(typeof(IToolboxService));
                }
                return this.m_ToolboxSvc;
            }
        }
    }
}