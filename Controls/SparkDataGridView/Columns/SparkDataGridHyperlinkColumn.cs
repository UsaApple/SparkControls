using System.Windows.Forms;

namespace SparkControls.Controls
{
	public class SparkDataGridHyperlinkColumn : DataGridViewLinkColumn
    {
        public SparkDataGridHyperlinkColumn()
        {
            CellTemplate = new SparkDataGridHyperlinkCell();
        }
    }
}