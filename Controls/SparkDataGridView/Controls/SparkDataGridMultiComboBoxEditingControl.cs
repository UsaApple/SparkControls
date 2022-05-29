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
	public class SparkDataGridMultiComboBoxEditingControl : SparkMultiselectComboBox, IDataGridViewEditingControl
	{
		private DataGridView dataGridView;
		private bool valueChanged;
		private int rowIndex;

		public SparkDataGridMultiComboBoxEditingControl() : base()
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
						this.SelectedIndex = null;
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

		public virtual bool RepositionEditingControlOnValueChange => false;

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

		public virtual void PrepareEditingControlForEdit(bool selectAll)
		{
			this.Items.Clear();
			SparkDataGridView dgv = this.dataGridView as SparkDataGridView;
			if (dgv.CurrentCell != null)
			{
				SparkDataGridMultiComboBoxColumn col = dgv.Columns[dgv.CurrentCell.ColumnIndex] as SparkDataGridMultiComboBoxColumn;
				if (col.Items != null) this.Items.AddRange(col.Items.Cast<object>().ToArray());
				this.SelectedItem = (("" + dgv.CurrentCell.Value) ?? "").Split(',');
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
			SparkGridMultiComboBoxSelectChangedEventArgs args = new SparkGridMultiComboBoxSelectChangedEventArgs()
			{
				RowIndex = EditingControlRowIndex,
				ColumnIndex = dgv.CurrentCell.ColumnIndex,
				Owner = dgv,
				SelectedIndex = this.SelectedIndex,
				SelectedText = this.SelectedText,
				Items = this.Items
			};
			dgv.RaiseGridMultiComboBoxSelectChanged(dgv.CurrentCell as SparkDataGridTextBoxCell, args);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			this.valueChanged = true;
			this.dataGridView.NotifyCurrentCellDirty(true);
		}

		protected override void OnItemCheck(ItemCheckEventArgs e)
		{
			base.OnItemCheck(e);
			if (this.SelectedIndex != null && this.SelectedIndex.Count() >= 0)
			{
				this.NotifyDataGridViewOfValueChange();
			}
		}
	}
}