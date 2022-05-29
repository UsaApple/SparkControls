using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace SparkControls.Controls
{
	/// <summary>
	/// 工具栏布局
	/// </summary>
	internal sealed partial class SparkTabStripLayoutEngine : LayoutEngine
	{
		internal const int TabOverlap = 1;

		private SparkNavigateToolStrip _strip;
		private TabStripScrollDirection _direction;

		private int? _availableWidth;
		private int _nearTab;
		private int _farTab;

		public SparkTabStripLayoutEngine(SparkNavigateToolStrip strip)
		{
			_strip = strip;
		}

		private bool RTL
		{
			get { return _strip.RightToLeft == RightToLeft.Yes; }
		}

		private Rectangle DisplayRectangle
		{
			get { return _strip.DisplayRectangle; }
		}

		private int AvailableWidth
		{
			get
			{
				if (_availableWidth == null)
				{
					_availableWidth = _strip.DisplayRectangle.Width;

					if (ContainsScrollNearButton)
					{
						_availableWidth -= ScrollNearButton.Width;
					}

					if (ContainsScrollFarButton)
					{
						_availableWidth -= ScrollFarButton.Width;
					}

					foreach (var item in GetAvailableUnknownItems())
					{
						_availableWidth -= item.Width;
					}

					if (_availableWidth < MinimumTabWidth)
					{
						_availableWidth = MinimumTabWidth;
					}
				}

				return _availableWidth.Value;
			}
		}

		private int MaximumTabWidth
		{
			get { return 140; }
		}

		private int MinimumTabWidth
		{
			get { return 20; }
		}

		private int TabCount
		{
			get { return _strip.TabCount; }
		}

		private int SelectedTabIndex
		{
			get { return _strip.SelectedTabIndex; }
			set { _strip.SelectedTabIndex = value; }
		}

		private SparkNavigationBarStripButton ScrollNearButton
		{
			get { return _strip.ScrollNearButton; }
		}

		private SparkNavigationBarStripButton ScrollFarButton
		{
			get { return _strip.ScrollFarButton; }
		}

		private bool ContainsScrollNearButton
		{
			get { return GetButtonVisibility(ScrollNearButton); }
			set { SetButtonVisibility(ScrollNearButton, value); }
		}

		private bool ContainsScrollFarButton
		{
			get { return GetButtonVisibility(ScrollFarButton); }
			set { SetButtonVisibility(ScrollFarButton, value); }
		}

		private void LayoutItem(ToolStripItem item, Point location, Size size)
		{
			_strip.LayoutItem(item, location, size);
		}

		private IEnumerable<SparkNavigationBarStripButton> GetTabItems()
		{
			return _strip.ItemsOfType<SparkNavigationBarStripButton>();
		}

		private IEnumerable<ToolStripItem> GetAvailableUnknownItems()
		{
			foreach (ToolStripItem item in _strip.Items)
			{
				if (!(item != ScrollNearButton &&
					  item != ScrollFarButton &&
					  item.Available))
				{
					yield return item;
				}
			}
		}

		private bool GetButtonVisibility(ToolStripItem button)
		{
			return _strip.Items.Contains(button);
		}

		private void SetButtonVisibility(ToolStripItem button, bool value)
		{
			if (GetButtonVisibility(button) != value)
			{
				_availableWidth = null;

				if (value)
				{
					_strip.Items.Insert(0, button);
				}
				else
				{
					_strip.Items.Remove(button);
				}
			}
		}

		internal void ClearVolatileState()
		{
			_availableWidth = null;
		}

		public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
		{
			var tabStrip = (container as SparkNavigateToolStrip);
			//if (tabStrip != _strip)
			//{
			//    throw new ArgumentException();
			//}

			//using (var transaction = new TabStripLayoutPass(this))
			//{
			//    transaction.DoLayout();
			//}

			return base.Layout(container, layoutEventArgs);
			//return tabStrip.AutoSize;
		}

		public override void InitLayout(object child, BoundsSpecified specified)
		{
			base.InitLayout(child, specified);
		}

		public TabStripScrollDirection ScrollDirection
		{
			get { return _direction; }
			set { _direction = value; }
		}
	}
}