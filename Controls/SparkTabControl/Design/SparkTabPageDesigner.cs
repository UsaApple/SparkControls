using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls.Design
{
	/// <summary>
	/// <see cref="SparkTabPage"/> 设计器。
	/// </summary>
	public class SparkTabPageDesigner : ParentControlDesigner
	{
		#region 私有变量

		private SparkTabPage TabPage;

		#endregion

		#region 重写事件
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="component"></param>
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			this.TabPage = component as SparkTabPage;
		}

		/// <summary>
		/// 属性过滤
		/// </summary>
		/// <param name="properties"></param>
		protected override void PreFilterProperties(System.Collections.IDictionary properties)
		{
			base.PreFilterProperties(properties);

			properties.Remove("Dock");
			properties.Remove("AutoScroll");
			properties.Remove("AutoScrollMargin");
			properties.Remove("AutoScrollMinSize");
			properties.Remove("DockPadding");
			properties.Remove("DrawGrid");
			properties.Remove("Font");
			properties.Remove("Padding");
			properties.Remove("MinimumSize");
			properties.Remove("MaximumSize");
			properties.Remove("Margin");
			properties.Remove("ForeColor");
			//properties.Remove("BackColor");
			properties.Remove("BackgroundImage");
			properties.Remove("BackgroundImageLayout");
			properties.Remove("RightToLeft");
			properties.Remove("GridSize");
			properties.Remove("ImeMode");
			properties.Remove("BorderStyle");
			properties.Remove("AutoSize");
			properties.Remove("AutoSizeMode");
			properties.Remove("Location");
		}

		/// <summary>
		/// 选中规则标识符
		/// </summary>
		public override SelectionRules SelectionRules
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// 指示指定设计器的控件是否可以成为此设计器控件的父级
		/// </summary>
		/// <param name="parentDesigner"></param>
		/// <returns></returns>
		public override bool CanBeParentedTo(IDesigner parentDesigner)
		{
			if (parentDesigner == null) return false;
			return (parentDesigner.Component is SparkTabControl);
		}

		/// <summary>
		/// 当设计器正在管理的控件绘制了它的表面时调用，这样设计器就可以在控件顶部绘制任意附加装饰
		/// </summary>
		/// <param name="pe"></param>
		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			if (this.TabPage != null)
			{
				using (Pen p = new Pen(SystemColors.ControlDark))
				{
					p.DashStyle = DashStyle.Dash;
					pe.Graphics.DrawRectangle(p, 0, 0, this.TabPage.Width - 1, this.TabPage.Height - 1);
				}
			}
		}

		#endregion
	}
}