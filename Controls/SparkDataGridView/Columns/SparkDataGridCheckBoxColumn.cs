using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public class SparkDataGridCheckBoxColumn : DataGridViewCheckBoxColumn
    {
        public SparkDataGridCheckBoxColumn()
        {
            base.CellTemplate = new SparkDataGridCheckBoxCell();
        }

        public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set
			{
				if (value != null && !value.GetType().IsAssignableFrom(typeof(SparkDataGridCheckBoxCell)))
				{
					throw new InvalidCastException("必须是一个复选框单元格。");
				}
				base.CellTemplate = value;
			}
		}
	}
}