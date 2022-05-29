using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
    internal sealed class ShapeContainerCommandSet : IDisposable
    {
        private IComponentChangeService m_ChangeService;
        private IDesignerHost m_DesignerHost;
        private IDesignerSerializationService m_DesignerSerializationService;
        private bool m_Disposed;
        private ShapeContainerDesigner m_DragShapeConDesigner;
        private ShapeContainerDesignerHelper m_Helper;
        private bool m_InOperation;
        private IMenuCommandService m_MenuCommandService;
        private List<MenuCommand> m_OwnMenuCommands;
        private IComponent m_PrimarySelection;
        private Hashtable m_PropDescLookup = new Hashtable();
        private bool m_SelectionOnControlsOnly;
        private bool m_SelectionOnShapesOnly;
        private ISelectionService m_SelectionService;
        private int m_SeletionCount;
        private List<ShapeContainerMenuCommand> m_ShapeMenuCommands;
        private bool m_ShouldClearClipboard;
        private bool m_ShouldDispose;
        private StatusBarCommand m_StatusBarCommand;
        private Dictionary<CommandID, MenuCommand> m_StdMenuCommands;
        private ISite m_SvcProvider;
        private bool m_UseSnapLines;
        private bool m_UseSnapLinesQueried;

        public ShapeContainerCommandSet(ISite site)
        {
            this.m_SvcProvider = site;
            IMenuCommandService menuService = (IMenuCommandService)this.m_SvcProvider.GetService(typeof(IMenuCommandService));
            ISelectionService service = (ISelectionService)this.m_SvcProvider.GetService(typeof(ISelectionService));
            IDesignerHost host = (IDesignerHost)this.m_SvcProvider.GetService(typeof(IDesignerHost));
            if (((host != null) && (menuService != null)) && (service != null))
            {
                MenuCommandHandler serviceInstance = new MenuCommandHandler(menuService, service);
                host.RemoveService(typeof(IMenuCommandService));
                host.AddService(typeof(IMenuCommandService), serviceInstance);
            }
            this.ReplaceStdMenuCommands();
            this.m_StatusBarCommand = new StatusBarCommand(this.m_SvcProvider);
        }

        protected bool CanCheckout(IComponent comp)
        {
            IComponentChangeService changeService = this.ChangeService;
            if (changeService != null)
            {
                try
                {
                    changeService.OnComponentChanging(comp, null);
                }
                catch (CheckoutException exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    CheckoutException exception = exception1;
                    if (!exception.Equals(CheckoutException.Canceled))
                    {
                        throw exception;
                    }
                    //ProjectData.ClearProjectError();
                    return false;
                }
            }
            return true;
        }

        private bool CheckSelectionParenting()
        {
            IEnumerator enumerator = null;
            IEnumerator enumerator2 = null;
            ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
            IComponent rootComponent = null;
            if (this.m_SvcProvider == null)
            {
                return false;
            }
            IDesignerHost service = (IDesignerHost)this.m_SvcProvider.GetService(typeof(IDesignerHost));
            if (service != null)
            {
                rootComponent = service.RootComponent;
            }
            Hashtable hashtable = new Hashtable(selectedComponents.Count);
            try
            {
                enumerator = selectedComponents.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                    Control control2 = (Control)objectValue;
                    if ((control2 == null) | (control2.Site == null))
                    {
                        return false;
                    }
                    hashtable.Add(RuntimeHelpers.GetObjectValue(objectValue), RuntimeHelpers.GetObjectValue(objectValue));
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            Control objB = null;
            try
            {
                enumerator2 = selectedComponents.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    object obj2 = RuntimeHelpers.GetObjectValue(enumerator2.Current);
                    Control control3 = (Control)obj2;
                    if ((control3 == null) | (control3.Site == null))
                    {
                        return false;
                    }
                    for (Control control4 = control3.Parent; control4 != null; control4 = control4.Parent)
                    {
                        if (!object.ReferenceEquals(control4, objB))
                        {
                            object obj4 = RuntimeHelpers.GetObjectValue(hashtable[control4]);
                            if ((obj4 != null) && !object.ReferenceEquals(RuntimeHelpers.GetObjectValue(obj4), RuntimeHelpers.GetObjectValue(obj2)))
                            {
                                return false;
                            }
                        }
                    }
                    objB = control3.Parent;
                }
            }
            finally
            {
                if (enumerator2 is IDisposable)
                {
                    (enumerator2 as IDisposable).Dispose();
                }
            }
            return true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_Disposed && disposing)
            {
                this.RestoreStdMenuCommands();
                if (this.m_ShouldClearClipboard)
                {
                    Clipboard.Clear();
                }
                IMenuCommandService service = (IMenuCommandService)this.m_SvcProvider.GetService(typeof(IMenuCommandService));
                MenuCommandHandler handler = null;
                if (service != null)
                {
                    handler = (MenuCommandHandler)service;
                }
                IDesignerHost host = (IDesignerHost)this.m_SvcProvider.GetService(typeof(IDesignerHost));
                if ((host != null) && (handler != null))
                {
                    IMenuCommandService menuService = handler.MenuService;
                    host.RemoveService(typeof(IMenuCommandService));
                    host.AddService(typeof(IMenuCommandService), menuService);
                }
                this.m_StatusBarCommand = null;
                this.m_Helper = null;
            }
            this.m_Disposed = true;
        }

        private void FindAndRemoveStdCommand(ref IMenuCommandService mcs, CommandID cmdId)
        {
            MenuCommand command = mcs.FindCommand(cmdId);
            if (command != null)
            {
                mcs.RemoveCommand(command);
                this.StdMenuCommands.Add(cmdId, command);
            }
        }

        public static void GetChildShapeContainers(ICollection selComps, ref List<ShapeContainer> shConCol)
        {
            IEnumerator enumerator = null;
            try
            {
                enumerator = selComps.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                    if (typeof(ShapeContainer).IsAssignableFrom(objectValue.GetType()))
                    {
                        ShapeContainer item = (ShapeContainer)objectValue;
                        if (!shConCol.Contains(item))
                        {
                            shConCol.Add(item);
                        }
                        return;
                    }
                    if (typeof(Control).IsAssignableFrom(objectValue.GetType()))
                    {
                        Control control = (Control)objectValue;
                        if (control.Controls.Count > 0)
                        {
                            GetChildShapeContainers(control.Controls, ref shConCol);
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
        }

        private ICollection GetCopySelection()
        {
            IEnumerator enumerator = null;
            ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
            if ((selectedComponents == null) || (selectedComponents.Count == 0))
            {
                return null;
            }
            ArrayList list2 = new ArrayList();
            ArrayList list = new ArrayList();
            try
            {
                enumerator = selectedComponents.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                    if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                    {
                        Shape shape = (Shape)objectValue;
                        list2.Add(RuntimeHelpers.GetObjectValue(objectValue));
                        if (shape.Site != null)
                        {
                            list.Add(shape.Site.Name);
                        }
                        else
                        {
                            list.Add(string.Empty);
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
            if (list2.Count == 0)
            {
                return null;
            }
            object[] array = new object[list2.Count + 1];
            string[] strArray = new string[(list.Count - 1) + 1];
            list.CopyTo(strArray, 0);
            array[0] = strArray;
            list2.CopyTo(array, 1);
            return array;
        }

        private Point GetLocation(IComponent comp)
        {
            PropertyDescriptor property = this.GetProperty(comp, "Location");
            if (property != null)
            {
                try
                {
                    return (Point)property.GetValue(comp);
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception ex = exception1;
                    if (DesignerUtility.IsCriticalException(ex))
                    {
                        throw;
                    }
                    //ProjectData.ClearProjectError();
                }
            }
            return Point.Empty;
        }

        internal PropertyDescriptor GetProperty(object comp, string propName)
        {
            if (this.m_PropDescLookup.Contains(comp.GetType().Name + propName))
            {
                return (PropertyDescriptor)this.m_PropDescLookup[comp.GetType().Name + propName];
            }
            PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(comp))[propName];
            this.m_PropDescLookup.Add(comp.GetType().Name + propName, descriptor2);
            return descriptor2;
        }

        private Size GetSize(IComponent comp)
        {
            PropertyDescriptor property = this.GetProperty(comp, "Size");
            if (property == null)
            {
                return Size.Empty;
            }
            object obj1 = property.GetValue(comp);
            if (obj1 == null) return Size.Empty;
            return (Size)obj1;
        }

        private bool InvokeStdMenuCmd(CommandID cmd, bool skip)
        {
            object obj2;
            ISelectionService selectionService = this.SelectionService;
            if (selectionService != null)
            {
                ICollection selectedComponents = selectionService.GetSelectedComponents();
                if ((selectedComponents != null) && (selectedComponents.Count > 0))
                {
                    IEnumerator enumerator = null;
                    try
                    {
                        enumerator = selectedComponents.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                            if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                            {
                                return false;
                            }
                            if (!skip)
                            {
                                goto Label_0088;
                            }
                            if (this.NeedSkipOperation(RuntimeHelpers.GetObjectValue(objectValue)))
                            {
                                return true;
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
            }
        Label_0088:
            obj2 = this.StdMenuCommands[cmd];
            ((MenuCommand)obj2).Invoke();
            return true;
        }

        private bool NeedSkipOperation(object o)
        {
            if ((this.DesignerHost != null) && o.Equals(this.DesignerHost.RootComponent))
            {
                return true;
            }
            if (typeof(Control).IsAssignableFrom(o.GetType()))
            {
                Control control = (Control)o;
                if ((typeof(ToolStripContentPanel).IsAssignableFrom(o.GetType()) || typeof(ToolStripPanel).IsAssignableFrom(o.GetType())) && ((control.Parent != null) && typeof(ToolStripContainer).IsAssignableFrom(control.Parent.GetType())))
                {
                    return true;
                }
                if ((typeof(TabPage).IsAssignableFrom(o.GetType()) && (control.Parent != null)) && typeof(TabControl).IsAssignableFrom(control.Parent.GetType()))
                {
                    return true;
                }
                if ((typeof(SplitterPanel).IsAssignableFrom(o.GetType()) && (control.Parent != null)) && typeof(SplitContainer).IsAssignableFrom(control.Parent.GetType()))
                {
                    return true;
                }
                if (typeof(ShapeContainer).IsAssignableFrom(o.GetType()))
                {
                    return true;
                }
            }
            return false;
        }

        protected void OnMenuAlignByPrimary(object sender, EventArgs e)
        {
            CommandID did = this.TryInvokeOldCommand(RuntimeHelpers.GetObjectValue(sender));
            if (did != null)
            {
                Point location = this.GetLocation(this.m_PrimarySelection);
                Size size = this.GetSize(this.m_PrimarySelection);
                if (this.SelectionService != null)
                {
                    Cursor current = Cursor.Current;
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                        IDesignerHost designerHost = this.DesignerHost;
                        DesignerTransaction transaction = null;
                        try
                        {
                            IEnumerator enumerator = null;
                            if (designerHost != null)
                            {
                                transaction = designerHost.CreateTransaction($"Format {selectedComponents.Count} shapes (alignment)");
                            }
                            bool flag = true;
                            Point empty = Point.Empty;
                            try
                            {
                                enumerator = selectedComponents.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                    if (!object.ReferenceEquals(RuntimeHelpers.GetObjectValue(objectValue), this.m_PrimarySelection))
                                    {
                                        IComponent comp = (IComponent)objectValue;
                                        PropertyDescriptor property = this.GetProperty(comp, "Location");
                                        PropertyDescriptor descriptor3 = this.GetProperty(comp, "Size");
                                        PropertyDescriptor descriptor = this.GetProperty(comp, "Locked");
                                        if ((((descriptor == null) || !Convert.ToBoolean(descriptor.GetValue(comp))) && !((property == null) | property.IsReadOnly)) && (((!did.Equals(StandardCommands.AlignBottom) && !did.Equals(StandardCommands.AlignRight)) && (!did.Equals(StandardCommands.AlignHorizontalCenters) && !did.Equals(StandardCommands.AlignVerticalCenters))) || !((descriptor3 == null) | descriptor3.IsReadOnly)))
                                        {
                                            if (did.Equals(StandardCommands.AlignBottom))
                                            {
                                                empty = (Point)property.GetValue(comp);
                                                Size size2 = (Size)descriptor3.GetValue(comp);
                                                empty.Y = (location.Y + size.Height) - size2.Height;
                                            }
                                            else if (did.Equals(StandardCommands.AlignHorizontalCenters))
                                            {
                                                empty = (Point)property.GetValue(comp);
                                                Size size3 = (Size)descriptor3.GetValue(comp);
                                                empty.Y = ((size.Height / 2) + location.Y) - (size3.Height / 2);
                                            }
                                            else if (did.Equals(StandardCommands.AlignLeft))
                                            {
                                                empty = (Point)property.GetValue(comp);
                                                empty.X = location.X;
                                            }
                                            else if (did.Equals(StandardCommands.AlignRight))
                                            {
                                                empty = (Point)property.GetValue(comp);
                                                Size size4 = (Size)descriptor3.GetValue(comp);
                                                empty.X = (location.X + size.Width) - size4.Width;
                                            }
                                            else if (did.Equals(StandardCommands.AlignTop))
                                            {
                                                empty = (Point)property.GetValue(comp);
                                                empty.Y = location.Y;
                                            }
                                            else if (did.Equals(StandardCommands.AlignVerticalCenters))
                                            {
                                                empty = (Point)property.GetValue(comp);
                                                Size size5 = (Size)descriptor3.GetValue(comp);
                                                empty.X = ((size.Width / 2) + location.X) - (size5.Width / 2);
                                            }
                                            if (flag & !this.CanCheckout(comp))
                                            {
                                                return;
                                            }
                                            flag = false;
                                            property.SetValue(comp, empty);
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
                        }
                        catch (Exception exception1)
                        {
                            //ProjectData.SetProjectError(exception1);
                            Exception exception = exception1;
                            if (transaction != null)
                            {
                                transaction.Cancel();
                                ((IDisposable)transaction).Dispose();
                                transaction = null;
                            }
                            //ProjectData.ClearProjectError();
                        }
                        finally
                        {
                            if (transaction != null)
                            {
                                transaction.Commit();
                                ((IDisposable)transaction).Dispose();
                                transaction = null;
                            }
                        }
                    }
                    finally
                    {
                        Cursor.Current = current;
                    }
                }
            }
        }

        protected void OnMenuAlignToGrid(object sender, EventArgs e)
        {
            if (this.TryInvokeOldCommand(RuntimeHelpers.GetObjectValue(sender)) != null)
            {
                Size empty = Size.Empty;
                PropertyDescriptor descriptor2 = null;
                PropertyDescriptor descriptor = null;
                Point point = Point.Empty;
                if (this.SelectionService != null)
                {
                    Cursor current = Cursor.Current;
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                        IDesignerHost designerHost = this.DesignerHost;
                        DesignerTransaction transaction = null;
                        try
                        {
                            IEnumerator enumerator = null;
                            if (designerHost != null)
                            {
                                transaction = designerHost.CreateTransaction($"Align {selectedComponents.Count} shape(s) to grid");
                                Control rootComponent = (Control)designerHost.RootComponent;
                                if (rootComponent != null)
                                {
                                    PropertyDescriptor property = this.GetProperty(rootComponent, "GridSize");
                                    if (property != null)
                                    {
                                        empty = (Size)property.GetValue(rootComponent);
                                    }
                                    if ((property == null) || empty.IsEmpty)
                                    {
                                        return;
                                    }
                                }
                            }
                            bool flag = true;
                            try
                            {
                                enumerator = selectedComponents.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                    descriptor = this.GetProperty(RuntimeHelpers.GetObjectValue(objectValue), "Locked");
                                    if ((descriptor == null) || !Convert.ToBoolean(descriptor.GetValue(RuntimeHelpers.GetObjectValue(objectValue))))
                                    {
                                        IComponent component = (IComponent)objectValue;
                                        if (((component == null) || (designerHost == null)) || (((ShapeDesigner)designerHost.GetDesigner(component)) != null))
                                        {
                                            descriptor2 = this.GetProperty(RuntimeHelpers.GetObjectValue(objectValue), "Location");
                                            if ((descriptor2 != null) && !descriptor2.IsReadOnly)
                                            {
                                                point = (Point)descriptor2.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                                                int num = Convert.ToInt32(decimal.Remainder(new decimal(point.X), new decimal(empty.Width)));
                                                if (num < (((double)empty.Width) / 2.0))
                                                {
                                                    point.X -= num;
                                                }
                                                else
                                                {
                                                    point.X += empty.Width - num;
                                                }
                                                num = Convert.ToInt32(decimal.Remainder(new decimal(point.Y), new decimal(empty.Height)));
                                                if (num < (((double)empty.Height) / 2.0))
                                                {
                                                    point.Y -= num;
                                                }
                                                else
                                                {
                                                    point.Y += empty.Height - num;
                                                }
                                                if (flag & !this.CanCheckout(component))
                                                {
                                                    return;
                                                }
                                                flag = false;
                                                descriptor2.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point);
                                            }
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
                        }
                        catch (Exception exception1)
                        {
                            //ProjectData.SetProjectError(exception1);
                            Exception exception = exception1;
                            if (transaction != null)
                            {
                                transaction.Cancel();
                                ((IDisposable)transaction).Dispose();
                                transaction = null;
                            }
                            //ProjectData.ClearProjectError();
                        }
                        finally
                        {
                            if (transaction != null)
                            {
                                transaction.Commit();
                                ((IDisposable)transaction).Dispose();
                                transaction = null;
                            }
                        }
                    }
                    finally
                    {
                        Cursor.Current = current;
                    }
                }
            }
        }

        public void OnMenuBringToFront(object sender, EventArgs e)
        {
            if (((!this.InvokeStdMenuCmd(StandardCommands.BringToFront, false) && (this.DesignerHost != null)) && (this.SelectionService != null)) && (this.ChangeService != null))
            {
                Cursor current = Cursor.Current;
                DesignerTransaction transaction = null;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                    if ((selectedComponents != null) && (selectedComponents.Count != 0))
                    {
                        object[] array = new object[(selectedComponents.Count - 1) + 1];
                        selectedComponents.CopyTo(array, 0);
                        if (typeof(Shape).IsAssignableFrom(array[0].GetType()))
                        {
                            List<ShapeContainer>.Enumerator enumerator = new List<ShapeContainer>.Enumerator();
                            this.SortSelection(array, SortBy.ZOrder);
                            transaction = this.DesignerHost.CreateTransaction("Bring Controls to front");
                            List<ShapeContainer> list = new List<ShapeContainer>();
                            int num2 = array.Length - 1;
                            for (int i = 0; i <= num2; i++)
                            {
                                if (typeof(Shape).IsAssignableFrom(array[i].GetType()))
                                {
                                    Shape shape = (Shape)array[i];
                                    if ((shape.Parent != null) && typeof(ShapeContainer).IsAssignableFrom(shape.Parent.GetType()))
                                    {
                                        ShapeContainer parent = shape.Parent;
                                        if (parent.Shapes.IndexOf(shape) >= 0)
                                        {
                                            if (!list.Contains(parent))
                                            {
                                                this.ChangeService.OnComponentChanging(parent, null);
                                                list.Add(parent);
                                            }
                                            shape.BringToFront();
                                        }
                                    }
                                }
                            }
                            try
                            {
                                enumerator = list.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    ShapeContainer component = enumerator.Current;
                                    this.ChangeService.OnComponentChanged(component, null, null, null);
                                }
                            }
                            finally
                            {
                                enumerator.Dispose();
                            }
                        }
                    }
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    if (transaction != null)
                    {
                        transaction.Cancel();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    Cursor.Current = current;
                }
            }
        }

        protected void OnMenuCenterSelection(object sender, EventArgs e)
        {
            CommandID did = this.TryInvokeOldCommand(RuntimeHelpers.GetObjectValue(sender));
            if ((did != null) && (this.SelectionService != null))
            {
                Cursor current = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                    Control parent = null;
                    Size empty = Size.Empty;
                    Point point = Point.Empty;
                    IDesignerHost designerHost = this.DesignerHost;
                    DesignerTransaction transaction = null;
                    try
                    {
                        object objectValue;
                        IEnumerator enumerator = null;
                        if (designerHost != null)
                        {
                            string str;
                            if (did.Equals(StandardCommands.CenterHorizontally))
                            {
                                str = $"Horizontal center of {selectedComponents.Count} shape(s)";
                            }
                            else
                            {
                                str = $"Vertical center of {selectedComponents.Count} shape(s)";
                            }
                            transaction = designerHost.CreateTransaction(str);
                        }
                        int y = 0x7fffffff;
                        int x = 0x7fffffff;
                        int num9 = -2147483648;
                        int num = -2147483648;
                        try
                        {
                            enumerator = selectedComponents.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                if (objectValue is Shape)
                                {
                                    IComponent comp = (IComponent)objectValue;
                                    PropertyDescriptor property = this.GetProperty(comp, "Location");
                                    PropertyDescriptor descriptor3 = this.GetProperty(comp, "Size");
                                    if (((property != null) && (descriptor3 != null)) && (!property.IsReadOnly && !descriptor3.IsReadOnly))
                                    {
                                        PropertyDescriptor descriptor = this.GetProperty(comp, "Locked");
                                        if ((descriptor == null) || !Convert.ToBoolean(descriptor.GetValue(comp)))
                                        {
                                            empty = (Size)descriptor3.GetValue(comp);
                                            point = (Point)property.GetValue(comp);
                                            if (parent == null)
                                            {
                                                parent = ((Shape)comp).Parent;
                                            }
                                            if (point.X < x)
                                            {
                                                x = point.X;
                                            }
                                            if (point.Y < y)
                                            {
                                                y = point.Y;
                                            }
                                            if ((point.X + empty.Width) > num9)
                                            {
                                                num9 = point.X + empty.Width;
                                            }
                                            if ((point.Y + empty.Height) > num)
                                            {
                                                num = point.Y + empty.Height;
                                            }
                                        }
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
                        if (parent != null)
                        {
                            IEnumerator enumerator2 = null;
                            int num4 = (int)Math.Round((double)(((double)(x + num9)) / 2.0));
                            int num5 = (int)Math.Round((double)(((double)(y + num)) / 2.0));
                            int num2 = (int)Math.Round((double)(((double)parent.ClientSize.Width) / 2.0));
                            int num3 = (int)Math.Round((double)(((double)parent.ClientSize.Height) / 2.0));
                            int num6 = 0;
                            int num7 = 0;
                            bool flag3 = false;
                            bool flag2 = false;
                            if (num2 >= num4)
                            {
                                num6 = num2 - num4;
                                flag3 = true;
                            }
                            else
                            {
                                num6 = num4 - num2;
                            }
                            if (num3 >= num5)
                            {
                                num7 = num3 - num5;
                                flag2 = true;
                            }
                            else
                            {
                                num7 = num5 - num3;
                            }
                            bool flag = true;
                            try
                            {
                                enumerator2 = selectedComponents.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    objectValue = RuntimeHelpers.GetObjectValue(enumerator2.Current);
                                    if (objectValue is Shape)
                                    {
                                        IComponent component = (IComponent)objectValue;
                                        PropertyDescriptor descriptor4 = TypeDescriptor.GetProperties(component)["Location"];
                                        if (!descriptor4.IsReadOnly)
                                        {
                                            point = (Point)descriptor4.GetValue(component);
                                            if (did.Equals(StandardCommands.CenterHorizontally))
                                            {
                                                if (flag3)
                                                {
                                                    point.X += num6;
                                                }
                                                else
                                                {
                                                    point.X -= num6;
                                                }
                                            }
                                            else if (did.Equals(StandardCommands.CenterVertically))
                                            {
                                                if (flag2)
                                                {
                                                    point.Y += num7;
                                                }
                                                else
                                                {
                                                    point.Y -= num7;
                                                }
                                            }
                                            if (flag && !this.CanCheckout(component))
                                            {
                                                return;
                                            }
                                            flag = false;
                                            descriptor4.SetValue(component, point);
                                        }
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
                    catch (Exception exception1)
                    {
                        //ProjectData.SetProjectError(exception1);
                        Exception exception = exception1;
                        if (transaction != null)
                        {
                            transaction.Cancel();
                            ((IDisposable)transaction).Dispose();
                            transaction = null;
                        }
                        //ProjectData.ClearProjectError();
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                            ((IDisposable)transaction).Dispose();
                            transaction = null;
                        }
                    }
                }
                finally
                {
                    Cursor.Current = current;
                }
            }
        }

        public void OnMenuCopy(object sender, EventArgs e)
        {
            if (((!this.InvokeStdMenuCmd(StandardCommands.Copy, true) && (this.DesignerHost != null)) && (this.SelectionService != null)) && (this.ChangeService != null))
            {
                Cursor current = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ICollection copySelection = this.GetCopySelection();
                    if (copySelection != null)
                    {
                        IDesignerSerializationService designerSerializationService = this.DesignerSerializationService;
                        if (designerSerializationService != null)
                        {
                            object objectValue = RuntimeHelpers.GetObjectValue(designerSerializationService.Serialize(copySelection));
                            MemoryStream serializationStream = new MemoryStream();
                            new BinaryFormatter().Serialize(serializationStream, RuntimeHelpers.GetObjectValue(objectValue));
                            serializationStream.Seek(0L, SeekOrigin.Begin);
                            byte[] data = serializationStream.GetBuffer();
                            IDataObject obj2 = new DataObject("SHAPECONTAINER_DESIGNERCOMPONENTS", data);
                            Clipboard.SetDataObject(obj2);
                        }
                    }
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    Cursor.Current = current;
                }
            }
        }

        public void OnMenuCut(object sender, EventArgs e)
        {
            if (!this.InvokeStdMenuCmd(StandardCommands.Cut, true))
            {
                IDesignerHost designerHost = this.DesignerHost;
                if (designerHost != null)
                {
                    ISelectionService selectionService = this.SelectionService;
                    if (selectionService != null)
                    {
                        IComponentChangeService changeService = this.ChangeService;
                        if (changeService != null)
                        {
                            DesignerTransaction transaction = null;
                            Cursor current = Cursor.Current;
                            try
                            {
                                this.InOperation = true;
                                Cursor.Current = Cursors.WaitCursor;
                                transaction = designerHost.CreateTransaction("Cut components");
                                ICollection copySelection = this.GetCopySelection();
                                if (copySelection != null)
                                {
                                    IEnumerator enumerator = null;
                                    IDesignerSerializationService designerSerializationService = this.DesignerSerializationService;
                                    if (designerSerializationService != null)
                                    {
                                        object objectValue = RuntimeHelpers.GetObjectValue(designerSerializationService.Serialize(copySelection));
                                        MemoryStream serializationStream = new MemoryStream();
                                        new BinaryFormatter().Serialize(serializationStream, RuntimeHelpers.GetObjectValue(objectValue));
                                        serializationStream.Seek(0L, SeekOrigin.Begin);
                                        byte[] data = serializationStream.GetBuffer();
                                        IDataObject obj2 = new DataObject("SHAPECONTAINER_DESIGNERCOMPONENTS", data);
                                        Clipboard.SetDataObject(obj2);
                                    }
                                    List<ShapeContainer> list = new List<ShapeContainer>();
                                    try
                                    {
                                        enumerator = copySelection.GetEnumerator();
                                        while (enumerator.MoveNext())
                                        {
                                            object obj4 = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                            if (typeof(Shape).IsAssignableFrom(obj4.GetType()))
                                            {
                                                Shape shape = (Shape)obj4;
                                                ShapeContainer parent = shape.Parent;
                                                if (parent != null)
                                                {
                                                    changeService.OnComponentChanging(parent, null);
                                                    parent.Shapes.Remove(shape);
                                                    designerHost.DestroyComponent(shape);
                                                    if (!list.Contains(parent))
                                                    {
                                                        list.Add(parent);
                                                    }
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
                                    IComponent rootComponent = null;
                                    if (list.Count > 0)
                                    {
                                        ShapeContainer container2 = list[list.Count - 1];
                                        if (container2.Shapes.Count == 0)
                                        {
                                            rootComponent = container2.Parent;
                                        }
                                        else
                                        {
                                            rootComponent = (IComponent)container2.Shapes[0];
                                        }
                                    }
                                    else
                                    {
                                        rootComponent = designerHost.RootComponent;
                                    }
                                    foreach (ShapeContainer container3 in list)
                                    {
                                        changeService.OnComponentChanged(container3, null, null, null);
                                        if (container3.Shapes.Count == 0)
                                        {
                                            designerHost.DestroyComponent(container3);
                                        }
                                    }
                                    selectionService.SetSelectedComponents(new IComponent[] { rootComponent }, SelectionTypes.Replace);
                                }
                            }
                            catch (CheckoutException exception1)
                            {
                                //ProjectData.SetProjectError(exception1);
                                CheckoutException exception = exception1;
                                if (transaction != null)
                                {
                                    transaction.Cancel();
                                    ((IDisposable)transaction).Dispose();
                                }
                                if (exception == CheckoutException.Canceled)
                                {
                                }
                                //ProjectData.ClearProjectError();
                            }
                            catch (Exception exception3)
                            {
                                //ProjectData.SetProjectError(exception3);
                                Exception exception2 = exception3;
                                if (transaction != null)
                                {
                                    transaction.Cancel();
                                    ((IDisposable)transaction).Dispose();
                                }
                                //ProjectData.ClearProjectError();
                            }
                            finally
                            {
                                if (transaction != null)
                                {
                                    transaction.Commit();
                                    ((IDisposable)transaction).Dispose();
                                }
                                Cursor.Current = current;
                                this.InOperation = false;
                                if (this.ShouldDispose)
                                {
                                    this.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void OnMenuDelete(object sender, EventArgs e)
        {
            Cursor cursor;
            ISelectionService selectionService = this.SelectionService;
            if (selectionService == null)
            {
                return;
            }
            bool flag = true;
            ICollection selectedComponents = selectionService.GetSelectedComponents();
            if ((selectedComponents != null) && (selectedComponents.Count > 0))
            {
                IEnumerator enumerator = null;
                try
                {
                    enumerator = selectedComponents.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                        if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                        {
                            flag = false;
                            goto Label_0089;
                        }
                        if (this.NeedSkipOperation(RuntimeHelpers.GetObjectValue(objectValue)))
                        {
                            return;
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
        Label_0089:
            cursor = Cursor.Current;
            DesignerTransaction transaction = null;
            try
            {
                this.InOperation = true;
                Cursor.Current = Cursors.WaitCursor;
                IDesignerHost designerHost = this.DesignerHost;
                if (designerHost != null)
                {
                    IComponentChangeService changeService = this.ChangeService;
                    if (changeService != null)
                    {
                        List<ShapeContainer> shConCol = new List<ShapeContainer>();
                        if (flag)
                        {
                            GetChildShapeContainers(selectedComponents, ref shConCol);
                            if (shConCol.Count > 0)
                            {
                                transaction = designerHost.CreateTransaction("Delete components");
                                for (int i = shConCol.Count; i >= 1; i += -1)
                                {
                                    ShapeContainer component = shConCol[i - 1];
                                    changeService.OnComponentChanging(component, null);
                                    for (int j = component.Shapes.Count; j >= 1; j += -1)
                                    {
                                        Shape shape = (Shape)component.Shapes[j - 1];
                                        component.Shapes.Remove(shape);
                                        designerHost.DestroyComponent(shape);
                                    }
                                    changeService.OnComponentChanged(component, null, null, null);
                                    Control parent = component.Parent;
                                    changeService.OnComponentChanging(parent, null);
                                    parent.Controls.Remove(component);
                                    changeService.OnComponentChanged(parent, null, null, null);
                                    designerHost.DestroyComponent(component);
                                }
                                shConCol.Clear();
                                selectionService.SetSelectedComponents(selectedComponents, SelectionTypes.Replace);
                            }
                            MenuCommand command = this.m_StdMenuCommands[StandardCommands.Delete];
                            if (command != null)
                            {
                                command.Invoke();
                            }
                        }
                        else
                        {
                            IEnumerator enumerator2 = null;
                            transaction = designerHost.CreateTransaction("Delete components");
                            int index = 0;
                            ShapeContainer item = null;
                            this.Helper.SelectionManager.HideAdorner();
                            try
                            {
                                enumerator2 = selectionService.GetSelectedComponents().GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    object obj3 = RuntimeHelpers.GetObjectValue(enumerator2.Current);
                                    if (typeof(Shape).IsAssignableFrom(obj3.GetType()))
                                    {
                                        Shape shape2 = (Shape)obj3;
                                        item = shape2.Parent;
                                        if (item != null)
                                        {
                                            index = item.Shapes.IndexOf(shape2);
                                            if (!shConCol.Contains(item))
                                            {
                                                changeService.OnComponentChanging(item, null);
                                                shConCol.Add(item);
                                            }
                                            shape2.Invalidate();
                                            item.Shapes.Remove(shape2);
                                            designerHost.DestroyComponent(shape2);
                                        }
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
                            Control control = null;
                            foreach (ShapeContainer container3 in shConCol)
                            {
                                changeService.OnComponentChanged(container3, null, null, null);
                                if (container3.Shapes.Count == 0)
                                {
                                    if (container3.Equals(item))
                                    {
                                        control = container3.Parent;
                                        if (control != null)
                                        {
                                            index = control.Controls.IndexOf(container3);
                                        }
                                    }
                                    designerHost.DestroyComponent(container3);
                                }
                            }
                            if (control != null)
                            {
                                if (control.Controls.Count == 0)
                                {
                                    selectionService.SetSelectedComponents(new IComponent[] { control });
                                }
                                else
                                {
                                    if (index >= control.Controls.Count)
                                    {
                                        index = 0;
                                    }
                                    selectionService.SetSelectedComponents(new IComponent[] { control.Controls[index] });
                                }
                            }
                            else if (item != null)
                            {
                                if (index >= item.Shapes.Count)
                                {
                                    index = 0;
                                }
                                selectionService.SetSelectedComponents(new IComponent[] { (IComponent)item.Shapes[index] });
                            }
                        }
                    }
                }
            }
            catch (Exception exception1)
            {
                //ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                if (transaction != null)
                {
                    transaction.Cancel();
                    ((IDisposable)transaction).Dispose();
                }
                //ProjectData.ClearProjectError();
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Commit();
                    ((IDisposable)transaction).Dispose();
                }
                Cursor.Current = cursor;
                this.InOperation = false;
                if (this.ShouldDispose)
                {
                    this.Dispose();
                }
            }
        }

        public void OnMenuKeyCancel(object sender, EventArgs e)
        {
            if (this.DragShapeConDesigner != null)
            {
                this.DragShapeConDesigner.CancelMouseDrag();
            }
            this.StdMenuCommands[MenuCommands.KeyCancel].Invoke();
        }

        public void OnMenuKeyDefault(object sender, EventArgs v)
        {
            ISelectionService selectionService = this.SelectionService;
            if (selectionService != null)
            {
                object objectValue = RuntimeHelpers.GetObjectValue(selectionService.PrimarySelection);
                if ((objectValue != null) && !typeof(ShapeContainer).IsAssignableFrom(objectValue.GetType()))
                {
                    IDesignerHost designerHost = this.DesignerHost;
                    if (designerHost != null)
                    {
                        IDesigner designer = null;
                        if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                        {
                            Control parent = ((Shape)objectValue).Parent;
                            designer = designerHost.GetDesigner(parent);
                            if (!typeof(ShapeContainerDesigner).IsAssignableFrom(designer.GetType()))
                            {
                                designer = null;
                            }
                        }
                        else
                        {
                            designer = designerHost.GetDesigner((IComponent)objectValue);
                        }
                        if (designer != null)
                        {
                            designer.DoDefaultAction();
                        }
                    }
                }
            }
        }

        public void OnMenuKeyMove(object sender, EventArgs v)
        {
            CommandID commandID = ((MenuCommand)sender).CommandID;
            if (!this.InvokeStdMenuCmd(commandID, true))
            {
                IDesignerHost designerHost = this.DesignerHost;
                if (designerHost != null)
                {
                    ISelectionService selectionService = this.SelectionService;
                    if (selectionService != null)
                    {
                        Shape primarySelection = (Shape)selectionService.PrimarySelection;
                        if (primarySelection != null)
                        {
                            DesignerTransaction transaction = null;
                            Cursor current = Cursor.Current;
                            try
                            {
                                IEnumerator enumerator = null;
                                Cursor.Current = Cursors.WaitCursor;
                                if (selectionService.SelectionCount > 1)
                                {
                                    transaction = designerHost.CreateTransaction($"Move {selectionService.SelectionCount} shapes.");
                                }
                                else
                                {
                                    transaction = designerHost.CreateTransaction($"Move {primarySelection.Site.Name}");
                                }
                                bool flag = false;
                                int x = 0;
                                int y = 0;
                                if (commandID.Equals(MenuCommands.KeyMoveDown))
                                {
                                    y = 1;
                                }
                                else if (commandID.Equals(MenuCommands.KeyMoveLeft))
                                {
                                    x = -1;
                                }
                                else if (commandID.Equals(MenuCommands.KeyMoveRight))
                                {
                                    x = 1;
                                }
                                else if (commandID.Equals(MenuCommands.KeyMoveUp))
                                {
                                    y = -1;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeDown))
                                {
                                    y = 1;
                                    flag = true;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeLeft))
                                {
                                    x = -1;
                                    flag = true;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeRight))
                                {
                                    x = 1;
                                    flag = true;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeUp))
                                {
                                    y = -1;
                                    flag = true;
                                }
                                Point p = new Point(x, y);
                                if (flag)
                                {
                                    Size gridSize = DesignerUtility.GetGridSize(designerHost);
                                    if (!gridSize.IsEmpty)
                                    {
                                        p = new Point(x * gridSize.Width, y * gridSize.Height);
                                    }
                                }
                                try
                                {
                                    enumerator = selectionService.GetSelectedComponents().GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                        if (typeof(SparkLine).IsAssignableFrom(objectValue.GetType()))
                                        {
                                            PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(objectValue))["StartPoint"];
                                            if (descriptor2 != null)
                                            {
                                                Point point2 = (Point)descriptor2.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                                                point2.Offset(p);
                                                descriptor2.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point2);
                                            }
                                            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(objectValue))["EndPoint"];
                                            if (descriptor != null)
                                            {
                                                Point point3 = (Point)descriptor.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                                                point3.Offset(p);
                                                descriptor.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point3);
                                            }
                                        }
                                        //levy 去掉SimpleShape的分支
                                        //else if (typeof(SimpleShape).IsAssignableFrom(objectValue.GetType()))
                                        //{
                                        //    PropertyDescriptor descriptor3 = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(objectValue))["Location"];
                                        //    if (descriptor3 != null)
                                        //    {
                                        //        Point point4 = (Point)descriptor3.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                                        //        point4.Offset(p);
                                        //        descriptor3.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point4);
                                        //    }
                                        //}
                                        if (primarySelection.Equals(RuntimeHelpers.GetObjectValue(objectValue)))
                                        {
                                            this.StatusBarCommand.SetStatusBarInfo(primarySelection);
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
                            finally
                            {
                                if (transaction != null)
                                {
                                    transaction.Commit();
                                    ((IDisposable)transaction).Dispose();
                                }
                                Cursor.Current = current;
                            }
                        }
                    }
                }
            }
        }

        public void OnMenuKeyResize(object sender, EventArgs v)
        {
            CommandID commandID = ((MenuCommand)sender).CommandID;
            if (!this.InvokeStdMenuCmd(commandID, true))
            {
                IDesignerHost designerHost = this.DesignerHost;
                if (designerHost != null)
                {
                    ISelectionService selectionService = this.SelectionService;
                    if (selectionService != null)
                    {
                        Shape primarySelection = (Shape)selectionService.PrimarySelection;
                        if (primarySelection != null)
                        {
                            DesignerTransaction transaction = null;
                            Cursor current = Cursor.Current;
                            try
                            {
                                IEnumerator enumerator = null;
                                Cursor.Current = Cursors.WaitCursor;
                                if (selectionService.SelectionCount > 1)
                                {
                                    transaction = designerHost.CreateTransaction($"Resize {selectionService.SelectionCount} shapes.");
                                }
                                else
                                {
                                    transaction = designerHost.CreateTransaction($"Resize {primarySelection.Site.Name}");
                                }
                                bool flag2 = false;
                                int num = 0;
                                int num2 = 0;
                                if (commandID.Equals(MenuCommands.KeySizeHeightDecrease))
                                {
                                    num = -1;
                                }
                                else if (commandID.Equals(MenuCommands.KeySizeHeightIncrease))
                                {
                                    num = 1;
                                }
                                else if (commandID.Equals(MenuCommands.KeySizeWidthDecrease))
                                {
                                    num2 = -1;
                                }
                                else if (commandID.Equals(MenuCommands.KeySizeWidthIncrease))
                                {
                                    num2 = 1;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeHeightDecrease))
                                {
                                    num = -1;
                                    flag2 = true;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeHeightIncrease))
                                {
                                    num = 1;
                                    flag2 = true;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeWidthDecrease))
                                {
                                    num2 = -1;
                                    flag2 = true;
                                }
                                else if (commandID.Equals(MenuCommands.KeyNudgeWidthIncrease))
                                {
                                    num2 = 1;
                                    flag2 = true;
                                }
                                if (flag2)
                                {
                                    Size gridSize = DesignerUtility.GetGridSize(designerHost);
                                    if (!gridSize.IsEmpty)
                                    {
                                        num2 *= gridSize.Width;
                                        num *= gridSize.Height;
                                    }
                                }
                                bool flag = false;
                                try
                                {
                                    enumerator = selectionService.GetSelectedComponents().GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                                        if (typeof(SparkLine).IsAssignableFrom(objectValue.GetType()))
                                        {
                                            Point empty = Point.Empty;
                                            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(objectValue))["EndPoint"];
                                            if (descriptor != null)
                                            {
                                                empty = (Point)descriptor.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                                                Point point2 = Point.Empty;
                                                PropertyDescriptor descriptor2 = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(objectValue))["StartPoint"];
                                                if (descriptor2 == null)
                                                {
                                                    continue;
                                                }
                                                point2 = (Point)descriptor2.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                                                if (num2 == 0)
                                                {
                                                    if (empty.Y > point2.Y)
                                                    {
                                                        empty.Y = Math.Max(empty.Y + num, point2.Y);
                                                        descriptor.SetValue(RuntimeHelpers.GetObjectValue(objectValue), empty);
                                                        flag = true;
                                                    }
                                                    else if (empty.Y < point2.Y)
                                                    {
                                                        point2.Y = Math.Max(point2.Y + num, empty.Y);
                                                        descriptor2.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point2);
                                                        flag = true;
                                                    }
                                                    else if (num > 0)
                                                    {
                                                        if (point2.X > empty.X)
                                                        {
                                                            point2.Y += num;
                                                            descriptor2.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point2);
                                                        }
                                                        else
                                                        {
                                                            empty.Y += num;
                                                            descriptor.SetValue(RuntimeHelpers.GetObjectValue(objectValue), empty);
                                                        }
                                                        flag = true;
                                                    }
                                                }
                                                else if (empty.X > point2.X)
                                                {
                                                    empty.X = Math.Max(empty.X + num2, point2.X);
                                                    descriptor.SetValue(RuntimeHelpers.GetObjectValue(objectValue), empty);
                                                    flag = true;
                                                }
                                                else if (empty.X < point2.X)
                                                {
                                                    point2.X = Math.Max(point2.X + num2, empty.X);
                                                    descriptor2.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point2);
                                                    flag = true;
                                                }
                                                else if (num2 > 0)
                                                {
                                                    if (point2.Y > empty.Y)
                                                    {
                                                        point2.X += num2;
                                                        descriptor2.SetValue(RuntimeHelpers.GetObjectValue(objectValue), point2);
                                                    }
                                                    else
                                                    {
                                                        empty.X += num2;
                                                        descriptor.SetValue(RuntimeHelpers.GetObjectValue(objectValue), empty);
                                                    }
                                                    flag = true;
                                                }
                                            }
                                        }
                                        //levy 去掉SimpleShape的分支
                                        //else if (typeof(SimpleShape).IsAssignableFrom(objectValue.GetType()))
                                        //{
                                        //    PropertyDescriptor descriptor3 = TypeDescriptor.GetProperties(RuntimeHelpers.GetObjectValue(objectValue))["Size"];
                                        //    if (descriptor3 != null)
                                        //    {
                                        //        Size size2 = (Size)descriptor3.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                                        //        int num4 = Math.Max(size2.Width + num2, 0);
                                        //        int num3 = Math.Max(size2.Height + num, 0);
                                        //        if ((num4 != size2.Width) || (num3 != size2.Height))
                                        //        {
                                        //            size2.Width = num4;
                                        //            size2.Height = num3;
                                        //            descriptor3.SetValue(RuntimeHelpers.GetObjectValue(objectValue), size2);
                                        //            flag = true;
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                                finally
                                {
                                    if (enumerator is IDisposable)
                                    {
                                        (enumerator as IDisposable).Dispose();
                                    }
                                }
                                if (flag)
                                {
                                    this.StatusBarCommand.SetStatusBarInfo(primarySelection);
                                }
                            }
                            finally
                            {
                                if (transaction != null)
                                {
                                    transaction.Commit();
                                    ((IDisposable)transaction).Dispose();
                                }
                                Cursor.Current = current;
                            }
                        }
                    }
                }
            }
        }

        public void OnMenuSelectAll(object sender, EventArgs e)
        {
            if ((this.DesignerHost != null) && (this.SelectionService != null))
            {
                Cursor current = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    IDesignerHost designerHost = this.DesignerHost;
                    ISelectionService selectionService = this.SelectionService;
                    ComponentCollection components = designerHost.Container.Components;
                    IComponent[] array = null;
                    if ((components == null) || (components.Count == 0))
                    {
                        array = new IComponent[0];
                    }
                    else
                    {
                        array = new IComponent[(components.Count - 1) + 1];
                        IComponent rootComponent = designerHost.RootComponent;
                        object objectValue = RuntimeHelpers.GetObjectValue(selectionService.PrimarySelection);
                        if ((objectValue != null) && typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                        {
                            IEnumerator enumerator = null;
                            int index = 0;
                            try
                            {
                                enumerator = components.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    IComponent component2 = (IComponent)enumerator.Current;
                                    if (component2 is Shape)
                                    {
                                        array[index] = component2;
                                        index++;
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
                            Array.Resize<IComponent>(ref array, index);
                        }
                        else
                        {
                            IEnumerator enumerator2 = null;
                            int num2 = 0;
                            try
                            {
                                enumerator2 = components.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    IComponent component3 = (IComponent)enumerator2.Current;
                                    object obj3 = component3;
                                    if (((obj3 != null) && !typeof(Shape).IsAssignableFrom(obj3.GetType())) && (!typeof(ShapeContainer).IsAssignableFrom(obj3.GetType()) && !obj3.Equals(rootComponent)))
                                    {
                                        array[num2] = component3;
                                        num2++;
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
                            Array.Resize<IComponent>(ref array, num2);
                        }
                    }
                    if (array.Length > 0)
                    {
                        selectionService.SetSelectedComponents(array, SelectionTypes.Replace);
                    }
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    Cursor.Current = current;
                }
            }
        }

        public void OnMenuSendToBack(object sender, EventArgs e)
        {
            if ((!this.InvokeStdMenuCmd(StandardCommands.SendToBack, false) && (this.DesignerHost != null)) && (this.SelectionService != null))
            {
                Cursor current = Cursor.Current;
                DesignerTransaction transaction = null;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                    if ((selectedComponents != null) && (selectedComponents.Count != 0))
                    {
                        object[] array = new object[(selectedComponents.Count - 1) + 1];
                        selectedComponents.CopyTo(array, 0);
                        if (typeof(Shape).IsAssignableFrom(array[0].GetType()))
                        {
                            List<ShapeContainer>.Enumerator enumerator = new List<ShapeContainer>.Enumerator();
                            this.SortSelection(array, SortBy.ZOrder);
                            transaction = this.DesignerHost.CreateTransaction("Bring Controls to front");
                            List<ShapeContainer> list = new List<ShapeContainer>();
                            for (int i = array.Length - 1; i >= 0; i += -1)
                            {
                                if (typeof(Shape).IsAssignableFrom(array[i].GetType()))
                                {
                                    Shape shape = (Shape)array[i];
                                    if ((shape.Parent != null) && typeof(ShapeContainer).IsAssignableFrom(shape.Parent.GetType()))
                                    {
                                        ShapeContainer parent = shape.Parent;
                                        int index = parent.Shapes.IndexOf(shape);
                                        if ((index >= 0) && (index < (parent.Shapes.Count - 1)))
                                        {
                                            if (!list.Contains(parent))
                                            {
                                                this.ChangeService.OnComponentChanging(parent, null);
                                                list.Add(parent);
                                            }
                                            shape.SendToBack();
                                        }
                                    }
                                }
                            }
                            try
                            {
                                enumerator = list.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    ShapeContainer component = enumerator.Current;
                                    this.ChangeService.OnComponentChanged(component, null, null, null);
                                }
                            }
                            finally
                            {
                                enumerator.Dispose();
                            }
                        }
                    }
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    if (transaction != null)
                    {
                        transaction.Cancel();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    Cursor.Current = current;
                }
            }
        }

        protected void OnMenuSizeToGrid(object sender, EventArgs e)
        {
            if ((this.TryInvokeOldCommand(RuntimeHelpers.GetObjectValue(sender)) != null) && (this.SelectionService != null))
            {
                Cursor current = Cursor.Current;
                IDesignerHost designerHost = this.DesignerHost;
                DesignerTransaction transaction = null;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                    object[] array = new object[(selectedComponents.Count - 1) + 1];
                    selectedComponents.CopyTo(array, 0);
                    Size empty = Size.Empty;
                    Point point = Point.Empty;
                    Size size = Size.Empty;
                    PropertyDescriptor descriptor2 = null;
                    PropertyDescriptor descriptor = null;
                    if (designerHost != null)
                    {
                        transaction = designerHost.CreateTransaction($"Size {array.Length} shape(s) to grid");
                        IComponent rootComponent = designerHost.RootComponent;
                        if ((rootComponent != null) && (rootComponent is Control))
                        {
                            PropertyDescriptor property = this.GetProperty(rootComponent, "CurrentGridSize");
                            if (property != null)
                            {
                                size = (Size)property.GetValue(rootComponent);
                            }
                        }
                    }
                    if (!size.IsEmpty)
                    {
                        object[] objArray3 = array;
                        for (int i = 0; i < objArray3.Length; i++)
                        {
                            object objectValue = RuntimeHelpers.GetObjectValue(objArray3[i]);
                            IComponent comp = (IComponent)objectValue;
                            if (objectValue != null)
                            {
                                descriptor2 = this.GetProperty(comp, "Size");
                                descriptor = this.GetProperty(comp, "Location");
                                if (((descriptor2 != null) && (descriptor != null)) && (!descriptor2.IsReadOnly && !descriptor.IsReadOnly))
                                {
                                    empty = (Size)descriptor2.GetValue(comp);
                                    point = (Point)descriptor.GetValue(comp);
                                    empty.Width = ((empty.Width + (size.Width / 2)) / size.Width) * size.Width;
                                    empty.Height = ((empty.Height + (size.Height / 2)) / size.Height) * size.Height;
                                    point.X = (point.X / size.Width) * size.Width;
                                    point.Y = (point.Y / size.Height) * size.Height;
                                    descriptor2.SetValue(comp, empty);
                                    descriptor.SetValue(comp, point);
                                }
                            }
                        }
                    }
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    if (transaction != null)
                    {
                        transaction.Cancel();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    Cursor.Current = current;
                }
            }
        }

        protected void OnMenuSizingCommand(object sender, EventArgs e)
        {
            CommandID did = this.TryInvokeOldCommand(RuntimeHelpers.GetObjectValue(sender));
            if ((did != null) && (this.SelectionService != null))
            {
                Cursor current = Cursor.Current;
                try
                {
                    PropertyDescriptor property;
                    Cursor.Current = Cursors.WaitCursor;
                    ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                    object[] array = new object[(selectedComponents.Count - 1) + 1];
                    selectedComponents.CopyTo(array, 0);
                    object objectValue = RuntimeHelpers.GetObjectValue(this.SelectionService.PrimarySelection);
                    Size empty = Size.Empty;
                    Size size = Size.Empty;
                    IComponent comp = (IComponent)objectValue;
                    if (comp != null)
                    {
                        property = this.GetProperty(comp, "Size");
                        if (property == null)
                        {
                            return;
                        }
                        empty = (Size)property.GetValue(comp);
                    }
                    if (objectValue != null)
                    {
                        IDesignerHost designerHost = this.DesignerHost;
                        DesignerTransaction transaction = null;
                        try
                        {
                            if (designerHost != null)
                            {
                                transaction = designerHost.CreateTransaction($"Size {array.Length} shapes");
                            }
                            object[] objArray3 = array;
                            for (int i = 0; i < objArray3.Length; i++)
                            {
                                object obj3 = RuntimeHelpers.GetObjectValue(objArray3[i]);
                                if (!obj3.Equals(RuntimeHelpers.GetObjectValue(objectValue)))
                                {
                                    IComponent component2 = (IComponent)obj3;
                                    if (component2 != null)
                                    {
                                        PropertyDescriptor descriptor2 = this.GetProperty(RuntimeHelpers.GetObjectValue(obj3), "Locked");
                                        if ((descriptor2 == null) || !Convert.ToBoolean(descriptor2.GetValue(RuntimeHelpers.GetObjectValue(obj3))))
                                        {
                                            property = this.GetProperty(component2, "Size");
                                            if ((property != null) && !property.IsReadOnly)
                                            {
                                                size = (Size)property.GetValue(component2);
                                                if (did.Equals(StandardCommands.SizeToControlHeight) || did.Equals(StandardCommands.SizeToControl))
                                                {
                                                    size.Height = empty.Height;
                                                }
                                                if (did.Equals(StandardCommands.SizeToControlWidth) || did.Equals(StandardCommands.SizeToControl))
                                                {
                                                    size.Width = empty.Width;
                                                }
                                                property.SetValue(component2, size);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exception1)
                        {
                            //ProjectData.SetProjectError(exception1);
                            Exception exception = exception1;
                            if (transaction != null)
                            {
                                transaction.Cancel();
                                ((IDisposable)transaction).Dispose();
                                transaction = null;
                            }
                            //ProjectData.ClearProjectError();
                        }
                        finally
                        {
                            if (transaction != null)
                            {
                                transaction.Commit();
                                ((IDisposable)transaction).Dispose();
                                transaction = null;
                            }
                        }
                    }
                }
                finally
                {
                    Cursor.Current = current;
                }
            }
        }

        protected void OnMenuSpacingCommand(object sender, EventArgs e)
        {
            CommandID did = this.TryInvokeOldCommand(RuntimeHelpers.GetObjectValue(sender));
            if ((did != null) && (this.SelectionService != null))
            {
                Cursor current = Cursor.Current;
                IDesignerHost designerHost = this.DesignerHost;
                DesignerTransaction transaction = null;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Size empty = Size.Empty;
                    ICollection selectedComponents = this.SelectionService.GetSelectedComponents();
                    object[] array = new object[(selectedComponents.Count - 1) + 1];
                    selectedComponents.CopyTo(array, 0);
                    if (designerHost != null)
                    {
                        transaction = designerHost.CreateTransaction($"Format {array.Length} shapes (spacing)");
                        IComponent rootComponent = designerHost.RootComponent;
                        if ((rootComponent != null) & (rootComponent is Control))
                        {
                            PropertyDescriptor property = this.GetProperty(rootComponent, "CurrentGridSize");
                            if (property != null)
                            {
                                empty = (Size)property.GetValue(rootComponent);
                            }
                        }
                    }
                    int num2 = 0;
                    PropertyDescriptor descriptor2 = null;
                    PropertyDescriptor descriptor4 = null;
                    PropertyDescriptor descriptor = null;
                    PropertyDescriptor descriptor3 = null;
                    Size size = Size.Empty;
                    Size size3 = Size.Empty;
                    Point point = Point.Empty;
                    Point point2 = Point.Empty;
                    Point point3 = Point.Empty;
                    IComponent comp = null;
                    IComponent component2 = null;
                    SortBy horizontal = SortBy.Horizontal;
                    if ((did.Equals(StandardCommands.HorizSpaceConcatenate) || did.Equals(StandardCommands.HorizSpaceDecrease)) || (did.Equals(StandardCommands.HorizSpaceIncrease) || did.Equals(StandardCommands.HorizSpaceMakeEqual)))
                    {
                        horizontal = SortBy.Horizontal;
                    }
                    else if ((did.Equals(StandardCommands.VertSpaceConcatenate) || did.Equals(StandardCommands.VertSpaceDecrease)) || (did.Equals(StandardCommands.VertSpaceIncrease) || did.Equals(StandardCommands.VertSpaceMakeEqual)))
                    {
                        horizontal = SortBy.Vertical;
                    }
                    this.SortSelection(array, horizontal);
                    object objectValue = RuntimeHelpers.GetObjectValue(this.SelectionService.PrimarySelection);
                    int num3 = 0;
                    if (objectValue != null)
                    {
                        num3 = Array.IndexOf<object>(array, RuntimeHelpers.GetObjectValue(objectValue));
                    }
                    int index = 0;
                    if (did.Equals(StandardCommands.HorizSpaceMakeEqual) || did.Equals(StandardCommands.VertSpaceMakeEqual))
                    {
                        int num4 = 0;
                        int num6 = array.Length - 1;
                        index = 0;
                        while (index <= num6)
                        {
                            size = Size.Empty;
                            comp = (IComponent)array[index];
                            if (comp != null)
                            {
                                descriptor2 = this.GetProperty(comp, "Size");
                                if (descriptor2 != null)
                                {
                                    size = (Size)descriptor2.GetValue(comp);
                                }
                            }
                            if (horizontal == SortBy.Horizontal)
                            {
                                num4 += size.Width;
                            }
                            else
                            {
                                num4 += size.Height;
                            }
                            index++;
                        }
                        component2 = null;
                        comp = null;
                        size = Size.Empty;
                        point = Point.Empty;
                        int num7 = array.Length - 1;
                        index = 0;
                        while (index <= num7)
                        {
                            comp = (IComponent)array[index];
                            if (comp != null)
                            {
                                descriptor2 = this.GetProperty(comp, "Size");
                                descriptor = this.GetProperty(comp, "Location");
                                component2 = comp;
                                if (descriptor != null)
                                {
                                    point = (Point)descriptor.GetValue(comp);
                                    if (descriptor2 != null)
                                    {
                                        size = (Size)descriptor2.GetValue(comp);
                                        if (!size.IsEmpty && !point.IsEmpty)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            index++;
                        }
                        index = array.Length - 1;
                        while (index >= 0)
                        {
                            comp = (IComponent)array[index];
                            if (comp != null)
                            {
                                descriptor2 = this.GetProperty(comp, "Size");
                                descriptor = this.GetProperty(comp, "Location");
                                component2 = comp;
                                if (descriptor != null)
                                {
                                    point2 = (Point)descriptor.GetValue(comp);
                                    if (descriptor2 != null)
                                    {
                                        size3 = (Size)descriptor2.GetValue(comp);
                                        if ((descriptor2 != null) && (descriptor != null))
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            index += -1;
                        }
                        if ((descriptor2 != null) && (descriptor != null))
                        {
                            if (horizontal == SortBy.Horizontal)
                            {
                                num2 = (int)Math.Round((double)(((double)(((size3.Width + point2.X) - point.X) - num4)) / ((double)(array.Length - 1))));
                            }
                            else
                            {
                                num2 = (int)Math.Round((double)(((double)(((size3.Height + point2.Y) - point.Y) - num4)) / ((double)(array.Length - 1))));
                            }
                            if (num2 < 0)
                            {
                                num2 = 0;
                            }
                        }
                    }
                    comp = null;
                    component2 = null;
                    if (objectValue != null)
                    {
                        PropertyDescriptor descriptor6 = this.GetProperty(RuntimeHelpers.GetObjectValue(objectValue), "Location");
                        if (descriptor6 != null)
                        {
                            point3 = (Point)descriptor6.GetValue(RuntimeHelpers.GetObjectValue(objectValue));
                        }
                    }
                    int num8 = array.Length - 1;
                    for (index = 0; index <= num8; index++)
                    {
                        comp = (IComponent)array[index];
                        PropertyDescriptor descriptor7 = this.GetProperty(comp, "Locked");
                        if ((descriptor7 == null) || !Convert.ToBoolean(descriptor7.GetValue(comp)))
                        {
                            descriptor2 = this.GetProperty(comp, "Size");
                            descriptor = this.GetProperty(comp, "Location");
                            if (descriptor != null)
                            {
                                point = (Point)descriptor.GetValue(comp);
                                if (descriptor2 != null)
                                {
                                    size = (Size)descriptor2.GetValue(comp);
                                    int num5 = Math.Max(0, index - 1);
                                    component2 = (IComponent)array[num5];
                                    descriptor4 = this.GetProperty(component2, "Size");
                                    descriptor3 = this.GetProperty(component2, "Location");
                                    if (descriptor3 != null)
                                    {
                                        point2 = (Point)descriptor3.GetValue(component2);
                                        if (descriptor4 != null)
                                        {
                                            size3 = (Size)descriptor4.GetValue(component2);
                                            if (did.Equals(StandardCommands.HorizSpaceConcatenate) && (index > 0))
                                            {
                                                point.X = point2.X + size3.Width;
                                            }
                                            else if (did.Equals(StandardCommands.HorizSpaceDecrease))
                                            {
                                                if (num3 < index)
                                                {
                                                    point.X -= empty.Width * (index - num3);
                                                    if (point.X < point3.X)
                                                    {
                                                        point.X = point3.X;
                                                    }
                                                }
                                                else if (num3 > index)
                                                {
                                                    point.X += empty.Width * (num3 - index);
                                                    if (point.X > point3.X)
                                                    {
                                                        point.X = point3.X;
                                                    }
                                                }
                                            }
                                            else if (did.Equals(StandardCommands.HorizSpaceIncrease))
                                            {
                                                if (num3 < index)
                                                {
                                                    point.X += empty.Width * (index - num3);
                                                }
                                                else if (num3 > index)
                                                {
                                                    point.X -= empty.Width * (num3 - index);
                                                }
                                            }
                                            else if (did.Equals(StandardCommands.HorizSpaceMakeEqual) && (index > 0))
                                            {
                                                point.X = (point2.X + size3.Width) + num2;
                                            }
                                            else if (did.Equals(StandardCommands.VertSpaceConcatenate) && (index > 0))
                                            {
                                                point.Y = point2.Y + size3.Height;
                                            }
                                            else if (did.Equals(StandardCommands.VertSpaceDecrease))
                                            {
                                                if (num3 < index)
                                                {
                                                    point.Y -= empty.Height * (index - num3);
                                                    if (point.Y < point3.Y)
                                                    {
                                                        point.Y = point3.Y;
                                                    }
                                                }
                                                else if (num3 > index)
                                                {
                                                    point.Y += empty.Height * (num3 - index);
                                                    if (point.Y > point3.Y)
                                                    {
                                                        point.Y = point3.Y;
                                                    }
                                                }
                                            }
                                            else if (did.Equals(StandardCommands.VertSpaceIncrease))
                                            {
                                                if (num3 < index)
                                                {
                                                    point.Y += empty.Height * (index - num3);
                                                }
                                                else if (num3 > index)
                                                {
                                                    point.Y -= empty.Height * (num3 - index);
                                                }
                                            }
                                            else if (did.Equals(StandardCommands.VertSpaceMakeEqual) && (index > 0))
                                            {
                                                point.Y = (point2.Y + size3.Height) + num2;
                                            }
                                            if (!descriptor.IsReadOnly)
                                            {
                                                descriptor.SetValue(comp, point);
                                            }
                                            component2 = comp;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    if (transaction != null)
                    {
                        transaction.Cancel();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                        ((IDisposable)transaction).Dispose();
                        transaction = null;
                    }
                    Cursor.Current = current;
                }
            }
        }

        internal void OnSelectionChanged(ISelectionService selectionService)
        {
            if (selectionService == null)
            {
                return;
            }
            this.m_SeletionCount = selectionService.SelectionCount;
            this.m_PrimarySelection = (IComponent)selectionService.PrimarySelection;
            IDesignerHost designerHost = this.DesignerHost;
            if ((this.m_SeletionCount > 0) && (designerHost != null))
            {
                object rootComponent = designerHost.RootComponent;
                if ((rootComponent != null) && selectionService.GetComponentSelected(RuntimeHelpers.GetObjectValue(rootComponent)))
                {
                    this.m_SeletionCount = 0;
                }
            }
            this.m_SelectionOnShapesOnly = false;
            this.m_SelectionOnControlsOnly = this.m_SeletionCount > 0;
            if (this.m_SeletionCount > 0)
            {
                IEnumerator enumerator = null;
                ICollection selectedComponents = selectionService.GetSelectedComponents();
                try
                {
                    enumerator = selectedComponents.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                        if (!typeof(Control).IsAssignableFrom(objectValue.GetType()))
                        {
                            this.m_SelectionOnControlsOnly = false;
                        }
                        if (typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                        {
                            this.m_SelectionOnShapesOnly = true;
                        }
                        if (this.m_SelectionOnShapesOnly && !this.m_SelectionOnControlsOnly)
                        {
                            goto Label_0104;
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
        Label_0104:
            foreach (ShapeContainerMenuCommand command in this.ShapeMenuCommands)
            {
                command.OnSelectionChanged();
            }
        }

        private void OnStatusControlsOrShapesOnlySelection(object sender, EventArgs e)
        {
            ShapeContainerMenuCommand command = (ShapeContainerMenuCommand)sender;
            command.Enabled = (this.m_SelectionOnControlsOnly || this.m_SelectionOnShapesOnly) && (this.m_SeletionCount > 0);
        }

        private void OnStatusControlsOrShapesOnlySelectionAndGrid(object sender, EventArgs e)
        {
            ShapeContainerMenuCommand command = (ShapeContainerMenuCommand)sender;
            command.Enabled = (this.m_SelectionOnControlsOnly || this.m_SelectionOnShapesOnly) && ((this.m_SeletionCount > 0) && !this.UseSnapLines);
        }

        private void OnStatusMultiSelect(object sender, EventArgs e)
        {
            ShapeContainerMenuCommand command = (ShapeContainerMenuCommand)sender;
            command.Enabled = (this.m_SelectionOnControlsOnly || this.m_SelectionOnShapesOnly) && (this.m_SeletionCount > 1);
        }

        private void OnStatusMultiSelectNonContained(object sender, EventArgs e)
        {
            this.OnStatusMultiSelect(RuntimeHelpers.GetObjectValue(sender), e);
            ShapeContainerMenuCommand command = (ShapeContainerMenuCommand)sender;
            if (command.Enabled && this.m_SelectionOnControlsOnly)
            {
                command.Enabled = this.CheckSelectionParenting();
            }
        }

        private void OnStatusMultiSelectPrimary(object sender, EventArgs e)
        {
            ShapeContainerMenuCommand command = (ShapeContainerMenuCommand)sender;
            command.Enabled = (this.m_SelectionOnControlsOnly || this.m_SelectionOnShapesOnly) && ((this.m_SeletionCount > 1) && (this.m_PrimarySelection != null));
        }

        private void PrepareFormatMenuCommands(IMenuCommandService mcs, ShapeContainerMenuCommand shapeMenuCommand, CommandID cmdId)
        {
            this.OwnMenuCommands.Add(shapeMenuCommand);
            this.ShapeMenuCommands.Add(shapeMenuCommand);
            this.FindAndRemoveStdCommand(ref mcs, cmdId);
        }

        private void ReplaceStdMenuCommands()
        {
            IMenuCommandService menuCommandService = this.MenuCommandService;
            if (menuCommandService != null)
            {
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuBringToFront), StandardCommands.BringToFront));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuCopy), StandardCommands.Copy));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuCut), StandardCommands.Cut));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuDelete), StandardCommands.Delete));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyCancel), MenuCommands.KeyCancel));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyDefault), MenuCommands.KeyDefaultAction));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyMoveDown));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyMoveLeft));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyMoveRight));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyMoveUp));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyNudgeDown));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyNudgeLeft));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyNudgeRight));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyMove), MenuCommands.KeyNudgeUp));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeySizeHeightDecrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeySizeHeightIncrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeySizeWidthDecrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeySizeWidthIncrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeyNudgeHeightDecrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeyNudgeHeightIncrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeyNudgeWidthDecrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuKeyResize), MenuCommands.KeyNudgeWidthIncrease));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuSelectAll), StandardCommands.SelectAll));
                this.OwnMenuCommands.Add(new MenuCommand(new EventHandler(this.OnMenuSendToBack), StandardCommands.SendToBack));
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuAlignByPrimary), StandardCommands.AlignBottom, false), StandardCommands.AlignBottom);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuAlignByPrimary), StandardCommands.AlignHorizontalCenters, false), StandardCommands.AlignHorizontalCenters);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuAlignByPrimary), StandardCommands.AlignLeft, false), StandardCommands.AlignLeft);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuAlignByPrimary), StandardCommands.AlignRight, false), StandardCommands.AlignRight);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuAlignByPrimary), StandardCommands.AlignTop, false), StandardCommands.AlignTop);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuAlignByPrimary), StandardCommands.AlignVerticalCenters, false), StandardCommands.AlignVerticalCenters);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusControlsOrShapesOnlySelectionAndGrid), new EventHandler(this.OnMenuAlignToGrid), StandardCommands.AlignToGrid, false), StandardCommands.AlignToGrid);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusControlsOrShapesOnlySelection), new EventHandler(this.OnMenuCenterSelection), StandardCommands.CenterHorizontally, false), StandardCommands.CenterHorizontally);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusControlsOrShapesOnlySelection), new EventHandler(this.OnMenuCenterSelection), StandardCommands.CenterVertically, false), StandardCommands.CenterVertically);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.HorizSpaceConcatenate, false), StandardCommands.HorizSpaceConcatenate);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.HorizSpaceDecrease, false), StandardCommands.HorizSpaceDecrease);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.HorizSpaceIncrease, false), StandardCommands.HorizSpaceIncrease);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.HorizSpaceMakeEqual, false), StandardCommands.HorizSpaceMakeEqual);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.VertSpaceConcatenate, false), StandardCommands.VertSpaceConcatenate);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.VertSpaceDecrease, false), StandardCommands.VertSpaceDecrease);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.VertSpaceIncrease, false), StandardCommands.VertSpaceIncrease);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectNonContained), new EventHandler(this.OnMenuSpacingCommand), StandardCommands.VertSpaceMakeEqual, false), StandardCommands.VertSpaceMakeEqual);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuSizingCommand), StandardCommands.SizeToControl, false), StandardCommands.SizeToControl);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuSizingCommand), StandardCommands.SizeToControlHeight, false), StandardCommands.SizeToControlHeight);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusMultiSelectPrimary), new EventHandler(this.OnMenuSizingCommand), StandardCommands.SizeToControlWidth, false), StandardCommands.SizeToControlWidth);
                this.PrepareFormatMenuCommands(menuCommandService, new ShapeContainerMenuCommand(new EventHandler(this.OnStatusControlsOrShapesOnlySelectionAndGrid), new EventHandler(this.OnMenuSizeToGrid), StandardCommands.SizeToGrid, false), StandardCommands.SizeToGrid);
                this.FindAndRemoveStdCommand(ref menuCommandService, StandardCommands.BringToFront);
                this.FindAndRemoveStdCommand(ref menuCommandService, StandardCommands.Copy);
                this.FindAndRemoveStdCommand(ref menuCommandService, StandardCommands.Cut);
                this.FindAndRemoveStdCommand(ref menuCommandService, StandardCommands.Delete);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyCancel);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyDefaultAction);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyMoveDown);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyMoveLeft);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyMoveRight);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyMoveUp);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeDown);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeLeft);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeRight);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeUp);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeySizeHeightDecrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeySizeHeightIncrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeySizeWidthDecrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeySizeWidthIncrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeHeightDecrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeHeightIncrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeWidthDecrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, MenuCommands.KeyNudgeWidthIncrease);
                this.FindAndRemoveStdCommand(ref menuCommandService, StandardCommands.SelectAll);
                this.FindAndRemoveStdCommand(ref menuCommandService, StandardCommands.SendToBack);
                foreach (MenuCommand command in this.OwnMenuCommands)
                {
                    menuCommandService.AddCommand(command);
                }
            }
        }

        private void RestoreStdMenuCommands()
        {
            IMenuCommandService menuCommandService = this.MenuCommandService;
            if (menuCommandService != null)
            {
                if (this.OwnMenuCommands != null)
                {
                    foreach (MenuCommand command in this.OwnMenuCommands)
                    {
                        menuCommandService.RemoveCommand(command);
                    }
                    this.OwnMenuCommands.Clear();
                    this.m_OwnMenuCommands = null;
                }
                if (this.StdMenuCommands != null)
                {
                    foreach (KeyValuePair<CommandID, MenuCommand> pair in this.StdMenuCommands)
                    {
                        if (menuCommandService.FindCommand(pair.Key) == null)
                        {
                            menuCommandService.AddCommand(pair.Value);
                        }
                    }
                    this.StdMenuCommands.Clear();
                    this.m_StdMenuCommands = null;
                }
            }
            this.m_MenuCommandService = null;
        }

        private void SortSelection(object[] selectedObjects, SortBy nSortBy)
        {
            IComparer comparer = null;
            if (nSortBy == SortBy.Horizontal)
            {
                comparer = new ComponentLeftCompare(this);
            }
            else if (nSortBy == SortBy.Vertical)
            {
                comparer = new ComponentTopCompare(this);
            }
            else if (nSortBy == SortBy.ZOrder)
            {
                comparer = new ComponentZOrderCompare();
            }
            if (comparer != null)
            {
                Array.Sort(selectedObjects, comparer);
            }
        }

        private CommandID TryInvokeOldCommand(object sender)
        {
            ShapeContainerMenuCommand command = (ShapeContainerMenuCommand)sender;
            if (command == null)
            {
                return null;
            }
            CommandID commandID = command.CommandID;
            if (!this.m_SelectionOnShapesOnly)
            {
                this.InvokeStdMenuCmd(commandID, false);
                return null;
            }
            return commandID;
        }

        private IComponentChangeService ChangeService
        {
            get
            {
                if (this.m_ChangeService == null)
                {
                    this.m_ChangeService = (IComponentChangeService)this.m_SvcProvider.GetService(typeof(IComponentChangeService));
                }
                return this.m_ChangeService;
            }
        }

        private IDesignerHost DesignerHost
        {
            get
            {
                if (this.m_DesignerHost == null)
                {
                    this.m_DesignerHost = (IDesignerHost)this.m_SvcProvider.GetService(typeof(IDesignerHost));
                }
                return this.m_DesignerHost;
            }
        }

        private IDesignerSerializationService DesignerSerializationService
        {
            get
            {
                if (this.m_DesignerSerializationService == null)
                {
                    this.m_DesignerSerializationService = (IDesignerSerializationService)this.m_SvcProvider.GetService(typeof(IDesignerSerializationService));
                }
                return this.m_DesignerSerializationService;
            }
        }

        public ShapeContainerDesigner DragShapeConDesigner
        {
            get
            {
                return this.m_DragShapeConDesigner;
            }
            set
            {
                this.m_DragShapeConDesigner = value;
            }
        }

        internal ShapeContainerDesignerHelper Helper
        {
            get
            {
                return this.m_Helper;
            }
            set
            {
                this.m_Helper = value;
            }
        }

        public bool InOperation
        {
            get
            {
                return this.m_InOperation;
            }
            set
            {
                this.m_InOperation = value;
            }
        }

        private IMenuCommandService MenuCommandService
        {
            get
            {
                if (this.m_MenuCommandService == null)
                {
                    this.m_MenuCommandService = (IMenuCommandService)this.m_SvcProvider.GetService(typeof(IMenuCommandService));
                }
                return this.m_MenuCommandService;
            }
        }

        private List<MenuCommand> OwnMenuCommands
        {
            get
            {
                if (this.m_OwnMenuCommands == null)
                {
                    this.m_OwnMenuCommands = new List<MenuCommand>();
                }
                return this.m_OwnMenuCommands;
            }
        }

        private ISelectionService SelectionService
        {
            get
            {
                if (this.m_SelectionService == null)
                {
                    this.m_SelectionService = (ISelectionService)this.m_SvcProvider.GetService(typeof(ISelectionService));
                }
                return this.m_SelectionService;
            }
        }

        private List<ShapeContainerMenuCommand> ShapeMenuCommands
        {
            get
            {
                if (this.m_ShapeMenuCommands == null)
                {
                    this.m_ShapeMenuCommands = new List<ShapeContainerMenuCommand>();
                }
                return this.m_ShapeMenuCommands;
            }
        }

        public bool ShouldDispose
        {
            get
            {
                return this.m_ShouldDispose;
            }
            set
            {
                this.m_ShouldDispose = value;
            }
        }

        internal StatusBarCommand StatusBarCommand
        {
            get
            {
                return this.m_StatusBarCommand;
            }
        }

        private Dictionary<CommandID, MenuCommand> StdMenuCommands
        {
            get
            {
                if (this.m_StdMenuCommands == null)
                {
                    this.m_StdMenuCommands = new Dictionary<CommandID, MenuCommand>();
                }
                return this.m_StdMenuCommands;
            }
        }

        internal bool UseSnapLines
        {
            get
            {
                if (!this.m_UseSnapLinesQueried)
                {
                    this.m_UseSnapLinesQueried = true;
                    this.m_UseSnapLines = DesignerUtility.UseSnapLines(this.m_SvcProvider);
                }
                return this.m_UseSnapLines;
            }
        }

        private class ComponentLeftCompare : IComparer
        {
            private ShapeContainerCommandSet m_cmdSet;

            public ComponentLeftCompare(ShapeContainerCommandSet cmdSet)
            {
                if (cmdSet == null)
                {
                    throw new ArgumentNullException("cmdSet");
                }
                this.m_cmdSet = cmdSet;
            }

            public int Compare(object p, object q)
            {
                Point location = this.m_cmdSet.GetLocation((IComponent)p);
                Point point2 = this.m_cmdSet.GetLocation((IComponent)q);
                if (location.X == point2.X)
                {
                    return (location.Y - point2.Y);
                }
                return (location.X - point2.X);
            }
        }

        private class ComponentTopCompare : IComparer
        {
            private ShapeContainerCommandSet m_cmdSet;

            public ComponentTopCompare(ShapeContainerCommandSet cmdSet)
            {
                if (cmdSet == null)
                {
                    throw new ArgumentNullException("cmdSet");
                }
                this.m_cmdSet = cmdSet;
            }

            public int Compare(object p, object q)
            {
                Point location = this.m_cmdSet.GetLocation((IComponent)p);
                Point point2 = this.m_cmdSet.GetLocation((IComponent)q);
                if (location.Y == point2.Y)
                {
                    return (location.X - point2.X);
                }
                return (location.Y - point2.Y);
            }
        }

        internal class ComponentZOrderCompare : IComparer
        {
            public int Compare(object x, object y)
            {
                Shape child = (Shape)x;
                Shape shape2 = (Shape)y;
                if (child.Equals(shape2))
                {
                    return 0;
                }
                ShapeContainer parent = child.Parent;
                ShapeContainer objB = shape2.Parent;
                if ((parent == null) && (objB == null))
                {
                    return 0;
                }
                if (parent != null)
                {
                    if (objB == null)
                    {
                        return -1;
                    }
                    if (!object.ReferenceEquals(parent, objB))
                    {
                        return (parent.Handle.ToInt32() - objB.Handle.ToInt32());
                    }
                    if (parent.Shapes.GetChildIndex(child) > parent.Shapes.GetChildIndex(shape2))
                    {
                        return -1;
                    }
                }
                return 1;
            }
        }

        private enum SortBy
        {
            Horizontal,
            Vertical,
            ZOrder
        }
    }
}