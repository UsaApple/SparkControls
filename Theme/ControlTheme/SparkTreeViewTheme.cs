using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
	/// <summary>
	/// 树控件的样式
	/// </summary>
	public class SparkTreeViewTheme : SparkEditTheme
	{
		private Color nodeSplitLineColor = SparkThemeConsts.TreeViewNodeSplitLineColor;

		/// <summary>
		/// 初始 <see cref="SparkTreeViewTheme"/> 类型的新实例。
		/// </summary>
		/// <param name="control">应用主题的控件。</param>
		public SparkTreeViewTheme(Control control) : base(control)
		{
			this.CheckBoxTheme = new SparkCombineCheckBoxTheme(control);
		}

		/// <summary>
		/// 节点分割线颜色
		/// </summary>
		[DefaultValue(typeof(Color), SparkThemeConsts.TreeViewNodeSplitLineColorString)]
		[Description("节点分割线颜色")]
		public virtual Color NodeSplitLineColor
		{
			get => this.nodeSplitLineColor;
			set
			{
				if (this.nodeSplitLineColor != value)
				{
					Color oldValue = this.nodeSplitLineColor;
					this.nodeSplitLineColor = value;
					this.OnPropertyChanged(nameof(this.NodeSplitLineColor), oldValue, this.nodeSplitLineColor);
				}
			}
		}

		/// <summary>
		/// 复选框的主题
		/// </summary>
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[Description("复选框的主题")]
		public SparkCombineCheckBoxTheme CheckBoxTheme { get; private set; }
	}
}