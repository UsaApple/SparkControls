using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 显示在表格中的按钮。
	/// </summary>
	public class SparkDataGridButtonCell : DataGridViewButtonCell
	{
		#region 字段

		/// <summary>
		/// 按钮背景色
		/// </summary>
		private Color _backColor = default(Color);

		/// <summary>
		/// 按钮边框色
		/// </summary>
		private Color _borderColor = default(Color);

		/// <summary>
		/// 按钮前景色
		/// </summary>
		private Color _foreColor = default(Color);

		/// <summary>
		/// 控件当前状态
		/// </summary>
		private ControlState _controlState = ControlState.Default;

		#endregion

		#region 私有方法

		private void InitControlStyle(int rowIndex, DataGridViewCellStyle cellStyle)
		{
			SparkDataGridView dgv = this.DataGridView as SparkDataGridView;
			this._foreColor = dgv.Theme.ForeColor;
			switch (this._controlState)
			{
				case ControlState.Focused:
					this._backColor = dgv.Theme.MouseDownBackColor;
					this._borderColor = dgv.Theme.MouseDownBorderColor;
					break;
				case ControlState.Highlight:
					this._backColor = dgv.Theme.MouseOverBackColor;
					this._borderColor = dgv.Theme.MouseOverBorderColor;
					break;
				case ControlState.Selected:
					this._backColor = dgv.Theme.SelectedBackColor;
					this._borderColor = dgv.Theme.SelectedBorderColor;
					this._foreColor = cellStyle.SelectionForeColor;
					break;
				default:
					this._backColor = cellStyle.BackColor;
					this._borderColor = dgv.Theme.BorderColor;
					break;
			}
		}

		/// <summary>
		/// 重绘按钮
		/// </summary>
		/// <param name="g">上下文</param>
		/// <param name="rect">按钮区域大小</param>
		private void Draw(Graphics g, Rectangle rect, string value, DataGridViewCellStyle cellStyle, int rowIndex)
		{
			this.InitControlStyle(rowIndex, cellStyle);

			Color bColor = this.DataGridView.CurrentRow?.Index == rowIndex ? cellStyle.SelectionBackColor : cellStyle.BackColor;
			GDIHelper.FillRectangle(g, rect, bColor);
			rect.X -= 1;
			rect.Y -= 1;
			GDIHelper.DrawRectangle(g, rect, this.DataGridView.GridColor);
			rect.Inflate(-3, -3);
			GDIHelper.FillRectangle(g, rect, this._backColor);
			GDIHelper.DrawRectangle(g, rect, this._borderColor);
			GDIHelper.DrawString(g, rect, value, cellStyle.Font, this._foreColor);
		}

		#endregion

		#region 重写基类

		protected override void OnMouseEnter(int rowIndex)
		{
			this._controlState = ControlState.Highlight;
			base.OnMouseEnter(rowIndex);
		}

		protected override void OnMouseLeave(int rowIndex)
		{
			base.OnMouseLeave(rowIndex);
		}

		protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
		{
			this._controlState = ControlState.Focused;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
		{
			this._controlState = ControlState.Selected;
			base.OnMouseUp(e);
		}

		protected override void OnClick(DataGridViewCellEventArgs e)
		{
			base.OnClick(e);
			SparkDataGridView dgv = this.DataGridView as SparkDataGridView;
			SparkGridButtonClickEventArgs args = new SparkGridButtonClickEventArgs()
			{
				RowIndex = e.RowIndex,
				ColumnIndex = e.ColumnIndex,
				Owner = dgv
			};
			dgv?.RaiseGridButtonClick(this, args);
		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
			DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
			DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
			DataGridViewPaintParts paintParts)
		{
			if (this.DataGridView.CurrentRow?.Index == rowIndex && this._controlState != ControlState.Focused)
			{
				this._controlState = ControlState.Selected;
			}
			else if (this._controlState != ControlState.Focused) this._controlState = ControlState.Default;

			this.Draw(graphics, cellBounds, "" + formattedValue, cellStyle, rowIndex);
		}

		#endregion
	}
}