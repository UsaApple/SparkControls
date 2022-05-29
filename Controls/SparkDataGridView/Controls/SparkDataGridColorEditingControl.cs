using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	[ToolboxItem(false)]
    public class SparkDataGridColorEditingControl : TextBox, IDataGridViewEditingControl
    {
        public SparkDataGridColorEditingControl()
        {

        }

        #region IDataGridViewEditingControl 成员

        public DataGridView EditingControlDataGridView
        {
            get;
            set;
        }

        public object EditingControlFormattedValue
		{
			get => this.Text;
			set => this.Text = Convert.ToString(value);
		}

		public int EditingControlRowIndex
        {
            get;
            set;
        }

        public bool EditingControlValueChanged
        {
            get;
            set;
        }

		public Cursor EditingPanelCursor => base.Cursor;

		public bool RepositionEditingControlOnValueChange => false;

		public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {

        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
           
        }

        #endregion
        
        protected override void OnTextChanged(EventArgs e)
        {
            EditingControlValueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnTextChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
    }
}