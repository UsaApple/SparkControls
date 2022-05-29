using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    #region Enums
    ///// <summary>
    ///// 适用于 <see cref="SparkCollapsibleSplitter"/> 控件的视觉风格枚举。
    ///// </summary>
    //public enum VisualStyles
    //{
    //	Mozilla = 0,
    //	XP,
    //	Win9x,
    //	DoubleDots,
    //	Lines
    //}

    /// <summary>
    /// <see cref="SparkCollapsibleSplitter"/> 控件的动作状态枚举。
    /// </summary>
    public enum SplitterState
    {
        Collapsed = 0,
        Expanding,
        Expanded,
        Collapsing
    }

    #endregion

    /// <summary>
    /// 可折叠的分割条控件。
    /// </summary>
    [ToolboxBitmap(typeof(Splitter))]
    [Designer(typeof(CollapsibleSplitterDesigner))]
    public class SparkCollapsibleSplitter : SparkSplitter
    {
        #region Private Properties

        // declare and define some base properties
        private bool hot;
        private readonly System.Drawing.Color hotColor;
        private System.Windows.Forms.Control controlToHide;
        private System.Drawing.Rectangle rr;
        private System.Windows.Forms.Form parentForm;
        private bool expandParentForm;
        //private VisualStyles visualStyle = VisualStyles.Mozilla;

        // Border added in version 1.3
        private System.Windows.Forms.Border3DStyle borderStyle = System.Windows.Forms.Border3DStyle.Flat;

        // animation controls introduced in version 1.22
        private readonly System.Windows.Forms.Timer animationTimer;
        private int controlWidth;
        private int controlHeight;
        private int parentFormWidth;
        private int parentFormHeight;
        private SplitterState currentState;
        private int animationDelay = 20;
        private int animationStep = 20;
        private bool useAnimations;

        private const int WIDTH = 12;

        #endregion

        #region Public Properties

        /// <summary>
        /// The initial state of the Splitter. Set to True if the control to hide is not visible by default
        /// </summary>
        [Bindable(true), Category("Spark"), DefaultValue(false),
        Description("The initial state of the Splitter. Set to True if the control to hide is not visible by default")]
        public bool IsCollapsed
        {
            get
            {
                if (this.controlToHide != null)
                    return !this.controlToHide.Visible;
                else
                    return true;
            }
        }

        /// <summary>
        /// The System.Windows.Forms.Control that the splitter will collapse
        /// </summary>
        [Bindable(true), Category("Spark"), DefaultValue(null),
        Description("The System.Windows.Forms.Control that the splitter will collapse")]
        public Control ControlToHide
        {
            get { return this.controlToHide; }
            set { this.controlToHide = value; }
        }

        /// <summary>
        /// Determines if the collapse and expanding actions will be animated
        /// </summary>
        [Bindable(true), Category("Spark"), DefaultValue(true),
        Description("Determines if the collapse and expanding actions will be animated")]
        public bool UseAnimations
        {
            get { return this.useAnimations; }
            set { this.useAnimations = value; }
        }

        /// <summary>
        /// The delay in millisenconds between animation steps
        /// </summary>
        [Bindable(true), Category("Spark"), DefaultValue(20),
        Description("The delay in millisenconds between animation steps")]
        public int AnimationDelay
        {
            get { return this.animationDelay; }
            set { this.animationDelay = value; }
        }

        /// <summary>
        /// The amount of pixels moved in each animation step
        /// </summary>
        [Bindable(true), Category("Spark"), DefaultValue(20),
        Description("The amount of pixels moved in each animation step")]
        public int AnimationStep
        {
            get { return this.animationStep; }
            set { this.animationStep = value; }
        }

        /// <summary>
        /// When true the entire parent form will be expanded and collapsed, otherwise just the contol to expand will be changed
        /// </summary>
        [Bindable(true), Category("Spark"), DefaultValue(false),
        Description("When true the entire parent form will be expanded and collapsed, otherwise just the contol to expand will be changed")]
        public bool ExpandParentForm
        {
            get { return this.expandParentForm; }
            set { this.expandParentForm = value; }
        }

        ///// <summary>
        ///// The visual style that will be painted on the control
        ///// </summary>
        //[Bindable(true), Category("Spark"), DefaultValue(VisualStyles.Mozilla),
        //Description("The visual style that will be painted on the control")]
        //public VisualStyles VisualStyle
        //{
        //	get { return this.visualStyle; }
        //	set
        //	{
        //		this.visualStyle = value;
        //		this.Invalidate();
        //	}
        //}

        /// <summary>
        /// An optional border style to paint on the control. Set to Flat for no border
        /// </summary>
        [Bindable(true), Category("Spark"), DefaultValue(Border3DStyle.Flat),
        Description("An optional border style to paint on the control. Set to Flat for no border")]
        public Border3DStyle BorderStyle3D
        {
            get { return this.borderStyle; }
            set
            {
                this.borderStyle = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// 切换状态，从隐藏到显示，或从显示到隐藏
        /// </summary>
        public void ToggleState()
        {
            this.ToggleSplitter();
        }

        /// <summary>
        /// 切换到隐藏
        /// </summary>
        public void ToggleToHide()
        {
            if (this.IsCollapsed == false)
            {
                this.ToggleState();
            }
        }

        /// <summary>
        /// 切换到显示
        /// </summary>
        public void ToggleToShow()
        {
            if (this.IsCollapsed == true)
            {
                this.ToggleState();
            }
        }
        #endregion

        #region Constructor

        public SparkCollapsibleSplitter()
        {
            // Register mouse events
            this.Click += new System.EventHandler(this.OnClick);
            this.Resize += new System.EventHandler(this.OnResize);
            this.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.MouseMove += new MouseEventHandler(this.OnMouseMove);

            // Setup the animation timer control
            this.animationTimer = new System.Windows.Forms.Timer
            {
                Interval = this.animationDelay
            };
            this.animationTimer.Tick += new System.EventHandler(this.animationTimerTick);

            this.hotColor = this.Theme.MouseOverBackColor;
        }

        #endregion

        #region Overrides

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.parentForm = this.FindForm();

            // set the current state
            if (this.controlToHide != null)
            {
                if (this.controlToHide.Visible)
                {
                    this.currentState = SplitterState.Expanded;
                }
                else
                {
                    this.currentState = SplitterState.Collapsed;
                }
            }
        }

        protected override void OnEnabledChanged(System.EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.Invalidate();
        }

        #endregion

        #region Event Handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // if the hider control isn't hot, let the base resize action occur
            if (this.controlToHide != null)
            {
                if (!this.hot && this.controlToHide.Visible)
                {
                    base.OnMouseDown(e);
                }
            }
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            this.Invalidate();
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // check to see if the mouse cursor position is within the bounds of our control
            if (e.X >= this.rr.X && e.X <= this.rr.X + this.rr.Width && e.Y >= this.rr.Y && e.Y <= this.rr.Y + this.rr.Height)
            {
                if (!this.hot)
                {
                    this.hot = true;
                    this.Cursor = Cursors.Hand;
                    this.Invalidate();
                }
            }
            else
            {
                if (this.hot)
                {
                    this.hot = false;
                    this.Invalidate(); ;
                }

                this.Cursor = Cursors.Default;

                if (this.controlToHide != null)
                {
                    if (!this.controlToHide.Visible)
                        this.Cursor = Cursors.Default;
                    else // Changed in v1.2 to support Horizontal Splitters
                    {
                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            this.Cursor = Cursors.VSplit;
                        }
                        else
                        {
                            this.Cursor = Cursors.HSplit;
                        }
                    }
                }
            }
        }

        private void OnMouseLeave(object sender, System.EventArgs e)
        {
            // ensure that the hot state is removed
            this.hot = false;
            this.Invalidate(); ;
        }

        private void OnClick(object sender, System.EventArgs e)
        {
            if (this.controlToHide != null && this.hot &&
                this.currentState != SplitterState.Collapsing &&
                this.currentState != SplitterState.Expanding)
            {
                this.ToggleSplitter();
            }
        }

        private void ToggleSplitter()
        {

            // if an animation is currently in progress for this control, drop out
            if (this.currentState == SplitterState.Collapsing || this.currentState == SplitterState.Expanding)
                return;

            this.controlWidth = this.controlToHide.Width;
            this.controlHeight = this.controlToHide.Height;

            if (this.controlToHide.Visible)
            {
                if (this.useAnimations)
                {
                    this.currentState = SplitterState.Collapsing;

                    if (this.parentForm != null)
                    {
                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            this.parentFormWidth = this.parentForm.Width - this.controlWidth;
                        }
                        else
                        {
                            this.parentFormHeight = this.parentForm.Height - this.controlHeight;
                        }
                    }

                    this.animationTimer.Enabled = true;
                }
                else
                {
                    // no animations, so just toggle the visible state
                    this.currentState = SplitterState.Collapsed;
                    this.controlToHide.Visible = false;
                    if (this.expandParentForm && this.parentForm != null)
                    {
                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            this.parentForm.Width -= this.controlToHide.Width;
                        }
                        else
                        {
                            this.parentForm.Height -= this.controlToHide.Height;
                        }
                    }
                }
            }
            else
            {
                // control to hide is collapsed
                if (this.useAnimations)
                {
                    this.currentState = SplitterState.Expanding;

                    if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                    {
                        if (this.parentForm != null)
                        {
                            this.parentFormWidth = this.parentForm.Width + this.controlWidth;
                        }
                        this.controlToHide.Width = 0;

                    }
                    else
                    {
                        if (this.parentForm != null)
                        {
                            this.parentFormHeight = this.parentForm.Height + this.controlHeight;
                        }
                        this.controlToHide.Height = 0;
                    }
                    this.controlToHide.Visible = true;
                    this.animationTimer.Enabled = true;
                }
                else
                {
                    // no animations, so just toggle the visible state
                    this.currentState = SplitterState.Expanded;
                    this.controlToHide.Visible = true;
                    if (this.expandParentForm && this.parentForm != null)
                    {
                        if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                        {
                            this.parentForm.Width += this.controlToHide.Width;
                        }
                        else
                        {
                            this.parentForm.Height += this.controlToHide.Height;
                        }
                    }
                }
            }

        }

        #endregion

        #region Implementation

        #region Animation Timer Tick

        private void animationTimerTick(object sender, System.EventArgs e)
        {
            switch (this.currentState)
            {
                case SplitterState.Collapsing:

                    if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                    {
                        // vertical splitter
                        if (this.controlToHide.Width > this.animationStep)
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Width -= this.animationStep;
                            }
                            this.controlToHide.Width -= this.animationStep;
                        }
                        else
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Width = this.parentFormWidth;
                            }
                            this.controlToHide.Visible = false;
                            this.animationTimer.Enabled = false;
                            this.controlToHide.Width = this.controlWidth;
                            this.currentState = SplitterState.Collapsed;
                            this.Invalidate();
                        }
                    }
                    else
                    {
                        // horizontal splitter
                        if (this.controlToHide.Height > this.animationStep)
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Height -= this.animationStep;
                            }
                            this.controlToHide.Height -= this.animationStep;
                        }
                        else
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Height = this.parentFormHeight;
                            }
                            this.controlToHide.Visible = false;
                            this.animationTimer.Enabled = false;
                            this.controlToHide.Height = this.controlHeight;
                            this.currentState = SplitterState.Collapsed;
                            this.Invalidate();
                        }
                    }
                    break;

                case SplitterState.Expanding:

                    if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                    {
                        // vertical splitter
                        if (this.controlToHide.Width < (this.controlWidth - this.animationStep))
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Width += this.animationStep;
                            }
                            this.controlToHide.Width += this.animationStep;
                        }
                        else
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Width = this.parentFormWidth;
                            }
                            this.controlToHide.Width = this.controlWidth;
                            this.controlToHide.Visible = true;
                            this.animationTimer.Enabled = false;
                            this.currentState = SplitterState.Expanded;
                            this.Invalidate();
                        }
                    }
                    else
                    {
                        // horizontal splitter
                        if (this.controlToHide.Height < (this.controlHeight - this.animationStep))
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Height += this.animationStep;
                            }
                            this.controlToHide.Height += this.animationStep;
                        }
                        else
                        {
                            if (this.expandParentForm && this.parentForm.WindowState != FormWindowState.Maximized
                                && this.parentForm != null)
                            {
                                this.parentForm.Height = this.parentFormHeight;
                            }
                            this.controlToHide.Height = this.controlHeight;
                            this.controlToHide.Visible = true;
                            this.animationTimer.Enabled = false;
                            this.currentState = SplitterState.Expanded;
                            this.Invalidate();
                        }

                    }
                    break;
            }
        }

        #endregion

        #region Paint the control

        //// OnPaint is now an override rather than an event in version 1.1
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //	// create a Graphics object
        //	Graphics g = e.Graphics;

        //	// find the rectangle for the splitter and paint it
        //	Rectangle r = this.ClientRectangle; // fixed in version 1.1
        //	g.FillRectangle(new SolidBrush(this.BackColor), r);

        //	#region Vertical Splitter
        //	// Check the docking style and create the control rectangle accordingly
        //	if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
        //	{
        //		// create a new rectangle in the vertical center of the splitter for our collapse control button
        //		this.rr = new Rectangle(r.X, r.Y + ((r.Height - 115) / 2), 8, 115);
        //		// force the width to 8px so that everything always draws correctly
        //		this.Width = 8;

        //		// draw the background color for our control image
        //		if (this.hot)
        //		{
        //			g.FillRectangle(new SolidBrush(this.hotColor), new Rectangle(this.rr.X + 1, this.rr.Y, 6, 115));
        //		}
        //		else
        //		{
        //			g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(this.rr.X + 1, this.rr.Y, 6, 115));
        //		}

        //		// draw the top & bottom lines for our control image
        //		g.DrawLine(new Pen(SystemColors.ControlDark, 1), this.rr.X + 1, this.rr.Y, this.rr.X + this.rr.Width - 2, this.rr.Y);
        //		g.DrawLine(new Pen(SystemColors.ControlDark, 1), this.rr.X + 1, this.rr.Y + this.rr.Height, this.rr.X + this.rr.Width - 2, this.rr.Y + this.rr.Height);

        //		if (this.Enabled)
        //		{
        //			// draw the arrows for our control image
        //			// the ArrowPointArray is a point array that defines an arrow shaped polygon
        //			g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), this.ArrowPointArray(this.rr.X + 2, this.rr.Y + 3));
        //			g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), this.ArrowPointArray(this.rr.X + 2, this.rr.Y + this.rr.Height - 9));
        //		}

        //		// draw the dots for our control image using a loop
        //		int x = this.rr.X + 3;
        //		int y = this.rr.Y + 14;

        //		// Visual Styles added in version 1.1
        //		switch (this.visualStyle)
        //		{
        //			case VisualStyles.Mozilla:

        //				for (int i = 0; i < 30; i++)
        //				{
        //					// light dot
        //					g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y + (i * 3), x + 1, y + 1 + (i * 3));
        //					// dark dot
        //					g.DrawLine(new Pen(SystemColors.ControlDarkDark), x + 1, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
        //					// overdraw the background color as we actually drew 2px diagonal lines, not just dots
        //					if (this.hot)
        //					{
        //						g.DrawLine(new Pen(this.hotColor), x + 2, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
        //					}
        //					else
        //					{
        //						g.DrawLine(new Pen(this.BackColor), x + 2, y + 1 + (i * 3), x + 2, y + 2 + (i * 3));
        //					}
        //				}
        //				break;

        //			case VisualStyles.DoubleDots:
        //				for (int i = 0; i < 30; i++)
        //				{
        //					// light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x, y + 1 + (i * 3), 1, 1);
        //					// dark dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlDark), x - 1, y + (i * 3), 1, 1);
        //					i++;
        //					// light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 2, y + 1 + (i * 3), 1, 1);
        //					// dark dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlDark), x + 1, y + (i * 3), 1, 1);
        //				}
        //				break;

        //			case VisualStyles.Win9x:

        //				g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 2, y);
        //				g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 90);
        //				g.DrawLine(new Pen(SystemColors.ControlDark), x + 2, y, x + 2, y + 90);
        //				g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 90, x + 2, y + 90);
        //				break;

        //			case VisualStyles.XP:

        //				for (int i = 0; i < 18; i++)
        //				{
        //					// light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLight), x, y + (i * 5), 2, 2);
        //					// light light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1, y + 1 + (i * 5), 1, 1);
        //					// dark dark dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x, y + (i * 5), 1, 1);
        //					// dark fill
        //					g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 5), x, y + (i * 5) + 1);
        //					g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 5), x + 1, y + (i * 5));
        //				}
        //				break;

        //			case VisualStyles.Lines:

        //				for (int i = 0; i < 44; i++)
        //				{
        //					g.DrawLine(new Pen(SystemColors.ControlDark), x, y + (i * 2), x + 2, y + (i * 2));
        //				}

        //				break;
        //		}

        //		// Added in version 1.3
        //		if (this.borderStyle != System.Windows.Forms.Border3DStyle.Flat)
        //		{
        //			// Paint the control border
        //			ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Left);
        //			ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Right);
        //		}
        //	}

        //	#endregion

        //	// Horizontal Splitter support added in v1.2

        //	#region Horizontal Splitter

        //	else if (this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom)
        //	{
        //		// create a new rectangle in the horizontal center of the splitter for our collapse control button
        //		this.rr = new Rectangle(r.X + ((r.Width - 115) / 2), r.Y, 115, 8);
        //		// force the height to 8px
        //		this.Height = 8;

        //		// draw the background color for our control image
        //		if (this.hot)
        //		{
        //			g.FillRectangle(new SolidBrush(this.hotColor), new Rectangle(this.rr.X, this.rr.Y + 1, 115, 6));
        //		}
        //		else
        //		{
        //			g.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(this.rr.X, this.rr.Y + 1, 115, 6));
        //		}

        //		// draw the left & right lines for our control image
        //		g.DrawLine(new Pen(SystemColors.ControlDark, 1), this.rr.X, this.rr.Y + 1, this.rr.X, this.rr.Y + this.rr.Height - 2);
        //		g.DrawLine(new Pen(SystemColors.ControlDark, 1), this.rr.X + this.rr.Width, this.rr.Y + 1, this.rr.X + this.rr.Width, this.rr.Y + this.rr.Height - 2);

        //		if (this.Enabled)
        //		{
        //			// draw the arrows for our control image
        //			// the ArrowPointArray is a point array that defines an arrow shaped polygon
        //			g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), this.ArrowPointArray(this.rr.X + 3, this.rr.Y + 2));
        //			g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), this.ArrowPointArray(this.rr.X + this.rr.Width - 9, this.rr.Y + 2));
        //		}

        //		// draw the dots for our control image using a loop
        //		int x = this.rr.X + 14;
        //		int y = this.rr.Y + 3;

        //		// Visual Styles added in version 1.1
        //		switch (this.visualStyle)
        //		{
        //			case VisualStyles.Mozilla:

        //				for (int i = 0; i < 30; i++)
        //				{
        //					// light dot
        //					g.DrawLine(new Pen(SystemColors.ControlLightLight), x + (i * 3), y, x + 1 + (i * 3), y + 1);
        //					// dark dot
        //					g.DrawLine(new Pen(SystemColors.ControlDarkDark), x + 1 + (i * 3), y + 1, x + 2 + (i * 3), y + 2);
        //					// overdraw the background color as we actually drew 2px diagonal lines, not just dots
        //					if (this.hot)
        //					{
        //						g.DrawLine(new Pen(this.hotColor), x + 1 + (i * 3), y + 2, x + 2 + (i * 3), y + 2);
        //					}
        //					else
        //					{
        //						g.DrawLine(new Pen(this.BackColor), x + 1 + (i * 3), y + 2, x + 2 + (i * 3), y + 2);
        //					}
        //				}
        //				break;

        //			case VisualStyles.DoubleDots:

        //				for (int i = 0; i < 30; i++)
        //				{
        //					// light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 3), y, 1, 1);
        //					// dark dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i * 3), y - 1, 1, 1);
        //					i++;
        //					// light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 3), y + 2, 1, 1);
        //					// dark dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlDark), x + (i * 3), y + 1, 1, 1);
        //				}
        //				break;

        //			case VisualStyles.Win9x:

        //				g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x, y + 2);
        //				g.DrawLine(new Pen(SystemColors.ControlLightLight), x, y, x + 88, y);
        //				g.DrawLine(new Pen(SystemColors.ControlDark), x, y + 2, x + 88, y + 2);
        //				g.DrawLine(new Pen(SystemColors.ControlDark), x + 88, y, x + 88, y + 2);
        //				break;

        //			case VisualStyles.XP:

        //				for (int i = 0; i < 18; i++)
        //				{
        //					// light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLight), x + (i * 5), y, 2, 2);
        //					// light light dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlLightLight), x + 1 + (i * 5), y + 1, 1, 1);
        //					// dark dark dot
        //					g.DrawRectangle(new Pen(SystemColors.ControlDarkDark), x + (i * 5), y, 1, 1);
        //					// dark fill
        //					g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 5), y, x + (i * 5) + 1, y);
        //					g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 5), y, x + (i * 5), y + 1);
        //				}
        //				break;

        //			case VisualStyles.Lines:

        //				for (int i = 0; i < 44; i++)
        //				{
        //					g.DrawLine(new Pen(SystemColors.ControlDark), x + (i * 2), y, x + (i * 2), y + 2);
        //				}

        //				break;
        //		}

        //		// Added in version 1.3
        //		if (this.borderStyle != System.Windows.Forms.Border3DStyle.Flat)
        //		{
        //			// Paint the control border
        //			ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Top);
        //			ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Bottom);
        //		}
        //	}

        //	#endregion

        //	else
        //	{
        //		throw new Exception("The Collapsible Splitter control cannot have the Filled or None Dockstyle property");
        //	}



        //	// dispose the Graphics object
        //	g.Dispose();
        //}


        /// <summary>
        /// 引发绘制事件。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // create a Graphics object
            Graphics g = e.Graphics;

            // find the rectangle for the splitter and paint it
            Rectangle r = this.ClientRectangle; // fixed in version 1.1
            g.FillRectangle(new SolidBrush(this.BackColor), r);

            #region Vertical Splitter

            // Check the docking style and create the control rectangle accordingly
            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
            {
                // create a new rectangle in the vertical center of the splitter for our collapse control button
                this.rr = new Rectangle(r.X, r.Y + ((r.Height - 120) / 2), WIDTH, 120);
                // force the width to 8px so that everything always draws correctly
                this.Width = WIDTH;

                // draw the background color for our control image
                if (this.hot)
                {
                    g.FillRectangle(new SolidBrush(this.hotColor), new Rectangle(this.rr.X + 1, this.rr.Y, WIDTH - 2, 120));
                }
                else
                {
                    g.FillRectangle(new SolidBrush(this.Theme.HandlerBackColor), new Rectangle(this.rr.X + 1, this.rr.Y, WIDTH - 2, 120));
                }

                // draw the arrows for our control image，the ArrowPointArray is a point array that defines an arrow shaped polygon
                g.FillPolygon(new SolidBrush(Color.White), this.ArrowPointArray(this.rr.X + 5, this.rr.Y + 3));
                g.FillPolygon(new SolidBrush(Color.White), this.ArrowPointArray(this.rr.X + 5, this.rr.Y + this.rr.Height - 9));

                // draw the dots for our control image using a loop
                int x = this.rr.X + 5;
                int y = this.rr.Y + 14;

                for (int i = 0; i < 32; i++)
                {
                    g.DrawLine(new Pen(Color.White), x, y + (i * 3), x + 1, y + (i * 3));
                }

                // Added in version 1.3
                if (this.borderStyle != System.Windows.Forms.Border3DStyle.Flat)
                {
                    // Paint the control border
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Left);
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Right);
                }
            }

            #endregion

            #region Horizontal Splitter

            else if (this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom)
            {
                // create a new rectangle in the horizontal center of the splitter for our collapse control button
                this.rr = new Rectangle(r.X + (r.Width - 120) / 2, r.Y, 120, WIDTH);
                // force the height to 8px
                this.Height = WIDTH;

                // draw the background color for our control image
                if (this.hot)
                {
                    g.FillRectangle(new SolidBrush(this.hotColor), new Rectangle(this.rr.X, this.rr.Y, 120, WIDTH));
                }
                else
                {
                    g.FillRectangle(new SolidBrush(this.Theme.HandlerBackColor), new Rectangle(this.rr.X, this.rr.Y, 120, WIDTH));
                }

                // draw the arrows for our control image，the ArrowPointArray is a point array that defines an arrow shaped polygon
                g.FillPolygon(new SolidBrush(Color.White), this.ArrowPointArray(this.rr.X + 3, this.rr.Y + 4));
                g.FillPolygon(new SolidBrush(Color.White), this.ArrowPointArray(this.rr.X + this.rr.Width - 9, this.rr.Y + 4));

                // draw the dots for our control image using a loop
                int x = this.rr.X + 14;
                int y = this.rr.Y + 5;

                for (int i = 0; i < 32; i++)
                {
                    g.DrawLine(new Pen(Color.White), x + (i * 3), y, x + (i * 3), y + 1);
                }

                // Added in version 1.3
                if (this.borderStyle != System.Windows.Forms.Border3DStyle.Flat)
                {
                    // Paint the control border
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Top);
                    ControlPaint.DrawBorder3D(e.Graphics, this.ClientRectangle, this.borderStyle, Border3DSide.Bottom);
                }
            }

            #endregion

            else
            {
                throw new Exception("The Collapsible Splitter control cannot have the Filled or None Dockstyle property");
            }

            // dispose the Graphics object
            g.Dispose();
        }
        #endregion

        #region Arrow Polygon Array

        // This creates a point array to draw a arrow-like polygon
        private Point[] ArrowPointArray(int x, int y)
        {
            Point[] point = new Point[3];

            if (this.controlToHide != null)
            {
                // decide which direction the arrow will point
                if ((this.Dock == DockStyle.Right && this.controlToHide.Visible) || (this.Dock == DockStyle.Left && !this.controlToHide.Visible))
                {
                    // right arrow
                    point[0] = new Point(x, y);
                    point[1] = new Point(x + 3, y + 3);
                    point[2] = new Point(x, y + 6);
                }
                else if ((this.Dock == DockStyle.Right && !this.controlToHide.Visible) || (this.Dock == DockStyle.Left && this.controlToHide.Visible))
                {
                    // left arrow
                    point[0] = new Point(x + 3, y);
                    point[1] = new Point(x, y + 3);
                    point[2] = new Point(x + 3, y + 6);
                }

                // Up/Down arrows added in v1.2
                else if ((this.Dock == DockStyle.Top && this.controlToHide.Visible) || (this.Dock == DockStyle.Bottom && !this.controlToHide.Visible))
                {
                    // up arrow
                    point[0] = new Point(x + 3, y);
                    point[1] = new Point(x + 6, y + 4);
                    point[2] = new Point(x, y + 4);
                }
                else if ((this.Dock == DockStyle.Top && !this.controlToHide.Visible) || (this.Dock == DockStyle.Bottom && this.controlToHide.Visible))
                {
                    // down arrow
                    point[0] = new Point(x, y);
                    point[1] = new Point(x + 6, y);
                    point[2] = new Point(x + 3, y + 3);
                }
            }

            return point;
        }

        #endregion

        #region Color Calculator

        // this method was borrowed from the RichUI Control library by Sajith M
        private static Color CalculateColor(Color front, Color back, int alpha)
        {
            // solid color obtained as a result of alpha-blending

            Color frontColor = Color.FromArgb(255, front);
            Color backColor = Color.FromArgb(255, back);

            float frontRed = frontColor.R;
            float frontGreen = frontColor.G;
            float frontBlue = frontColor.B;
            float backRed = backColor.R;
            float backGreen = backColor.G;
            float backBlue = backColor.B;

            float fRed = frontRed * alpha / 255 + backRed * ((float)(255 - alpha) / 255);
            byte newRed = (byte)fRed;
            float fGreen = frontGreen * alpha / 255 + backGreen * ((float)(255 - alpha) / 255);
            byte newGreen = (byte)fGreen;
            float fBlue = frontBlue * alpha / 255 + backBlue * ((float)(255 - alpha) / 255);
            byte newBlue = (byte)fBlue;

            return Color.FromArgb(255, newRed, newGreen, newBlue);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// <see cref="SparkCollapsibleSplitter"/> 控件的设计器。
    /// </summary>
    public class CollapsibleSplitterDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public CollapsibleSplitterDesigner()
        {
        }

        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            properties.Remove("IsCollapsed");
            properties.Remove("BorderStyle");
            properties.Remove("Size");
        }
    }
}