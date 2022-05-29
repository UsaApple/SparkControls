using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 显示在表格中的多选组合框。
	/// </summary>
	public class SparkDataGridMultiComboBoxCell : DataGridViewTextBoxCell
    {
		public override Type EditType => typeof(SparkDataGridMultiComboBoxEditingControl);
	}
}