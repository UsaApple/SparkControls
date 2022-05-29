using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using SparkControls.Theme;
using SparkControls.Win32;

namespace SparkControls.Controls
{
    /// <summary>
    /// 时间日期控件
    /// </summary>
    [ToolboxBitmap(typeof(DateTimePicker))]
    [DefaultProperty("Value"), DefaultBindingProperty("Value")]
    public class SparkDateTimePicker : DateTimePicker, ISparkTheme<SparkDateTimePickerTheme>, IDataBinding
    {
        #region 常量
        private const int BORDER_OFFSET = 2;
        private const int DELETE_WIDTH = 16;
        private const int CALENDAR_WIDTH = 23;
        private const int DROP_AND_ICO_WIDTH = 36;
        private const int WM_USER = 0x0400;
        private const int WM_REFLECT = WM_USER + 0x1C00;
        private const string CALL_BACK_FIELD = "X";
        #endregion

        #region 变量
        private bool _isSetNullDt = false;
        private bool _isInitState = false;
        private bool _formatReset = false;
        private bool _popFormClosed = true;
        internal bool _userHasSetValue = false;
        private bool _isFocusOnDtpLbl = false;
        private bool _dtpSupportKeysPressed = false;
        private ControlState _controlState = ControlState.Default;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkDateTimePicker() : base()
        {
            this._isInitState = true;
            this.SetStyle(ControlStyles.ResizeRedraw |            // 调整大小时重绘
                ControlStyles.DoubleBuffer |                      // 双缓冲
                ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
                ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息WM_ERASEBKGND减少闪烁
                ControlStyles.SupportsTransparentBackColor, true  // 控件绘制代替系统绘制
            );
            this.Theme = new SparkDateTimePickerTheme(this);

            //添加下拉标签小图标(边框绘制使用主题的边框颜色)
            this.AddDropImgLabel();

            this.Format = base.Format;
            this.Font = this.mFont;
            this.AllowNull = false;
            this.Size = new Size(150, this.Size.Height);
        }
        #endregion

        #region 属性
        private Font mFont = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get => this.mFont;
            set
            {
                base.Font = this.mFont = value;
                this.PerformLayout();
                this.Invalidate();
                this.OnFontChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 获取或设置控件的背景色。
        /// </summary>
        [Category("Spark"), Description("控件的背景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.BackColorString)]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        [Category("Spark"), Description("控件的前景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否将控件的元素对齐以支持使用从右向左的字体的区域设置。
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new RightToLeft RightToLeft => base.RightToLeft;

        /// <summary>
        /// 获取或设置一个值，该值指示是否使用数值调节钮控件（也称为 up-down 控件）调整日期/时间值。
        /// </summary>
        [Category("Spark"), Description("指示是否使用数值调节钮控件")]
        [DefaultValue(false)]
        public new bool ShowUpDown
        {
            get => base.ShowUpDown;
            set
            {
                base.ShowUpDown = value;
                this.SetDropImgLblProp(!value && this.AllowNull, !value);
            }
        }

        private DateTimePickerFormat _format;
        /// <summary>
        /// 获取或设置自定义日期/时间格式字符串。
        /// </summary>
        [Category("Spark"), Description("获取或设置控件中显示的日期和时间格式")]
        [DefaultValue(DateTimePickerFormat.Long)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new DateTimePickerFormat Format
        {
            get => this._format;
            set
            {
                this._format = value;
                base.Format = DateTimePickerFormat.Custom;
                if (this.Value != null)
                {
                    if (this._format == DateTimePickerFormat.Custom)
                    {
                        //以下两句不设置，设计界面会刷新不了
                        this._formatReset = false;
                        base.CustomFormat = (string.IsNullOrEmpty(this.CustomFormat) ?
                            CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern :
                            this.CustomFormat) + CALL_BACK_FIELD;
                    }
                    else
                    {
                        this._formatReset = true;
                        if (this._format == DateTimePickerFormat.Long) base.CustomFormat =
                                CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern + CALL_BACK_FIELD;
                        if (this._format == DateTimePickerFormat.Short) base.CustomFormat =
                                CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + CALL_BACK_FIELD;
                        if (this._format == DateTimePickerFormat.Time) base.CustomFormat =
                                CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + CALL_BACK_FIELD;
                    }
                }
            }
        }

        private string _customFormat = null;
        /// <summary>
        /// 获取或设置自定义日期/时间格式字符串。
        /// </summary>
        [Category("Spark"), Description("获取或设置自定义日期/时间格式字符串")]
        [DefaultValue(null)]
        [Localizable(true)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public new string CustomFormat
        {
            get => this._customFormat;
            set
            {
                this._customFormat = value;
                if (!this._formatReset && this.Value != null)
                {
                    base.CustomFormat = (string.IsNullOrEmpty(value) ?
                        CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern :
                        value) + CALL_BACK_FIELD;
                }
            }
        }

        private bool _allowNull = false;
        /// <summary>
        /// 是否允许为空
        /// </summary>
        [Category("Spark"), Description("值是否允许为空")]
        [DefaultValue(false)]
        public bool AllowNull
        {
            get => this._allowNull;
            set
            {
                this._allowNull = value;
                if (value && this.Value == null) this.Value = null;
                if (!this.ShowUpDown) this.SetDropImgLblProp(value);
            }
        }

        /// <summary>
        /// 获取或设置分配给控件的日期/时间值
        /// </summary>
        [Category("Spark"), Description("获取或设置分配给控件的日期/时间值")]
        [Bindable(true)]
        [RefreshProperties(RefreshProperties.All)]
        public new DateTime? Value
        {
            get
            {
                if (this._isSetNullDt || (this.ShowCheckBox && !this.Checked)) return null;
                return base.Value;
            }
            set
            {
                bool lastValIsNull = this._isSetNullDt;
                this._isSetNullDt = false;
                if (value == null || (LtMinTreatAsNull && value < this.MinDate))
                {
                    if (this.AllowNull)
                    {
                        this._isSetNullDt = true;
                        this._userHasSetValue = true;
                        this.ClearDateTimeValue();
                        if (!lastValIsNull) OnValueChanged(EventArgs.Empty);
                    }
                    else
                    {
                        this.RestoreDtpFormat();
                        base.Value = DateTime.Now;
                        this._userHasSetValue = false;
                    }
                }
                else
                {
                    this.RestoreDtpFormat();
                    if (value < this.MinDate) this.MinDate = DateTime.MinValue;
                    base.Value = value.Value;
                    this._userHasSetValue = true;
                }
            }
        }

        /// <summary>
        /// 没有任何改变时(直接点击确定)重新构造时间
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool ReCtorDtWhenNoChange { get; set; } = false;

        /// <summary>
        /// 小于最小值时当作null
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        public bool LtMinTreatAsNull { get; set; } = false;

        /// <summary>
        /// 获取创建控件句柄时所需要的创建参数
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                return base.CreateParams;
                //if (!DateTimeImgLabel.IsVistaOrLater) return base.CreateParams;
                //CreateParams cp = base.CreateParams;
                ////设置WS_EX_COMPOSITED解决闪烁
                //cp.ExStyle |= (int)WindowExStyles.WS_EX_COMPOSITED;
                //return cp;
            }
        }

        /// <summary>
        /// 右侧按钮的矩形区域
        /// </summary>
        internal Rectangle ButtonRect
        {
            get
            {
                return new Rectangle(this.Width - DROP_AND_ICO_WIDTH,
                    0, DROP_AND_ICO_WIDTH, this.Height);
            }
        }

        /// <summary>
        /// 边框样式
        /// </summary>
        [Category("Spark"), Description("边框样式")]
        [DefaultValue(BorderStyle.FixedSingle)]
        public virtual BorderStyle BorderStyle
        {
            get; set;
        } = BorderStyle.FixedSingle;

        private bool isEnterToTab = true;
        /// <summary>
        /// 获取或设置一个值，该值指示是否将回车键的动作转为 Tab 键。
        /// </summary>
        [Category("Spark"), Description("是否将回车键的动作转为 Tab 键。")]
        [DefaultValue(true)]
        public bool IsEnterToTab
        {
            get => this.isEnterToTab;
            set => this.isEnterToTab = value;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this.Focused && !this._isFocusOnDtpLbl)
            {
                this._controlState = ControlState.Highlight;
            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.Focused && !this._isFocusOnDtpLbl)
            {
                this._controlState = ControlState.Default;
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// 获取焦点事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            this._controlState = ControlState.Focused;
            base.OnGotFocus(e);
        }

        /// <summary>
        /// 失去焦点事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (!this._isFocusOnDtpLbl)
            {
                this._controlState = ControlState.Default;
            }
            base.OnLostFocus(e);
        }

        /// <summary>
        /// 处理Windows消息
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                //case (int)WindowMessages.WM_NCPAINT:
                //    NativeMethods.SendMessage(Handle, (int)WindowMessages.WM_ERASEBKGND, hDC, 0);
                //    SendPrintClientMsg();
                //    NativeMethods.SendMessage(Handle, (int)WindowMessages.WM_PAINT, IntPtr.Zero, 0);
                //    DrawDtpBorder(gdc);

                //    m.Result = Win32Consts.TRUE;
                //    break;
                case (int)Msgs.WM_PAINT:
                    base.WndProc(ref m);

                    IntPtr hDC = NativeMethods.GetWindowDC(m.HWnd);
                    Graphics gdc = Graphics.FromHdc(hDC);
                    this.DrawDtpBorder(gdc);
                    NativeMethods.ReleaseDC(m.HWnd, hDC);
                    gdc.Dispose();
                    //if (!ShowUpDown) DrawButton(gdc);
                    break;
                case (int)Msgs.WM_SETCURSOR:
                    base.WndProc(ref m);
                    break;
                case WM_REFLECT + (int)Msgs.WM_NOTIFY:
                    base.WndProc(ref m);
                    this.AutoToNextPart(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源,为true；否则为false</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl.IsDisposed || ctrl.Disposing) continue;
                    ctrl.Dispose();
                }
            }
        }

        /// <summary>
        /// 句柄创建完处理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            this._isInitState = false;
            //某些情况下右侧lbl的宽度会改变，这里重新设置
            if (!this.ShowUpDown) this.SetDropImgLblProp(this.AllowNull);
        }

        /// <summary>
        /// 处理命令键
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F4)
            {
                this.LblCalendar_Click(this.Controls["lblCalendar"], EventArgs.Empty);
                return true;
            }

            if (keyData == Keys.Enter && this.IsEnterToTab) SendKeys.SendWait("{Tab}");
            if (keyData == Keys.Up || keyData == Keys.Down ||
                keyData == Keys.Home || keyData == Keys.End)
            {
                this._dtpSupportKeysPressed = true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 在指定的设备(打印机)上下文中绘制客户区
        /// </summary>
        private void SendPrintClientMsg()
        {
            Graphics gClient = this.CreateGraphics();
            IntPtr ptrClientDC = gClient.GetHdc();
            NativeMethods.SendMessage(this.Handle, (int)Msgs.WM_PRINTCLIENT, ptrClientDC, 0);
            gClient.ReleaseHdc(ptrClientDC);
            gClient.Dispose();
        }

        /// <summary>
        /// 获取下拉按钮的矩形区域
        /// </summary>
        /// <returns></returns>
        private Rectangle GetDropDownBtnRect()
        {
            //获取下拉按钮的宽度
            ComboBox cb = new ComboBox();
            ComboBoxInfo cbi = new ComboBoxInfo();
            cbi.cbSize = Marshal.SizeOf(cbi);
            NativeMethods.GetComboBoxInfo(cb.Handle, ref cbi);
            cb.Dispose();
            int dropBtnWidth = cbi.rcButton.Rect.Width;

            Rectangle rect = new Rectangle(this.Width - dropBtnWidth - BORDER_OFFSET * 2,
                BORDER_OFFSET, this.Height - BORDER_OFFSET, this.Height - BORDER_OFFSET * 2);
            return rect;
        }

        /// <summary>
        /// 绘制原始时间下拉框区域
        /// </summary>
        /// <param name="g"></param>
        private void DrawDtpBorder(Graphics g)
        {
            GDIHelper.InitializeGraphics(g);
            Rectangle borderRect = new Rectangle(Point.Empty, this.Size);

            //如果设置为透明色的话，就恢复到日期控件本来的边框颜色
            Color boderColor = this.GetBorderColorByState();
            if (boderColor == Color.Transparent)
            {
                boderColor = Color.FromArgb(122, 122, 122);
            }
            if (this.Controls["lblDelete"] != null)
            {
                this.Controls["lblDelete"].ForeColor = boderColor;
            }
            if (this.Controls["lblCalendar"] != null)
            {
                this.Controls["lblCalendar"].ForeColor = boderColor;
            }

            ControlPaint.DrawBorder(g, borderRect, boderColor, ButtonBorderStyle.Solid);
        }

        /// <summary>
        /// 根据控件状态获取边框颜色
        /// </summary>
        /// <returns></returns>
        private Color GetBorderColorByState()
        {
            switch (this._controlState)
            {
                case ControlState.Focused:
                    return this.Theme.MouseDownBorderColor;
                case ControlState.Highlight:
                    return this.Theme.MouseOverBorderColor;
                default:
                    return this.Theme.BorderColor;
            }
        }

        /// <summary>
        /// 绘制日期控件的右侧按钮
        /// </summary>
        /// <param name="g"></param>
        private void DrawButton(Graphics g)
        {
            GDIHelper.InitializeGraphics(g);
            //填充背景色
            RoundRectangle btnRoundRect = new RoundRectangle(this.ButtonRect, 0);
            //原始的日期时间控件不能设置BackColor
            GDIHelper.FillRectangle(g, btnRoundRect, this.Enabled ? this.Theme.BackColor : this.Theme.DisabledBackColor);

            //绘制按钮
            Size btnSize = new Size(this.Height - BORDER_OFFSET, this.Height - BORDER_OFFSET);
            //调整ButtonRect
            GDIHelper.DrawImage(g, this.ButtonRect, Properties.Resources.Calendar, btnSize);
        }

        /// <summary>
        /// 添加下拉标签
        /// </summary>
        private void AddDropImgLabel()
        {
            DateTimeImgLabel lblDelete = new DateTimeImgLabel(false)
            {
                Name = "lblDelete",
                BackColor = Color.White,
                ForeColor = this.Theme.BorderColor,
                AutoSize = false,
                Width = DELETE_WIDTH,
                Dock = DockStyle.Right,
                Image = Properties.Resources.Delete,
                ImageAlign = ContentAlignment.MiddleCenter
            };
            lblDelete.Click += this.LblDelete_Click;
            this.Controls.Add(lblDelete);

            DateTimeImgLabel lblCalendar = new DateTimeImgLabel
            {
                Name = "lblCalendar",
                BackColor = Color.White,
                ForeColor = this.Theme.BorderColor,
                AutoSize = false,
                Width = CALENDAR_WIDTH,
                Dock = DockStyle.Right,
                Image = Properties.Resources.Calendar,
                ImageAlign = ContentAlignment.MiddleCenter
            };
            lblCalendar.Click += this.LblCalendar_Click;
            lblCalendar.GotFocus += this.LblCalendar_GotFocus;
            this.Controls.Add(lblCalendar);
        }

        /// <summary>
        /// 设置下拉标签相关属性
        /// </summary>
        /// <param name="delState"></param>
        /// <param name="calendarState"></param>
        private void SetDropImgLblProp(bool delState, bool calendarState = true)
        {
            if (this.Controls["lblDelete"] != null)
            {
                this.Controls["lblDelete"].Visible = delState;
                this.Controls["lblDelete"].Height = this.Size.Height;
                this.Controls["lblDelete"].Width = DELETE_WIDTH;
            }

            if (this.Controls["lblCalendar"] != null)
            {
                this.Controls["lblCalendar"].Visible = calendarState;
                this.Controls["lblCalendar"].Height = this.Size.Height;
                this.Controls["lblCalendar"].Width = delState ? CALENDAR_WIDTH : DROP_AND_ICO_WIDTH;
                this.Controls["lblCalendar"].Location = new Point(this.Size.Width - this.Controls["lblCalendar"].Width, 0);

                if (this.Controls["lblDelete"] != null)
                {
                    this.Controls["lblDelete"].Location = new Point(this.Size.Width -
                        this.Controls["lblCalendar"].Width - this.Controls["lblDelete"].Width, 0);
                }
            }
        }

        /// <summary>
        /// 是否序列化
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeValue()
        {
            return this._userHasSetValue;
        }

        /// <summary>
        /// 清除时间日期的值
        /// </summary>
        private void ClearDateTimeValue()
        {
            base.Format = DateTimePickerFormat.Custom;
            base.CustomFormat = " ";
        }

        /// <summary>
        /// 恢复Dtp格式
        /// </summary>
        private void RestoreDtpFormat()
        {
            if (base.Format != DateTimePickerFormat.Custom || base.CustomFormat != " ") return;

            this.Format = this.Format;
            this.CustomFormat = this.CustomFormat;
        }

        /// <summary>
        /// 自动跳转到下个日期时间部分
        /// </summary>
        /// <param name="m"></param>
        private void AutoToNextPart(ref Message m)
        {
            if (m.HWnd == this.Handle)
            {
                NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR));
                if (nmhdr.code == ((0 - 760) + 1) && !this._dtpSupportKeysPressed)
                {
                    if (this.Value.HasValue && !this._isInitState) SendKeys.SendWait("{Right}");
                }
            }
            this._dtpSupportKeysPressed = false;
        }
        #endregion

        #region 事件处理
        /// <summary>
        /// 点击日历标签处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblCalendar_Click(object sender, EventArgs e)
        {
            if (!(sender is DateTimeImgLabel lblCalendar))
            {
                return;
            }

            if (!this._popFormClosed)
            {
                return;
            }

            if (!lblCalendar.Focused)
            {
                this._isFocusOnDtpLbl = true;
            }

            //先让当前选中的值生效,因为直接填写一位数时没有触发改变
            lblCalendar.Focus();

            string customFormat = this.CustomFormat;
            if (this.Format == DateTimePickerFormat.Custom && string.IsNullOrEmpty(this.CustomFormat))
            {
                customFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            }
            SparkDateTimeSelect dtmSelect = new SparkDateTimeSelect(this.Value, this.Format, customFormat)
            {
                ShowExecBtn = true,
                ReCtorDtWhenNoChange = this.ReCtorDtWhenNoChange
            };
            dtmSelect.ExecOKBtnClick += this.DtSelect_ExecOKBtnClick;
            dtmSelect.ExecCancelBtnClick += this.DtSelect_ExecCancelBtnClick;

            Form ownerFrm = this.GetValidAncestor();
            SparkPopup popup = new SparkPopup(this, dtmSelect)
            {
                Owner = ownerFrm
            };

            popup.FormClosed += this.SparkPopup_FormClosed;
            this._popFormClosed = false;
            popup.Show(ownerFrm);
        }

        /// <summary>
        /// 弹出窗体关闭处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SparkPopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            this._popFormClosed = true;
            this._isFocusOnDtpLbl = false;
            if (!(sender is SparkPopup popup) || popup.Owner == null)
            {
                return;
            }

            popup.Owner.Focus();
            this.Focus();
        }

        /// <summary>
        /// 点击清除标签处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblDelete_Click(object sender, EventArgs e)
        {
            if (!(sender is DateTimeImgLabel lblDelete))
            {
                return;
            }

            //lblDelete.Focus();
            if (!this.Focused)
            {
                this.Focus();
            }
            this.Value = null;
        }

        /// <summary>
        /// 时间日期选择界面操作按钮处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtSelect_ExecOKBtnClick(object sender, EventArgs e)
        {
            if (!(sender is SparkDateTimeSelect dtmSelect))
            {
                return;
            }

            this.Value = dtmSelect.SelectValue;
            dtmSelect?.FindForm()?.Close();
        }

        /// <summary>
        /// 取消按钮处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DtSelect_ExecCancelBtnClick(object sender, EventArgs e)
        {
            if (!(sender is SparkDateTimeSelect dtmSelect))
            {
                return;
            }

            dtmSelect?.FindForm()?.Close();
        }

        /// <summary>
        /// 日历标签获取焦点处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblCalendar_GotFocus(object sender, EventArgs e)
        {
            if (!this.Focused)
            {
                this.Focus();
            }
        }

        /// <summary>
        /// 获取有效的祖先Form
        /// </summary>
        /// <returns></returns>
        private Form GetValidAncestor()
        {
            Form form = this.FindForm();
            while (!form.TopLevel)
            {
                if (form.Parent is Form frm)
                {
                    form = frm;
                }
                else
                {
                    form = form.Parent.FindForm();
                }
            }
            return form;
        }
        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkDateTimePickerTheme Theme { get; private set; }

        #endregion

        #region IDataBinding 接口成员

        /// <summary>
        /// 获取或设置控件绑定的字段名。
        /// </summary>
        [Category("Spark"), Description("控件绑定的字段名。")]
        [DefaultValue(null)]
        public virtual string FieldName { get; set; } = null;

        /// <summary>
        /// 获取或设置控件的值。
        /// </summary>
        [Browsable(false)]
        [DefaultValue("")]
        object IDataBinding.Value
        {
            get => this.Value;
            set => this.Value = value?.ToString().ToDateTime();
        }

        #endregion
    }
}