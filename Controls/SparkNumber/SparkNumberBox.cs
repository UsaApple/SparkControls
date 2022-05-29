using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 数字输入控件。
    /// </summary>
    [ToolboxBitmap(typeof(NumericUpDown)), Description("数字控件")]
    public class SparkNumberBox : SparkTextBox, IDataBinding, ISupportInitialize
    {
        #region 字段
        protected bool isInit = false;

        protected bool _isDecimal = true;

        /// <summary>
        /// 小数位数
        /// </summary>
        private uint _decimalPlaces = 0;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置控件的文本类型。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override TextType TextType
        {
            set => base.TextType = value == TextType.String ? TextType.Decimal : value;
            get => base.TextType;
        }

        /// <summary>
        /// 获取或设置控件的验证类别。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ValidateType ValidateType
        {
            get => base.ValidateType;
            set => base.ValidateType = value;
        }

        /// <summary>
        /// 获取或设置验证数据的匹配模式，只有当 ValidateType 的值为“Custom”时才有效。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string MatchPattern
        {
            set => base.MatchPattern = value;
            get => base.MatchPattern;
        }

        private HorizontalAlignment _textAlign = HorizontalAlignment.Right;
        private bool isShowDefaultValue = true;

        /// <summary>
        /// 获取或设置控件中文本的对齐方式。
        /// </summary>
        [Category("Spark"), DefaultValue(HorizontalAlignment.Right), Description("控件中文本的对齐方式。")]
        public new HorizontalAlignment TextAlign
        {
            get => this._textAlign;
            set => base.TextAlign = this._textAlign = value;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否允许输入小数。
        /// </summary>
        [Category("Spark"), Description("是否允许输入小数。")]
        [DefaultValue("true")]
        public bool IsDecimal
        {
            get => this._isDecimal;
            set
            {
                this._isDecimal = value;
                this.TextType = value ? TextType.Decimal : TextType.Int;
            }
        }

        /// <summary>
        /// 获取或设置控件允许输入的最大值。
        /// </summary>
        [Category("Spark"), Description("允许输入的最大值。")]
        [DefaultValue(typeof(decimal), "2147483647")]
        public decimal MaxValue { set; get; } = int.MaxValue;

        /// <summary>
        /// 获取或设置控件允许输入的最小值。
        /// </summary>
        [Category("Spark"), Description("允许输入的最小值。")]
        [DefaultValue(typeof(decimal), "0")]
        public decimal MinValue { set; get; }

        /// <summary>
        /// 获取或设置控件关联的值。
        /// </summary>
        [Category("Spark"), Description("控件关联的值。")]
        [DefaultValue(typeof(decimal), "0")]
        public new decimal Value
        {
            get
            {
                decimal.TryParse(this.Text, out decimal d);
                return d;
            }
            set
            {
                if (IsShowDefaultValue == true || this.isInit == false)
                {
                    this.Text = value.ToString();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 获取或设置控件允许输入的小数位数。
        /// </summary>
        [Category("Spark"), Description("允许输入的小数位数。")]
        [DefaultValue(typeof(uint), "0")]
        public uint DecimalPlaces
        {
            get => this._decimalPlaces;
            set
            {
                this._decimalPlaces = value;
                this.TextType = value == 0 ? TextType.Int : TextType.Decimal;
            }
        }

        /// <summary>
        /// 是否显示默认初始值。
        /// </summary>
        [Category("Spark"), Description("是否显示默认初始值。")]
        [DefaultValue(true)]
        public virtual bool IsShowDefaultValue
        {
            get => isShowDefaultValue;
            set
            {
                isShowDefaultValue = value;
                if (isShowDefaultValue)
                {//显示初始值
                    if (this.Text.IsNullOrEmpty()) this.Text = "0";
                }
                else
                {
                    //不显示初始值0
                    if (this.Text == "0") this.Text = "";
                }
            }
        }
        #endregion

        /// <summary>
        /// 初始 <see cref="SparkNumberBox"/> 类型的新实例。
        /// </summary>
        public SparkNumberBox()
        {
            this.TextAlign = HorizontalAlignment.Right;
            this.TextType = TextType.Decimal;

            if (IsShowDefaultValue)
            {
                this.Text = "0";
            }
        }

        #region 方法

        protected override bool CheckHandledByKeyChar(char c, string text)
        {
            if (c == 8) return false;
            if (this.DecimalPlaces == 0 && text.Contains(".")) return true;
            if (this.DecimalPlaces != 0 && text.Contains("."))
            {
                int index = text.IndexOf(".") + 1;
                if (text.Substring(index).Length > this.DecimalPlaces) return true;
            }
            if (!decimal.TryParse(text, out decimal d)) return true;
            if (this.MaxValue != this.MinValue && (d > this.MaxValue)) return true;
            return base.CheckHandledByKeyChar(c, text);
        }

        protected override bool CheckData()
        {
            var flag = Check(out var error);
            if (flag == false)
            {
                if (!error.IsNullOrEmpty())
                {
                    SparkMessageBox.ShowErrorMessage(this, error);
                }
            }
            return flag;
        }

        public bool Check(out string error)
        {
            error = "";
            if (this.Enabled == false || this.ReadOnly == true || (this.IsValidateEmpty == false && string.IsNullOrEmpty(this.Text)))
            {
                return true;
            }
            if (this.CheckDataAction != null)
            {
                return this.CheckDataAction();
            }

            string txt = this.Text;
            //如果为空的话，填上默认值
            if (string.IsNullOrEmpty(this.Text))
            {
                txt = this.MinValue > 0 ? $"{this.MinValue}" : "0";
                if (IsShowDefaultValue) this.Text = txt;
            }
            string mess = "";
            if (!decimal.TryParse(txt, out decimal d)) mess = "数据格式不正确。";
            if (d < this.MinValue) mess = $"不能小于最小值{this.MinValue}。";
            if (d > this.MaxValue) mess = $"不能大于最大值{this.MaxValue}";
            if (!string.IsNullOrEmpty(mess))
            {
                //Comm.ShowErrorMessage(this, mess);
                error = mess;
                this.Focus();
                this.SelectAll();
                return false;
            }
            return true;
        }

        public void BeginInit()
        {
            isInit = true;
        }

        public void EndInit()
        {
            isInit = false;
        }

        #endregion



        #region IDataBinding 接口成员

        /// <summary>
        /// 获取或设置控件绑定的字段名。
        /// </summary>
        [Category("Spark"), Description("控件绑定的字段名。")]
        [DefaultValue(null)]
        public override string FieldName { get; set; } = null;

        /// <summary>
        /// 获取或设置控件的值。
        /// </summary>
        [Browsable(false)]
        [DefaultValue("")]
        object IDataBinding.Value
        {
            get => this.Value;
            set => this.Value = decimal.TryParse(value?.ToString(), out decimal d) ? d : default;
        }

        #endregion
    }
}