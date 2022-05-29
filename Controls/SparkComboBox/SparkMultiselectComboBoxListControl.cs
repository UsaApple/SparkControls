using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 多选组合框下拉列表控件。
	/// </summary>
	[ToolboxItem(false)]
	public class SparkMultiselectComboBoxListControl : SparkCheckedListBox
	{
		private readonly SparkMultiselectComboBox mOwner;

		/// <summary>
		/// 初始 <see cref="SparkMultiselectComboBoxListControl" /> 类型的新实例。
		/// </summary>
		/// <param name="owner">所属多选组合框控件。</param>
		public SparkMultiselectComboBoxListControl(SparkMultiselectComboBox owner) : base()
		{
			mOwner = owner;

			CheckOnClick = true;
			DoubleBuffered = true;
			ResizeRedraw = true;
			MinimumSize = new Size(10, 10);
			MaximumSize = new Size(960, 720);
			this.DisplayMember = owner.DisplayMember;
			this.ValueMember = owner.ValueMember;
		}

		/// <summary>
		/// 处理 Windows 消息。
		/// </summary>
		/// <param name="m">Windows 消息。</param>
		protected override void WndProc(ref Message m)
		{
			if ((Parent is Popup popup) && popup.ProcessResizing(ref m))
			{
				return;
			}
			base.WndProc(ref m);
		}

		/// <summary>
		/// 引发 VisibleChanged 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnVisibleChanged(EventArgs e)
		{
			SynchronizeControlsWithComboBoxItems();
			base.OnVisibleChanged(e);
		}

		/// <summary>
		/// 同步选项列表。
		/// </summary>
		public void SynchronizeControlsWithComboBoxItems()
		{
			var thisItems = Items.Cast<object>();
			var ownerItems = mOwner.Items.Cast<object>();

			var except1 = thisItems.Except(ownerItems);
			var except2 = ownerItems.Except(thisItems);

			if (except1.Any() || except2.Any())
			{
				Items.Clear();
				Items.AddRange(mOwner.Items);
			}
		}
	}
}