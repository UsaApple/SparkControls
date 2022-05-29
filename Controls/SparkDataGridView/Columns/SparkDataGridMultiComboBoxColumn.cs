using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public class SparkDataGridMultiComboBoxColumn : DataGridViewTextBoxColumn
	{
		public SparkDataGridMultiComboBoxColumn()
		{
			base.CellTemplate = new SparkDataGridMultiComboBoxCell();
		}

		/// <summary>
		/// 获取一个对象，该对象表示此 <see cref="SparkComboBox"/> 所含项的集合。
		/// </summary>
		[Category("Spark"), Description("列表项的集合。")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[Localizable(true)]
		[MergableProperty(false)]
		public List<string> Items { get; private set; } = new List<string>();
	}
}