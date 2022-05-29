namespace SparkControls.Controls
{
	public class SparkGridHyperlinkClickEventArgs : SparkGridCellBaseEventArgs
    {
        public string Value { set; get; }

        public bool LinkVisited { get; set; }

        public SparkGridHyperlinkClickEventArgs() : base() { }

        public SparkGridHyperlinkClickEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, string value)
            : base(rowIndex, columnIndex, owner)
        {
            this.Value = value;
        }
    }
}