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
	public class SparkDataGridComboBoxEditingControl : SparkComboBox, IDataGridViewEditingControl
	{
		private DataGridView dataGridView;
		private bool valueChanged;
		private int rowIndex;

		public SparkDataGridComboBoxEditingControl() : base()
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
						this.SelectedIndex = -1;
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
			//在编辑模式下，上下键可以切换单元格
			if (this.IsDroppedDown && ((keyData & Keys.KeyCode) == Keys.Escape) || (keyData & Keys.KeyCode) == Keys.Enter)
			{
				return true;
			}
			return !dataGridViewWantsInputKey;
		}

		public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			if (this.Text == this.SelectedItem?.ToString())
				return this.SelectedObject ?? this.SelectedItem;
			return this.Text;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			this.valueChanged = true;
			this.dataGridView.NotifyCurrentCellDirty(true);
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
			this.DropDownWidth = this.Width;
			this.Items.Clear();
			SparkDataGridView dgv = this.dataGridView as SparkDataGridView;
			if (dgv.CurrentCell != null)
			{
				int index = dgv.CurrentCell.ColumnIndex;
				DataGridViewColumn column = dgv.CurrentCell.OwningColumn;
				ComboBoxStyle dds = dgv.QueryComboBoxStyle(index, column.Name, column.HeaderText);
				if (dds != this.DropDownStyle) this.DropDownStyle = dds;
				DataGridViewComboBoxColumn col = dgv.Columns[index] as DataGridViewComboBoxColumn;
				if (col.Items != null) this.Items.AddRange(col.Items.Cast<object>().ToArray());
				if (this.Items.Count == 0) this.SelectedIndex = -1;
				this.Text = "" + dgv.CurrentCell.Value;
				this.IsDroppedDown = true;
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
			SparkGridComboBoxSelectChangedEventArgs args = new SparkGridComboBoxSelectChangedEventArgs()
			{
				RowIndex = EditingControlRowIndex,
				ColumnIndex = dgv.CurrentCell.ColumnIndex,
				Owner = dgv,
				SelectedIndex = this.SelectedIndex,
				SelectedItem = this.SelectedItem,
				SelectedText = this.SelectedText,
				SelectedValue = this.SelectedValue,
				ValueMember = this.ValueMember,
				DataSource = this.DataSource,
				DisplayMember = this.DisplayMember,
				Items = this.Items
			};
			dgv.RaiseGridComboBoxSelectChanged(dgv.CurrentCell as SparkDataGridComboBoxCell, args);
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			if (this.SelectedIndex != -1)
			{
				this.NotifyDataGridViewOfValueChange();
			}
		}
	}
}