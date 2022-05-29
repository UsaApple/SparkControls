using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
	/// <summary>
	/// 控件 <see cref="SparkComboBox"/> 的设计模式类。
	/// </summary>
	public class SparkComboBoxDesigner : ControlDesigner
	{
		private readonly ComboBoxStyle mComboBoxStyle = ComboBoxStyle.DropDownList;

		/// <summary>
		/// 初始 <see cref="SparkComboBoxDesigner" /> 类型的新实例。
		/// </summary>
		public SparkComboBoxDesigner()
		{

		}

		/// <summary>
		/// 初始 <see cref="SparkComboBoxDesigner" /> 类型的新实例。
		/// <param name="comboBoxStyle">绑定到的 ComboBox 对象的样式。</param>
		/// </summary>
		public SparkComboBoxDesigner(ComboBoxStyle comboBoxStyle)
		{
			mComboBoxStyle = comboBoxStyle;
		}

		/// <summary>
		/// 获取指示组件移动功能的选择规则。
		/// </summary>
		public override SelectionRules SelectionRules
		{
			get
			{
				if (mComboBoxStyle == ComboBoxStyle.Simple)
				{
					return base.SelectionRules;
				}
				else
				{
					return SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
				}
			}
		}
	}
}