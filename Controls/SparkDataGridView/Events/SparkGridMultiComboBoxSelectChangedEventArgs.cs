using System.Collections.Generic;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public class SparkGridMultiComboBoxSelectChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public IEnumerable<int> SelectedIndex { get; set; }

        public IEnumerable<string> SelectedText { get; set; }

        public ListBox.ObjectCollection Items { get; set; }

        public SparkGridMultiComboBoxSelectChangedEventArgs() : base() { }

        public SparkGridMultiComboBoxSelectChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner)
            : base(rowIndex, columnIndex, owner)
        {
            
        }
    }
}