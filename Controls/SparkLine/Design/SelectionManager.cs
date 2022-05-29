using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal sealed class SelectionManager : IDisposable
    {
        private BehaviorService m_BehaviorService;
        private Dictionary<Shape, SelectObject> m_CurSelectedShapes;
        private IDesignerHost m_DesignerHost;
        private bool m_Disposed;
        private Bitmap m_DrawImage;
        private ShapeContainerDesignerHelper m_Helper;
        private HitFlag m_HitFlag = HitFlag.HitNone;
        private Shape m_HitShape;
        private IDesignerHost m_Host;
        private bool m_InSelectionChange;
        private bool m_NeedRefresh;
        private Dictionary<Shape, SelectObject> m_PrevSelectedShapes;
        private Shape m_PrimarySelection;
        private Control m_RootComponent;
        private List<Shape> m_SelectedShapes;
        private Adorner m_SelectionAdorner;
        private SelectionGlyph m_SelectionGlyph;
        private ISelectionService m_SelectionService;
        private IServiceProvider m_ServiceProvider;
        private Dictionary<Shape, ShapeDesigner> m_ShapeDesignerMap;
        private const float SIZERATIO = 1.2f;

        public SelectionManager(IServiceProvider svcProvider)
        {
            this.m_ServiceProvider = svcProvider;
            this.m_BehaviorService = (BehaviorService)this.m_ServiceProvider.GetService(typeof(BehaviorService));
            this.m_DesignerHost = (IDesignerHost)this.m_ServiceProvider.GetService(typeof(IDesignerHost));
            this.m_Host = (IDesignerHost)this.m_ServiceProvider.GetService(typeof(IDesignerHost));
            this.m_SelectionService = (ISelectionService)this.m_ServiceProvider.GetService(typeof(ISelectionService));
            if (this.m_BehaviorService != null)
            {
                this.m_BehaviorService.Synchronize += new EventHandler(this.OnSynchronize);
            }
            if (this.m_SelectionService != null)
            {
                this.m_SelectionService.SelectionChanged += new EventHandler(this.OnSelectionChanged);
            }
            this.m_RootComponent = (Control)this.m_DesignerHost.RootComponent;
            this.m_SelectionAdorner = new Adorner();
            this.m_SelectionGlyph = new SelectionGlyph(this.m_RootComponent, this);
            this.m_SelectionAdorner.Glyphs.Add(this.m_SelectionGlyph);
            this.m_SelectedShapes = new List<Shape>();
            this.m_ShapeDesignerMap = new Dictionary<Shape, ShapeDesigner>();
            IComponentChangeService service = (IComponentChangeService)this.m_ServiceProvider.GetService(typeof(IComponentChangeService));
            if (service != null)
            {
                service.ComponentAdded += new ComponentEventHandler(this.OnComponentAdded);
                service.ComponentRemoved += new ComponentEventHandler(this.OnComponentRemoved);
                service.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
            }
            this.m_DesignerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
        }

        private void Clear()
        {
            this.m_PrimarySelection = null;
            this.SelectedShapes.Clear();
            this.HideAdorner();
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
                if (this.m_DesignerHost != null)
                {
                    this.m_DesignerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
                    this.m_DesignerHost = null;
                }
                if (this.m_ServiceProvider != null)
                {
                    IComponentChangeService service = (IComponentChangeService)this.m_ServiceProvider.GetService(typeof(IComponentChangeService));
                    if (service != null)
                    {
                        service.ComponentAdded -= new ComponentEventHandler(this.OnComponentAdded);
                        service.ComponentChanged -= new ComponentChangedEventHandler(this.OnComponentChanged);
                        service.ComponentRemoved -= new ComponentEventHandler(this.OnComponentRemoved);
                    }
                    if (this.m_SelectionService != null)
                    {
                        this.m_SelectionService.SelectionChanged -= new EventHandler(this.OnSelectionChanged);
                        this.m_SelectionService = null;
                    }
                    this.m_ServiceProvider = null;
                }
                if (this.m_SelectedShapes != null)
                {
                    this.m_SelectedShapes.Clear();
                    this.m_SelectedShapes = null;
                }
                if (this.m_BehaviorService != null)
                {
                    if (this.m_BehaviorService.Adorners.Contains(this.m_SelectionAdorner))
                    {
                        this.m_BehaviorService.Adorners.Remove(this.m_SelectionAdorner);
                    }
                    this.m_BehaviorService.Synchronize -= new EventHandler(this.OnSynchronize);
                    this.m_BehaviorService = null;
                }
                if (this.m_SelectionAdorner != null)
                {
                    this.m_SelectionAdorner.Glyphs.Clear();
                    this.m_SelectionAdorner = null;
                }
                if (this.m_DrawImage != null)
                {
                    this.m_DrawImage.Dispose();
                    this.m_DrawImage = null;
                }
                if (this.m_CurSelectedShapes != null)
                {
                    this.m_CurSelectedShapes.Clear();
                }
                if (this.m_PrevSelectedShapes != null)
                {
                    this.m_PrevSelectedShapes.Clear();
                }
                this.m_Helper = null;
                this.m_Host = null;
            }
            this.m_Disposed = true;
        }

        public void DrawAdornments()
        {
            if (this.m_DrawImage != null)
            {
                Graphics adornerWindowGraphics = this.BehaviorService.AdornerWindowGraphics;
                try
                {
                    Rectangle rect = new Rectangle(0, 0, this.m_DrawImage.Width, this.m_DrawImage.Height);
                    adornerWindowGraphics.SetClip(rect);
                    Point point = new Point(0, 0);
                    adornerWindowGraphics.DrawImage(this.m_DrawImage, point);
                }
                catch (ArgumentNullException exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    ArgumentNullException exception = exception1;
                    //ProjectData.ClearProjectError();
                }
                finally
                {
                    if (adornerWindowGraphics != null)
                    {
                        adornerWindowGraphics.Dispose();
                    }
                }
            }
        }

        private void DrawAdornmentsToImage()
        {
            Point screenOffset = this.BehaviorService.AdornerWindowToScreen();
            object objectValue = RuntimeHelpers.GetObjectValue(this.SelectionService.PrimarySelection);
            Size screenSize = DesignerUtility.GetScreenSize();
            Size designSurfaceSize = this.GetDesignSurfaceSize();
            if (this.m_DrawImage != null)
            {
                this.m_DrawImage.Dispose();
            }
            int width = Math.Max(screenSize.Width, designSurfaceSize.Width);
            this.m_DrawImage = new Bitmap(width, Math.Max(screenSize.Height, designSurfaceSize.Height), PixelFormat.Format32bppPArgb);
            this.m_CurSelectedShapes = new Dictionary<Shape, SelectObject>();
            using (Graphics graphics = Graphics.FromImage(this.m_DrawImage))
            {
                int count = this.SelectedShapes.Count;
                for (int i = 1; i <= count; i++)
                {
                    Shape sh = this.SelectedShapes[i - 1];
                    ShapeDesigner designer = this.EnsureCachedDesigner(sh);
                    if (designer != null)
                    {
                        Control parent = null;
                        if (sh.Parent != null)
                        {
                            parent = sh.Parent.Parent;
                        }
                        if (parent != null)
                        {
                            SelectObject obj3 = null;
                            if (sh.Equals(RuntimeHelpers.GetObjectValue(objectValue)))
                            {
                                obj3 = designer.DrawAdornments(graphics, true, screenOffset, parent.BackColor);
                            }
                            else
                            {
                                obj3 = designer.DrawAdornments(graphics, false, screenOffset, parent.BackColor);
                            }
                            this.m_CurSelectedShapes.Add(sh, obj3);
                        }
                    }
                }
            }
        }

        public ShapeDesigner EnsureCachedDesigner(Shape sh)
        {
            ShapeDesigner designer = null;
            if (this.m_ShapeDesignerMap.ContainsKey(sh))
            {
                return this.m_ShapeDesignerMap[sh];
            }
            IDesignerHost designerHost = this.DesignerHost;
            if (designerHost != null)
            {
                designer = (ShapeDesigner)designerHost.GetDesigner(sh);
                if (designer != null)
                {
                    this.m_ShapeDesignerMap.Add(sh, designer);
                }
            }
            return designer;
        }

        private Size GetDesignSurfaceSize()
        {
            Size size = new Size(0, 0);
            IDesignerHost designerHost = this.DesignerHost;
            if (designerHost != null)
            {
                Control rootComponent = (Control)designerHost.RootComponent;
                if (rootComponent != null)
                {
                    size = rootComponent.Size;
                }
            }
            return new Size((int)Math.Round((double)(size.Width * 1.2f)), (int)Math.Round((double)(size.Height * 1.2f)));
        }

        public Cursor GetHitTest(Point p)
        {
            Point point = this.BehaviorService.AdornerWindowToScreen();
            p.Offset(point);
            int count = this.SelectedShapes.Count;
            for (int i = 1; i <= count; i++)
            {
                Shape sh = this.SelectedShapes[i - 1];
                ShapeDesigner designer = this.EnsureCachedDesigner(sh);
                if (designer != null)
                {
                    HitFlag adornmentsHitTest = designer.GetAdornmentsHitTest(p, false);
                    if (adornmentsHitTest != HitFlag.HitNone)
                    {
                        this.m_HitShape = sh;
                        this.m_HitFlag = adornmentsHitTest;
                        return designer.GetCursor(adornmentsHitTest);
                    }
                }
            }
            this.m_HitShape = null;
            this.m_HitFlag = HitFlag.HitNone;
            return null;
        }

        private Region GetUpdateRegion(bool updateAll)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.FillMode = FillMode.Winding;
                if (updateAll)
                {
                    foreach (SelectObject obj2 in this.m_PrevSelectedShapes.Values)
                    {
                        path.AddRectangle(obj2.DrawBounds[0]);
                        path.AddRectangle(obj2.DrawBounds[1]);
                        if (typeof(SelectSimpleObject).IsAssignableFrom(obj2.GetType()))
                        {
                            path.AddRectangle(obj2.DrawBounds[2]);
                            path.AddRectangle(obj2.DrawBounds[3]);
                        }
                    }
                }
                else
                {
                    foreach (Shape shape in this.m_PrevSelectedShapes.Keys)
                    {
                        SelectObject obj3 = this.m_PrevSelectedShapes[shape];
                        if (this.m_CurSelectedShapes.ContainsKey(shape))
                        {
                            SelectObject selShape = this.m_CurSelectedShapes[shape];
                            if (!obj3.StatesEquals(selShape))
                            {
                                continue;
                            }
                        }
                        path.AddRectangle(obj3.DrawBounds[0]);
                        path.AddRectangle(obj3.DrawBounds[1]);
                        if (typeof(SelectSimpleObject).IsAssignableFrom(obj3.GetType()))
                        {
                            path.AddRectangle(obj3.DrawBounds[2]);
                            path.AddRectangle(obj3.DrawBounds[3]);
                        }
                    }
                }
                return new Region(path);
            }
        }

        public void HideAdorner()
        {
            if (this.BehaviorService.Adorners.Contains(this.SelectionAdorner))
            {
                if (this.m_CurSelectedShapes != null)
                {
                    if (this.m_PrevSelectedShapes != null)
                    {
                        this.m_CurSelectedShapes.Clear();
                        this.m_CurSelectedShapes = null;
                    }
                    else
                    {
                        this.m_PrevSelectedShapes = this.m_CurSelectedShapes;
                        this.m_CurSelectedShapes = null;
                    }
                }
                if (this.m_PrevSelectedShapes != null)
                {
                    using (Region region = this.GetUpdateRegion(true))
                    {
                        this.BehaviorService.Invalidate(region);
                    }
                    this.m_PrevSelectedShapes.Clear();
                    this.m_PrevSelectedShapes = null;
                }
                if (this.m_DrawImage != null)
                {
                    this.m_DrawImage.Dispose();
                    this.m_DrawImage = null;
                }
                this.BehaviorService.Adorners.Remove(this.SelectionAdorner);
            }
        }

        public bool IsHit()
        {
            return (this.m_HitShape != null);
        }

        private void OnComponentAdded(object source, ComponentEventArgs ce)
        {
            if (typeof(Shape).IsAssignableFrom(ce.Component.GetType()))
            {
                IDesigner designer = this.m_DesignerHost.GetDesigner(ce.Component);
                if ((designer != null) && typeof(ShapeDesigner).IsAssignableFrom(designer.GetType()))
                {
                    this.m_ShapeDesignerMap.Add((Shape)ce.Component, (ShapeDesigner)designer);
                }
            }
        }

        private void OnComponentChanged(object source, ComponentChangedEventArgs ce)
        {
            if (this.SelectionService.GetComponentSelected(RuntimeHelpers.GetObjectValue(ce.Component)))
            {
                if (!this.m_DesignerHost.InTransaction)
                {
                    this.Refresh();
                }
                else
                {
                    this.m_NeedRefresh = true;
                }
            }
        }

        private void OnComponentRemoved(object source, ComponentEventArgs ce)
        {
            if (typeof(Shape).IsAssignableFrom(ce.Component.GetType()))
            {
                Shape component = (Shape)ce.Component;
                if (this.m_ShapeDesignerMap.ContainsKey(component))
                {
                    this.m_ShapeDesignerMap.Remove(component);
                }
            }
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (!this.m_InSelectionChange)
            {
                ISelectionService selectionService = null;
                try
                {
                    this.m_InSelectionChange = true;
                    selectionService = this.SelectionService;
                    if (selectionService != null)
                    {
                        if (selectionService.SelectionCount == 0)
                        {
                            this.Clear();
                        }
                        else if ((selectionService.PrimarySelection != null) && !typeof(Shape).IsAssignableFrom(selectionService.PrimarySelection.GetType()))
                        {
                            this.Clear();
                        }
                        else
                        {
                            IDesignerHost designerHost = this.DesignerHost;
                            if (designerHost != null)
                            {
                                IEnumerator enumerator = null;
                                List<IComponent> components = new List<IComponent>();
                                this.SelectedShapes.Clear();
                                try
                                {
                                    enumerator = this.SelectionService.GetSelectedComponents().GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        IComponent current = (IComponent)enumerator.Current;
                                        if (!typeof(Shape).IsAssignableFrom(((Component)current).GetType()))
                                        {
                                            components.Add(current);
                                        }
                                        else
                                        {
                                            object designer = designerHost.GetDesigner(current);
                                            if ((designer != null) && typeof(ShapeDesigner).IsAssignableFrom(designer.GetType()))
                                            {
                                                this.SelectedShapes.Add((Shape)current);
                                                continue;
                                            }
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
                                if ((components.Count > 0) && (this.SelectedShapes.Count > 0))
                                {
                                    this.Clear();
                                    this.SelectionService.SetSelectedComponents(components, SelectionTypes.Replace);
                                }
                                else
                                {
                                    if (this.SelectedShapes.Count > 0)
                                    {
                                        if (this.m_PrevSelectedShapes == null)
                                        {
                                            this.ShowAdorner();
                                        }
                                        else
                                        {
                                            this.DrawAdornmentsToImage();
                                            using (Region region = this.GetUpdateRegion(false))
                                            {
                                                this.BehaviorService.Invalidate(region);
                                            }
                                        }
                                        this.DrawAdornments();
                                        if (this.m_PrevSelectedShapes != null)
                                        {
                                            this.m_PrevSelectedShapes.Clear();
                                        }
                                        this.m_PrevSelectedShapes = this.m_CurSelectedShapes;
                                        this.m_CurSelectedShapes = null;
                                    }
                                    object objectValue = RuntimeHelpers.GetObjectValue(this.SelectionService.PrimarySelection);
                                    if ((objectValue != null) && typeof(Shape).IsAssignableFrom(objectValue.GetType()))
                                    {
                                        this.m_PrimarySelection = (Shape)objectValue;
                                        this.Helper.ShapeContainerCommandSet.StatusBarCommand.SetStatusBarInfo(this.m_PrimarySelection);
                                    }
                                    else
                                    {
                                        this.m_PrimarySelection = null;
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (selectionService != null)
                    {
                        this.Helper.ShapeContainerCommandSet.OnSelectionChanged(selectionService);
                    }
                    this.m_InSelectionChange = false;
                }
            }
        }

        private void OnSynchronize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (e.LastTransaction && this.m_NeedRefresh)
            {
                this.Refresh();
            }
        }

        internal void Refresh()
        {
            this.m_NeedRefresh = false;
            this.OnSelectionChanged(this, null);
        }

        public void ShowAdorner()
        {
            if (!this.BehaviorService.Adorners.Contains(this.SelectionAdorner))
            {
                this.DrawAdornmentsToImage();
                this.BehaviorService.Adorners.Add(this.SelectionAdorner);
            }
        }

        public void StartResize(Point p)
        {
            if (!this.Helper.InDrag)
            {
                this.Helper.InDrag = true;
                try
                {
                    Point point = this.BehaviorService.AdornerWindowToScreen();
                    p.Offset(point);
                    ShapeContainer parent = this.m_HitShape.Parent;
                    ShapeContainerDesigner sourceDesigner = (ShapeContainerDesigner)this.m_DesignerHost.GetDesigner(parent);
                    List<Shape> selectedShapes = null;
                    if (this.m_HitFlag == HitFlag.HitMove)
                    {
                        selectedShapes = new List<Shape>();
                        foreach (Shape shape in this.SelectedShapes)
                        {
                            if ((shape.Parent != null) && shape.Parent.Equals(this.m_HitShape.Parent))
                            {
                                selectedShapes.Add(shape);
                            }
                        }
                    }
                    else
                    {
                        selectedShapes = this.SelectedShapes;
                    }
                    object[] array = new object[(selectedShapes.Count - 1) + 1];
                    int num2 = selectedShapes.Count - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        array[i] = selectedShapes[i];
                    }
                    Array.Sort(array, new ShapeContainerCommandSet.ComponentZOrderCompare());
                    List<Shape> comps = new List<Shape>();
                    foreach (Shape shape2 in array)
                    {
                        comps.Add(shape2);
                    }
                    this.Helper.DragManager.DoDragDrop(sourceDesigner, comps, this.m_HitFlag, p, false);
                }
                finally
                {
                    this.Helper.InDrag = false;
                }
            }
        }

        private BehaviorService BehaviorService
        {
            get
            {
                return this.m_BehaviorService;
            }
        }

        private IDesignerHost DesignerHost
        {
            get
            {
                return this.m_Host;
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

        internal Shape PrimarySelection
        {
            get
            {
                return this.m_PrimarySelection;
            }
        }

        private List<Shape> SelectedShapes
        {
            get
            {
                return this.m_SelectedShapes;
            }
        }

        private Adorner SelectionAdorner
        {
            get
            {
                return this.m_SelectionAdorner;
            }
        }

        private ISelectionService SelectionService
        {
            get
            {
                return this.m_SelectionService;
            }
        }
    }
}