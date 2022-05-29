using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 显示在表格中的复选框。
    /// </summary>
    public class SparkDataGridCheckBoxCell : DataGridViewCheckBoxCell
    {
        #region 字段

        /// <summary>
        /// 背景色
        /// </summary>
        private Color _backColor = default(Color);

        /// <summary>
        /// 单元格背景色
        /// </summary>
        private Color _cellBackColor = default(Color);

        /// <summary>
        /// 边框色
        /// </summary>
        private Color _borderColor = default(Color);

        /// <summary>
        /// 勾的颜色
        /// </summary>
        private Color _tickColor = default(Color);

        /// <summary>
        /// 控件当前状态
        /// </summary>
        private ControlState _controlState = ControlState.Default;

        /// <summary>
        /// 是否点击内容区域
        /// </summary>
        private bool _isClickContent = false;

        /// <summary>
        /// 当前状态
        /// </summary>
        private CheckState _currentCheckState = CheckState.Unchecked;

        #endregion

        #region 私有方法

        private void InitControlStyle(int rowIndex, DataGridViewCellStyle cellStyle, CheckState state)
        {
			SparkDataGridView dgv = this.DataGridView as SparkDataGridView;
            if (_controlState == ControlState.Selected && state == CheckState.Checked)
            {
                //组合选中
                _backColor = dgv.Theme.CheckBoxTheme.CombinedBackColor;
                _borderColor = dgv.Theme.CheckBoxTheme.CombinedSelectedColor;
                _tickColor = dgv.Theme.CheckBoxTheme.CombinedSelectedColor;
            }
            else if (state == CheckState.Checked)
            {
                _backColor = dgv.Theme.CheckBoxTheme.SelectedBackColor;
                _borderColor = dgv.Theme.CheckBoxTheme.SelectedBorderColor;
                _tickColor = dgv.Theme.CheckBoxTheme.TickColor;
            }
            else
            {
                _backColor = dgv.Theme.CheckBoxTheme.BackColor;
                _borderColor = dgv.Theme.CheckBoxTheme.BorderColor;
                _tickColor = dgv.Theme.CheckBoxTheme.TickColor;
            }

            if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect)
            {
                var flag = DataGridView.CurrentRow?.Index == this.RowIndex;
                _cellBackColor = flag ? cellStyle.SelectionBackColor : cellStyle.BackColor;
            }
            else
            {
                var cell = DataGridView.CurrentCell;
                if (cell != null && cell.RowIndex == this.RowIndex && cell.ColumnIndex == this.ColumnIndex)
                {
                    _cellBackColor = cellStyle.SelectionBackColor;
                }
                else _cellBackColor = cellStyle.BackColor;
            }
        }

        /// <summary>
        /// 重绘按钮
        /// </summary>
        /// <param name="g">上下文</param>
        /// <param name="rect">按钮区域大小</param>
        private void Draw(Graphics g, Rectangle clipBounds, Rectangle rect,
            CheckState state, DataGridViewCellStyle cellStyle, int rowIndex,
            DataGridViewPaintParts paintParts, DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            InitControlStyle(rowIndex, cellStyle, state);
            GDIHelper.FillRectangle(g, rect, _cellBackColor);
            //设置无单元格边框时不需要画边框
            if ((paintParts & DataGridViewPaintParts.Border) != 0)
            {
                PaintBorder(g, clipBounds, rect, cellStyle, advancedBorderStyle);
            }

            if (rect.Width < Consts.CHECK_BOX_SIZE.Width || rect.Height < Consts.CHECK_BOX_SIZE.Height) return;
            var x = rect.X + (rect.Width - Consts.CHECK_BOX_SIZE.Width) / 2;
            var y = rect.Y + (rect.Height - Consts.CHECK_BOX_SIZE.Height) / 2;
            var boxRect = new Rectangle(x, y, Consts.CHECK_BOX_SIZE.Width, Consts.CHECK_BOX_SIZE.Height);

            GDIHelper.DrawCheckBox(g, new RoundRectangle(boxRect,
                new CornerRadius(0, 0, 0, 0)), _backColor, _borderColor, 1);
            switch (state)
            {
                case CheckState.Checked:
                    var graphicStaet = g.Save();
                    GDIHelper.DrawCheckTick(g, boxRect, _tickColor);
                    g.Restore(graphicStaet);
                    break;
                case CheckState.Indeterminate:
                    boxRect.Inflate(-3, -3);
                    GDIHelper.FillRectangle(g, boxRect, _borderColor);
                    break;
            }
            if (state != _currentCheckState) OnGridCheckBoxStateChanged(state);
            _currentCheckState = state;
        }

        /// <summary>
        /// 获取状态
        /// </summary
        private CheckState GetCheckState(object value)
        {
            CheckState checkState = CheckState.Unchecked;
            if (value != null && value is CheckState) checkState = (CheckState)value;
            else if (value != null && value is bool)
            {
                if ((bool)value) checkState = CheckState.Checked;
                else checkState = CheckState.Unchecked;
            }
            return checkState;
        }

        /// <summary>
        /// 触发状态改变事件
        /// </summary>
        private void OnGridCheckBoxStateChanged(CheckState state)
        {
            if (!_isClickContent) return;
            _isClickContent = false;

			SparkDataGridView dgv = this.DataGridView as SparkDataGridView;
			SparkGridCheckBoxStateChangedEventArgs args = new SparkGridCheckBoxStateChangedEventArgs()
            {
                RowIndex = this.RowIndex,
                ColumnIndex = this.ColumnIndex,
                Owner = dgv,
                CheckState = state
            };
            dgv?.RaiseGridCheckBoxStateChanged(this, args);
        }

        #endregion

        #region 重写基类

        protected override void OnMouseEnter(int rowIndex)
        {
            if (this.DataGridView.CurrentCell?.RowIndex == rowIndex) _controlState = ControlState.Highlight;
            base.OnMouseEnter(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            _controlState = ControlState.Default;
            base.OnMouseLeave(rowIndex);
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (this.ContentBounds.Contains(e.Location)) _controlState = ControlState.Focused;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            _controlState = ControlState.Selected;
            base.OnMouseUp(e);
        }

        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            _isClickContent = true;
            base.OnContentClick(e);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            var grid = this.DataGridView;
            //if (grid.CurrentRow?.Index == rowIndex && _controlState != ControlState.Focused)
            //{
            //    _controlState = ControlState.Selected;
            //}
            //else if (_controlState != ControlState.Focused) _controlState = ControlState.Default;

            CheckState checkState = GetCheckState(formattedValue);
            Draw(graphics, clipBounds, cellBounds, checkState, cellStyle, rowIndex, paintParts, advancedBorderStyle);
        }
        #endregion
    }
}