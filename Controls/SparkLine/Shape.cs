using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    [DefaultEvent("Click")]
    public abstract class Shape : Component
    {
        internal const AnchorStyles DefaultAnchor = (AnchorStyles.Left | AnchorStyles.Top);
        private AccessibleObject m_AccessibilityObject;
        private string m_AccessibleDefaultActionDescription;
        private string m_AccessibleDescription;
        private string m_AccessibleName;
        private AccessibleRole m_AccessibleRole;
        private AnchorStyles m_Anchor;
        private int m_BorderWidth;
        private ContextMenu m_ContextMenu;
        private ContextMenuStrip m_ContextMenuStrip;
        private Region m_HitTestRegion;
        private bool m_InSetVirtualBounds;
        private string m_Name;
        private Cursor m_OldCursor;
        private ShapeContainer m_Parent;
        private Pen m_Pen;
        private Dictionary<string, object> m_Properties;
        private Region m_Region;
        private Color m_SelectionColor;
        private int m_State;
        private object m_Tag;
        private Rectangle m_VirtualBounds;
        internal const int MaxInt = 0x7fff;
        internal const int STATE_CANFOCUS = 1;
        internal const int STATE_CANSELECT = 2;
        internal const int STATE_CREATED = 8;
        internal const int STATE_DISPOSED = 0x10;
        internal const int STATE_DISPOSING = 0x20;
        internal const int STATE_ENABLED = 0x40;
        internal const int STATE_FOCUSED = 0x80;
        internal const int STATE_ISACCESSIBLE = 0x100;
        internal const int STATE_RECREATE = 0x200;
        internal const int STATE_SELECTED = 0x800;
        internal const int STATE_SUSPENDPAINT = 0x400;
        internal const int STATE_USEWAITCURSOR = 0x2000;
        internal const int STATE_VISIBLE = 0x4000;

        [Category("Behavior"), Description("ShapeEventChangeUICues"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
        public event UICuesEventHandler ChangeUICues;

        [Category("Action"), Description("ShapeEventClick"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
        public event EventHandler Click;

        [EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventContextMenuChanged"), Category("PropertyChanged"), Browsable(true)]
        public event EventHandler ContextMenuChanged;

        [Category("PropertyChanged"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Description("ShapeEventContextMenuStripChanged")]
        public event EventHandler ContextMenuStripChanged;

        [EditorBrowsable(EditorBrowsableState.Always), Category("PropertyChanged"), Browsable(true), Description("ShapeEventCursorChanged")]
        public event EventHandler CursorChanged;

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventDoubleClick"), Category("Action")]
        public event EventHandler DoubleClick;

        [Category("PropertyChanged"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventEnabledChanged")]
        public event EventHandler EnabledChanged;

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventEnter"), Category("Focus")]
        public event EventHandler Enter;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public event EventHandler GotFocus;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public event InvalidateEventHandler Invalidated;

        [Category("Key"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventKeyDown")]
        public event KeyEventHandler KeyDown;

        [Browsable(true), Category("Key"), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventKeyPress")]
        public event KeyPressEventHandler KeyPress;

        [EditorBrowsable(EditorBrowsableState.Always), Category("Key"), Browsable(true), Description("ShapeEventKeyUp")]
        public event KeyEventHandler KeyUp;

        [Category("Focus"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventLeave")]
        public event EventHandler Leave;

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Focus"), Description("ShapeEventLostFocus")]
        public event EventHandler LostFocus;

        [Browsable(true), Description("ShapeEventMouseClick"), Category("Action"), EditorBrowsable(EditorBrowsableState.Always)]
        public event MouseEventHandler MouseClick;

        [Description("ShapeEventMouseDoubleClick"), EditorBrowsable(EditorBrowsableState.Always), Category("Action"), Browsable(true)]
        public event MouseEventHandler MouseDoubleClick;

        [Description("ShapeEventMouseDown"), EditorBrowsable(EditorBrowsableState.Always), Category("Mouse"), Browsable(true)]
        public event MouseEventHandler MouseDown;

        [EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventMouseEnter"), Category("Mouse"), Browsable(true)]
        public event EventHandler MouseEnter;

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Category("Mouse"), Description("ShapeEventMouseHover")]
        public event EventHandler MouseHover;

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventMouseLeave"), Category("Mouse")]
        public event EventHandler MouseLeave;

        [Browsable(true), Category("Mouse"), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventMouseMove")]
        public event MouseEventHandler MouseMove;

        [EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventMouseUp"), Category("Mouse"), Browsable(true)]
        public event MouseEventHandler MouseUp;

        [Category("Mouse"), Description("ShapeEventMouseWheel"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
        public event MouseEventHandler MouseWheel;

        [EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventMove"), Category("Layout"), Browsable(true)]
        public event EventHandler Move;

        [EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventPaint"), Category("Appearance"), Browsable(true)]
        public event PaintEventHandler Paint;

        [Category("PropertyChanged"), Description("ShapeEventParentChanged"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
        public event EventHandler ParentChanged;

        [EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventPreviewKeyDown"), Category("Key"), Browsable(true)]
        public event PreviewKeyDownEventHandler PreviewKeyDown;

        [Category("Behavior"), Description("ShapeEventQueryAccessibilityHelp"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp;

        [Browsable(true), Category("PropertyChanged"), EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventRegionChanged")]
        public event EventHandler RegionChanged;

        [EditorBrowsable(EditorBrowsableState.Always), Description("ShapeEventVisibleChanged"), Category("PropertyChanged"), Browsable(true)]
        public event EventHandler VisibleChanged;

        protected Shape()
        {
            this.m_AccessibleDefaultActionDescription = "Click";
            this.m_AccessibleDescription = null;
            this.m_AccessibleName = null;
            this.m_AccessibleRole = AccessibleRole.Client;
            this.m_Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this.m_BorderWidth = 1;
            this.m_Name = string.Empty;
            this.m_Pen = (Pen)Pens.Black.Clone();
            this.m_VirtualBounds = Rectangle.Empty;
            this.Initialize();
        }

        protected Shape(ShapeContainer parent)
        {
            this.m_AccessibleDefaultActionDescription = "Click";
            this.m_AccessibleDescription = null;
            this.m_AccessibleName = null;
            this.m_AccessibleRole = AccessibleRole.Client;
            this.m_Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this.m_BorderWidth = 1;
            this.m_Name = string.Empty;
            this.m_Pen = (Pen)Pens.Black.Clone();
            this.m_VirtualBounds = Rectangle.Empty;
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            this.Initialize();
            parent.Shapes.Add(this);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void BringToFront()
        {
            ShapeContainer parent = this.Parent;
            if ((parent != null) && (parent.Shapes.IndexOf(this) != 0))
            {
                parent.Shapes.SetChildIndex(this, 0);
                this.Invalidate();
            }
        }

        internal void ClearCachedRegion()
        {
            if (this.m_HitTestRegion != null)
            {
                this.m_HitTestRegion.Dispose();
                this.m_HitTestRegion = null;
            }
        }

        internal void ClearVirtualBounds()
        {
            if (!this.m_InSetVirtualBounds)
            {
                this.m_VirtualBounds = Rectangle.Empty;
            }
        }

        protected abstract AccessibleObject CreateAccessibilityInstance();
        private void DetachContextMenu(object sender, EventArgs e)
        {
            this.ContextMenu = null;
        }

        private void DetachContextMenuStrip(object sender, EventArgs e)
        {
            this.ContextMenuStrip = null;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.SetState(0x20, true);
                    this.ContextMenu = null;
                    this.ContextMenuStrip = null;
                    if ((this.Parent != null) && (this.Parent is ShapeContainer))
                    {
                        this.Parent.Shapes.Remove(this);
                    }
                    if (this.m_Pen != null)
                    {
                        this.m_Pen.Dispose();
                        this.m_Pen = null;
                    }
                    if (this.m_Region != null)
                    {
                        this.m_Region.Dispose();
                        this.m_Region = null;
                    }
                    if (this.m_Properties != null)
                    {
                        this.m_Properties.Clear();
                        this.m_Properties = null;
                    }
                }
            }
            catch (NotSupportedException exception1)
            {
                //ProjectData.SetProjectError(exception1);
                NotSupportedException exception = exception1;
                //ProjectData.ClearProjectError();
            }
            finally
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    this.SetState(0x20, false);
                    this.SetState(0x10, true);
                }
            }
        }

        internal void DoQueryAccessibilityHelp(QueryAccessibilityHelpEventArgs e)
        {
            QueryAccessibilityHelp?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public abstract void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Form FindForm()
        {
            if (this.Parent != null)
            {
                return this.Parent.FindForm();
            }
            return null;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool Focus()
        {
            if (!this.DesignMode && this.CanFocus)
            {
                if (this.GetState(0x80))
                {
                    return true;
                }
                ShapeContainer parent = this.Parent;
                if (parent == null)
                {
                    return false;
                }
                Shape selectedShape = parent.SelectedShape;
                Shape focusedShape = parent.FocusedShape;
                if (focusedShape != null)
                {
                    focusedShape.OnLostFocus(EventArgs.Empty);
                    parent.FocusedShape = this;
                    if (selectedShape != null)
                    {
                        selectedShape.OnLeave(EventArgs.Empty);
                    }
                    parent.SelectedShape = this;
                    this.OnEnter(EventArgs.Empty);
                    this.OnGotFocus(EventArgs.Empty);
                    return true;
                }
                if ((selectedShape != null) && !selectedShape.Equals(this))
                {
                    selectedShape.OnLeave(EventArgs.Empty);
                }
                parent.SelectedShape = this;
                if (parent.Focus())
                {
                    return true;
                }
                parent.FocusedShape = null;
                this.SetState(0x80, false);
                parent.SelectedShape = null;
                if (this.GetState(0x800))
                {
                    this.SetState(0x800, false);
                    this.InvalidateFocusAreaOnly();
                }
            }
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IContainerControl GetContainerControl()
        {
            if ((this.Parent != null) && (this.Parent.Parent != null))
            {
                return this.Parent.Parent.GetContainerControl();
            }
            return null;
        }

        internal Region GetInvalidateRegion()
        {
            Region shapeRegion = this.ShapeRegion;
            if (!this.DesignMode)
            {
                Region focusRegion = this.FocusRegion;
                if (shapeRegion == null)
                {
                    return focusRegion;
                }
                if (focusRegion != null)
                {
                    shapeRegion.Union(focusRegion);
                    focusRegion.Dispose();
                }
            }
            return shapeRegion;
        }

        internal virtual Region GetRegionInternal(RegionType type)
        {
            return null;
        }

        internal bool GetState(int flag)
        {
            return ((this.m_State & flag) != 0);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Hide()
        {
            this.Visible = false;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public abstract bool HitTest(int x, int y);
        private void Initialize()
        {
            this.SetState(0x4140, true);
            this.SetState(0x400, false);
            this.m_Pen.DashStyle = DashStyle.Solid;
            this.m_Pen.Color = DefaultBorderColor;
            this.m_SelectionColor = DefaultSelectionColor;
            this.SetState(8, true);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void Invalidate()
        {
            if (this.CanInvalidate && this.Visible)
            {
                using (Region region = this.ShapeRegion)
                {
                    this.Invalidate(region);
                }
            }
        }

        internal void Invalidate(Region region)
        {
            if ((region != null) && (this.CanInvalidate || !this.Visible))
            {
                if (!this.DesignMode && this.GetState(0x800))
                {
                    Region focusRegion = this.FocusRegion;
                    if (focusRegion != null)
                    {
                        region.Union(focusRegion);
                    }
                }
                if (this.Region != null)
                {
                    region.Intersect(this.Region);
                }
                this.InvalidateInternal(region, true);
            }
        }

        internal virtual void InvalidateFocusAreaOnly()
        {
            if (this.CanInvalidate)
            {
                using (Region region = this.GetRegionInternal(RegionType.FocusInvalidate))
                {
                    if (region != null)
                    {
                        if (this.Region != null)
                        {
                            region.Intersect(this.Region);
                        }
                        this.InvalidateInternal(region, false);
                    }
                }
            }
        }

        internal void InvalidateInternal(Region region, bool fireEvent)
        {
            Point p = new Point(0, 0);
            Point point = this.Parent.PointToScreen(p);
            point = this.Parent.Parent.PointToClient(point);
            Rectangle boundRect = this.BoundRect;
            region.Translate((int)(boundRect.X + point.X), (int)(this.BoundRect.Y + point.Y));
            this.Parent.Parent.Invalidate(region);
            region.Translate((int)(0 - point.X), (int)(0 - point.Y));
            this.Parent.Invalidate(region);
            if (fireEvent)
            {
                p = new Point(0, 0);
                boundRect = new Rectangle(p, this.BoundRect.Size);
                this.OnInvalidated(new InvalidateEventArgs(boundRect));
            }
        }

        internal void InvalidateInternal(Region oldRegion, Region newRegion, bool fireEvent)
        {
            Region region = null;
            if ((oldRegion != null) && (newRegion == null))
            {
                region = oldRegion;
            }
            else if ((oldRegion != null) && (newRegion != null))
            {
                region = oldRegion;
                region.Union(newRegion);
            }
            else if ((oldRegion == null) && (newRegion != null))
            {
                region = newRegion;
            }
            if (region != null)
            {
                this.InvalidateInternal(region, fireEvent);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool IsKeyLocked(Keys keyVal)
        {
            return Control.IsKeyLocked(keyVal);
        }

        protected internal virtual void OnClick(EventArgs e)
        {
            Click?.Invoke(this, e);
        }

        protected virtual void OnContextMenuChanged(EventArgs e)
        {
            ContextMenuChanged?.Invoke(this, e);
        }

        protected virtual void OnContextMenuStripChanged(EventArgs e)
        {
            ContextMenuStripChanged?.Invoke(this, e);
        }

        protected virtual void OnCursorChanged(EventArgs e)
        {
            CursorChanged?.Invoke(this, e);
        }

        protected internal virtual void OnDoubleClick(EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        protected virtual void OnEnabledChanged(EventArgs e)
        {
            EnabledChanged?.Invoke(this, e);
        }

        protected internal virtual void OnEnter(EventArgs e)
        {
            if (!this.DesignMode && !this.GetState(0x800))
            {
                this.InvalidateFocusAreaOnly();
                this.SetState(0x800, true);
            }
            Enter?.Invoke(this, e);
        }

        protected internal virtual void OnGotFocus(EventArgs e)
        {
            if (!this.DesignMode && !this.GetState(0x80))
            {
                this.SetState(0x80, true);
            }
            GotFocus?.Invoke(this, e);
        }

        protected virtual void OnInvalidated(InvalidateEventArgs e)
        {
            Invalidated?.Invoke(this, e);
        }

        protected internal virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        protected internal virtual void OnKeyPress(KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }

        protected internal virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        protected internal virtual void OnLeave(EventArgs e)
        {
            Leave?.Invoke(this, e);
        }

        protected internal virtual void OnLostFocus(EventArgs e)
        {
            if (!this.DesignMode && this.GetState(0x80))
            {
                this.SetState(0x80, false);
            }
            LostFocus?.Invoke(this, e);
        }

        protected internal virtual void OnMouseClick(MouseEventArgs e)
        {
            Cursor.Current = this.Cursor;
            MouseClick?.Invoke(this, e);
        }

        protected internal virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            Cursor.Current = this.Cursor;
            MouseDoubleClick?.Invoke(this, e);
        }

        protected internal virtual void OnMouseDown(MouseEventArgs e)
        {
            Cursor.Current = this.Cursor;
            if (e.Button == MouseButtons.Left)
            {
                this.Focus();
            }
            MouseDown?.Invoke(this, e);
        }

        protected internal virtual void OnMouseEnter(EventArgs e)
        {
            this.m_OldCursor = Cursor.Current;
            MouseEnter?.Invoke(this, e);
        }

        protected internal virtual void OnMouseHover(EventArgs e)
        {
            MouseHover?.Invoke(this, e);
        }

        protected internal virtual void OnMouseLeave(EventArgs e)
        {
            Cursor.Current = this.m_OldCursor;
            MouseLeave?.Invoke(this, e);
        }

        protected internal virtual void OnMouseMove(MouseEventArgs e)
        {
            Cursor.Current = this.Cursor;
            MouseMove?.Invoke(this, e);
        }

        protected internal virtual void OnMouseUp(MouseEventArgs e)
        {
            Cursor.Current = this.Cursor;
            if (e.Button == MouseButtons.Right)
            {
                ShapeContainer parent = this.Parent;
                if (parent != null)
                {
                    Point p = this.PointToScreen(e.Location);
                    p = parent.PointToClient(p);
                    parent.ShowContextMenu(this, p);
                }
            }
            MouseUp?.Invoke(this, e);
        }

        protected internal virtual void OnMouseWheel(MouseEventArgs e)
        {
            MouseWheel?.Invoke(this, e);
        }

        protected virtual void OnMove(EventArgs e)
        {
            Move?.Invoke(this, e);
        }

        protected internal virtual void OnPaint(PaintEventArgs e)
        {
            Paint?.Invoke(this, e);
        }

        protected virtual void OnParentChanged(EventArgs e)
        {
            ParentChanged?.Invoke(this, e);
        }

        protected internal virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            PreviewKeyDown?.Invoke(this, e);
        }

        protected void OnQueryAccessibilityHelp(QueryAccessibilityHelpEventArgs e)
        {
            QueryAccessibilityHelp?.Invoke(this, e);
        }

        protected virtual void OnRegionChanged(EventArgs e)
        {
            RegionChanged?.Invoke(this, e);
        }

        protected virtual void OnVisibleChanged(EventArgs e)
        {
            VisibleChanged?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public Point PointToClient(Point p)
        {
            Point point2 = p;
            if ((this.Parent != null) && typeof(ShapeContainer).IsAssignableFrom(this.Parent.GetType()))
            {
                point2 = this.Parent.PointToClient(p);
            }
            point2.Offset(0 - this.BoundRect.X, 0 - this.BoundRect.Y);
            return point2;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public Point PointToScreen(Point p)
        {
            Point point2 = p;
            if ((this.Parent != null) && typeof(ShapeContainer).IsAssignableFrom(this.Parent.GetType()))
            {
                point2 = this.Parent.PointToScreen(p);
            }
            point2.Offset(this.BoundRect.X, this.BoundRect.Y);
            return point2;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public Rectangle RectangleToClient(Rectangle rect)
        {
            return new Rectangle(this.PointToClient(rect.Location), rect.Size);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public Rectangle RectangleToScreen(Rectangle rect)
        {
            return new Rectangle(this.PointToScreen(rect.Location), rect.Size);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Refresh()
        {
            this.Invalidate();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void ResetAccessibleDescription()
        {
            this.AccessibleDescription = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void ResetAccessibleName()
        {
            this.AccessibleName = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void ResetBorderColor()
        {
            //levy 这里Color直接设置为空有没有问题
            Color color = Color.Empty;
            this.BorderColor = color;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void ResetContextMenuStrip()
        {
            this.ContextMenuStrip = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void ResetCursor()
        {
            this.Cursor = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void ResetSelectionColor()
        {
            this.SelectionColor = DefaultSelectionColor;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void ResetTag()
        {
            this.Tag = null;
        }

        internal void ResumeAutoUpdate()
        {
            this.SetState(0x400, false);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void ResumePaint(bool performPaint)
        {
            this.SetState(0x400, false);
            if (performPaint)
            {
                this.Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract void Scale(SizeF factor);
        public void Select()
        {
            if ((!this.DesignMode && this.CanSelect) && !this.GetState(0x800))
            {
                ShapeContainer parent = this.Parent;
                if (parent != null)
                {
                    Shape selectedShape = parent.SelectedShape;
                    Shape focusedShape = parent.FocusedShape;
                    if (selectedShape != null)
                    {
                        selectedShape.OnLeave(EventArgs.Empty);
                        parent.SelectedShape = this;
                        if (focusedShape != null)
                        {
                            focusedShape.OnLostFocus(EventArgs.Empty);
                            parent.FocusedShape = this;
                        }
                        this.OnEnter(EventArgs.Empty);
                        if (focusedShape != null)
                        {
                            this.OnGotFocus(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        parent.SelectedShape = this;
                        parent.Select();
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void SendToBack()
        {
            ShapeContainer parent = this.Parent;
            if ((parent != null) && (parent.Shapes.IndexOf(this) != (parent.Shapes.Count - 1)))
            {
                parent.Shapes.SetChildIndex(this, -1);
                this.Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void SetParent(ShapeContainer p)
        {
            if (this.m_Parent != null)
            {
                this.Invalidate();
            }
            this.m_Parent = p;
            if (this.m_Parent != null)
            {
                this.Invalidate();
            }
            this.OnParentChanged(EventArgs.Empty);
        }

        internal void SetState(int flag, bool value)
        {
            if (value)
            {
                this.m_State |= flag;
            }
            else
            {
                this.m_State &= ~flag;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeAccessibleDescription()
        {
            return (this.AccessibleDescription != null);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeAccessibleName()
        {
            return (this.AccessibleName != null);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeBorderColor()
        {
            return !this.BorderColor.Equals(DefaultBorderColor);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeContextMenuStrip()
        {
            return (this.ContextMenuStrip != null);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeCursor()
        {
            return (this.Properties.ContainsKey("Cursor") && (RuntimeHelpers.GetObjectValue(this.Properties["Cursor"]) != null));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeSelectionColor()
        {
            return !this.SelectionColor.Equals(SystemColors.Highlight);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeTag()
        {
            return (this.Tag != null);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Show()
        {
            this.Visible = true;
        }

        internal void SuspendAutoUpdate()
        {
            this.SetState(0x400, true);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void SuspendPaint()
        {
            this.SetState(0x400, true);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Update()
        {
            if ((this.Parent != null) && (this.Parent.Parent != null))
            {
                this.Parent.Parent.Update();
                this.Parent.Update();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), Category("Accessibility"), Description("ShapePropAccessibilityObject"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public AccessibleObject AccessibilityObject
        {
            get
            {
                if (this.m_AccessibilityObject == null)
                {
                    this.m_AccessibilityObject = this.CreateAccessibilityInstance();
                }
                return this.m_AccessibilityObject;
            }
        }

        [Category("Accessibility"), Description("ShapePropAccessibleDefaultActionDescription"), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(typeof(string), "Click")]
        public string AccessibleDefaultActionDescription
        {
            get
            {
                return this.m_AccessibleDefaultActionDescription;
            }
            set
            {
                this.m_AccessibleDefaultActionDescription = value;
            }
        }

        [Description("ShapePropAccessibleDescription"), Browsable(true), Category("Accessibility"), Localizable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string AccessibleDescription
        {
            get
            {
                return this.m_AccessibleDescription;
            }
            set
            {
                this.m_AccessibleDescription = value;
            }
        }

        [Browsable(true), Description("ShapePropAccessibleName"), EditorBrowsable(EditorBrowsableState.Advanced), Category("Accessibility"), Localizable(true)]
        public string AccessibleName
        {
            get
            {
                return this.m_AccessibleName;
            }
            set
            {
                this.m_AccessibleName = value;
            }
        }

        [Category("Accessibility"), DefaultValue(typeof(AccessibleRole), "Client"), Browsable(true), EditorBrowsable(EditorBrowsableState.Advanced), Description("ShapePropAccessibleRole")]
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

        [Category("Layout"), DefaultValue(typeof(AnchorStyles), "5"), RefreshProperties(RefreshProperties.Repaint), Localizable(true), Description("ShapePropAnchor")]
        public virtual AnchorStyles Anchor
        {
            get
            {
                return this.m_Anchor;
            }
            set
            {
                Label label = new Label
                {
                    Anchor = value
                };
                if (label.Anchor != this.m_Anchor)
                {
                    this.m_Anchor = label.Anchor;
                }
            }
        }

        [Localizable(true), Browsable(true), Description("ShapePropBorderColor"), Category("Appearance"), EditorBrowsable(EditorBrowsableState.Always)]
        public virtual Color BorderColor
        {
            get
            {
                if (this.m_Pen != null)
                {
                    return this.m_Pen.Color;
                }
                return Color.Empty;
            }
            set
            {
                if (value.IsEmpty)
                {
                    value = DefaultBorderColor;
                }
                if (!this.m_Pen.Color.Equals(value))
                {
                    this.m_Pen.Color = value;
                    if (this.CanInvalidate && !this.GetState(0x400))
                    {
                        this.InvalidateInternal(this.GetInvalidateRegion(), null, true);
                    }
                }
            }
        }

        [Category("Appearance"), Browsable(true), Localizable(true), Description("ShapePropBorderStyle"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(DashStyle), "Solid")]
        public DashStyle BorderStyle
        {
            get
            {
                if (this.m_Pen != null)
                {
                    return this.m_Pen.DashStyle;
                }
                return DashStyle.Custom;
            }
            set
            {
                if (!Enum.IsDefined(typeof(DashStyle), value))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(DashStyle));
                }
                try
                {
                    if (this.m_Pen.DashStyle != value)
                    {
                        this.m_Pen.DashStyle = value;
                        if (this.CanInvalidate && !this.GetState(0x400))
                        {
                            this.InvalidateInternal(this.GetInvalidateRegion(), null, true);
                        }
                    }
                }
                finally
                {
                }
            }
        }

        [Description("ShapePropBorderWidth"), Browsable(true), DefaultValue(typeof(int), "1"), Localizable(true), Category("Appearance"), EditorBrowsable(EditorBrowsableState.Always)]
        public virtual int BorderWidth
        {
            get
            {
                return this.m_BorderWidth;
            }
            set
            {
                value = Math.Max(1, value);
                value = Utility.CheckInteger(value);
                if (this.m_BorderWidth != value)
                {
                    this.ClearCachedRegion();
                    if (!this.CanInvalidate)
                    {
                        this.m_BorderWidth = value;
                    }
                    else
                    {
                        Region invalidateRegion = this.GetInvalidateRegion();
                        this.m_BorderWidth = value;
                        Region newRegion = this.GetInvalidateRegion();
                        this.InvalidateInternal(invalidateRegion, newRegion, true);
                    }
                }
            }
        }

        internal virtual Rectangle BoundRect
        {
            get
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this)["Bounds"];
                if (descriptor == null)
                {
                    return Rectangle.Empty;
                }
                object obj1 = descriptor.GetValue(this);
                //levy 这里为null时直接返回Rectangle.Empty
                if (obj1 == null) return Rectangle.Empty;
                return (Rectangle)obj1;
            }
            set
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this)["Bounds"];
                if (descriptor != null)
                {
                    descriptor.SetValue(this, value);
                }
            }
        }

        [Description("ShapePropCanFocus"), Browsable(false), Category("Focus"), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(typeof(bool), "True")]
        public bool CanFocus
        {
            get
            {
                return (((this.Parent != null) && this.Parent.CanFocus) && (this.GetState(0x40) && this.GetState(0x4000)));
            }
        }

        internal bool CanInvalidate
        {
            get
            {
                return (this.ParentAvailable && (this.Parent.Parent != null));
            }
        }

        [Browsable(false), DefaultValue(typeof(bool), "True"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Always), Description("ShapePropCanSelect"), Category("Focus")]
        public bool CanSelect
        {
            get
            {
                return (((this.Parent != null) && this.Parent.CanSelect) && (this.GetState(0x40) && this.GetState(0x4000)));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(false)]
        public Control Container
        {
            get
            {
                return null;
            }
        }

        [Browsable(false), Description("ShapePropContainsFocus"), Category("Focus"), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContainsFocus
        {
            get
            {
                return this.Focused;
            }
        }

        [Category("Behavior"), Description("ShapePropContextMenu"), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual ContextMenu ContextMenu
        {
            get
            {
                return this.m_ContextMenu;
            }
            set
            {
                if (!object.Equals(this.m_ContextMenu, value))
                {
                    if (this.m_ContextMenu != null)
                    {
                        this.m_ContextMenu.Disposed -= new EventHandler(this.DetachContextMenu);
                    }
                    if (this.m_ContextMenu != null)
                    {
                        this.m_ContextMenu.Dispose();
                    }
                    this.m_ContextMenu = value;
                    if (this.m_ContextMenu != null)
                    {
                        this.m_ContextMenu.Disposed += new EventHandler(this.DetachContextMenu);
                    }
                    this.OnContextMenuChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Behavior"), Browsable(true), Description("ShapePropContextMenuStrip"), EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(typeof(ContextMenuStrip), "Nothing")]
        public virtual ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return this.m_ContextMenuStrip;
            }
            set
            {
                if (!object.Equals(this.m_ContextMenuStrip, value))
                {
                    if (this.m_ContextMenuStrip != null)
                    {
                        this.m_ContextMenuStrip.Disposed -= new EventHandler(this.DetachContextMenuStrip);
                    }
                    if (this.m_ContextMenuStrip != null)
                    {
                        this.m_ContextMenuStrip.Dispose();
                    }
                    this.m_ContextMenuStrip = value;
                    if (this.m_ContextMenuStrip != null)
                    {
                        this.m_ContextMenuStrip.Disposed += new EventHandler(this.DetachContextMenuStrip);
                    }
                    this.OnContextMenuStripChanged(EventArgs.Empty);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("ShapePropCreated")]
        public bool Created
        {
            get
            {
                return this.GetState(8);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), Category("Appearance"), Description("ShapePropCursor")]
        public Cursor Cursor
        {
            get
            {
                if (this.UseWaitCursor)
                {
                    return Cursors.WaitCursor;
                }
                Cursor cursor = null;
                if (this.Properties.ContainsKey("Cursor"))
                {
                    cursor = (Cursor)this.Properties["Cursor"];
                }
                if (cursor != null)
                {
                    return cursor;
                }
                if ((this.Parent != null) && (this.Parent.Parent != null))
                {
                    return this.Parent.Parent.Cursor;
                }
                return DefaultCursor;
            }
            set
            {
                if (this.Properties.ContainsKey("Cursor"))
                {
                    if (object.Equals(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(this.Properties["Cursor"])), value))
                    {
                        return;
                    }
                    this.Properties["Cursor"] = value;
                }
                else
                {
                    this.Properties.Add("Cursor", value);
                }
                this.OnCursorChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static Color DefaultBorderColor
        {
            get
            {
                return Control.DefaultForeColor;
            }
        }

        private static Cursor DefaultCursor
        {
            get
            {
                return Cursors.Default;
            }
        }

        private static Color DefaultSelectionColor
        {
            get
            {
                //return SystemColors.Highlight;
                return Color.Transparent;
            }
        }

        [Description("ShapePropDisposing"), Category("Behavior"), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool Disposing
        {
            get
            {
                return this.GetState(0x20);
            }
        }

        [Localizable(true), Browsable(true), Category("Behavior"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(bool), "True"), Description("ShapePropEnabled")]
        public bool Enabled
        {
            get
            {
                return this.GetState(0x40);
            }
            set
            {
                if (this.GetState(0x40) != value)
                {
                    this.SetState(0x40, value);
                    this.OnEnabledChanged(EventArgs.Empty);
                }
            }
        }

        internal virtual Rectangle ExtentBounds
        {
            get
            {
                return Rectangle.Empty;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Category("Focus"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("ShapePropFocused")]
        public virtual bool Focused
        {
            get
            {
                if (this.Parent == null)
                {
                    return false;
                }
                return this.GetState(0x80);
            }
        }

        internal Region FocusRegion
        {
            get
            {
                return this.GetRegionInternal(RegionType.FocusInvalidate);
            }
        }

        internal Region HitTestRegion
        {
            get
            {
                if (this.m_HitTestRegion == null)
                {
                    this.m_HitTestRegion = this.GetRegionInternal(RegionType.HitTest);
                }
                return this.m_HitTestRegion;
            }
        }

        internal bool InDesignMode
        {
            get
            {
                return this.DesignMode;
            }
        }

        internal bool InSetVirtulBounds
        {
            get
            {
                return this.m_InSetVirtualBounds;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), Description("ShapePropIsAccessible"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Category("Accessibility")]
        public bool IsAccessible
        {
            get
            {
                return this.GetState(0x100);
            }
            set
            {
                if (this.GetState(0x100) != value)
                {
                    this.SetState(0x100, value);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Behavior"), EditorBrowsable(EditorBrowsableState.Advanced), Description("ShapePropIsDisposed"), Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return this.GetState(0x10);
            }
        }

        [Browsable(false), Description("ShapePropModifierKeys"), EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Keys ModifierKeys
        {
            get
            {
                return Control.ModifierKeys;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false), Description("ShapePropMouseButtons")]
        public static MouseButtons MouseButtons
        {
            get
            {
                return Control.MouseButtons;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false), Description("ShapePropMousePosition")]
        public static Point MousePosition
        {
            get
            {
                return Control.MousePosition;
            }
        }

        [Description("ShapePropName"), EditorBrowsable(EditorBrowsableState.Always), Browsable(false)]
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                if (!string.Equals(this.m_Name, value))
                {
                    this.m_Name = value;
                }
            }
        }

        [Browsable(false), Description("ShapePropParent"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Always)]
        public ShapeContainer Parent
        {
            get
            {
                return this.m_Parent;
            }
            set
            {
                if (value == null)
                {
                    if (this.m_Parent != null)
                    {
                        this.m_Parent.Shapes.Remove(this);
                    }
                }
                else if (!value.Equals(this.m_Parent))
                {
                    if (this.m_Parent != null)
                    {
                        this.m_Parent.Shapes.Remove(this);
                    }
                    value.Shapes.Add(this);
                }
            }
        }

        private bool ParentAvailable
        {
            get
            {
                return ((this.Parent != null) && typeof(ShapeContainer).IsAssignableFrom(this.Parent.GetType()));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        internal Pen Pen
        {
            get
            {
                return this.m_Pen;
            }
        }

        internal Dictionary<string, object> Properties
        {
            get
            {
                if (this.m_Properties == null)
                {
                    this.m_Properties = new Dictionary<string, object>();
                }
                return this.m_Properties;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("ShapePropRegion"), EditorBrowsable(EditorBrowsableState.Always), Browsable(false)]
        public Region Region
        {
            get
            {
                return this.m_Region;
            }
            set
            {
                if (!object.Equals(this.m_Region, value))
                {
                    if (this.m_Region != null)
                    {
                        if ((value != null) && (this.Parent != null))
                        {
                            using (Graphics graphics = this.Parent.CreateGraphics())
                            {
                                using (Region region = this.m_Region.Clone())
                                {
                                    region.Xor(value);
                                    if (region.IsEmpty(graphics))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        this.m_Region.Dispose();
                    }
                    if (!this.CanInvalidate || this.GetState(0x400))
                    {
                        this.m_Region = value;
                    }
                    else
                    {
                        Region invalidateRegion = this.GetInvalidateRegion();
                        this.m_Region = value;
                        Region newRegion = this.GetInvalidateRegion();
                        this.InvalidateInternal(invalidateRegion, newRegion, true);
                    }
                    this.OnRegionChanged(EventArgs.Empty);
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        internal bool Selected
        {
            get
            {
                return this.GetState(0x800);
            }
        }

        [Category("Appearance"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Localizable(true), Description("ShapePropSelectionColor")]
        public Color SelectionColor
        {
            get
            {
                return this.m_SelectionColor;
            }
            set
            {
                if (value.IsEmpty)
                {
                    value = DefaultSelectionColor;
                }
                if (!object.Equals(this.m_SelectionColor, value))
                {
                    this.m_SelectionColor = value;
                }
            }
        }

        internal Region ShapeRegion
        {
            get
            {
                return this.GetRegionInternal(RegionType.Invalidate);
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Advanced), TypeConverter(typeof(TagConverter)), Description("ShapePropTag"), Category("Data")]
        public object Tag
        {
            get
            {
                return this.m_Tag;
            }
            set
            {
                if (!object.Equals(RuntimeHelpers.GetObjectValue(this.m_Tag), RuntimeHelpers.GetObjectValue(value)))
                {
                    if ((this.m_Tag != null) && (this.m_Tag is IDisposable))
                    {
                        ((IDisposable)this.m_Tag).Dispose();
                    }
                    this.m_Tag = RuntimeHelpers.GetObjectValue(value);
                }
            }
        }

        [DefaultValue(typeof(bool), "False"), Browsable(true), EditorBrowsable(EditorBrowsableState.Advanced), Category("Appearance"), Description("ShapePropUseWaitCursor")]
        public bool UseWaitCursor
        {
            get
            {
                return (((this.Parent != null) && this.Parent.UseWaitCursor) || this.GetState(0x2000));
            }
            set
            {
                if (this.GetState(0x2000) != value)
                {
                    this.SetState(0x2000, value);
                }
            }
        }

        internal virtual Rectangle VirtualBounds
        {
            get
            {
                if (this.m_VirtualBounds.IsEmpty)
                {
                    return this.BoundRect;
                }
                return this.m_VirtualBounds;
            }
            set
            {
                this.m_VirtualBounds = value;
                if (value.Width < 0)
                {
                    value.Width = 0;
                }
                if (value.Height < 0)
                {
                    value.Height = 0;
                }
                this.m_InSetVirtualBounds = true;
                try
                {
                    this.BoundRect = value;
                }
                finally
                {
                    this.m_InSetVirtualBounds = false;
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示该控件。
        /// 如果显示该控件，则为 true；否则为 false。 默认值为 true。
        /// </summary>
        [Browsable(true), Category("Behavior"), EditorBrowsable(EditorBrowsableState.Always), Localizable(true), DefaultValue(true), Description("ShapePropVisible")]
        public bool Visible
        {
            get
            {
                return this.GetState(0x4000);
            }
            set
            {
                if (this.GetState(0x4000) != value)
                {
                    if (this.DesignMode)
                    {
                        this.SetState(0x4000, value);
                    }
                    else if (this.CanInvalidate)
                    {
                        if (value)
                        {
                            this.SetState(0x4000, value);
                            this.Invalidate();
                        }
                        else
                        {
                            this.Invalidate();
                            this.SetState(0x4000, value);
                        }
                    }
                    else
                    {
                        this.SetState(0x4000, value);
                    }
                    this.OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        internal enum RegionType
        {
            HitTest,
            Invalidate,
            Focus,
            FocusInvalidate
        }

        private sealed class TagConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
            {
                return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
            {
                return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                {
                    return value;
                }
                return base.ConvertFrom(context, culture, RuntimeHelpers.GetObjectValue(value));
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is object)
                {
                    return value.ToString();
                }
                return base.ConvertTo(context, culture, RuntimeHelpers.GetObjectValue(value), destinationType);
            }
        }
    }
}