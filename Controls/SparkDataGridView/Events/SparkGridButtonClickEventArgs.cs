namespace SparkControls.Controls
{
	public sealed class SparkGridButtonClickEventArgs : SparkGridCellBaseEventArgs
    {
        public SparkGridButtonClickEventArgs() : base() { }

        public SparkGridButtonClickEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner)
            : base(rowIndex, columnIndex, owner)
        {
        }
    }
}