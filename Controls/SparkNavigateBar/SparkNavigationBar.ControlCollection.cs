using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public partial class SparkNavigationBar
	{
		/// <summary>
		/// SparkNavigateBarPanel容器的集合
		/// </summary>
		private new class ControlCollection : Control.ControlCollection
		{
			public ControlCollection(SparkNavigationBar owner)
				: base(owner)
			{
			}

			public new SparkNavigationBar Owner => base.Owner as SparkNavigationBar;

			public override void Add(Control value)
			{
				if (value == this.Owner._tabStrip)
				{
					base.Add(value);
				}
				else
				{
					SparkNavigateBarPanel page = (value as SparkNavigateBarPanel);
					if (page == null)
					{
						throw new ArgumentException($"value的对象不是{nameof(SparkNavigateBarPanel)}。");
					}

					//this.Owner.SuspendLayout();

					base.Add(page);
					this.Owner.Add(page);
					this.Owner.Refresh();
					//this.Owner.ResumeLayout(false);
				}
			}

			public override void AddRange(Control[] controls)
			{
				foreach (Control item in controls)
				{
					if (!(item is SparkNavigateBarPanel))
					{
						throw new ArgumentException($"controls对象不是{nameof(SparkNavigateBarPanel)}的集合");
					}
				}

				SparkNavigateBarPanel[] pages = new SparkNavigateBarPanel[controls.Length];
				controls.CopyTo(pages, 0);

				//this.Owner.SuspendLayout();

				base.AddRange(pages);
				this.Owner.AddRange(pages);

				this.Owner.Refresh();
				//this.Owner.ResumeLayout(false);
			}

			public override void Clear()
			{
				this.Owner.RemoveAllTabs();
			}

			public override void Remove(Control value)
			{
				SparkNavigateBarPanel page = (value as SparkNavigateBarPanel);

				//this.Owner.SuspendLayout();

				if (page != null && this.Contains(value))
				{
					this.Owner.Remove(page);
				}

				base.Remove(value);

				this.Owner.Refresh();
				//this.Owner.ResumeLayout(false);
			}
		}
	}
}