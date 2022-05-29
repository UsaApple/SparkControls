using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	[ToolboxItem(false)]
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
	public class SparkDataGridMultiComboBoxDataGridEditingControl : SparkMultiselectDataGridComboBox, IDataGridViewEditingControl
	{
		private DataGridView dataGridView;
		private bool valueChanged;
		private int rowIndex;

		public SparkDataGridMultiComboBoxDataGridEditingControl() : base()
		{
			this.TabStop = false;
		}

		public virtual DataGridView EditingControlDataGridView
		{
			get => this.dataGridView;
			set => this.dataGridView = value;
		}

		public virtual object EditingControlFormattedValue
		{
			get => this.GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
			set
			{
				if (value is string valueStr)
				{
					this.Text = valueStr;
					if (string.Compare(valueStr, this.Text, true, CultureInfo.CurrentCulture) != 0)
					{
						this.Text = valueStr;
					}
				}
			}
		}

		public virtual int EditingControlRowIndex
		{
			get => this.rowIndex;
			set => this.rowIndex = value;
		}

		public virtual bool EditingControlValueChanged
		{
			get => this.valueChanged;
			set => this.valueChanged = value;
		}

		public virtual Cursor EditingPanelCursor => Cursors.Default;

		public virtual bool RepositionEditingControlOnValueChange => true;

		public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			this.Font = dataGridViewCellStyle.Font;
			if (dataGridViewCellStyle.BackColor.A < 255)
			{
				Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
				this.dataGridView.EditingPanel.BackColor = opaqueBackColor;
			}
		}

		public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
		{
			if ((keyData & Keys.KeyCode) == Keys.Down ||
				(keyData & Keys.KeyCode) == Keys.Up ||
				this.IsDroppedDown && ((keyData & Keys.KeyCode) == Keys.Escape) || (keyData & Keys.KeyCode) == Keys.Enter)
			{
				return true;
			}
			return !dataGridViewWantsInputKey;
		}

		public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return this.Text;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
		}

		protected override void OnDropDown(EventArgs e)
		{
			base.OnDropDown(e);
		}

		public virtual void PrepareEditingControlForEdit(bool selectAll)
		{
			this.Items.Clear();
			SparkDataGridView dgv = this.dataGridView as SparkDataGridView;
			if (dgv.CurrentCell != null)
			{
				int index = dgv.CurrentCell.ColumnIndex;
				DataGridViewColumn column = dgv.CurrentCell.OwningColumn;
				SparkDataGridMultiComboBoxDataGridColumn col = dgv.Columns[index] as SparkDataGridMultiComboBoxDataGridColumn;
				if (col.DataSource != null) this.DataSource = col.DataSource;
				this.Text = "" + dgv.CurrentCell.Value;
			}
			if (selectAll)
			{
				this.SelectAll();
			}
		}

		private void NotifyDataGridViewOfValueChange()
		{
			this.valueChanged = true;
			this.dataGridView.NotifyCurrentCellDirty(true);
			SparkDataGridView dgv = this.dataGridView as SparkDataGridView;
			SparkGridMultiComboBoxDataGridSelectChangedEventArgs args = new SparkGridMultiComboBoxDataGridSelectChangedEventArgs()
			{
				RowIndex = EditingControlRowIndex,
				ColumnIndex = dgv.CurrentCell.ColumnIndex,
				Owner = dgv,
				SelectedIndex = this.SelectedIndex,
				SelectedItem = this.SelectedItem,
				SelectedText = this.SelectedText,
				SelectedValue = this.SelectedValue,
				DisplayMember = this.DisplayMember,
				ValueMember = this.ValueMember,
				DataSource = this.DataSource,
			};
			dgv.RaiseGridMultiComboBoxDataGridSelectChanged(dgv.CurrentCell as SparkDataGridMultiComboBoxDataGridCell, args);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			this.valueChanged = true;
			this.dataGridView.NotifyCurrentCellDirty(true);
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			if (this.SelectedIndex != null && this.SelectedIndex.Count() >= 0)
			{
				this.NotifyDataGridViewOfValueChange();
			}
		}
	}
}