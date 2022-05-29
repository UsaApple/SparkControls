using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 显示在表格中的文本框。
	/// </summary>
	public class SparkDataGridTextBoxCell : DataGridViewTextBoxCell
    {
		public override Type EditType => typeof(SparkDataGridTextBoxEditingControl);
	}
}