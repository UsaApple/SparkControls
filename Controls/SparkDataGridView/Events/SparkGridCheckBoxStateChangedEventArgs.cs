using System.Windows.Forms;

namespace SparkControls.Controls
{
	public sealed class SparkGridCheckBoxStateChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public CheckState CheckState { set; get; }

        public SparkGridCheckBoxStateChangedEventArgs() : base() { }

        public SparkGridCheckBoxStateChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, CheckState state)
            : base(rowIndex, columnIndex, owner)
        {
            CheckState = state;
        }
    }
}