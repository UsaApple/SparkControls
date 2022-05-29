using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SparkControls.Controls
{
    /// <summary>
    /// 数字控件，Value可为空
    /// </summary>
    public class SparkNumberBox2 : SparkNumberBox, IDataBinding
    {
        public SparkNumberBox2() : base()
        {
            this.IsShowDefaultValue = false;
        }

        /// <summary>
        /// 是否显示默认初始值。
        /// </summary>
        [Category("Spark"), Description("是否显示默认初始值。")]
        [DefaultValue(false)]
        public override bool IsShowDefaultValue
        {
            get => base.IsShowDefaultValue;
            set => base.IsShowDefaultValue = value;
        }

        /// <summary>
        /// 获取或设置控件关联的值。
        /// </summary>
        [Category("Spark"), Description("控件关联的值。")]
        [DefaultValue(null)]
        public new decimal? Value
        {
            get
            {
                if (decimal.TryParse(this.Text, out decimal d))
                {
                    return d;
                }
                return null;
            }
            set
            {
                if (IsShowDefaultValue == true || this.isInit == false)
                {
                    this.Text = $"{value}";
                    this.Invalidate();
                }
            }
        }


        /// <summary>
        /// 获取或设置控件的值。
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        object IDataBinding.Value
        {
            get => this.Value;
            set
            {
                if (value == null)
                {
                    this.Value = null;
                }
                else if (decimal.TryParse(value?.ToString(), out decimal d))
                {
                    this.Value = d;
                }
                else
                {
                    this.Value = default;
                }

            }
        }
    }
}
