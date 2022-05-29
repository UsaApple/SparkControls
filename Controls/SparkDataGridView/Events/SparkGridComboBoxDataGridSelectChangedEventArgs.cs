namespace SparkControls.Controls
{
	public sealed class SparkGridComboBoxDataGridSelectChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public object SelectedItem { get; set; }

        public int SelectedIndex { get; set; }

        public object SelectedValue { get; set; }

        public string SelectedText { get; set; }

        public object DataSource { get; set; }

        public string DisplayMember { set; get; }

        public string ValueMember { get; set; }

        public SparkGridComboBoxDataGridSelectChangedEventArgs() : base() { }

        public SparkGridComboBoxDataGridSelectChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, string text)
            : base(rowIndex, columnIndex, owner)
        {
            SelectedText = text;
        }
    }
}