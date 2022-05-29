using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    public class SparkDataGridComboBoxColumn : DataGridViewComboBoxColumn
    {
        public SparkDataGridComboBoxColumn()
        {
            base.CellTemplate = new SparkDataGridComboBoxCell();
        }
        
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(SparkDataGridComboBoxCell)))
                {
                    throw new InvalidCastException("必须是一个下组合框单元格。");
                }
                base.CellTemplate = value;
            }
        }
    }
}