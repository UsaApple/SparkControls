using System.Windows.Forms;

namespace SparkControls.Controls
{
	public class SparkDataGridImageColumn : DataGridViewImageColumn
    {
        public SparkDataGridImageColumn()
        {
            CellTemplate = new SparkDataGridImageCell();
        }
    }
}