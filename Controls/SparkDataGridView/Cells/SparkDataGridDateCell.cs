using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 显示在表格中的日期框。
    /// </summary>
    public class SparkDataGridDateCell : DataGridViewTextBoxCell
    {
        #region 属性

        public override Type ValueType
        {
            get { return typeof(DateTime); }
        }

        public override Type EditType
        {
            get { return typeof(SparkDataGridDateEditingControl); }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return DateTime.Now;
            }
        }

        #endregion

        #region 重写方法

        protected override bool SetValue(int rowIndex, object value)
        {
            DateTime dt = DateTime.Now;
            if (!DateTime.TryParse(Convert.ToString(value), out dt)) return false;
            return base.SetValue(rowIndex, value);
        }

        #endregion
    }
}