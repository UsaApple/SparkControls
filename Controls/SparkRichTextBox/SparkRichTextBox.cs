using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	[ToolboxBitmap(typeof(RichTextBox)), Description("富文本框")]
	public class SparkRichTextBox : RichTextBox, IDataBinding
	{
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
}