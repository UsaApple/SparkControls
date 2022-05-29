using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	public class SparkDataGridButtonColumn : DataGridViewButtonColumn
    {
        public SparkDataGridButtonColumn()
        {
            this.CellTemplate = new SparkDataGridButtonCell();
        }

        public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set
			{
				if (value != null && !value.GetType().IsAssignableFrom(typeof(SparkDataGridButtonCell)))
				{
					throw new InvalidCastException("必须是一个按钮单元格。");
				}
				base.CellTemplate = value;
			}
		}
	}
}