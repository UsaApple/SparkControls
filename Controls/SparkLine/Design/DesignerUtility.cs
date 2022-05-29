using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
    internal class DesignerUtility
    {
        public const string CF_DESIGNER = "CF_DESIGNERCOMPONENTS_V2";
        public const int HANDLEOVERLAP = 0;
        public const int HANDLESIZE = 6;
        private static SolidBrush m_GrabHandleFillBrush;
        private static SolidBrush m_GrabHandleFillBrushPrimary;
        private static Pen m_GrabHandlePen;
        private static Pen m_GrabHandlePenPrimary;
        public const int MK_LBUTTON = 1;
        public const int SELECTIONBORDERHITAREA = 3;
        public const int SELECTIONBORDEROFFSET = 3;
        public const int SELECTIONBORDERSIZE = 1;
        public const string SHAPECONTAINER_DESIGNER = "SHAPECONTAINER_DESIGNERCOMPONENTS";
        public const int WM_CANCELMODE = 0x1f;
        public const int WM_CONTEXTMENU = 0x7b;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_NCLBUTTONDBLCLK = 0xa3;
        public const int WM_NCLBUTTONDOWN = 0xa1;
        public const int WM_NCLBUTTONUP = 0xa2;
        public const int WM_NCMOUSEMOVE = 160;
        public const int WM_NCRBUTTONDOWN = 0xa4;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_SETCURSOR = 0x20;

        private DesignerUtility()
        {
        }

        public static void ClearBrushes()
        {
            if (m_GrabHandleFillBrushPrimary != null)
            {
                m_GrabHandleFillBrushPrimary.Dispose();
                m_GrabHandleFillBrushPrimary = null;
            }
            if (m_GrabHandleFillBrush != null)
            {
                m_GrabHandleFillBrush.Dispose();
                m_GrabHandleFillBrush = null;
            }
            if (m_GrabHandlePenPrimary != null)
            {
                m_GrabHandlePenPrimary.Dispose();
                m_GrabHandlePenPrimary = null;
            }
            if (m_GrabHandlePen != null)
            {
                m_GrabHandlePen.Dispose();
                m_GrabHandlePen = null;
            }
        }

        public static void CopyShapeProperties(ref Shape srcShape, ref Shape tarShape)
        {
            if ((srcShape != null) && (tarShape != null))
            {
                foreach (PropertyInfo info in srcShape.GetType().GetProperties())
                {
                    if ((!info.Name.Equals("Parent") && !info.Name.Equals("Site")) && (!info.Name.Equals("AccessibilityObject") && info.CanWrite))
                    {
                        info.SetValue(tarShape, RuntimeHelpers.GetObjectValue(info.GetValue(srcShape, null)), null);
                    }
                }
            }
        }

        public static void DrawGrabHandler(Graphics g, Rectangle bounds, bool isPrimary)
        {
            SolidBrush grabHandleFillBrushPrimary = null;
            Pen grabHandlePenPrimary = null;
            if (isPrimary)
            {
                grabHandlePenPrimary = m_GrabHandlePenPrimary;
                grabHandleFillBrushPrimary = m_GrabHandleFillBrushPrimary;
            }
            else
            {
                grabHandlePenPrimary = m_GrabHandlePen;
                grabHandleFillBrushPrimary = m_GrabHandleFillBrush;
            }
            int num = 2;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(bounds.Right - (2 * num), bounds.Top, num * 2, num * 2, 270f, 90f);
                path.AddArc((int)(bounds.Right - (2 * num)), (int)(bounds.Bottom - (num * 2)), (int)(num * 2), (int)(num * 2), 0f, 90f);
                path.AddArc(bounds.Left, bounds.Bottom - (2 * num), num * 2, num * 2, 90f, 90f);
                path.AddArc(bounds.Left, bounds.Top, num * 2, num * 2, 180f, 90f);
                path.CloseFigure();
                g.FillPath(grabHandleFillBrushPrimary, path);
                g.DrawPath(grabHandlePenPrimary, path);
            }
        }

        public static void DrawReversibleFrame(Graphics g, Color bgColor, Region region)
        {
            Color controlDarkDark = SystemColors.ControlDarkDark;
            if (bgColor.GetBrightness() < 0.5)
            {
                controlDarkDark = SystemColors.ControlLight;
            }
            Brush brush = new HatchBrush(HatchStyle.Percent50, controlDarkDark, Color.Transparent);
            try
            {
                g.FillRegion(brush, region);
            }
            catch (ArgumentNullException exception1)
            {
                //ProjectData.SetProjectError(exception1);
                ArgumentNullException exception = exception1;
                //ProjectData.ClearProjectError();
            }
            finally
            {
                if (brush != null)
                {
                    brush.Dispose();
                }
            }
        }

        public static Size GetGridSize(IDesignerHost host)
        {
            Size empty = Size.Empty;
            if (host != null)
            {
                IComponent rootComponent = host.RootComponent;
                if ((rootComponent != null) && (rootComponent is Control))
                {
                    PropertyDescriptor descriptor = TypeDescriptor.GetProperties(rootComponent)["CurrentGridSize"];
                    if (descriptor != null)
                    {
                        empty = (Size)descriptor.GetValue(rootComponent);
                    }
                }
            }
            if (empty.IsEmpty)
            {
                DesignerOptions options = new DesignerOptions();
                empty = options.GridSize;
            }
            return empty;
        }

        public static Size GetMinDragSize()
        {
            Size dragSize = SystemInformation.DragSize;
            Size doubleClickSize = SystemInformation.DoubleClickSize;
            dragSize.Width = Math.Max(dragSize.Width, doubleClickSize.Width);
            dragSize.Height = Math.Max(dragSize.Height, doubleClickSize.Height);
            return dragSize;
        }

        public static Size GetScreenSize()
        {
            Size empty = Size.Empty;
            if (Screen.AllScreens.Length == 1)
            {
                empty = Screen.AllScreens[0].WorkingArea.Size;
            }
            if (empty.IsEmpty)
            {
                empty = SystemInformation.VirtualScreen.Size;
            }
            return empty;
        }

        public static ShapeContainer GetShapeContainer(IDesignerHost host, ISelectionService ss, IComponentChangeService cs)
        {
            object objectValue = RuntimeHelpers.GetObjectValue(ss.PrimarySelection);
            Control component = null;
            if (objectValue == null)
            {
                if (typeof(Control).IsAssignableFrom(host.RootComponent.GetType()))
                {
                    component = (Control)host.RootComponent;
                }
            }
            else if (typeof(TabControl).IsAssignableFrom(objectValue.GetType()))
            {
                component = ((TabControl)objectValue).SelectedTab;
            }
            else if (typeof(Control).IsAssignableFrom(objectValue.GetType()))
            {
                component = (Control)objectValue;
            }
            else if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
            {
                component = ((Shape)objectValue).Parent;
            }
            else if (typeof(ToolStripItem).IsAssignableFrom(objectValue.GetType()))
            {
                component = ((ToolStripItem)objectValue).Owner;
            }
            else if (typeof(Control).IsAssignableFrom(host.RootComponent.GetType()))
            {
                component = (Control)host.RootComponent;
            }
            while (component != null)
            {
                IDesigner designer = host.GetDesigner(component);
                if (((designer != null) && typeof(ParentControlDesigner).IsAssignableFrom(designer.GetType())) && (!typeof(TableLayoutPanel).IsAssignableFrom(component.GetType()) && !typeof(FlowLayoutPanel).IsAssignableFrom(component.GetType())))
                {
                    break;
                }
                component = component.Parent;
            }
            if ((component != null) && !typeof(ShapeContainer).IsAssignableFrom(component.GetType()))
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = component.Controls.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Control current = (Control)enumerator.Current;
                        if (typeof(ShapeContainer).IsAssignableFrom(current.GetType()))
                        {
                            return (ShapeContainer)current;
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
                ShapeContainer container2 = (ShapeContainer)host.CreateComponent(typeof(ShapeContainer));
                if (container2 != null)
                {
                    cs.OnComponentChanging(component, null);
                    component.Controls.Add(container2);
                    cs.OnComponentChanged(component, null, null, null);
                    return container2;
                }
            }
            return (ShapeContainer)component;
        }

        public static void InitBrushes()
        {
            m_GrabHandleFillBrushPrimary = new SolidBrush(SystemColors.Window);
            m_GrabHandleFillBrush = new SolidBrush(SystemColors.ControlText);
            m_GrabHandlePenPrimary = new Pen(SystemColors.ControlText);
            m_GrabHandlePen = new Pen(SystemColors.Window);
        }

        public static void InvalidateShapeBounds(object obj)
        {
            if (typeof(Shape).IsAssignableFrom(obj.GetType()))
            {
                Shape shape = (Shape)obj;
                if (shape != null)
                {
                    shape.Invalidate();
                }
            }
        }

        public static bool IsCriticalException(Exception ex)
        {
            return ((((ex is NullReferenceException) || (ex is StackOverflowException)) || ((ex is OutOfMemoryException) || (ex is ThreadAbortException))) || (((ex is ExecutionEngineException) || (ex is IndexOutOfRangeException)) || (ex is AccessViolationException)));
        }

        public static void PasteShapes(IDesignerHost host, List<Shape> shapeList, ShapeContainer shapecontainer, IComponentChangeService cs)
        {
            foreach (Shape shape in shapeList)
            {
                cs.OnComponentChanging(shape, null);
            }
            UpdatePastePositions(shapeList, shapecontainer, host);
            cs.OnComponentChanging(shapecontainer, null);
            foreach (Shape shape2 in shapeList)
            {
                shape2.Name = shape2.Site.Name;
                cs.OnComponentChanged(shape2, null, null, null);
                shapecontainer.Shapes.Insert(0, shape2);
            }
            cs.OnComponentChanged(shapecontainer, null, null, null);
        }

        public static void RefreshBrushes()
        {
            if (m_GrabHandleFillBrushPrimary != null)
            {
                m_GrabHandleFillBrushPrimary.Dispose();
            }
            m_GrabHandleFillBrushPrimary = new SolidBrush(SystemColors.Window);
            if (m_GrabHandleFillBrush != null)
            {
                m_GrabHandleFillBrush.Dispose();
            }
            m_GrabHandleFillBrush = new SolidBrush(SystemColors.ControlText);
            if (m_GrabHandlePenPrimary != null)
            {
                m_GrabHandlePenPrimary.Dispose();
            }
            m_GrabHandlePenPrimary = new Pen(SystemColors.ControlText);
            if (m_GrabHandlePen != null)
            {
                m_GrabHandlePen.Dispose();
            }
            m_GrabHandlePen = new Pen(SystemColors.Window);
        }

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        public static void ShowErrorMessage(IUIService uiSvc, string errStr)
        {
            if (uiSvc != null)
            {
                uiSvc.ShowError(errStr);
            }
        }

        public static void UpdatePastePositions(List<Shape> shapes, ShapeContainer newContainer, IDesignerHost host)
        {
            Point point = Point.Empty;
            Point point4 = Point.Empty;
            IEnumerator enumerator = null;
            List<Shape>.Enumerator enumerator3 = new List<Shape>.Enumerator();
            Shape shape = shapes[0];
            Point location = shape.BoundRect.Location;
            foreach (Shape shape2 in shapes)
            {
                location.X = Math.Min(shape2.BoundRect.Left, location.X);
                location.Y = Math.Min(shape2.BoundRect.Top, location.Y);
                point.X = Math.Max(shape2.BoundRect.Right, point.X);
                point.Y = Math.Max(shape2.BoundRect.Bottom, point.Y);
            }
            Point pos = new Point(0 - location.X, 0 - location.Y);
            bool flag = false;
            bool flag2 = false;
            Size clientSize = newContainer.ClientSize;
            if (newContainer.Parent != null)
            {
                clientSize = newContainer.Parent.ClientSize;
            }
            Size empty = Size.Empty;
            point4 = new Point(clientSize.Width / 2, clientSize.Height / 2)
            {
                X = point4.X - ((point.X - location.X) / 2),
                Y = point4.Y - ((point.Y - location.Y) / 2)
            };
        Label_0174:
            flag = false;
            Rectangle boundRect = shape.BoundRect;
            boundRect.Offset(pos);
            boundRect.Offset(point4);
            if (newContainer == null)
            {
                goto Label_02CE;
            }
            try
            {
                enumerator = newContainer.Shapes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Shape current = (Shape)enumerator.Current;
                    Rectangle r = current.BoundRect;
                    if (newContainer.Parent != null)
                    {
                        r = newContainer.RectangleToScreen(r);
                        r = newContainer.Parent.RectangleToClient(r);
                    }
                    if (boundRect.Equals(r))
                    {
                        flag = true;
                        if (empty.IsEmpty)
                        {
                            empty = GetGridSize(host);
                        }
                        point4.Offset(empty.Width, empty.Height);
                        Point point5 = point4;
                        if (shapes.Count > 1)
                        {
                            point5.Offset(point.X - location.X, point.Y - location.Y);
                        }
                        else
                        {
                            point5.Offset(empty.Width, empty.Height);
                        }
                        if ((point5.X > clientSize.Width) || (point5.Y > clientSize.Height))
                        {
                            point4 = new Point(0, 0);
                            if (flag2)
                            {
                                flag = false;
                            }
                            else
                            {
                                flag2 = true;
                            }
                        }
                        goto Label_02C4;
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
        Label_02C4:
            if (flag)
            {
                goto Label_0174;
            }
        Label_02CE:
            pos.Offset(point4);
            if (newContainer.Parent != null)
            {
                Point point6 = newContainer.Location;
                pos.Offset(0 - point6.X, 0 - point6.Y);
            }
            try
            {
                enumerator3 = shapes.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    Shape shape4 = enumerator3.Current;
                    if (typeof(SparkLine).IsAssignableFrom(shape4.GetType()))
                    {
                        SparkLine shape5 = (SparkLine)shape4;
                        Point startPoint = shape5.StartPoint;
                        startPoint.Offset(pos);
                        shape5.StartPoint = startPoint;
                        startPoint = shape5.EndPoint;
                        startPoint.Offset(pos);
                        shape5.EndPoint = startPoint;
                    }
                    //else
                    //{
                    //    SimpleShape shape6 = (SimpleShape)shape4;
                    //    Point point8 = shape6.Location;
                    //    point8.Offset(pos);
                    //    shape6.Location = point8;
                    //}
                }
            }
            finally
            {
                enumerator3.Dispose();
            }
        }

        public static bool UseSnapLines(IServiceProvider provider)
        {
            bool flag = true;
            try
            {
                object objectValue = null;
                DesignerOptionService service = (DesignerOptionService)provider.GetService(typeof(DesignerOptionService));
                if (service != null)
                {
                    PropertyDescriptor descriptor = service.Options.Properties["UseSnapLines"];
                    if (descriptor != null)
                    {
                        objectValue = RuntimeHelpers.GetObjectValue(descriptor.GetValue(null));
                    }
                }
                if ((objectValue != null) & (objectValue is bool))
                {
                    flag = Convert.ToBoolean(objectValue);
                }
            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                //ProjectData.ClearProjectError();
            }
            return flag;
        }

        public enum DragResult
        {
            Cancel,
            Canceled,
            Dropped,
            None
        }
    }
}