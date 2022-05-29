using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using SparkControls.Foundation;

namespace SparkControls.Controls.Style
{

    /// <summary>
    /// 设置grid的样式
    /// </summary>
    internal partial class FrmDataGridTemplateNew : SparkControls.Forms.SparkFormBase
    {
        #region 字段
        SparkDataGridView _grid = null;
        List<DataGridViewColumnStyle> listStyle = null;
        #endregion

        public FrmDataGridTemplateNew(SparkDataGridView sourceDataGridView)
            : base()
        {
            InitializeComponent();
            if (sourceDataGridView == null)
                return;
            _grid = sourceDataGridView;
            bdgv1.AutoGenerateColumns = false;
            var names = Enum.GetNames(typeof(DataGridViewContentAlignment)).ToArray();//.Select(a => Enum.Parse(typeof(DataGridViewContentAlignment), a)).OrderBy(a => (int)a).ToArray();

            Column4.Items.AddRange(names);

            Column2.ReadOnly = true;

            listStyle = new List<DataGridViewColumnStyle>();
            if (_grid.ListStyle != null && _grid.ListStyle.Any())
            {
                _grid.ListStyle.ForEach(i =>
                {
                    listStyle.Add(i.Clone() as DataGridViewColumnStyle);
                    listStyle.Last().IsModify = false;
                });

            }
            bdgv1.DataSource = listStyle;
            this.Text = "表格列设置 - " + _grid.TemplateFilePath;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.listStyle.Any(a => a.IsModify))
            {
                this.listStyle.ForEach(a => a.Sort = this.listStyle.IndexOf(a));
                //先取消列冻结，否则无法调整列的顺序
                _grid.ClearFrozen();
                foreach (var row in this.listStyle)
                {
                    _grid.SetColsStyle(row);
                    row.IsModify = false;
                    _grid.ListStyle.FirstOrDefault(a => a.DataPropertyName == row.DataPropertyName)?.SetStyle(row);//同步原来的列
                }
                var frozen = this.listStyle.Where(a => a.ColFrozen);
                string colName = "";
                if (frozen != null && frozen.Any())
                {
                    colName = frozen.FirstOrDefault(a => a.Sort == frozen.Max(b => b.Sort)).DataPropertyName;
                }
                _grid.SetFrozen(colName);
                _grid.SaveTemplateFile();
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FrmDataGridTemplateNew_Shown(object sender, EventArgs e)
        {
            if (_grid == null || (_grid.DataSource == null && _grid.ColumnCount == 0))
            {
                SparkMessageBox.ShowErrorMessage(this, "Grid没有绑定数据，无法设置表格列！");
                this.Close();
                this.DialogResult = DialogResult.Cancel;
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (bdgv1.CurrentCell == null)
            {
                SparkMessageBox.ShowInfoMessage(this, "请选中需要操作的行!");
                return;
            }

            int currentRowIndex = bdgv1.CurrentCell.RowIndex;
            if (currentRowIndex == 0)
            {
                return;
            }
            DataGridViewColumnStyle currentRow = (bdgv1.DataSource as List<DataGridViewColumnStyle>)[currentRowIndex].Clone() as DataGridViewColumnStyle;
            currentRow.IsModify = true;
            (bdgv1.DataSource as List<DataGridViewColumnStyle>).RemoveAt(currentRowIndex);
            (bdgv1.DataSource as List<DataGridViewColumnStyle>).Insert(currentRowIndex - 1, currentRow);
            bdgv1.CurrentCell = bdgv1.Rows[currentRowIndex - 1].Cells[0];
            bdgv1.Rows[currentRowIndex - 1].Selected = true;
            bdgv1.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bdgv1.CurrentCell == null)
            {
                SparkMessageBox.ShowInfoMessage(this, "请选中需要操作的行!");
                return;
            }

            int currentRowIndex = bdgv1.CurrentCell.RowIndex;
            if (currentRowIndex == (bdgv1.RowCount - 1))
            {
                return;
            }
            DataGridViewColumnStyle currentRow = (bdgv1.DataSource as List<DataGridViewColumnStyle>)[currentRowIndex].Clone() as DataGridViewColumnStyle;
            currentRow.IsModify = true;
            (bdgv1.DataSource as List<DataGridViewColumnStyle>).RemoveAt(currentRowIndex);
            (bdgv1.DataSource as List<DataGridViewColumnStyle>).Insert(currentRowIndex + 1, currentRow);
            bdgv1.CurrentCell = bdgv1.Rows[currentRowIndex + 1].Cells[0];
            bdgv1.Rows[currentRowIndex + 1].Selected = true;
            bdgv1.Refresh();
        }
    }

}
