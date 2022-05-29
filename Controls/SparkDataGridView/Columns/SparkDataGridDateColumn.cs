using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    public class SparkDataGridDateColumn : DataGridViewTextBoxColumn
    {
        public SparkDataGridDateColumn()
        {
            base.CellTemplate = new SparkDataGridDateCell();
        }

        public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set
			{
				if (value != null && !value.GetType().IsAssignableFrom(typeof(SparkDataGridDateCell)))
				{
					throw new InvalidCastException("必须是一个日期框单元格。");
				}
				base.CellTemplate = value;
			}
		}
	}
}