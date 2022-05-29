using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 显示在表格中的组合框。
	/// </summary>
	public class SparkDataGridComboBoxCell : DataGridViewComboBoxCell
    {
        #region 字段

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backColor = default(Color);

        /// <summary>
        /// 边框色
        /// </summary>
        private Color _borderColor = default(Color);

        /// <summary>
        /// 前景色
        /// </summary>
        private Color _foreColor = default(Color);

        /// <summary>
        /// 控件当前状态
        /// </summary>
        private ControlState _controlState = ControlState.Default;

        /// <summary>
        /// 按钮宽度
        /// </summary>
        private int _buttonWidth = 15;

        #endregion

        #region 属性

        public override Type EditType
        {
            get { return typeof(SparkDataGridComboBoxEditingControl); }
        }

        #endregion

        #region 私有方法

        private void InitControlStyle(int rowIndex, DataGridViewCellStyle cellStyle)
        {
            var dgv = this.DataGridView as SparkDataGridView;
            _foreColor = dgv.Theme.ForeColor;
            switch (_controlState)
            {
                case ControlState.Focused:
                    _backColor = dgv.Theme.MouseDownBackColor;
                    _borderColor = dgv.Theme.MouseDownBorderColor;
                    break;
                case ControlState.Highlight:
                    _backColor = dgv.Theme.MouseOverBackColor;
                    _borderColor = dgv.Theme.MouseOverBorderColor;
                    break;
                case ControlState.Selected:
                    _backColor = dgv.Theme.SelectedBackColor;
                    _borderColor = dgv.Theme.SelectedBorderColor;
                    _foreColor = cellStyle.SelectionForeColor;
                    break;
                default:
                    _backColor = cellStyle.BackColor;
                    _borderColor = dgv.Theme.BorderColor;
                    break;
            }
        }

        /// <summary>
        /// 重绘按钮
        /// </summary>
        /// <param name="g">上下文</param>
        /// <param name="rect">按钮区域大小</param>
        private void Draw(Graphics g, Rectangle rect, DataGridViewCellStyle cellStyle, 
            string value, int rowIndex, bool cellSelected)
        {
            InitControlStyle(rowIndex, cellStyle);
            Color bColor;
            if (DataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect)
            {
                bColor = cellSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor;
            }
            else
            {
                bColor = this.DataGridView.CurrentRow?.Index == rowIndex ? cellStyle.SelectionBackColor : cellStyle.BackColor;
            }
            GDIHelper.FillRectangle(g, rect, bColor);
            rect.X -= 1;
            rect.Y -= 1;
            GDIHelper.DrawRectangle(g, rect, this.DataGridView.GridColor);
            GDIHelper.DrawString(g, rect, value, cellStyle.Font, _foreColor, StringAlignment.Near);
            DrawButton(g, rect, bColor);
        }

        /// <summary>
        ///  绘制按钮。
        /// </summary>
        private void DrawButton(Graphics g, Rectangle rect, Color backColor)
        {
            Rectangle btnRect = new Rectangle(rect.Right - _buttonWidth, rect.Top, _buttonWidth, rect.Height);
            GDIHelper.FillRectangle(g, btnRect, backColor);
            GDIHelper.DrawRectangle(g, btnRect, this.DataGridView.GridColor);
            GDIHelper.DrawArrow(g, ArrowDirection.Down, btnRect, new Size(10, 7), 0f, _borderColor);
        }

        #endregion

        #region 重写基类

        protected override void OnMouseEnter(int rowIndex)
        {
            _controlState = ControlState.Highlight;
            base.OnMouseEnter(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            _controlState = ControlState.Focused;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            _controlState = ControlState.Selected;
            base.OnMouseUp(e);
        }

        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            base.OnContentClick(e);
        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            var column = this.DataGridView.Columns[this.ColumnIndex];
            var style = (this.DataGridView as SparkDataGridView).QueryComboBoxStyle(this.ColumnIndex, column.Name, column.HeaderText);
            if (style == ComboBoxStyle.DropDown) return formattedValue;
            if (formattedValueTypeConverter == null || valueTypeConverter == null) return formattedValue;
            return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;
            if (cellSelected && _controlState != ControlState.Focused)
            {
                _controlState = ControlState.Selected;
            }
            else if (_controlState != ControlState.Focused) _controlState = ControlState.Default;
            var column = this.DataGridView.Columns[this.ColumnIndex];
            var style = (this.DataGridView as SparkDataGridView).QueryComboBoxStyle(this.ColumnIndex, column.Name, column.HeaderText);
            var text = "" + formattedValue;
            //if (style == ComboBoxStyle.DropDown) 
                text = "" + value;
            Draw(graphics, cellBounds, cellStyle, "" + text, rowIndex, cellSelected);
        }

        #endregion
    }
}