using System.Windows.Forms;

namespace SparkControls.Controls
{
	public sealed class SparkGridComboBoxSelectChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public object SelectedItem { get; set; }

        public int SelectedIndex { get; set; }

        public object SelectedValue { get; set; }

        public string SelectedText { get; set; }

        public string DisplayMember { get; set; }

        public object DataSource { get; set; }

        public string ValueMember { get; set; }

        public ListBox.ObjectCollection Items { get; set; }

        public SparkGridComboBoxSelectChangedEventArgs() : base() { }

        public SparkGridComboBoxSelectChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, string text)
            : base(rowIndex, columnIndex, owner)
        {
            SelectedText = text;
        }
    }
}