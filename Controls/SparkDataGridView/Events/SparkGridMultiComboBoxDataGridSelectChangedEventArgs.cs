using System.Collections.Generic;

namespace SparkControls.Controls
{
	public sealed class SparkGridMultiComboBoxDataGridSelectChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public IEnumerable<object> SelectedItem { get; set; }

        public IEnumerable<int> SelectedIndex { get; set; }

        public IEnumerable<object> SelectedValue { get; set; }

        public IEnumerable<string> SelectedText { get; set; }

        public string DisplayMember { get; set; }

        public object DataSource { get; set; }

        public string ValueMember { get; set; }

        public SparkGridMultiComboBoxDataGridSelectChangedEventArgs() : base() { }

        public SparkGridMultiComboBoxDataGridSelectChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, IEnumerable<string> text)
            : base(rowIndex, columnIndex, owner)
        {
            SelectedText = text;
        }
    }
}