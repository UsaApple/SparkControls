using System;

namespace SparkControls.Controls
{
	public abstract class SparkGridCellBaseEventArgs : EventArgs
    {
        public int RowIndex { set; get; }

        public int ColumnIndex { set; get; }

        public SparkDataGridView Owner { set; get; }

        protected SparkGridCellBaseEventArgs() { }

        protected SparkGridCellBaseEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
            this.Owner = owner;
        }
    }
}