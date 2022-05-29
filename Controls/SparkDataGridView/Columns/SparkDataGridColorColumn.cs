using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    public class SparkDataGridColorColumn : DataGridViewColumn
    {
        public SparkDataGridColorColumn()
        {
            this.CellTemplate = new SparkDataGridColorCell();
        }

        public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set
			{
				if (value != null && !value.GetType().IsAssignableFrom(typeof(SparkDataGridColorCell)))
				{
					throw new InvalidCastException("必须是一个颜色选择器单元格。");
				}
				base.CellTemplate = value;
			}
		}
	}
}