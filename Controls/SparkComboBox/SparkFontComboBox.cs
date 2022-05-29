using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 字体组合框控件。
	/// </summary>
	[ToolboxBitmap(typeof(SparkFontComboBox), "ttf.bmp")]
	public class SparkFontComboBox : SparkComboBox
	{
		private readonly Image ttimg = Properties.Resources.ttf;
		private int maxWidth = 0;

		/// <summary>
		/// 获取组合框的样式。
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//[EditorBrowsable(EditorBrowsableState.Never)]
		public new ComboBoxStyle DropDownStyle
		{
			get { return base.DropDownStyle; }
			private set { base.DropDownStyle = value; }
		}

		/// <summary>
		/// 获取组合框的绘制模式。
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//[EditorBrowsable(EditorBrowsableState.Never)]
		public new DrawMode DrawMode
		{
			get { return base.DrawMode; }
			private set { base.DrawMode = value; }
		}

		/// <summary>
		/// 初始 <see cref="SparkFontComboBox" /> 类型的新实例。
		/// </summary>
		public SparkFontComboBox()
		{
			DrawMode = DrawMode.OwnerDrawVariable;
			DropDownStyle = ComboBoxStyle.DropDownList;
			MaxDropDownItems = 20;

			Items.AddRange(FontFamily.Families.Where(ff => ff.IsStyleAvailable(FontStyle.Regular)).Select(ff => ff.Name).ToArray());
		}

		/// <summary>
		/// 引发 MeasureItem 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			if (e.Index > -1)
			{
				string itemText = Items[e.Index].ToString();
				using (Font font = new Font(itemText, this.Font.Size))
				{
					Size itemSize = Size.Ceiling(GDIHelper.MeasureString(CreateGraphics(), itemText, font));
					e.ItemHeight = Math.Min(itemSize.Height, 20);

					int w = itemSize.Width + ttimg.Width;
					if (w > maxWidth) { maxWidth = w; }
				}
			}

			base.OnMeasureItem(e);
		}

		/// <summary>
		/// 引发 DrawItem 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index > -1)
			{
				string itemText = Items[e.Index].ToString();
				Font font = new Font(itemText, this.Font.Size);
				if ((e.State & DrawItemState.Focus) == 0)
				{
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window),
						e.Bounds.X + ttimg.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
					e.Graphics.DrawString(itemText, font, new SolidBrush(SystemColors.WindowText),
						e.Bounds.X + ttimg.Width, e.Bounds.Y);

				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight),
						e.Bounds.X + ttimg.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
					e.Graphics.DrawString(itemText, font, new SolidBrush(SystemColors.HighlightText),
						e.Bounds.X + ttimg.Width, e.Bounds.Y);
				}
				e.Graphics.DrawImage(ttimg, new Point(e.Bounds.X, e.Bounds.Y));
			}

			base.OnDrawItem(e);
		}

		/// <summary>
		/// 引发 DropDown 事件。
		/// </summary>
		/// <param name="e">事件参数。</param>
		protected override void OnDropDown(EventArgs e)
		{
			this.DropDownWidth = Math.Max(maxWidth + 10, this.Width);
			base.OnDropDown(e);
		}
	}
}