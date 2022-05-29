using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    public class SparkDataGridCalendarCell : DataGridViewTextBoxCell
    {
        public SparkDataGridCalendarCell() : base()
        {
        }

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            SparkDataGridCalendarEditingControl ctl =
                DataGridView.EditingControl as SparkDataGridCalendarEditingControl;
            // Use the default row value when Value property is null.
            SparkDataGridCalendarColumn parent = (SparkDataGridCalendarColumn)OwningColumn;
            string format = parent.DefaultCellStyle.Format;
            ctl.CustomFormat = format.IsNullOrEmpty() ? "yyyy-MM-dd" : format;
            if (this.Value == null || this.Value == DBNull.Value)
            {
                ctl.Value = (DateTime)this.DefaultNewRowValue;
            }
            else
            {
                ctl.Value = (DateTime)this.Value;
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that CalendarCell uses.
                return typeof(SparkDataGridCalendarEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that CalendarCell contains.
                return typeof(DateTime);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use the current date and time as the default value.
                return DateTime.Now;
            }
        }
    }
}