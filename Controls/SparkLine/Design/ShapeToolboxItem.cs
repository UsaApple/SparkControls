using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
    public abstract class ShapeToolboxItem : ToolboxItem
    {
        private const int InitHeight = 0x17;
        private const int InitWidth = 0x4b;
        private IComponentChangeService m_ChangeService;
        private IDesignerHost m_Host;
        private Point m_Location;
        private Control m_Parent;
        private ShapeContainer m_ShapeContainer;
        private Size m_Size;

        protected override IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
        {
            this.m_Host = host;
            if (host == null)
            {
                return null;
            }
            this.m_ChangeService = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
            if (this.m_ChangeService == null)
            {
                return null;
            }
            ITypeResolutionService service = (ITypeResolutionService)this.m_Host.GetService(typeof(ITypeResolutionService));
            if (service != null)
            {
                if (this.AssemblyName != null)
                {
                    service.ReferenceAssembly(this.AssemblyName);
                }
                else
                {
                    Type type = typeof(Shape);
                    if (type != null)
                    {
                        service.ReferenceAssembly(type.Assembly.GetName());
                    }
                }
            }
            this.m_Parent = (Control)defaultValues["Parent"];
            if (this.m_Parent == null)
            {
                ISelectionService ss = (ISelectionService)host.GetService(typeof(ISelectionService));
                if (ss == null)
                {
                    return null;
                }
                this.m_ShapeContainer = DesignerUtility.GetShapeContainer(host, ss, this.m_ChangeService);
                if (this.m_ShapeContainer == null)
                {
                    return null;
                }
                this.SetCreationMode();
                this.m_Location = new Point(0, 0);
                this.m_Size = new Size(0x4b, 0x17);
                Shape shape2 = this.CreateShape(this.m_Location, this.m_Size, this.m_Host, this.m_ChangeService);
                if (shape2 == null)
                {
                    return null;
                }
                List<Shape> shapeList = new List<Shape>(1) {
                    shape2
                };
                DesignerUtility.PasteShapes(host, shapeList, this.m_ShapeContainer, this.m_ChangeService);
                return new IComponent[] { shape2 };
            }
            if ((this.m_Parent != null) && (typeof(TableLayoutPanel).IsAssignableFrom(this.m_Parent.GetType()) || typeof(FlowLayoutPanel).IsAssignableFrom(this.m_Parent.GetType())))
            {
                IUIService uiSvc = (IUIService)host.GetService(typeof(IUIService));
                DesignerUtility.ShowErrorMessage(uiSvc, $"Cannot create component of type '{typeof(Shape).Name}' in container of type '{this.m_Parent.GetType().Name}'.");
                return null;
            }
            if (defaultValues.Contains("Size"))
            {
                this.m_Size = (Size)defaultValues["Size"];
            }
            else
            {
                this.m_Size = new Size(0x4b, 0x17);
            }
            IComponent[] componentArray = null;
            this.GetShapeContainer(defaultValues);
            Shape shape = this.CreateShape(this.m_Location, this.m_Size, this.m_Host, this.m_ChangeService);
            componentArray = new IComponent[] { shape };
            if (this.m_ChangeService != null)
            {
                this.m_ChangeService.OnComponentChanging(this.m_ShapeContainer, null);
            }
            this.m_ShapeContainer.Shapes.Insert(0, shape);
            if (this.m_ChangeService != null)
            {
                this.m_ChangeService.OnComponentChanged(this.m_ShapeContainer, null, null, null);
            }
            return componentArray;
        }

        protected abstract Shape CreateShape(Point loc, Size size, IDesignerHost host, IComponentChangeService cs);
        protected void GetShapeContainer(IDictionary defaultValues)
        {
            bool flag;
            IEnumerator enumerator = null;
            bool flag2 = false;
            if (!defaultValues.Contains("Location"))
            {
                flag2 = true;
            }
            if (!flag2)
            {
                Point p = (Point)defaultValues["Location"];
                p = this.m_Parent.PointToClient(p);
                p.X = Math.Max(p.X, this.m_Parent.ClientRectangle.Left - this.m_Parent.Left);
                p.X = Math.Min(p.X, this.m_Parent.ClientRectangle.Right);
                p.Y = Math.Max(p.Y, this.m_Parent.ClientRectangle.Top - this.m_Parent.Top);
                p.Y = Math.Min(p.Y, this.m_Parent.Bottom);
                this.m_Location = p;
                goto Label_0324;
            }
            this.m_Location = new Point(0, 0);
            if (this.m_Host == null)
            {
                goto Label_0324;
            }
            Shape primarySelection = null;
            try
            {
                enumerator = this.m_Host.Container.Components.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Component current = (Component)enumerator.Current;
                    ShapeContainer component = null;
                    if (typeof(ShapeContainer).IsAssignableFrom(current.GetType()))
                    {
                        component = (ShapeContainer)current;
                    }
                    else if (typeof(Shape).IsAssignableFrom(current.GetType()))
                    {
                        component = ((Shape)current).Parent;
                    }
                    if (component != null)
                    {
                        ShapeContainerDesigner designer = (ShapeContainerDesigner)this.m_Host.GetDesigner(component);
                        primarySelection = designer.SelectionManager.PrimarySelection;
                        goto Label_00EA;
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
        Label_00EA:
            if (primarySelection != null)
            {
                this.m_ShapeContainer = primarySelection.Parent;
                Point location = primarySelection.BoundRect.Location;
                Size gridSize = DesignerUtility.GetGridSize(this.m_Host);
                location.Offset(gridSize.Width, gridSize.Height);
                Rectangle r = new Rectangle(location, this.m_Size);
                if ((this.m_ShapeContainer.Parent != null) && typeof(TabPage).IsAssignableFrom(this.m_ShapeContainer.Parent.GetType()))
                {
                    r = this.m_ShapeContainer.RectangleToScreen(r);
                    r = this.m_ShapeContainer.Parent.RectangleToClient(r);
                    if ((r.Right > this.m_ShapeContainer.Parent.Width) || (r.Bottom > this.m_ShapeContainer.Parent.Height))
                    {
                        this.m_Location = new Point(0 - this.m_ShapeContainer.Left, 0 - this.m_ShapeContainer.Top);
                    }
                    else
                    {
                        this.m_Location = location;
                    }
                }
                else if ((r.Right > this.m_ShapeContainer.Right) || (r.Bottom > this.m_ShapeContainer.Bottom))
                {
                    this.m_Location = new Point(0, 0);
                }
                else
                {
                    this.m_Location = location;
                }
                this.SetCreationMode();
                return;
            }
        Label_0324:
            flag = false;
            if (this.m_Parent != null)
            {
                if (typeof(ShapeContainer).IsAssignableFrom(this.m_Parent.GetType()))
                {
                    this.m_ShapeContainer = (ShapeContainer)this.m_Parent;
                    flag = true;
                }
                else
                {
                    IEnumerator enumerator2 = null;
                    try
                    {
                        enumerator2 = this.m_Parent.Controls.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Control control = (Control)enumerator2.Current;
                            if (typeof(ShapeContainer).IsAssignableFrom(control.GetType()))
                            {
                                this.m_ShapeContainer = (ShapeContainer)control;
                                if (!typeof(GroupBox).IsAssignableFrom(this.m_Parent.GetType()))
                                {
                                    this.m_Location = this.m_Parent.PointToScreen(this.m_Location);
                                    this.m_Location = this.m_ShapeContainer.PointToClient(this.m_Location);
                                }
                                flag = true;
                                goto Label_0421;
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
        Label_0421:
            if (flag)
            {
                if (flag2)
                {
                    this.SetCreationMode();
                }
            }
            else if (this.m_Parent != null)
            {
                this.m_ShapeContainer = (ShapeContainer)this.m_Host.CreateComponent(typeof(ShapeContainer));
                this.SetCreationMode();
                if (this.m_ChangeService != null)
                {
                    this.m_ChangeService.OnComponentChanging(this.m_Parent, null);
                }
                this.m_Parent.Controls.Add(this.m_ShapeContainer);
                if (!flag2 || typeof(TabPage).IsAssignableFrom(this.m_Parent.GetType()))
                {
                    this.m_Location = this.m_Parent.PointToScreen(this.m_Location);
                    this.m_Location = this.m_ShapeContainer.PointToClient(this.m_Location);
                }
                if (this.m_ChangeService != null)
                {
                    this.m_ChangeService.OnComponentChanged(this.m_Parent, null, null, null);
                }
            }
        }

        private void SetCreationMode()
        {
            if (this.m_Host != null)
            {
                ComponentDesigner designer = (ComponentDesigner)this.m_Host.GetDesigner(this.m_ShapeContainer);
                if ((designer != null) && typeof(ShapeContainerDesigner).IsAssignableFrom(designer.GetType()))
                {
                    ShapeContainerDesigner designer2 = (ShapeContainerDesigner)designer;
                    if (designer2 != null)
                    {
                        designer2.CreationMode = true;
                    }
                }
            }
        }
    }
}