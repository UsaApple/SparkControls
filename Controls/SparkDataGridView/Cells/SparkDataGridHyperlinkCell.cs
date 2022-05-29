using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 显示在表格中的超链接标签。
	/// </summary>
	public class SparkDataGridHyperlinkCell : DataGridViewLinkCell
    {
        protected override void OnContentClick(DataGridViewCellEventArgs e)
        {
            base.OnContentClick(e);
			SparkDataGridView dgv = this.DataGridView as SparkDataGridView;
			SparkGridHyperlinkClickEventArgs args = new SparkGridHyperlinkClickEventArgs()
            {
                RowIndex = this.RowIndex,
                ColumnIndex = this.ColumnIndex,
                Owner = dgv,
                Value = Convert.ToString(this.Value),
                LinkVisited = this.LinkVisited
            };
            dgv?.RaiseGridHyperlinkClick(this, args);
        }
    }
}