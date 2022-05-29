using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    public class SparkDataGridCalendarColumn : DataGridViewColumn
    {
        public SparkDataGridCalendarColumn() : base(new SparkDataGridCalendarCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CalendarCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(SparkDataGridCalendarCell)))
                {
                    throw new InvalidCastException("Must be a SparkDataGridCalendarCell");
                }
                base.CellTemplate = value;
            }
        }

        private SparkDataGridCalendarCell CalendarCellTemplate
        {
            get
            {
                return (SparkDataGridCalendarCell)this.CellTemplate;
            }
        }
    }
}