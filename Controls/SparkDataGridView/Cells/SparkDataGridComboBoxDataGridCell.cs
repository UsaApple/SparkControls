using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 显示在表格中的辅助输入框。
    /// </summary>
    public class SparkDataGridComboBoxDataGridCell : DataGridViewTextBoxCell
    {
		public override Type EditType => typeof(SparkDataGridComboBoxDataGridEditingControl);
	}
}