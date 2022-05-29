using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 掩码输入控件。
    /// </summary>
    [ToolboxBitmap(typeof(MaskedTextBox))]
    public class SparkMaskedTextBox : MaskedTextBox, ISparkTheme<SparkMaskedTextBoxTheme>, IDataBinding
    {
        #region 字段

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backColor = default;

        /// <summary>
        /// 边框色
        /// </summary>
        private Color _borderColor = default;

        /// <summary>
        /// 前景色
        /// </summary>
        private Color _foreColor = default;

        /// <summary>
        /// 控件当前状态
        /// </summary>
        private ControlState _controlState = ControlState.Default;

        #endregion

        #region 属性

        private Font font = Consts.DEFAULT_FONT;
        /// <summary>
        /// 获取或设置控件显示的文本的字体。
        /// </summary>
        [Category("Spark"), Description("控件显示的文本的字体。")]
        [DefaultValue(typeof(Font), Consts.DEFAULT_FONT_STRING)]
        public override Font Font
        {
            get => this.font;
            set
            {
                if (this.font != value)
                {
                    base.Font = this.font = value;
                    this.OnFontChanged(EventArgs.Empty);
                }
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
        /// 获取或设置控件的边框样式。
        /// </summary>
        [Category("Spark"), Description("控件的边框样式。")]
        public new BorderStyle BorderStyle
        {
            set { base.BorderStyle = value != BorderStyle.None ? BorderStyle.FixedSingle : value; }
            get { return base.BorderStyle; }
        }

        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkMaskedTextBox()
        {
            base.Font = Consts.DEFAULT_FONT;
            this.Theme = new SparkMaskedTextBoxTheme(this);
        }

        #region 方法

        /// <summary>
        /// 初始化样式
        /// </summary>
        private void InitControlStyle()
        {
            switch (this._controlState)
            {
                case ControlState.Focused:
                    this._backColor = this.Theme.MouseDownBackColor;
                    this._borderColor = this.Theme.MouseDownBorderColor;
                    break;
                case ControlState.Highlight:
                    this._backColor = this.Theme.MouseOverBackColor;
                    this._borderColor = this.Theme.MouseOverBorderColor;
                    break;
                default:
                    this._backColor = this.Theme.BackColor;
                    this._borderColor = this.Theme.BorderColor;
                    this._foreColor = this.Theme.ForeColor;
                    break;
            }
            if (!this.Enabled || this.ReadOnly == true)
            {
                this._backColor = this.Theme.DisabledBackColor;
            }
        }

        /// <summary>
        /// 重绘文本框
        /// </summary>
        /// <param name="m">系统消息对象</param>
        private void Draw(ref Message m)
        {
            this.InitControlStyle();

            this.ForeColor = this._foreColor;
            this.BackColor = this._backColor;
            if (this.BorderStyle == BorderStyle.FixedSingle)
            {
                GDIHelper.DrawNonWorkAreaBorder(this, this._borderColor);
            }

            m.Result = IntPtr.Zero;
        }

        #endregion

        #region 重写基类

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this.Focused) this._controlState = ControlState.Highlight;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.Focused) this._controlState = ControlState.Default;
            base.OnMouseLeave(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            this._controlState = ControlState.Focused;
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            this._controlState = ControlState.Default;
            base.OnLostFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                this.Copy();
            }
            else if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
            {
                this.Cut();
            }
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                this.SelectAll();
            }

            base.OnKeyDown(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0xF || m.Msg == 0x133) this.Draw(ref m);
        }

        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkMaskedTextBoxTheme Theme { get; private set; }

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
        public virtual object Value
        {
            get => this.Text;
            set => this.Text = value?.ToString();
        }

        #endregion
    }
}