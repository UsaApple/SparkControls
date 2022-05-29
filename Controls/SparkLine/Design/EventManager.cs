using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal sealed class EventManager : IDisposable
    {
        private BehaviorService m_BehaviorSvc;
        private IComponentChangeService m_ChangeSvc;
        private bool m_Disposed;
        private bool m_InBehaviorSvcDrag;
        private Control m_PreParent;
        private Control[] m_PreParentControls;
        private IComponent m_PreRootComponent;
        private IComponent m_RootComponent;
        private bool m_ShouldDispose;

        public EventManager(IComponent root)
        {
            this.m_RootComponent = root;
            this.Initialize();
        }

        private static void AddToContainer(Control ctrl, IDesignerHost host)
        {
            if (typeof(ShapeContainer).IsAssignableFrom(ctrl.GetType()))
            {
                IEnumerator enumerator = null;
                ShapeContainer container = (ShapeContainer)ctrl;
                try
                {
                    enumerator = container.Shapes.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Shape current = (Shape)enumerator.Current;
                        host.Container.Add(current);
                        current.Invalidate();
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
            else if (ctrl.Controls.Count != 0)
            {
                IEnumerator enumerator2 = null;
                try
                {
                    enumerator2 = ctrl.Controls.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Control control = (Control)enumerator2.Current;
                        AddToContainer(control, host);
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_Disposed && disposing)
            {
                if (this.m_BehaviorSvc != null)
                {
                    this.m_BehaviorSvc.BeginDrag -= new BehaviorDragDropEventHandler(this.OnBeginDrag);
                    this.m_BehaviorSvc.EndDrag -= new BehaviorDragDropEventHandler(this.OnEndDrag);
                }
                if (this.m_ChangeSvc != null)
                {
                    this.m_ChangeSvc.ComponentAdded -= new ComponentEventHandler(this.OnComponentAdded);
                    this.m_ChangeSvc.ComponentRemoving -= new ComponentEventHandler(this.OnComponentRemoving);
                }
                SystemEvents.DisplaySettingsChanged -= new EventHandler(this.OnSystemSettingChanged);
                SystemEvents.InstalledFontsChanged -= new EventHandler(this.OnSystemSettingChanged);
                SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.OnUserPreferenceChanged);
            }
            this.m_Disposed = true;
        }

        private void Initialize()
        {
            this.m_BehaviorSvc = (BehaviorService)this.m_RootComponent.Site.GetService(typeof(BehaviorService));
            if (this.m_BehaviorSvc != null)
            {
                this.m_BehaviorSvc.BeginDrag += new BehaviorDragDropEventHandler(this.OnBeginDrag);
                this.m_BehaviorSvc.EndDrag += new BehaviorDragDropEventHandler(this.OnEndDrag);
            }
            this.m_ChangeSvc = (IComponentChangeService)this.m_RootComponent.Site.GetService(typeof(IComponentChangeService));
            if (this.m_ChangeSvc != null)
            {
                this.m_ChangeSvc.ComponentAdded += new ComponentEventHandler(this.OnComponentAdded);
                this.m_ChangeSvc.ComponentRemoving += new ComponentEventHandler(this.OnComponentRemoving);
            }
            SystemEvents.DisplaySettingsChanged += new EventHandler(this.OnSystemSettingChanged);
            SystemEvents.InstalledFontsChanged += new EventHandler(this.OnSystemSettingChanged);
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.OnUserPreferenceChanged);
        }

        private void OnBeginDrag(object sender, BehaviorDragDropEventArgs e)
        {
            IEnumerator enumerator = null;
            this.InBehaviorSvcDrag = true;
            try
            {
                enumerator = e.DragComponents.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                    if (typeof(Control).IsAssignableFrom(objectValue.GetType()))
                    {
                        Control ctrl = (Control)objectValue;
                        if (ParentContainsShapeContainer(ctrl))
                        {
                            this.m_PreParent = ctrl.Parent;
                            this.m_PreParentControls = new Control[(ctrl.Parent.Controls.Count - 1) + 1];
                            ctrl.Parent.Controls.CopyTo(this.m_PreParentControls, 0);
                        }
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

        private void OnComponentAdded(object sender, ComponentEventArgs e)
        {
        }

        private void OnComponentRemoving(object sender, ComponentEventArgs e)
        {
        }

        private void OnEndDrag(object sender, BehaviorDragDropEventArgs e)
        {
            IDesignerHost service = null;
            ShapeContainer shCon = null;
            IEnumerator enumerator = null;
            try
            {
                enumerator = e.DragComponents.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    object objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
                    if (typeof(Control).IsAssignableFrom(objectValue.GetType()))
                    {
                        Control control = (Control)objectValue;
                        service = (IDesignerHost)control.Site.GetService(typeof(IDesignerHost));
                        if ((control.Parent != null) && typeof(ShapeContainer).IsAssignableFrom(control.Parent.GetType()))
                        {
                            shCon = (ShapeContainer)control.Parent;
                        }
                    }
                    goto Label_00AF;
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        Label_00AF:
            if ((service != null) && !object.Equals(this.m_RootComponent, service.RootComponent))
            {
                IEnumerator enumerator2 = null;
                try
                {
                    enumerator2 = e.DragComponents.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        object obj3 = RuntimeHelpers.GetObjectValue(enumerator2.Current);
                        if (typeof(Control).IsAssignableFrom(obj3.GetType()))
                        {
                            AddToContainer((Control)obj3, service);
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
            if (shCon != null)
            {
                this.Reparent(shCon);
            }
            this.m_PreParent = null;
            this.m_PreParentControls = null;
            this.InBehaviorSvcDrag = false;
            if (this.ShouldDispose)
            {
                this.Dispose();
            }
        }

        private void OnSystemSettingChanged(object sender, EventArgs e)
        {
            DesignerUtility.RefreshBrushes();
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            DesignerUtility.RefreshBrushes();
        }

        private static bool ParentContainsShapeContainer(Control ctrl)
        {
            bool flag = false;
            IEnumerator enumerator = null;
            Control parent = ctrl.Parent;
            if (parent == null)
            {
                return false;
            }
            try
            {
                enumerator = parent.Controls.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Control current = (Control)enumerator.Current;
                    if (typeof(ShapeContainer).IsAssignableFrom(current.GetType()))
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
            return flag;
        }

        private void Reparent(ShapeContainer shCon)
        {
            if (shCon.Parent != null)
            {
                for (int i = shCon.Controls.Count; i >= 1; i += -1)
                {
                    Control control = shCon.Controls[i - 1];
                    shCon.Controls.Remove(control);
                    Point location = control.Location;
                    Control parent = shCon.Parent;
                    location = shCon.PointToScreen(location);
                    location = parent.PointToClient(location);
                    control.Location = location;
                    parent.Controls.Add(control);
                    parent.Controls.SetChildIndex(control, 0);
                }
                if ((this.m_PreParent != null) && this.m_PreParent.Equals(shCon.Parent))
                {
                    this.m_PreParent.Controls.Clear();
                    int num3 = this.m_PreParentControls.Length - 1;
                    for (int j = 0; j <= num3; j++)
                    {
                        this.m_PreParent.Controls.Add(this.m_PreParentControls[j]);
                    }
                }
            }
        }

        public bool InBehaviorSvcDrag
        {
            get
            {
                return this.m_InBehaviorSvcDrag;
            }
            set
            {
                this.m_InBehaviorSvcDrag = value;
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
    }
}