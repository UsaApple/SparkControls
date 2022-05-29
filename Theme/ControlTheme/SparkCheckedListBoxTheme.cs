using System.ComponentModel;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 复选框列表控件的样式
	/// </summary>
	public class SparkCheckedListBoxTheme : SparkEditTheme
	{
		/// <summary>
		/// 初始 <see cref="SparkCheckedListBoxTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkCheckedListBoxTheme(Control control) : base(control)
		{
			this.CheckBoxTheme = new SparkCombineCheckBoxTheme(control);
		}

		/// <summary>
		/// CheckBox的主题
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("CheckBox的主题"), Category("Spark")]
		public SparkCombineCheckBoxTheme CheckBoxTheme { get; private set; }
	}
}