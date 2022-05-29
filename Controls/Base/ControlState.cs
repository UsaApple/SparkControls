using System;

namespace SparkControls.Controls
{
	/// <summary>
	/// 控件状态枚举
	/// </summary>
	[Flags]
	internal enum ControlState
	{
		/// <summary>
		/// 默认状态
		/// </summary>
		Default = 1,

		/// <summary>
		/// 高亮状态（鼠标悬浮）
		/// </summary>
		Highlight = 2,

		/// <summary>
		/// 焦点状态（鼠标按下，输入等）
		/// </summary>
		Focused = 4,

		/// <summary>
		/// 选中状态
		/// </summary>
		Selected = 8
	}
}