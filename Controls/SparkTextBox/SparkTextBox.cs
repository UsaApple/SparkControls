using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
    /// <summary>
    /// 按钮控件
    /// </summary>
    [ToolboxBitmap(typeof(TextBox)), Description("文本框")]
    public class SparkTextBox : TextBox, ISparkTheme<SparkTextBoxTheme>, IDataBinding
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
        /// 文本类型
        /// </summary>
        private TextType _textType = TextType.String;

        /// <summary>
        /// 控件当前状态
        /// </summary>
        private ControlState _controlState = ControlState.Default;

        /// <summary>
        /// 验证类别
        /// </summary>
        private ValidateType _validateType = ValidateType.None;

        /// <summary>
        /// 验证标志，true表示成功，false表示失败
        /// </summary>
        private bool _isValidated = true;

        /// <summary>
        /// 是否使用数字键盘
        /// </summary>
        private bool _useNumberKeyboard = false;

        /// <summary>
        /// 描述输入预期值的提示信息
        /// </summary>
        private string _placeholder = "";
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
            get { return this.mFont; }
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
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        /// 获取或设置控件的前景色。
        /// </summary>
        [Category("Spark"), Description("控件的前景色。")]
        [DefaultValue(typeof(Color), SparkThemeConsts.ForeColorString)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

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

		/// <summary>
		/// 获取或设置描述输入预期值的提示信息。
		/// </summary>
		[Category("Spark"), Description("描述输入预期值的提示信息。")]
        [DefaultValue("")]
        public string Placeholder
        {
            get { return this._placeholder; }
            set
            {
                this._placeholder = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 获取或设置控件的文本类型。
        /// </summary>
        [Category("Spark"), Description("文本类型。")]
        [DefaultValue(TextType.String)]
        public virtual TextType TextType
        {
            set
            {
                this._textType = value;
                this.ImeMode = value != TextType.String ? ImeMode.Disable : ImeMode.NoControl;
            }
            get { return this._textType; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示数字键盘。
        /// </summary>
        [Category("Spark"), Description("是否显示数字键盘。")]
        [DefaultValue(false)]
        public bool UseNumberKeyboard
        {
            get { return this._useNumberKeyboard; }
            set
            {
                this._useNumberKeyboard = value;
                if (value && this.TextType == TextType.String) this.TextType = TextType.Decimal;
            }
        }

        /// <summary>
        /// 获取或设置控件的验证类别。
        /// </summary>
        [Category("Spark"), Description("验证类别。")]
        [DefaultValue(ValidateType.None)]
        public virtual ValidateType ValidateType
        {
            get { return this._validateType; }
            set
            {
                this._validateType = value;
                this.TextType = value == ValidateType.MobilePhone ? TextType.Int : TextType.String;
            }
        }

        /// <summary>
        /// 获取或设置验证数据的匹配模式，只有当 ValidateType 的值为“Custom”时才有效。
        /// </summary>
        [Category("Spark"), Description("验证数据的匹配模式，只有当 ValidateType 的值为“Custom”时才有效。")]
        [DefaultValue("")]
        public virtual string MatchPattern { set; get; } = "";

        /// <summary>
        /// 获取或设置控件的边框样式。
        /// </summary>
        [Category("Spark"), Description("控件的边框样式。")]
        [DefaultValue(BorderStyle.FixedSingle)]
        public new BorderStyle BorderStyle
        {
            set { base.BorderStyle = value != BorderStyle.None ? BorderStyle.FixedSingle : value; }
            get { return base.BorderStyle; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否验证空值。
        /// </summary>
        [Category("Spark"), Description("表示是否验证空值，默认不验证。")]
        [DefaultValue(false)]
        public bool IsValidateEmpty { get; set; } = false;

        /// <summary>
        /// 根据身份证获取出生日期，年龄和性别
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IDCardInfo IDCardInfo
        {
            get { return this.GetIDCardInfo(this.Text); }
        }
        #endregion

        #region 事件
        [DefaultValue(null)]
        public Func<bool> CheckDataAction { get; set; } = null;
        #endregion

        public SparkTextBox()
        {
            this.SetStyle(ControlStyles.ResizeRedraw |            // 调整大小时重绘
                ControlStyles.DoubleBuffer |                      // 双缓冲
                ControlStyles.OptimizedDoubleBuffer |             // 双缓冲
                ControlStyles.AllPaintingInWmPaint |              // 忽略窗口消息 WM_ERASEBKGND 减少闪烁
                ControlStyles.SupportsTransparentBackColor, true  // 模拟透明度
            );

            this.BorderStyle = BorderStyle.FixedSingle;
            this.Font = this.mFont;
            this.CausesValidation = true;
            this.Theme = new SparkTextBoxTheme(this);
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
                Color border = this._isValidated ? this._borderColor : this.Theme.ValidatedFailBorderColor;
                GDIHelper.DrawNonWorkAreaBorder(this, border);
            }

            if (this.Text.Length == 0 && !string.IsNullOrEmpty(this._placeholder) && !this.Focused)
            {
                Graphics g = Graphics.FromHwnd(base.Handle);
                TextRenderer.DrawText(g, this._placeholder, this.Font, this.ClientRectangle, Color.FromArgb(100, 100, 100), TextFormatFlags.VerticalCenter);
            }
            m.Result = IntPtr.Zero;
        }

        /// <summary>
        /// 验证18位身份证是否有效
        /// </summary>
        private bool IsIDCard18(string Id)
        {
            if (long.TryParse(Id.Remove(17), out long n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1) return false;//省份验证
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false) return false;//生日验证
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower()) return false;//校验码验证
            return true;//符合GB11643-1999标准
        }

        /// <summary>
        /// 验证15位身份证
        /// </summary>
        private bool IsIDCard15(string Id)
        {
            if (long.TryParse(Id, out long n) == false || n < Math.Pow(10, 14)) return false;//数字验证
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1) return false;//省份验证
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false) return false;//生日验证
            return true;//符合15位身份证标准
        }

        /// <summary>
        /// 根据身份证获取性别等信息
        /// </summary>
        /// <param name="idCardNo">身份证号</param>
        /// <returns></returns>
        private IDCardInfo GetIDCardInfo(string idCardNo)
        {
            IDCardInfo entity = new IDCardInfo();
            if (string.IsNullOrEmpty(idCardNo)) return entity;
            if (idCardNo.Length != 15 && idCardNo.Length != 18)//身份证号码只能为15位或18位其它不合法
            {
                return entity;
            }
            string strSex = string.Empty;
            if (idCardNo.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
            {
                entity.Birthday = idCardNo.Substring(6, 4) + "-" + idCardNo.Substring(10, 2) + "-" + idCardNo.Substring(12, 2);
                strSex = idCardNo.Substring(14, 3);
            }
            if (idCardNo.Length == 15)
            {
                entity.Birthday = "19" + idCardNo.Substring(6, 2) + "-" + idCardNo.Substring(8, 2) + "-" + idCardNo.Substring(10, 2);
                strSex = idCardNo.Substring(12, 3);
            }
            entity.Age = this.CalculateAge(entity.Birthday);//根据生日计算年龄
            entity.Sex = int.Parse(strSex) % 2 == 0 ? "女" : "男";
            return entity;
        }

        /// <summary>
        /// 根据出生日期，计算精确的年龄
        /// </summary>
        /// <param name="birthDate">生日</param>
        /// <returns></returns>
        private int CalculateAge(string birthDay)
        {
            DateTime birthDate = DateTime.Parse(birthDay);
            DateTime nowDateTime = DateTime.Now;
            int age = nowDateTime.Year - birthDate.Year;
            //再考虑月、天的因素
            if (nowDateTime.Month < birthDate.Month || (nowDateTime.Month == birthDate.Month && nowDateTime.Day < birthDate.Day))
            {
                age--;
            }
            return age;
        }

        /// <summary>
        /// 验证输入的字符
        /// </summary>
        /// <param name="c">当前输入的字符</param>
        /// <param name="text">输入字符后的字符串</param>
        /// <returns></returns>
        protected virtual bool CheckHandledByKeyChar(char c, string text)
        {
            return false;
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        protected virtual bool CheckData()
        {
            if (this.Enabled == false || this.ReadOnly == true || (this.IsValidateEmpty == false && string.IsNullOrEmpty(this.Text))) return true;
            if (this.CheckDataAction != null)
            {
                return this.CheckDataAction();
            }
            if (this.ValidateType == ValidateType.None) return true;
            if (this.ValidateType == ValidateType.Custom && string.IsNullOrEmpty(this.MatchPattern)) return true;
            bool flag = false;
            string msg = "";
            if (this.ValidateType == ValidateType.IdentityCard)
            {
                msg = "不是有效的身份证号码！";
                if (this.Text?.Length == 18) flag = this.IsIDCard18(this.Text);
                else flag = this.IsIDCard15(this.Text);
            }
            else
            {
                string partten = "";
                switch (this.ValidateType)
                {
                    case ValidateType.Telphone: msg = "不是有效的电话号码！"; partten = @"^(\d{3,4}-)?\d{6,8}$"; break;
                    case ValidateType.MobilePhone: msg = "不是有效的手机号码！"; partten = @"^[1]+[3,5]+\d{9}"; break;
                    case ValidateType.Email: msg = "不是有效的E-MAIN！"; partten = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"; break;
                    case ValidateType.Custom: msg = "数据格式不正确！"; partten = this.MatchPattern; break;
                }
                flag = Regex.Match(this.Text, partten).Success;
            }
            if (flag) return true;
            SparkMessageBox.ShowErrorMessage(this, msg);
            return false;
        }

        #endregion

        #region 重写基类
        protected override void OnValidating(CancelEventArgs e)
        {
            //使用此方法验证，用OnLostFocus的方式，有时候焦点无法回来，导致提示框无限循环
            base.OnValidating(e);
            if (e.Cancel == true)
            {
                if (this.IsValidateEmpty == true || !this.Text.IsNullOrEmpty())
                {
                    e.Cancel = true;
                    this._isValidated = false;
                    this.Invalidate();
                    return;
                }
                else
                {
                    e.Cancel = false;
                }
            }
            if (!this.CheckData())
            {
                e.Cancel = true;
                this._isValidated = false;
                this.Invalidate();
                return;
            }
            this._isValidated = true;
            this.Invalidate();
        }

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

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            switch (this.TextType)
            {
                case TextType.Int:
                    {
                        var flag = (e.KeyChar >= 48 && e.KeyChar <= 57);
                        if (flag)
                        {
                            string oldTxt = this.Text;
                            if (this.SelectionStart >= 0 && this.SelectionLength > 0)
                            {
                                oldTxt = oldTxt.Remove(this.SelectionStart, this.SelectionLength);
                                oldTxt = oldTxt.Insert(this.SelectionStart, e.KeyChar.ToString());
                            }
                            else
                            {
                                oldTxt = this.Text + e.KeyChar;
                            }
                            if (int.TryParse(oldTxt, out var value))
                            {
                                flag = CheckHandledByKeyChar(e.KeyChar, oldTxt);
                                e.Handled = flag;
                            }
                            else
                            {
                                e.Handled = true;
                            }
                        }
                        else
                        {
                            if (e.KeyChar == 8 || e.KeyChar == (char)Keys.Enter)
                            {

                            }
                            else
                            {
                                e.Handled = true;
                            }
                        }
                        break;
                    }
                case TextType.Decimal:
                    {
                        if (e.KeyChar == (char)Keys.Enter) break;
                        string oldTxt = this.Text;
                        if (this.SelectionStart >= 0 && this.SelectionLength > 0)
                        {
                            //oldTxt = oldTxt.Substring(this.SelectionStart, this.SelectionLength);
                            oldTxt = oldTxt.Remove(this.SelectionStart, this.SelectionLength);
                            oldTxt = oldTxt.Insert(this.SelectionStart, e.KeyChar.ToString());
                        }
                        else
                        {
                            oldTxt = this.Text + e.KeyChar;
                        }
                        if (oldTxt.Count(a => a == '.') > 1)
                        {
                            e.Handled = true;
                            return;
                        }
                        //var text = this.Text + e.KeyChar;
                        string[] array = oldTxt.Split(new char[] { '.' }, StringSplitOptions.None);
                        bool flag = ((e.KeyChar < (int)Keys.D0 || e.KeyChar > (int)Keys.D9)
                                    && e.KeyChar != (int)Keys.Back && e.KeyChar != (int)Keys.Delete) ||
                                    (e.KeyChar == (int)Keys.Delete && array.Length > 2);
                        if (!flag)
                        {
                            flag = this.CheckHandledByKeyChar(e.KeyChar, oldTxt);
                            e.Handled = flag;
                        }
                        else
                        {
                            e.Handled = true;
                        }
                        break;
                    }
            }
            base.OnKeyPress(e);
            if (!e.Handled)
            {
                if (e.KeyChar == (char)Keys.Enter && this.isEnterToTab)
                {
                    SendKeys.Send("{Tab}");
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (!this.UseNumberKeyboard || this.Enabled == false || this.ReadOnly == true) return;

            SparkNumberKeyboard con = SparkNumberKeyboard.Show(this);
            con.CanUseDot = this.TextType != TextType.Int;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0xF || m.Msg == 0x133) this.Draw(ref m);
        }

        #endregion

        #region public 方法
        /// <summary>
        /// 触发验证OnValidating方法
        /// </summary>
        public virtual void Check()
        {
            this.OnValidating(new CancelEventArgs(false));
        }
        #endregion

        #region ISparkTheme 接口成员

        /// <summary>
        /// 获取控件的主题。
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("控件的主题。")]
        public SparkTextBoxTheme Theme { get; private set; }

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

    public enum ValidateType
    {
        /// <summary>
        /// 默认
        /// </summary>
        None,
        /// <summary>
        /// 座机号码
        /// </summary>
        Telphone,
        /// <summary>
        /// 移动电话，手机
        /// </summary>
        MobilePhone,
        /// <summary>
        /// 身份证
        /// </summary>
        IdentityCard,
        /// <summary>
        /// 邮箱
        /// </summary>
        Email,
        /// <summary>
        /// 自定义正则
        /// </summary>
        Custom
    }

    /// <summary>
    /// 包含身份证信息的实体。
    /// </summary>
    public struct IDCardInfo
    {
        public string Birthday { get; set; }

        public int Age { get; set; }

        public string Sex { get; set; }
    }
}