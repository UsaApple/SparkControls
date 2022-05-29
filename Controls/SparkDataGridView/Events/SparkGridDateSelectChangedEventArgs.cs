using System;

namespace SparkControls.Controls
{
	public class SparkGridDateSelectChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public DateTime Value { set; get; }

        public SparkGridDateSelectChangedEventArgs() : base() { }

        public SparkGridDateSelectChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, DateTime date)
            : base(rowIndex, columnIndex, owner)
        {
            this.Value = date;
        }
    }
}