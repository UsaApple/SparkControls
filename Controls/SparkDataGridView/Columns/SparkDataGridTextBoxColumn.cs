using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    public class SparkDataGridTextBoxColumn : DataGridViewTextBoxColumn
    {
        public SparkDataGridTextBoxColumn()
        {
            base.CellTemplate = new SparkDataGridTextBoxCell();
        }

        public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set
			{
				if (value != null && !typeof(SparkDataGridTextBoxCell).IsAssignableFrom(value.GetType()))
				{
					throw new InvalidCastException("必须是一个文本框单元格。");
				}
				base.CellTemplate = value;
			}
		}
	}
}