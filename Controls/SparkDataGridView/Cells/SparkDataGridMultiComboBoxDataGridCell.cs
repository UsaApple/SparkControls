using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 显示在表格中的多选辅助输入框。
	/// </summary>
	public class SparkDataGridMultiComboBoxDataGridCell : DataGridViewTextBoxCell
	{
		public override Type EditType
		{
			get { return typeof(SparkDataGridMultiComboBoxDataGridEditingControl); }
		}
	}
}