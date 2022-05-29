using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	[ToolboxItem(false)]
	[ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
	public class SparkDataGridTextBoxEditingControl : SparkTextBox, IDataGridViewEditingControl
	{
		private static readonly DataGridViewContentAlignment anyTop = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.TopRight;
		private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
		private static readonly DataGridViewContentAlignment anyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;

		private DataGridView dataGridView;
		private bool valueChanged;
		private bool repositionOnValueChange;
		private int rowIndex;

		public SparkDataGridTextBoxEditingControl() : base()
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
			set => this.Text = (string)value;
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

		public virtual bool RepositionEditingControlOnValueChange => this.repositionOnValueChange;

		public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			this.Font = dataGridViewCellStyle.Font;
			if (dataGridViewCellStyle.BackColor.A < 255)
			{
				Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
				this.BackColor = opaqueBackColor;
				this.dataGridView.EditingPanel.BackColor = opaqueBackColor;
			}
			else
			{
				this.BackColor = dataGridViewCellStyle.BackColor;
			}
			this.ForeColor = dataGridViewCellStyle.ForeColor;
			if (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True)
			{
				this.WordWrap = true;
			}
			this.TextAlign = TranslateAlignment(dataGridViewCellStyle.Alignment);
			this.repositionOnValueChange = (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True && (dataGridViewCellStyle.Alignment & anyTop) == 0);
		}

		public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
		{
			switch (keyData & Keys.KeyCode)
			{
				case Keys.Right:
					if ((this.RightToLeft == RightToLeft.No && !(this.SelectionLength == 0 && this.SelectionStart == this.Text.Length)) ||
						(this.RightToLeft == RightToLeft.Yes && !(this.SelectionLength == 0 && this.SelectionStart == 0)))
					{
						return true;
					}
					break;
				case Keys.Left:
					if ((this.RightToLeft == RightToLeft.No && !(this.SelectionLength == 0 && this.SelectionStart == 0)) ||
						(this.RightToLeft == RightToLeft.Yes && !(this.SelectionLength == 0 && this.SelectionStart == this.Text.Length)))
					{
						return true;
					}
					break;
				case Keys.Down:
					int end = this.SelectionStart + this.SelectionLength;
					if (this.Text.IndexOf("\r\n", end) != -1)
					{
						return true;
					}
					break;
				case Keys.Up:
					if (!(this.Text.IndexOf("\r\n") < 0 || this.SelectionStart + this.SelectionLength < this.Text.IndexOf("\r\n")))
					{
						return true;
					}
					break;
				case Keys.Home:
				case Keys.End:
					if (this.SelectionLength != this.Text.Length)
					{
						return true;
					}
					break;

				case Keys.Prior:
				case Keys.Next:
					if (this.valueChanged)
					{
						return true;
					}
					break;

				case Keys.Delete:
					if (this.SelectionLength > 0 ||
						this.SelectionStart < this.Text.Length)
					{
						return true;
					}
					break;

				case Keys.Enter:
					if ((keyData & (Keys.Control | Keys.Shift | Keys.Alt)) == Keys.Shift && this.Multiline && this.AcceptsReturn)
					{
						return true;
					}
					break;
			}
			return !dataGridViewWantsInputKey;
		}

		public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return this.Text;
		}

		public virtual void PrepareEditingControlForEdit(bool selectAll)
		{
			if (this.dataGridView?.CurrentCell is SparkDataGridTextBoxCell cell) this.TextType = this.GetTextTypeByType(cell.ValueType);
			this.UseNumberKeyboard = (this.dataGridView as SparkDataGridView)?.ShowNumberKeyBoard == true &&
									  this.TextType != TextType.String;
			if (selectAll) this.SelectAll();
			else this.SelectionStart = this.Text.Length;
			this.BorderStyle = BorderStyle.FixedSingle;
			this.Margin = new Padding();
			this.Location = new Point(2, 2);
		}

		private void NotifyDataGridViewOfValueChange()
		{
			this.valueChanged = true;
			this.dataGridView.NotifyCurrentCellDirty(true);
			SparkDataGridView dgv = this.dataGridView as SparkDataGridView;
			SparkGridTextBoxTextChangedEventArgs args = new SparkGridTextBoxTextChangedEventArgs()
			{
				RowIndex = EditingControlRowIndex,
				ColumnIndex = dgv.CurrentCell.ColumnIndex,
				Owner = dgv,
				Text = this.Text
			};
			dgv.RaiseGridTextBoxTextChanged(dgv.CurrentCell as SparkDataGridTextBoxCell, args);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			this.NotifyDataGridViewOfValueChange();
		}

		protected override bool ProcessKeyEventArgs(ref Message m)
		{
			switch ((Keys)(int)m.WParam)
			{
				case Keys.Enter:
					if (m.Msg == 0x0102 &&
						!(ModifierKeys == Keys.Shift && this.Multiline && this.AcceptsReturn))
					{
						// Ignore the Enter key and don't add it to the textbox content. This happens when failing validation brings
						// up a dialog box for example.
						// Shift-Enter for multiline textboxes need to be accepted however.
						return true;
					}
					break;

				case Keys.LineFeed:
					if (m.Msg == 0x0102 &&
						ModifierKeys == Keys.Control && this.Multiline && this.AcceptsReturn)
					{
						// Ignore linefeed character when user hits Ctrl-Enter to commit the cell.
						return true;
					}
					break;

				case Keys.A:
					if (m.Msg == 0x0100 && ModifierKeys == Keys.Control)
					{
						this.SelectAll();
						return true;
					}
					break;

			}
			return base.ProcessKeyEventArgs(ref m);
		}

		private static HorizontalAlignment TranslateAlignment(DataGridViewContentAlignment align)
		{
			if ((align & anyRight) != 0)
			{
				return HorizontalAlignment.Right;
			}
			else if ((align & anyCenter) != 0)
			{
				return HorizontalAlignment.Center;
			}
			else
			{
				return HorizontalAlignment.Left;
			}
		}

		private TextType GetTextTypeByType(Type type)
		{
			if (type == typeof(short) ||
				type == typeof(ushort) ||
				type == typeof(int) ||
				type == typeof(short) ||
				type == typeof(int) ||
				type == typeof(long) ||
				type == typeof(uint) ||
				type == typeof(ushort) ||
				type == typeof(uint) ||
				type == typeof(ulong) ||
				type == typeof(long) ||
				type == typeof(ulong)) return TextType.Int;
			else if (type == typeof(float) ||
				type == typeof(double) ||
				type == typeof(decimal)) return TextType.Decimal;
			return TextType.String;
		}
	}
}