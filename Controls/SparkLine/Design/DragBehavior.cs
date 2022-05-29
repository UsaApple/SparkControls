using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal sealed class DragBehavior : Behavior
    {
        private BehaviorService m_BehaviorService;
        private bool m_CancelDrag;
        private IComponentChangeService m_ChangeService;
        private DragObject m_DragObject;
        private DragGlyph m_Glyph;
        private IDesignerHost m_Host;
        private ShapeContainer m_Source;

        private ShapeContainer GetShapeContainerAtPoint(Point loc, ShapeContainer source)
        {
            Control control = this.GetTopContainerAtPoint(loc, source, true);
            if (control != null)
            {
                return (ShapeContainer)control;
            }
            return null;
        }

        private Control GetTopContainerAtPoint(Point loc, ShapeContainer source, bool getShapeContainer)
        {
            Control component = null;
            Point empty = Point.Empty;
            Control childAtPoint = null;
            if (source.RectangleToScreen(source.Bounds).Contains(loc))
            {
                component = source.Parent;
                empty = component.PointToClient(loc);
                childAtPoint = component.GetChildAtPoint(empty);
                if (source.Equals(childAtPoint))
                {
                    return source;
                }
            }
            IDesignerHost designerHost = this.DesignerHost;
            if (designerHost != null)
            {
                if (childAtPoint == null)
                {
                    childAtPoint = (Control)designerHost.RootComponent;
                    if (childAtPoint == null)
                    {
                        return null;
                    }
                }
                while (childAtPoint != null)
                {
                    component = childAtPoint;
                    empty = component.PointToClient(loc);
                    childAtPoint = component.GetChildAtPoint(empty);
                }
                while (component != null)
                {
                    if (getShapeContainer)
                    {
                        IEnumerator enumerator = null;
                        if (typeof(ShapeContainer).IsAssignableFrom(component.GetType()))
                        {
                            ShapeContainer container = (ShapeContainer)component;
                            if (container != null)
                            {
                                Control parent = container.Parent;
                                if (typeof(TabPage).IsAssignableFrom(parent.GetType()))
                                {
                                    component = container.Parent;
                                }
                                else
                                {
                                    IDesigner designer = designerHost.GetDesigner(parent);
                                    if ((designer != null) && typeof(ParentControlDesigner).IsAssignableFrom(designer.GetType()))
                                    {
                                        return container;
                                    }
                                    if (parent.Parent == null)
                                    {
                                        return null;
                                    }
                                    component = parent.Parent;
                                }
                            }
                        }
                        if (typeof(TabPage).IsAssignableFrom(component.GetType()))
                        {
                            Control control5 = ((TabPage)component).Parent;
                            if ((control5 != null) && typeof(TabControl).IsAssignableFrom(control5.GetType()))
                            {
                                component = ((TabControl)control5).SelectedTab;
                            }
                        }
                        try
                        {
                            enumerator = component.Controls.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                Control current = (Control)enumerator.Current;
                                if ((current != null) && typeof(ShapeContainer).IsAssignableFrom(current.GetType()))
                                {
                                    ShapeContainer container2 = (ShapeContainer)current;
                                    if (container2 != null)
                                    {
                                        return container2;
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
                    if (designerHost.GetDesigner(component) is ParentControlDesigner)
                    {
                        break;
                    }
                    component = component.Parent;
                }
                if (!getShapeContainer)
                {
                    return component;
                }
                if (component != null)
                {
                    IComponentChangeService changeService = this.ChangeService;
                    ShapeContainer container3 = (ShapeContainer)designerHost.CreateComponent(typeof(ShapeContainer));
                    changeService.OnComponentChanging(component, null);
                    component.Controls.Add(container3);
                    changeService.OnComponentChanged(component, null, null, null);
                    return container3;
                }
            }
            return null;
        }

        public void GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (this.m_CancelDrag)
            {
                e.UseDefaultCursors = true;
            }
            else if (this.m_DragObject != null)
            {
                if (this.m_DragObject.OnDragMove)
                {
                    e.UseDefaultCursors = true;
                }
                else
                {
                    e.UseDefaultCursors = false;
                }
                Control control = this.GetTopContainerAtPoint(Control.MousePosition, this.Source, false);
                if (this.Source.Equals(control))
                {
                    this.m_DragObject.GiveFeedback(null);
                }
                else
                {
                    this.m_DragObject.GiveFeedback(control);
                }
            }
        }

        public override void OnDragDrop(Glyph g, DragEventArgs e)
        {
            if (this.m_DragObject != null)
            {
                IDesignerHost designerHost = this.DesignerHost;
                if (designerHost != null)
                {
                    using (DesignerTransaction transaction = designerHost.CreateTransaction("Drag Drop"))
                    {
                        DragObject dragObject = this.m_DragObject;
                        this.m_DragObject = null;
                        if (dragObject.OnDragMove)
                        {
                            Point loc = new Point(e.X, e.Y);
                            ShapeContainer shapeContainerAtPoint = this.GetShapeContainerAtPoint(loc, this.Source);
                            if (shapeContainerAtPoint.Equals(this.Source))
                            {
                                dragObject.Drop(e.X, e.Y);
                            }
                            else
                            {
                                dragObject.Drop(e.X, e.Y, this.Source, shapeContainerAtPoint);
                            }
                        }
                        else
                        {
                            dragObject.Drop(e.X, e.Y);
                        }
                        this.BehaviorService.Adorners.Remove(this.Glyph.Adorenr);
                        transaction.Commit();
                    }
                }
            }
        }

        public override void OnDragEnter(Glyph g, DragEventArgs e)
        {
            object objectValue = RuntimeHelpers.GetObjectValue(e.Data.GetData(typeof(DragObject)));
            if ((objectValue != null) && typeof(DragObject).IsAssignableFrom(objectValue.GetType()))
            {
                this.m_DragObject = (DragObject)objectValue;
                if (this.m_DragObject != null)
                {
                    if (Control.ModifierKeys == Keys.ControlKey)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.Move;
                    }
                    this.m_DragObject.Result = DesignerUtility.DragResult.None;
                    return;
                }
            }
            this.BehaviorService.Adorners.Remove(this.Glyph.Adorenr);
        }

        public override void OnDragLeave(Glyph g, EventArgs e)
        {
        }

        public override void OnDragOver(Glyph g, DragEventArgs e)
        {
            base.OnDragOver(g, e);
            if (this.m_CancelDrag)
            {
                e.Effect = DragDropEffects.None;
            }
            else if ((this.m_DragObject != null) && this.m_DragObject.OnDragMove)
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        public override bool OnMouseUp(Glyph g, MouseButtons button)
        {
            if (this.m_CancelDrag && (this.m_DragObject != null))
            {
                this.m_DragObject.CancelDrag();
                this.m_DragObject = null;
            }
            if (this.m_DragObject != null)
            {
                if (this.m_DragObject.OnDragMove)
                {
                    Rectangle rootRect = this.RootRect;
                    if (!rootRect.IsEmpty && rootRect.Contains(Control.MousePosition))
                    {
                        this.m_DragObject.Result = DesignerUtility.DragResult.Cancel;
                    }
                }
                else
                {
                    DragObject dragObject = this.m_DragObject;
                    this.m_DragObject = null;
                    dragObject.Drop(Control.MousePosition.X, Control.MousePosition.Y);
                    this.BehaviorService.Adorners.Remove(this.Glyph.Adorenr);
                }
            }
            return base.OnMouseUp(g, button);
        }

        public void QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this.m_DragObject != null)
            {
                if (e.EscapePressed)
                {
                    e.Action = DragAction.Cancel;
                    this.m_DragObject.Result = DesignerUtility.DragResult.Cancel;
                    this.m_CancelDrag = true;
                }
                else
                {
                    Rectangle rootRect = this.RootRect;
                    if (rootRect.IsEmpty || !rootRect.Contains(Control.MousePosition))
                    {
                        if (this.m_DragObject.OnDragMove)
                        {
                            this.m_DragObject.ClearAllDragImage(true);
                            this.m_CancelDrag = true;
                        }
                        else
                        {
                            this.m_CancelDrag = false;
                        }
                    }
                    else
                    {
                        if (this.m_DragObject.OnDragMove)
                        {
                            IDesignerHost designerHost = this.DesignerHost;
                            if (designerHost != null)
                            {
                                Control control = this.Source.FindForm();
                                Point mousePosition = Control.MousePosition;
                                while (control != null)
                                {
                                    Point pt = control.PointToClient(mousePosition);
                                    Control childAtPoint = control.GetChildAtPoint(pt);
                                    if (childAtPoint == null)
                                    {
                                        break;
                                    }
                                    object designer = designerHost.GetDesigner(childAtPoint);
                                    if (designer == null || !typeof(ParentControlDesigner).IsAssignableFrom(designer.GetType()))
                                    {
                                        break;
                                    }
                                    control = childAtPoint;
                                }
                                if ((control != null) && (typeof(TableLayoutPanel).IsAssignableFrom(control.GetType()) || typeof(FlowLayoutPanel).IsAssignableFrom(control.GetType())))
                                {
                                    this.m_DragObject.ClearAllDragImage(true);
                                    this.m_CancelDrag = true;
                                    return;
                                }
                            }
                        }
                        this.m_CancelDrag = false;
                    }
                }
            }
        }

        public BehaviorService BehaviorService
        {
            get
            {
                return this.m_BehaviorService;
            }
            set
            {
                this.m_BehaviorService = value;
            }
        }

        private IComponentChangeService ChangeService
        {
            get
            {
                if (this.m_ChangeService == null)
                {
                    this.m_ChangeService = (IComponentChangeService)this.DesignerHost.GetService(typeof(IComponentChangeService));
                }
                return this.m_ChangeService;
            }
        }

        public IDesignerHost DesignerHost
        {
            get
            {
                return this.m_Host;
            }
            set
            {
                this.m_Host = value;
            }
        }

        public DragGlyph Glyph
        {
            get
            {
                return this.m_Glyph;
            }
            set
            {
                this.m_Glyph = value;
            }
        }

        public Rectangle RootRect
        {
            get
            {
                Rectangle empty = Rectangle.Empty;
                Control control = null;
                if (this.DesignerHost != null)
                {
                    Component rootComponent = (Component)this.DesignerHost.RootComponent;
                    if ((rootComponent != null) && typeof(Control).IsAssignableFrom(rootComponent.GetType()))
                    {
                        control = (Control)rootComponent;
                    }
                }
                if ((control == null) && (this.Source != null))
                {
                    control = this.Source.FindForm();
                }
                if (control != null)
                {
                    empty = new Rectangle(0, 0, control.ClientSize.Width, control.ClientSize.Height);
                    empty = control.RectangleToScreen(empty);
                }
                return empty;
            }
        }

        public ShapeContainer Source
        {
            get
            {
                return this.m_Source;
            }
            set
            {
                this.m_Source = value;
            }
        }
    }
}