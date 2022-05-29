using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    internal sealed class DragObject : IDisposable
    {
        private bool m_AddSelection;
        private Bitmap m_BackImage;
        private Cursor m_Cursor;
        private bool m_Disposed;
        private bool m_DragCleared;
        private Bitmap m_DragImage;
        private Size m_DragImageSize;
        private List<DragShape> m_DragShapes;
        private DragGlyph m_Glyph;
        private HitFlag m_HitFlag = HitFlag.HitNone;
        private Point m_InitialMouseLocation;
        private bool m_LastCtrlPressed;
        private Point m_LastFeedbackLocation;
        private DesignerUtility.DragResult m_Result = DesignerUtility.DragResult.None;
        private Bitmap m_ScreenShot;
        private Bitmap m_ScreenShotCtrlPressed;
        private ShapeContainerDesigner m_SourceDesigner;
        private ShapeContainer m_SourceShapeContainer;
        private StatusBarCommand m_StatusbarCommand;
        private ShapeContainer m_TargetShapeContainer;

        public DragObject(ShapeContainerDesigner designer, List<Shape> comps, DragGlyph glyph, HitFlag hitFlag, Point loc, Size screenSize, bool addSelection)
        {
            this.m_AddSelection = addSelection;
            this.m_DragImageSize = screenSize;
            this.m_SourceDesigner = designer;
            this.m_Glyph = glyph;
            this.m_HitFlag = hitFlag;
            this.m_InitialMouseLocation = loc;
            this.m_DragShapes = new List<DragShape>();
            this.InitiateDragShapes(comps);
        }

        public void CancelDrag()
        {
            this.ClearAllDragImage(true);
            IDesignerHost designerHost = this.DesignerHost;
            if (designerHost != null)
            {
                foreach (DragShape shape in this.m_DragShapes)
                {
                    ShapeDesigner designer = (ShapeDesigner)designerHost.GetDesigner(shape.DraggedShape);
                    designer.OnDragging = false;
                    DesignerUtility.InvalidateShapeBounds(shape.DraggedShape);
                }
            }
            this.Glyph.BehaviorService.Adorners.Remove(this.Glyph.Adorenr);
            this.Result = DesignerUtility.DragResult.Canceled;
        }

        public void ClearAllDragImage(bool repaint)
        {
            if (!this.m_DragCleared)
            {
                if (repaint && (this.m_BackImage != null))
                {
                    Point p = new Point(0, 0);
                    Point point = this.Glyph.FromScreenToAdorner(p);
                    this.Glyph.Graphics.DrawImage(this.m_BackImage, point.X, point.Y, this.m_BackImage.Width, this.m_BackImage.Height);
                }
                if (this.m_DragImage != null)
                {
                    this.m_DragImage.Dispose();
                    this.m_DragImage = null;
                }
                this.m_DragCleared = true;
            }
        }

        private ArrayList CreateCopiedShapes(ShapeContainer shCon, int x, int y)
        {
            IDesignerHost designerHost = this.DesignerHost;
            if (designerHost == null)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            foreach (DragShape shape in this.m_DragShapes)
            {
                Shape draggedShape = shape.DraggedShape;
                Shape tarShape = (Shape)designerHost.CreateComponent(draggedShape.GetType());
                DesignerUtility.CopyShapeProperties(ref draggedShape, ref tarShape);
                list.Add(tarShape);
            }
            return list;
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
                this.ClearAllDragImage(false);
                if (this.m_ScreenShot != null)
                {
                    this.m_ScreenShot.Dispose();
                    this.m_ScreenShot = null;
                }
                if (this.m_ScreenShotCtrlPressed != null)
                {
                    this.m_ScreenShotCtrlPressed.Dispose();
                    this.m_ScreenShotCtrlPressed = null;
                }
                this.m_BackImage = null;
                foreach (DragShape shape in this.m_DragShapes)
                {
                    shape.Dispose();
                }
            }
            this.m_Disposed = true;
        }

        public void Drop(int x, int y)
        {
            this.ClearAllDragImage(true);
            Point point = new Point(x, y);
            x = point.X - this.m_InitialMouseLocation.X;
            y = point.Y - this.m_InitialMouseLocation.Y;
            if (this.DesignerHost != null)
            {
                try
                {
                    this.DropShapes(this.SourceShapeContainer, x, y, this.OnDragMove && (Control.ModifierKeys == Keys.Control), false);
                }
                catch (Exception exception1)
                {
                    //ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    //ProjectData.ClearProjectError();
                }
            }
            this.Result = DesignerUtility.DragResult.Dropped;
        }

        public void Drop(int x, int y, ShapeContainer source, ShapeContainer target)
        {
            this.ClearAllDragImage(true);
            Point p = new Point(0, 0);
            Point point = source.PointToScreen(p);
            p = new Point(0, 0);
            Point point2 = target.PointToScreen(p);
            Point point3 = new Point(x, y);
            x = ((point3.X - this.m_InitialMouseLocation.X) + point.X) - point2.X;
            y = ((point3.Y - this.m_InitialMouseLocation.Y) + point.Y) - point2.Y;
            try
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    this.DropShapes(target, x, y, true, false);
                }
                else
                {
                    IComponentChangeService changeService = this.ChangeService;
                    if (changeService != null)
                    {
                        changeService.OnComponentChanging(source, null);
                        changeService.OnComponentChanging(target, null);
                        this.DropShapes(target, x, y, false, true);
                        changeService.OnComponentChanged(source, null, null, null);
                        changeService.OnComponentChanged(target, null, null, null);
                        if (source.Shapes.Count == 0)
                        {
                            Control parent = source.Parent;
                            if (parent != null)
                            {
                                changeService.OnComponentChanging(parent, null);
                                parent.Controls.Remove(source);
                                changeService.OnComponentChanged(parent, null, null, null);
                                ISelectionService selectionService = this.SelectionService;
                                if (selectionService != null)
                                {
                                    ICollection selectedComponents = selectionService.GetSelectedComponents();
                                    this.DesignerHost.DestroyComponent(source);
                                    selectionService.SetSelectedComponents(selectedComponents, SelectionTypes.Replace);
                                }
                                else
                                {
                                    this.DesignerHost.DestroyComponent(source);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                this.Result = DesignerUtility.DragResult.Dropped;
            }
        }

        private void DropShapes(ShapeContainer shCon, int x, int y, bool copy, bool changeContainer)
        {
            IComponentChangeService changeService = this.ChangeService;
            if (changeService != null)
            {
                ISelectionService selectionService = this.SelectionService;
                if (selectionService != null)
                {
                    IDesignerHost designerHost = this.DesignerHost;
                    if (designerHost != null)
                    {
                        if (copy)
                        {
                            ArrayList components = this.CreateCopiedShapes(shCon, x, y);
                            if ((components != null) && (components.Count > 0))
                            {
                                IEnumerator enumerator = null;
                                IEnumerator enumerator2 = null;
                                changeService.OnComponentChanging(shCon, null);
                                try
                                {
                                    enumerator = components.GetEnumerator();
                                    while (enumerator.MoveNext())
                                    {
                                        Shape current = (Shape)enumerator.Current;
                                        shCon.Shapes.Insert(0, current);
                                    }
                                }
                                finally
                                {
                                    if (enumerator is IDisposable)
                                    {
                                        (enumerator as IDisposable).Dispose();
                                    }
                                }
                                changeService.OnComponentChanged(shCon, null, null, null);
                                try
                                {
                                    enumerator2 = components.GetEnumerator();
                                    while (enumerator2.MoveNext())
                                    {
                                        Shape component = (Shape)enumerator2.Current;
                                        ShapeDesigner designer = (ShapeDesigner)designerHost.GetDesigner(component);
                                        designer.OnDragging = true;
                                        designer.Drag(this.m_HitFlag, x, y);
                                        component.Invalidate();
                                    }
                                }
                                finally
                                {
                                    if (enumerator2 is IDisposable)
                                    {
                                        (enumerator2 as IDisposable).Dispose();
                                    }
                                }
                                selectionService.SetSelectedComponents(components, SelectionTypes.Replace);
                            }
                        }
                        else
                        {
                            try
                            {
                                List<DragShape>.Enumerator enumerator3 = new List<DragShape>.Enumerator();
                                try
                                {
                                    enumerator3 = this.m_DragShapes.GetEnumerator();
                                    while (enumerator3.MoveNext())
                                    {
                                        DragShape shape3 = enumerator3.Current;
                                        Shape draggedShape = shape3.DraggedShape;
                                        ShapeDesigner draggedDesigner = shape3.DraggedDesigner;
                                        changeService.OnComponentChanging(draggedShape, null);
                                        draggedDesigner.Drag(this.m_HitFlag, x, y);
                                        if (changeContainer)
                                        {
                                            shCon.Shapes.Insert(0, draggedShape);
                                        }
                                        changeService.OnComponentChanged(draggedShape, null, null, null);
                                    }
                                }
                                finally
                                {
                                    enumerator3.Dispose();
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
                            }
                            finally
                            {
                                List<DragShape>.Enumerator enumerator4 = new List<DragShape>.Enumerator();
                                try
                                {
                                    enumerator4 = this.m_DragShapes.GetEnumerator();
                                    while (enumerator4.MoveNext())
                                    {
                                        DragShape shape5 = enumerator4.Current;
                                        Shape shape6 = shape5.DraggedShape;
                                        shape5.DraggedDesigner.OnDragging = false;
                                        shape6.Invalidate();
                                    }
                                }
                                finally
                                {
                                    enumerator4.Dispose();
                                }
                            }
                            if (this.m_AddSelection && (selectionService != null))
                            {
                                Shape shape7 = this.m_DragShapes[this.m_DragShapes.Count - 1].DraggedShape;
                                selectionService.SetSelectedComponents(new IComponent[] { shape7 }, SelectionTypes.Add | SelectionTypes.Click);
                            }
                        }
                    }
                }
            }
        }

        private void GenerateBitmap(ref Bitmap image, Point offset, Control newContainer)
        {
            image = new Bitmap(this.m_DragImageSize.Width, this.m_DragImageSize.Height, PixelFormat.Format32bppPArgb);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                if (this.m_BackImage != null)
                {
                    graphics.DrawImage(this.m_BackImage, 0, 0, this.m_BackImage.Width, this.m_BackImage.Height);
                }
                int count = this.m_DragShapes.Count;
                for (int i = 1; i <= count; i++)
                {
                    Graphics graphics2;
                    ShapeDesigner draggedDesigner = this.m_DragShapes[i - 1].DraggedDesigner;
                    if (i == 1)
                    {
                        graphics2 = graphics;
                        draggedDesigner.DrawFeedback(this.m_HitFlag, offset, ref graphics2, this.StatusbarCommand, newContainer);
                    }
                    else
                    {
                        graphics2 = graphics;
                        draggedDesigner.DrawFeedback(this.m_HitFlag, offset, ref graphics2, null, null);
                    }
                }
            }
        }

        private Bitmap GetScreenShot(bool ctrlPressed)
        {
            if (ctrlPressed)
            {
                if (this.m_ScreenShotCtrlPressed == null)
                {
                    this.m_ScreenShotCtrlPressed = this.GetScreenShotInternal();
                }
                return this.m_ScreenShotCtrlPressed;
            }
            if (this.m_ScreenShot == null)
            {
                this.m_ScreenShot = this.GetScreenShotInternal();
            }
            return this.m_ScreenShot;
        }

        private Bitmap GetScreenShotInternal()
        {
            Bitmap image = null;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary)
                {
                    image = new Bitmap(this.m_DragImageSize.Width, this.m_DragImageSize.Height, PixelFormat.Format32bppPArgb);
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        Point upperLeftDestination = new Point(0, 0);
                        graphics.CopyFromScreen(screen.Bounds.Location, upperLeftDestination, this.m_DragImageSize);
                    }
                    return image;
                }
            }
            return image;
        }

        public void GiveFeedback(Control newContainer)
        {
            if (!this.OnDragMove)
            {
                Cursor.Current = this.m_Cursor;
            }
            Point mousePosition = Control.MousePosition;
            if (!mousePosition.Equals(this.m_LastFeedbackLocation))
            {
                this.HideShowShapes();
                Point offset = mousePosition;
                offset.Offset(0 - this.m_InitialMouseLocation.X, 0 - this.m_InitialMouseLocation.Y);
                this.ClearAllDragImage(false);
                this.GenerateBitmap(ref this.m_DragImage, offset, newContainer);
                Point p = new Point(0, 0);
                p = this.Glyph.FromScreenToAdorner(p);
                using (Graphics graphics = this.Glyph.Graphics)
                {
                    Rectangle rect = new Rectangle(p, this.m_DragImageSize);
                    graphics.SetClip(rect);
                    graphics.DrawImage(this.m_DragImage, p);
                }
                this.m_LastFeedbackLocation = mousePosition;
            }
            this.m_DragCleared = false;
        }

        private bool HideShowShapes()
        {
            bool flag2 = Control.ModifierKeys == Keys.Control;
            if (flag2 == this.m_LastCtrlPressed)
            {
                return false;
            }
            if (this.OnDragMove)
            {
                if (flag2)
                {
                    this.m_BackImage = this.m_ScreenShotCtrlPressed;
                }
                else
                {
                    this.m_BackImage = this.m_ScreenShot;
                }
                List<ShapeContainer> list = null;
                foreach (DragShape shape in this.m_DragShapes)
                {
                    Shape draggedShape = shape.DraggedShape;
                    ShapeDesigner draggedDesigner = shape.DraggedDesigner;
                    if (flag2)
                    {
                        draggedDesigner.OnDragging = false;
                        draggedShape.Invalidate();
                    }
                    else
                    {
                        draggedDesigner.OnDragging = true;
                        draggedShape.Invalidate();
                    }
                    if (list == null)
                    {
                        list = new List<ShapeContainer>();
                    }
                    if ((draggedShape.Parent != null) && !list.Contains(draggedShape.Parent))
                    {
                        list.Add(draggedShape.Parent);
                    }
                }
                if (list != null)
                {
                    foreach (ShapeContainer container in list)
                    {
                        if (container.Parent != null)
                        {
                            container.Parent.Update();
                            container.Update();
                        }
                    }
                    list.Clear();
                }
                if ((this.m_BackImage == null) && !flag2)
                {
                    this.Glyph.BehaviorService.Adorners.Remove(this.Glyph.Adorenr);
                    this.Glyph.BehaviorService.Invalidate();
                    this.m_BackImage = this.GetScreenShot(false);
                    this.Glyph.BehaviorService.Adorners.Add(this.Glyph.Adorenr);
                }
            }
            this.m_LastCtrlPressed = flag2;
            return true;
        }

        private void InitiateDragShapes(List<Shape> comps)
        {
            this.m_LastCtrlPressed = Control.ModifierKeys == Keys.Control;
            if (this.OnDragMove)
            {
                this.m_BackImage = this.GetScreenShot(true);
            }
            List<ShapeContainer> list = null;
            int count = comps.Count;
            for (int i = 1; i <= count; i++)
            {
                Shape component = comps[i - 1];
                ShapeDesigner des = (ShapeDesigner)this.DesignerHost.GetDesigner(component);
                component.Parent.PointToScreen(component.BoundRect.Location).Offset(0 - this.m_InitialMouseLocation.X, 0 - this.m_InitialMouseLocation.Y);
                DragShape item = null;
                if ((des != null) && des.CanDrag(this.m_HitFlag))
                {
                    item = new DragShape(component, des);
                    this.m_DragShapes.Add(item);
                    if (!this.OnDragMove || !this.m_LastCtrlPressed)
                    {
                        des.OnDragging = true;
                        component.Invalidate();
                        if (list == null)
                        {
                            list = new List<ShapeContainer>();
                        }
                        if ((component.Parent != null) && !list.Contains(component.Parent))
                        {
                            list.Add(component.Parent);
                        }
                    }
                }
            }
            if (list != null)
            {
                foreach (ShapeContainer container in list)
                {
                    if (container.Parent != null)
                    {
                        container.Parent.Update();
                        container.Update();
                    }
                }
                this.m_BackImage = this.GetScreenShot(false);
                list.Clear();
            }
            if (this.m_HitFlag == HitFlag.HitMove)
            {
                this.m_Cursor = Cursors.Default;
            }
            else
            {
                this.m_Cursor = this.m_DragShapes[0].DraggedDesigner.GetCursor(this.m_HitFlag);
            }
        }

        private IComponentChangeService ChangeService
        {
            get
            {
                return this.m_SourceDesigner.ChangeService;
            }
        }

        private IDesignerHost DesignerHost
        {
            get
            {
                return this.m_SourceDesigner.DesignerHost;
            }
        }

        private DragGlyph Glyph
        {
            get
            {
                return this.m_Glyph;
            }
        }

        public bool OnDragMove
        {
            get
            {
                return (this.m_HitFlag == HitFlag.HitMove);
            }
        }

        internal DesignerUtility.DragResult Result
        {
            get
            {
                return this.m_Result;
            }
            set
            {
                this.m_Result = value;
            }
        }

        private ISelectionService SelectionService
        {
            get
            {
                return this.m_SourceDesigner.SelectionService;
            }
        }

        private ShapeContainer SourceShapeContainer
        {
            get
            {
                if (this.m_SourceShapeContainer == null)
                {
                    this.m_SourceShapeContainer = this.m_SourceDesigner.ShapeContainer;
                }
                return this.m_SourceShapeContainer;
            }
        }

        internal StatusBarCommand StatusbarCommand
        {
            get
            {
                return this.m_StatusbarCommand;
            }
            set
            {
                this.m_StatusbarCommand = value;
            }
        }

        private ShapeContainer TargetShapeContainer
        {
            get
            {
                return this.m_TargetShapeContainer;
            }
            set
            {
                this.m_TargetShapeContainer = value;
            }
        }

        private class DragShape : IDisposable
        {
            private bool m_Disposed;
            private ShapeDesigner m_DraggedDesigner;
            private Shape m_DraggedShape;

            public DragShape(Shape obj, ShapeDesigner des)
            {
                this.m_DraggedShape = obj;
                this.m_DraggedDesigner = des;
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!this.m_Disposed && disposing)
                {
                    this.m_DraggedDesigner = null;
                    this.m_DraggedShape = null;
                }
                this.m_Disposed = true;
            }

            public ShapeDesigner DraggedDesigner
            {
                get
                {
                    return this.m_DraggedDesigner;
                }
            }

            public Shape DraggedShape
            {
                get
                {
                    return this.m_DraggedShape;
                }
            }
        }
    }
}