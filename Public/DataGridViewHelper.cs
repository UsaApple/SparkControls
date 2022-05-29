using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SparkControls.Controls;

namespace SparkControls.Public
{
    /// <summary>
    /// 过滤条件
    /// </summary>
    public class FilterCondition
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        /// <param name="editColName"></param>
        /// <param name="dropDownDs"></param>
        /// <param name="filterStr"></param>
        public FilterCondition(string editColName,
            DataView dropDownDs, string filterStr)
        {
            EditColName = editColName;
            DropDownDs = dropDownDs;
            FilterStr = filterStr;
        }

        /// <summary>
        /// 编辑列的名称
        /// </summary>
        public string EditColName { get; set; }

        /// <summary>
        /// 下拉数据源
        /// </summary>
        public DataView DropDownDs { get; internal set; }

        /// <summary>
        /// 过滤条件
        /// </summary>
        public string FilterStr { get; set; }

        /// <summary>
        /// 弹出框的宽度
        /// </summary>
        public int Width { get; set; } = 600;

        /// <summary>
        /// 弹出框的高度
        /// </summary>
        public int Height { get; set; } = 400;
    }

    /// <summary>
    /// 表格单元格编辑时辅助输入控件的帮助类
    /// </summary>
    public class DataGridViewHelper
    {
        #region 变量
        private SparkPopup _popFrm = null;
        /// <summary>
        /// 编辑的列关联的输入框控件
        /// </summary>
        private Control _editingControl;
        /// <summary>
        /// 当前编辑的表格
        /// </summary>
        private readonly SparkDataGridView _curEditGridView;
        /// <summary>
        /// 条件集合
        /// </summary>
        private readonly List<FilterCondition> _conditionList = new List<FilterCondition>();
        #endregion

        /// <summary>
        /// 下拉的表格
        /// </summary>
        public SparkDataGridView DropDownDgv { get; private set; }

        #region 事件
        /// <summary>
        /// 下拉行选中确认委托
        /// </summary>
        /// <param name="editCtrl"></param>
        /// <param name="selectedDr"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public delegate bool DropDownRowConfirmEventHandler(Control editCtrl,
            DataRow selectedDr, string colName);

        /// <summary>
        /// 下拉行选中确认
        /// </summary>
        public event DropDownRowConfirmEventHandler DropDownRowConfirmed = null;

        /// <summary>
        /// 智能排序委托委
        /// </summary>
        /// <param name="filterCond"></param>
        /// <param name="filterTxt"></param>
        /// <returns></returns>
        public delegate DataView IntelligentSortEventHandler(FilterCondition filterCond, string filterTxt);

        /// <summary>
        /// 智能排序
        /// </summary>
        public event IntelligentSortEventHandler IntelligentSorting = null;

        /// <summary>
        /// 添加行的委托
        /// </summary>
        /// <returns></returns>
        public delegate bool AddRowEventHandler();
        /// <summary>
        /// 添加行
        /// </summary>
        public event AddRowEventHandler AddRow = null;
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="curEditGridView"></param>
        public DataGridViewHelper(SparkDataGridView curEditGridView)
        {
            _curEditGridView = curEditGridView;
            InitFilter();
        }

        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataGridViewHelper AddFilter(FilterCondition condition)
        {
            var fc = FindFilterCondition(condition?.EditColName);
            if (fc != null) return this;
            _conditionList.Add(condition);
            return this;
        }

        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="editColName"></param>
        /// <param name="dropDownDs"></param>
        /// <param name="filterSt"></param>
        /// <returns></returns>
        public DataGridViewHelper AddFilter(string editColName,
            DataView dropDownDs, string filterSt)
        {
            //不运行重复添加
            var fc = FindFilterCondition(editColName);
            if (fc != null) return this;
            _conditionList.Add(new FilterCondition(editColName, dropDownDs, filterSt));
            return this;
        }

        /// <summary>
        /// 为条件添加数据源
        /// </summary>
        /// <param name="editColName"></param>
        /// <param name="dv"></param>
        /// <returns></returns>
        public bool AddDataView(string editColName, DataView dv)
        {
            var fc = FindFilterCondition(editColName);
            if (fc != null)
            {
                fc.DropDownDs = dv;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 构建过滤条件(内部初始化)
        /// </summary>
        private void InitFilter()
        {
            if (_curEditGridView == null)
            {
                throw new Exception("当前正在编辑的表格控件不能为空！");
            }

            //创建下拉中的表格控件
            DropDownDgv = new SparkDataGridView()
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                Visible = false,
                MultiSelect = false,
                ColumnHeadersHeight = 28
            };
            DropDownDgv.RowTemplate.Height = 28;
            DropDownDgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DropDownDgv.GotFocus += DropDownGridView_GotFocus;
            DropDownDgv.CellDoubleClick += DropDownGrid_CellDoubleClick;

            //注册当前编辑表格的控件
            _curEditGridView.KeyDown += EditDataView_KeyDown;
            _curEditGridView.CellBeginEdit += EditDataView_CellBeginEdit;
            _curEditGridView.CellEndEdit += EditDataView_CellEndEdit;
            _curEditGridView.EditingControlShowing += EditDataView_EditingControlShowing;
            _curEditGridView.Click += EditDataView_Click;
            _curEditGridView.LostFocus += EditGridView_LostFocus;
            _curEditGridView.ProcessCmdKeyAction += ProcessCmdKey;
        }

        #region 事件处理
        /// <summary>
        /// 下拉表格获取焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropDownGridView_GotFocus(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 过滤窗口单元格双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropDownGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && sender is SparkDataGridView grid &&
                grid.Tag is FilterCondition condition)
            {
                KeyDown(condition, Keys.Enter);
            }
        }

        /// <summary>
        /// 编辑的表格失去焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditGridView_LostFocus(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 显示用于编辑单元格的控件时发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDataView_EditingControlShowing(object sender,
            DataGridViewEditingControlShowingEventArgs e)
        {
            if (!(sender is SparkDataGridView editGridView)) return;
            if (editGridView.CurrentCell == null) return;

            string colName = editGridView.Columns[editGridView.CurrentCell.ColumnIndex].DataPropertyName;
            var condition = _conditionList.FirstOrDefault(p => p.EditColName == colName);
            if (condition == null) return;

            if (_editingControl != null)
            {
                _editingControl.TextChanged -= TxtBox_TextChanged;
                _editingControl.Tag = null;
                _editingControl = null;
            }
            _editingControl = e.Control;
            _editingControl.Tag = condition;
            _editingControl.TextChanged += TxtBox_TextChanged;

        }

        /// <summary>
        /// 单元格文本编辑改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is Control ctrl && ctrl.Tag is FilterCondition condition)
            {
                ShowFilter(ctrl, _curEditGridView.CurrentCell.RowIndex, _curEditGridView.CurrentCell.ColumnIndex);

                var filterTxt = $"{ctrl.Text}".Trim();
                string rowfilter = string.Format(condition.FilterStr, EscapeValue(filterTxt));
                //filter = string.Format(" CODE LIKE '%{0}%' OR NAME LIKE '%{0}%' OR PY_CODE LIKE '%{0}%' OR WB_CODE LIKE '%{0}%'", (sender as TextBox).Text);

                condition.DropDownDs.RowFilter = rowfilter;
                if (IntelligentSorting != null && !filterTxt.IsNullOrEmpty())
                {
                    //如果时只能排序,先筛选出数据,然后根据关键字智能排序
                    var dv = IntelligentSorting(condition, filterTxt);
                    //dv为null时 说明改列不需要排序
                    DropDownDgv.DataSource = dv ?? condition.DropDownDs;
                }
                else
                {
                    DropDownDgv.DataSource = condition.DropDownDs;
                }
                if (DropDownDgv.ColumnCount > 1)
                {
                    //没开放出来,按道理应该开放出来配置列宽
                    DropDownDgv.Columns[1].Width = 300;
                }
                if (DropDownDgv.RowCount > 0)
                {
                    DropDownDgv.Rows[0].Selected = true;
                    DropDownDgv.CurrentCell = DropDownDgv.Rows[0].Cells[0];
                }
            }
        }

        /// <summary>
        /// 单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDataView_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs mouse && sender is SparkDataGridView editGridView)
            {
                //点击到其他没有配置下拉的列，就隐藏下拉
                var obj = editGridView.HitTest(mouse.X, mouse.Y);
                if (obj == null || obj.RowIndex < 0 || obj.ColumnIndex < 0 ||
                    FindFilterCondition(editGridView.Columns[obj.ColumnIndex].DataPropertyName) == null)
                {
                    Hide();
                }
            }
        }

        /// <summary>
        /// 按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDataView_KeyDown(object sender, KeyEventArgs e)
        {
            var keyData = e.KeyCode;
            if (keyData == Keys.Down || keyData == Keys.Right ||
                keyData == Keys.Left || keyData == Keys.Up ||
                keyData == Keys.Enter || keyData == Keys.Escape)
            {
                bool isRet = KeyDown(DropDownDgv.Tag as FilterCondition, e.KeyCode);
                e.Handled = isRet;
            }
        }

        /// <summary>
        /// 单元格开始编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDataView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex >= 0 && sender is SparkDataGridView grid)
            {
                string colName = grid.Columns[e.ColumnIndex].DataPropertyName;
                if (FindFilterCondition(colName) == null)
                {
                    Hide();
                }
            }
        }

        /// <summary>
        /// 单元格结束编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDataView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_editingControl == null) return;
            _editingControl.TextChanged -= TxtBox_TextChanged;
            _editingControl = null;
        }
        #endregion

        /// <summary>
        /// 显示过滤窗口
        /// </summary>
        /// <param name="editControl">编辑的控件</param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        private void ShowFilter(Control editControl, int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || columnIndex < 0) return;
            string colName = _curEditGridView.Columns[columnIndex].DataPropertyName;
            var condition = FindFilterCondition(colName);

            if (condition != null && (_popFrm == null || _popFrm.IsDisposed))
            {
                //int dgvX = _curEditGridView.Location.X;
                //int dgvY = _curEditGridView.Location.Y;
                //int cellX = _curEditGridView.GetCellDisplayRectangle(columnIndex, rowIndex, false).X;
                //int cellY = _curEditGridView.GetCellDisplayRectangle(columnIndex, rowIndex, false).Y;
                //var point = new Point(dgvX + cellX - 1, dgvY + cellY + 26);
                var point1 = _curEditGridView.PointToScreen(new Point(0, 0));
                var point = editControl.PointToScreen(new Point(0, -editControl.Top)); //editControl.Height + rowHeight - editControl.Height - editControl.Top

                point.Offset(-point1.X - editControl.Left, -point1.Y);
                DropDownDgv.Tag = condition;
                DropDownDgv.Location = new Point();
                DropDownDgv.Width = condition.Width;
                DropDownDgv.Height = condition.Height;

                ShowFrom(condition, point);
            }
            DropDownDgv.Visible = true;
        }

        /// <summary>
        /// 显示弹出框
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="point"></param>
        private void ShowFrom(FilterCondition condition, Point point)
        {
            //计算输入框的高度
            int rowHeight = 0;
            if (_curEditGridView.CurrentRow != null)
            {
                rowHeight = _curEditGridView.CurrentRow.Height;
            }
            else
            {
                rowHeight = _curEditGridView.RowTemplate.Height;
            }

            var p = new Point(point.X, point.Y - _curEditGridView.Height + rowHeight);//SparkPopup内部会加上_curEditGridView.Height，所以外部先减去高度
            _popFrm = new SparkPopup(_curEditGridView, DropDownDgv, p)
            {
                Width = condition.Width,
                Height = condition.Height,
                RowHeight = rowHeight,
            };
            DropDownDgv.Dock = DockStyle.Fill;
            _popFrm.FormClosing += (sender, e) =>
            {
                _popFrm.Controls.Remove(DropDownDgv);
            };
            _popFrm.Controls.Add(DropDownDgv);
            _popFrm.Show();
        }

        /// <summary>
        /// 隐藏弹出框
        /// </summary>
        private void Hide()
        {
            if (DropDownDgv != null) DropDownDgv.Visible = false;
            if (_popFrm != null && !_popFrm.IsDisposed) _popFrm.Close();
        }

        /// <summary>
        /// 根据列名查询对应的过滤条件
        /// </summary>
        /// <param name="colName"></param>
        /// <returns></returns>
        private FilterCondition FindFilterCondition(string colName)
        {
            return _conditionList.FirstOrDefault(p => p.EditColName == colName);
        }

        /// <summary>
        /// 按键事件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        private bool ProcessCmdKey(Message msg, Keys keyData)
        {
            if (keyData == Keys.Down || keyData == Keys.Right ||
                keyData == Keys.Left || keyData == Keys.Up ||
                keyData == Keys.Enter || keyData == Keys.Escape)
            {
                return KeyDown(DropDownDgv.Tag as FilterCondition, keyData);
            }
            return false;
        }

        /// <summary>
        /// 转义检索文本中的字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string EscapeValue(string value)
        {
            if (value.IsNullOrEmpty()) return "";
            StringBuilder sb = new StringBuilder();
            value.ToList().ForEach(a =>
            {
                if (a == '*' || a == '%' || a == '[' || a == ']')
                {
                    sb.Append("[").Append(a).Append("]");
                }
                else if (a == '\'')
                {
                    sb.Append("''");
                }
                else
                {
                    sb.Append(a);
                }
            });
            return sb.ToString();
        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="KeyCode"></param>
        private bool KeyDown(FilterCondition condition, Keys KeyCode)
        {
            bool isRet = true;
            switch (KeyCode)
            {
                case Keys.Down:
                    isRet = SetRow(true);
                    break;
                case Keys.Up:
                    isRet = SetRow(false);
                    break;
                case Keys.Left:
                case Keys.Right:
                    isRet = false;
                    break;
                case Keys.Enter:
                    if (DropDownDgv.Visible && condition != null && DropDownRowConfirmed != null)
                    {
                        if (DropDownDgv.CurrentRow?.DataBoundItem is DataRowView drv)
                        {
                            if (DropDownRowConfirmed.Invoke(
                                _editingControl, drv.Row, condition.EditColName))
                            {
                                Hide();
                            }
                        }
                    }
                    else
                    {
                        //应该跳转到下一个可编辑的单元格中
                        ToNextEditCell();
                    }
                    break;
                case Keys.Escape:
                    Hide();
                    break;
            }
            return isRet;
        }

        /// <summary>
        /// 跳转到下一个可编辑的单元格,如果当前行没有,
        /// 通过DataGridViewFilter.AddRow添加一行,跳转到添加行的第一个可编辑单元格
        /// </summary>
        private void ToNextEditCell()
        {
            if (_curEditGridView != null && _curEditGridView.CurrentCell != null)
            {
                int row = _curEditGridView.CurrentCell.RowIndex;
                int col = _curEditGridView.CurrentCell.ColumnIndex;
                bool isEdit = false;

                while (!isEdit)
                {
                    col++;
                    if (_curEditGridView.ColumnCount <= col)
                    {
                        row++;
                        col = 0;
                    }

                    if (_curEditGridView.RowCount <= row)
                    {
                        //添加一行
                        if (AddRow != null)
                        {
                            if (!AddRow())
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (_curEditGridView.RowCount > row && _curEditGridView.ColumnCount > col)
                    {
                        var cell = _curEditGridView.Rows[row].Cells[col];
                        if (cell.Visible && cell.ReadOnly == false)
                        {
                            isEdit = true;
                            break;
                        }
                    }
                }

                if (isEdit)
                {
                    if (_curEditGridView.RowCount > row && _curEditGridView.ColumnCount > col)
                    {
                        _curEditGridView.CurrentCell = _curEditGridView.Rows[row].Cells[col];
                    }
                }
            }
        }

        /// <summary>
        /// 设置行的选中
        /// </summary>
        /// <param name="isDown"></param>
        private bool SetRow(bool isDown)
        {
            if (DropDownDgv == null || !DropDownDgv.Visible) return false;

            int row = -1;
            int count = DropDownDgv.RowCount;
            if (DropDownDgv.CurrentRow != null)
            {
                row = DropDownDgv.CurrentRow.Index;
            }
            int goRow;
            if (isDown)
            {
                if (row + 1 >= count)
                {
                    goRow = 0;
                }
                else
                {
                    goRow = row + 1;
                }
            }
            else
            {
                if (row - 1 < 0)
                {
                    goRow = count - 1;
                }
                else
                {
                    goRow = row - 1;
                }
            }
            if (goRow >= 0 && goRow < count)
            {
                DropDownDgv.Rows[goRow].Selected = true;
                DropDownDgv.CurrentCell = DropDownDgv.Rows[goRow].Cells[0];
            }
            return true;
        }
    }
}