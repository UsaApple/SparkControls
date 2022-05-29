using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    internal sealed class SparkDataGridColumnCollection : DataGridViewColumnCollection
    {
        private readonly SparkDataGridView _owner = null;

        public SparkDataGridColumnCollection(SparkDataGridView dataGridView) : base(dataGridView)
        {
            _owner = dataGridView;
        }

        public override int Add(DataGridViewColumn column)
        {
            var flag = ApplyStyleByValueType(column);
            if (!flag)
            {
                if (column is DataGridViewButtonColumn) column.CellTemplate = new SparkDataGridButtonCell();
                else if (column is DataGridViewCheckBoxColumn) column.CellTemplate = new SparkDataGridCheckBoxCell();
                else if (column is DataGridViewComboBoxColumn) column.CellTemplate = new SparkDataGridComboBoxCell();
                else if (column is DataGridViewLinkColumn) column.CellTemplate = new SparkDataGridHyperlinkCell();
                else if (column is DataGridViewImageColumn) column.CellTemplate = new SparkDataGridImageCell();
                else if (column is SparkDataGridTreeColumn) column.CellTemplate = new SparkDataGridTreeCell();
                else
                {//这里不能简单错报的来解决问题，如果外部自定义列和单元格，用默认的SparkDataGridTextBoxCell来创建会报错的。
                    if (column.CellTemplate == null || column is DataGridViewTextBoxColumn) column.CellTemplate = new SparkDataGridTextBoxCell();
                }
            }
            return base.Add(column);
        }

        private bool ApplyStyleByValueType(DataGridViewColumn column)
        {
            if (_owner.DisableValueTypeMappingColumnStyle) return false;
            if (column.ValueType == typeof(DateTime) || column.CellTemplate.ValueType == typeof(DateTime))
            {
                if (column is SparkDataGridCalendarColumn)
                {
                    column.CellTemplate = new SparkDataGridCalendarCell();
                }
                else
                {
                    column.CellTemplate = new SparkDataGridDateCell();
                    column.CellTemplate.Style.Format = GetDateTimeFormat();
                }
                return true;
            }
            return false;
        }

        private string GetDateTimeFormat()
        {
            if (string.IsNullOrEmpty(_owner.DateTimeFormat)) return "yyyy-MM-dd HH:mm:ss";
            return _owner.DateTimeFormat;
        }
    }
}