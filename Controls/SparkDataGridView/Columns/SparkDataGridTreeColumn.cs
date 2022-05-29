using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
#pragma warning disable
	public class SparkDataGridTreeColumn : DataGridViewTextBoxColumn
    {
        internal Image _defaultNodeImage;

        public SparkDataGridTreeColumn()
        {
            this.CellTemplate = new SparkDataGridTreeCell();
        }

        public override object Clone()
        {
            SparkDataGridTreeColumn c = (SparkDataGridTreeColumn)base.Clone();
            c._defaultNodeImage = this._defaultNodeImage;
            return c;
        }

        public Image DefaultNodeImage
        {
            get { return _defaultNodeImage; }
            set { _defaultNodeImage = value; }
        }
    }
}