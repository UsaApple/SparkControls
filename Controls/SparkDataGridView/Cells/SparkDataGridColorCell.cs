using System;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 显示在表格中的颜色选择器。
    /// </summary>
    public class SparkDataGridColorCell : DataGridViewCell
    {
        #region 字段

        /// <summary>
        /// 边框色
        /// </summary>
        private Color _borderColor = default(Color);

        /// <summary>
        /// 控件当前状态
        /// </summary>
        private ControlState _controlState = ControlState.Default;

        /// <summary>
        /// 内容大小
        /// </summary>
        private Rectangle _contentRect = default(Rectangle);

        #endregion

        #region 属性

        public override Type ValueType
        {
            get { return typeof(Color); }
        }

        public override Type EditType
        {
            get { return typeof(SparkDataGridColorEditingControl); }
        }

        #endregion

        #region 私有方法

        private void InitControlStyle(int rowIndex, DataGridViewCellStyle cellStyle)
        {
            var dgv = this.DataGridView as SparkDataGridView;
            switch (_controlState)
            {
                case ControlState.Focused:
                    _borderColor = dgv.Theme.MouseDownBorderColor;
                    break;
                case ControlState.Highlight:
                    _borderColor = dgv.Theme.MouseOverBorderColor;
                    break;
                case ControlState.Selected:
                    _borderColor = dgv.Theme.SelectedBorderColor;
                    break;
                default:
                    _borderColor = dgv.Theme.BorderColor;
                    break;
            }
        }

        /// <summary>
        /// 重绘按钮
        /// </summary>
        /// <param name="g">上下文</param>
        /// <param name="rect">按钮区域大小</param>
        private void Draw(Graphics g, Rectangle rect, object value, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            InitControlStyle(rowIndex, cellStyle);

            var bColor = this.DataGridView.CurrentRow?.Index == rowIndex ? cellStyle.SelectionBackColor : cellStyle.BackColor;
            GDIHelper.FillRectangle(g, rect, bColor);
            rect.X -= 1;
            rect.Y -= 1;
            GDIHelper.DrawRectangle(g, rect, this.DataGridView.GridColor);
            var width = rect.Width / 3;
            var height = rect.Height / 2;
            var r = new Rectangle(rect.X + (rect.Width - width) / 2, rect.Y + (rect.Height - height) / 2, width, height);
            _contentRect = r;
            GDIHelper.FillRectangle(g, r, (Color)value);
            GDIHelper.DrawRectangle(g, r, _borderColor);
        }

        /// <summary>
        /// 显示颜色选择对话框
        /// </summary>
        private void ShowColorDialog()
        {
            var color = (Color)this.FormattedValue;
            var dialog = new SparkColorDialog()
            {
                Color = color
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            this.Value = dialog.Color;
			SparkDataGridView dgv = this.DataGridView as SparkDataGridView;
			SparkGridColorSelectChangedEventArgs args = new SparkGridColorSelectChangedEventArgs()
            {
                RowIndex = this.RowIndex,
                ColumnIndex = this.ColumnIndex,
                Owner = dgv,
                Value = dialog.Color
            };
            dgv?.RaiseGridColorSelectChanged(this, args);
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

        protected override void OnClick(DataGridViewCellEventArgs e)
        {
            base.OnClick(e);
            var p = this.DataGridView.PointToClient(Control.MousePosition);
            if (_contentRect.Contains(p)) ShowColorDialog();
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            if (this.DataGridView.CurrentRow?.Index == rowIndex && _controlState != ControlState.Focused)
            {
                _controlState = ControlState.Selected;
            }
            else if (_controlState != ControlState.Focused) _controlState = ControlState.Default;

            Draw(graphics, cellBounds, formattedValue, cellStyle, rowIndex);
        }

        #endregion
    }
}