namespace SparkControls.Controls
{
	public sealed class SparkGridTextBoxTextChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public string Text { set; get; }

        public SparkGridTextBoxTextChangedEventArgs() : base() { }

        public SparkGridTextBoxTextChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, string text)
            : base(rowIndex, columnIndex, owner)
        {
            Text = text;
        }
    }
}