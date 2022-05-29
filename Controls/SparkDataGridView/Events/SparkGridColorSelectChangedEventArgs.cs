using System.Drawing;

namespace SparkControls.Controls
{
	public class SparkGridColorSelectChangedEventArgs : SparkGridCellBaseEventArgs
    {
        public Color Value { set; get; }

        public SparkGridColorSelectChangedEventArgs() : base() { }

        public SparkGridColorSelectChangedEventArgs(int rowIndex, int columnIndex, SparkDataGridView owner, Color color)
            : base(rowIndex, columnIndex, owner)
        {
            this.Value = color;
        }
    }
}