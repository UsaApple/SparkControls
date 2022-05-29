using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表格组合框下拉列表控件。
    /// </summary>
    [ToolboxItem(false)]
    internal partial class SparkDataGridComboBoxListControl : UserControl
    {
        private readonly SparkDataGridComboBoxBase mOwner;
        private readonly List<object> mSelectedItems = new List<object>();
        private readonly Dictionary<object, DataRowView> mItemRows = new Dictionary<object, DataRowView>();

        private object mDataSource = null;
        private bool mIsBaseObject = false;
        private bool mIsString = false;

        /// <summary>
        /// 选择项发生改变时发生。
        /// </summary>
        internal event SparkDataGridComboBoxSelectedChangedEventHandler SelectedChanged;

        /// <summary>
        /// 选择项发生改变前时发生
        /// </summary>
        internal event SparkDataGridComboBoxSelectingChangedEventHandler SelecingChanged;

        /// <summary>
        /// 获取或设置一个值，该值指示是否可以选择多行。
        /// </summary>
        internal bool MultiSelect
        {
            get => this.mDgvList.MultiSelect;
            set
            {
                this.mDgvList.MultiSelect = value;
                if (value)
                {
                    if (!this.mDgvList.Columns.Contains("dgvcChecked"))
                    {
                        SparkDataGridCheckBoxColumn dgvcChecked = new SparkDataGridCheckBoxColumn
                        {
                            Frozen = true,
                            HeaderText = "选择",
                            Name = "dgvcChecked",
                            ReadOnly = false,
                            Width = 38
                        };
                        this.mDgvList.Columns.Insert(0, dgvcChecked);
                    }
                }
                else if (this.mDgvList.Columns.Contains("dgvcChecked"))
                {
                    this.mDgvList.Columns.Remove("dgvcChecked");
                }
            }
        }

        /// <summary>
        /// 设置或获取Guid,用于保表格存样式文件的名称（需要唯一值）
        /// </summary>
        [DefaultValue("")]
        internal string Guid
        {
            get => this.mDgvList.Guid;
            set
            {
                this.mDgvList.Guid = value;
            }
        }

        /// <summary>
        /// 获取选项列表。
        /// </summary>
        internal IEnumerable<object> Items => this.mDgvList.Rows.Cast<DataGridViewRow>().Select(x => x.DataBoundItem);

        /// <summary>
        /// 初始 <see cref="SparkDataGridComboBoxListControl" /> 类型的新实例。
        /// </summary>
        /// <param name="owner">所属多选组合框控件。</param>
        internal SparkDataGridComboBoxListControl(SparkDataGridComboBoxBase owner) : base()
        {
            this.mOwner = owner;

            this.AutoScroll = true;
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.MinimumSize = new Size(10, 10);
            this.MaximumSize = new Size(960, 720);

            this.InitializeComponent();
            this.mTxtFilter.TextChanged += TxtFilter_TextChanged;
            this.mTxtFilter.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                {
                    this.DgvList_KeyDown(sender, e);
                }
            };
        }

        /// <summary>
        /// KeyDown事件处理
        /// </summary>
        /// <param name="e"></param>
        internal new void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.mBtnOk.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.mOwner.IsDroppedDown = false;
                e.SuppressKeyPress = false;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (this.mDgvList.Rows.Count <= 0 || this.mDgvList.Columns.Count <= 0) { return; }

                int selectIndex;
                if (this.mDgvList.CurrentRow == null)
                {
                    selectIndex = 0;
                }
                else if (this.mDgvList.CurrentRow.Index == this.mDgvList.Rows.Count - 1)
                {
                    return;
                }
                else
                {
                    selectIndex = this.mDgvList.CurrentRow.Index + 1;
                }

                this.mDgvList.Rows[selectIndex].Selected = true;
                this.mDgvList.CurrentCell = this.mDgvList.Rows[selectIndex].Cells[mDgvList.FirstDisplayedCell.ColumnIndex];
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (this.mDgvList.Rows.Count <= 0 || this.mDgvList.Columns.Count <= 0) { return; }

                int selectIndex;
                if (this.mDgvList.CurrentRow == null)
                {
                    selectIndex = this.mDgvList.Rows.Count - 1;
                }
                else if (this.mDgvList.CurrentRow.Index == 0)
                {
                    return;
                }
                else
                {
                    selectIndex = this.mDgvList.CurrentRow.Index - 1;
                }

                this.mDgvList.Rows[selectIndex].Selected = true;
                this.mDgvList.CurrentCell = this.mDgvList.Rows[selectIndex].Cells[mDgvList.FirstDisplayedCell.ColumnIndex];
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 获取选中项列表。
        /// </summary>
        /// <returns>选中项列表。</returns>
        internal List<object> GetSelectedItems()
        {
            return this.mSelectedItems;
        }

        /// <summary>
        /// 选择指定索引号的行。
        /// </summary>
        /// <param name="indexes">需要选中行索引号的集合。</param>
        internal void SelectRows(params int[] indexes)
        {
            this.mSelectedItems.Clear();
            if (indexes == null || !indexes.Any()) { return; }

            IEnumerable<object> items = SparkComboBoxDatasourceResolver.GetItemByIndex(this.mOwner.DataSource, indexes);
            if (items != null && items.Any())
            {
                this.mSelectedItems.AddRange(items);
            }
        }

        /// <summary>
        /// 清空选中项。
        /// </summary>
        internal void ClearSelectedItems()
        {
            if (this.mSelectedItems.Any())
            {
                this.mSelectedItems.Clear();
                this.SelectedChanged?.Invoke(this, new SparkDataGridComboBoxSelectedChangedEventArgs(this.mSelectedItems));
            }
        }

        /// <summary>
        /// 执行数据源筛选。
        /// </summary>
        internal void FilterDataSource()
        {
            string keyword = this.mTxtFilter.Text.Trim();

            if (this.mOwner.QueryableDataModel)
            {
                this.mOwner.QueryDataSource(keyword, false, true);
            }

            /*
             * 内部方法：关键字排序
             */
            DataView OrderDataRows(DataView dataView)
            {
                if (dataView == null || dataView.Count < 1)
                {
                    return dataView;
                }

                var strCols = dataView.Table.Columns.OfType<DataColumn>().Where(dc => dc.DataType == typeof(string)).Select(dc => dc.ColumnName);
                var dataView1 = dataView.Cast<DataRowView>().OrderBy(_ =>
                {
                    IEnumerable<int> filterIndexes = strCols.Select(f => $"{_[f]}".ToUpper().IndexOf((keyword ?? "").Trim().ToUpper())).Where(x => x >= 0);
                    return filterIndexes.Any() ? filterIndexes.Min() : int.MaxValue;
                }).ThenBy(a => strCols.Select(f => $"{a[f]}".ToUpper() == (keyword ?? "").Trim().ToUpper() ? -1 : $"{a[f]}".Length).Min()).Select(a => a.Row).CopyToDataTable().DefaultView;//完全相同的再最前，后面根据字的多少再次排序

                return dataView1;
            }

            this.mDgvList.DataSource = null;

            if (this.mDataSource is DataTable dt)
            {
                //this.mDgvList.Tag = this.mOwner.DataSource?.GetHashCode();
                var filterView = dt.DefaultView.FilterByKeyword(keyword);
                this.mDgvList.DataSource = filterView.Count > 0 ? OrderDataRows(filterView) : filterView;
            }
            else if (this.mDataSource is DataView dv)
            {
                //this.mDgvList.Tag = this.mOwner.DataSource?.GetHashCode();
                var filterView = dv.FilterByKeyword(keyword);
                this.mDgvList.DataSource = filterView.Count > 0 ? OrderDataRows(filterView) : filterView;
            }
            else if (this.mDataSource is IEnumerable enumerable)
            {
                // 转换成枚举集合，如果关键字不为空则执行筛选
                IEnumerable<object> objs = enumerable.Cast<object>();
                if (!keyword.IsNullOrEmpty())
                {
                    // 用于筛选的属性
                    IEnumerable<PropertyInfo> filterProps = null;
                    objs = objs.Where(o =>
                    {
                        if (o is string s)
                        {
                            return s.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
                        }
                        else
                        {
                            if (filterProps == null)
                            {
                                // 获取字符串类型的属性
                                filterProps = o.GetType().GetProperties().Where(pi => pi.PropertyType == typeof(string));
                            }
                            foreach (PropertyInfo pi in filterProps)
                            {
                                if (pi.GetValue(o, null)?.ToString()?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) { return true; }
                            }
                            return false;
                        }
                    });

                    objs = objs.Where(n => filterProps.Any(p => p.GetValue(n, null)?.ToString()?.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                    .OrderBy(_ =>
                    {
                        IEnumerable<int> filterIndexes = filterProps.Select(p => (p.GetValue(_, null)?.ToString() ?? "").ToUpper().IndexOf((keyword ?? "").Trim().ToUpper())).Where(x => x >= 0);
                        return filterIndexes != null && filterIndexes.Any() ? filterIndexes.Min() : int.MaxValue;
                    }).ThenBy(a => filterProps.Select(p => (p.GetValue(a, null)?.ToString() ?? "").ToUpper() == (keyword ?? "").Trim().ToUpper() ? -1 : (p.GetValue(a, null)?.ToString() ?? "").Length).Min());//完全相同的再最前，后面根据字的多少再次排序
                }
                this.mDgvList.DataSource = objs?.ToList();
            }

            if (this.mDgvList.Rows.Count > 0)
            {
                //设置当前行，用于上、下键的定位
                this.mDgvList.CurrentCell = this.mDgvList.Rows[0].Cells[mDgvList.FirstDisplayedCell.ColumnIndex];
            }
            //筛选的结果中如果有已经选中的项，需要把这个项打勾
            this.CheckedRowByItem();
        }

        /// <summary>
        /// 执行数据源筛选。
        /// </summary>
        /// <param name="keyword">筛选关键字。</param>
        internal void FilterDataSource(string keyword)
        {
            this.mTxtFilter.Text = keyword;
        }

        /// <summary>
        /// 同步选项列表。
        /// </summary>
        internal void SynchronizeDataSource()
        {
            if (this.mOwner == null)
            {
                return;
            }
            if (this.mOwner.DataSource is DataTable dt)
            {
                // 数据源类型发生改变，移除选择项
                //if (this.mDgvList.DataSource != dt.DefaultView && $"{dt.GetHashCode()}" != tag)
                //{
                //    this.ClearSelectedItems();
                //    this.mDgvList.DataSource = null;
                //    this.mDgvList.DataSource = dt.DefaultView;
                //}
                this.mDgvList.DataSource = dt.DefaultView;
            }
            else if (this.mOwner.DataSource is DataView dv)
            {
                // 数据源类型发生改变，移除选择项
                //if (this.mDgvList.DataSource != dv && $"{dv.GetHashCode()}" != tag)
                //{
                //    this.ClearSelectedItems();
                //    this.mDgvList.DataSource = null;
                //    this.mDgvList.DataSource = dv;
                //}
                this.mDgvList.DataSource = dv;
            }
            else if (this.mOwner.DataSource is IEnumerable enumerable)
            {
                this.mDgvList.DataSource = null;

                IEnumerable<object> objs = enumerable.Cast<object>();
                if (!objs.Any()) { return; }

                if (objs.First() is BaseObject)
                {
                    // 转为DataView
                    // 筛选性能需要，此处将枚举集合转为 DataTable 对象
                    this.mDgvList.DataSource = objs.OfType<BaseObject>().Parse(false).DefaultView;
                }
                else if (objs.First() is string)
                {
                    // 转为DataView
                    // DataGridView 数据源不能接收基础类型的枚举集合，故需转换为 BaseObject 类型的数据源
                    this.mDgvList.DataSource = objs.Select(o => new BaseObject(o?.ToString(), o?.ToString())).Parse().DefaultView;
                }
                else
                {
                    // 不支持非 BaseObject 类型的数据源
                    throw new Exception("不支持的数据源类型。");
                }
                DataView dvObj = this.mDgvList.DataSource as DataView;
                this.mItemRows.Clear();
                this.mItemRows.AddRange(objs.Select((value, index) => new KeyValuePair<object, DataRowView>(value, dvObj[index])), true);
            }
            else if (this.mOwner.DataSource != null)
            {
                throw new NotSupportedException("不被支持的数据源类型。");
            }

            // 通过设置多选属性实现复选框的添加或移除
            this.MultiSelect = this.MultiSelect;

            if (this.mDgvList.Rows.Count > 0)
            {
                //设置当前行，用于上、下键的定位
                this.mDgvList.CurrentCell = this.mDgvList.Rows[0].Cells[mDgvList.FirstDisplayedCell.ColumnIndex];
            }

            // 缓存数据源
            this.mDataSource = this.mDgvList.DataSource;
        }

        /// <summary>
        /// 处理 Windows 消息。
        /// </summary>
        /// <param name="m">Windows 消息。</param>
        protected override void WndProc(ref Message m)
        {
            if ((this.Parent is Popup popup) && popup.ProcessResizing(ref m))
            {
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 引发 VisibleChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.mOwner.QueryableDataModel == false)
            {
                this.mTxtFilter.Text = string.Empty;
            }

            this.CheckedRowByItem();

            base.OnVisibleChanged(e);
        }

        private void CheckedRowByItem()
        {
            if (this.mSelectedItems.Any())
            {
                IEnumerable<object> items = this.mSelectedItems;
                // 集合类型数据源需要转换为 DataViewRow 类型
                if (this.mItemRows.Count > 0 && this.mDgvList.DataSource is DataView dv)
                {
                    IEnumerable<DataRowView> drvs = dv.Cast<DataRowView>();
                    items = this.mSelectedItems.Where(i => drvs.Any(x => this.mItemRows.ContainsKey(i) && x.Row == this.mItemRows[i].Row)).Select(i => this.mItemRows[i]).ToArray();
                }

                if (this.MultiSelect)
                {
                    foreach (object item in items)
                    {
                        DataGridViewRow dgvRow = this.mDgvList.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => SparkComboBoxDatasourceResolver.Equals(row.DataBoundItem, item));
                        if (dgvRow != null)
                        {
                            dgvRow.Cells["dgvcChecked"].Value = true;
                        }
                    }
                    //var column = this.mDgvList.Columns["dgvcChecked"];
                    //if(column!=null)
                    //{
                    //    this.mDgvList.Sort(column, ListSortDirection.Ascending);
                    //}
                }
                else
                {
                    DataGridViewRow dgvRow = this.mDgvList.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => SparkComboBoxDatasourceResolver.Equals(row.DataBoundItem, items.FirstOrDefault()));
                    if (dgvRow != null)
                    {
                        dgvRow.Selected = true;
                    }
                }
            }
        }

        private void TxtFilter_TextChanged(object sender, EventArgs e)
        {
            this.FilterDataSource();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (this.SelecingChanged != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                this.SelecingChanged(this, args);
                if (args.Cancel)
                {
                    return;
                }
            }
            void SetSelectedItems(IEnumerable<object> items)
            {
                if (this.MultiSelect)
                {
                    // 多选
                    IEnumerable<object> delItems = this.mSelectedItems.Where(x =>
                    {
                        object drv = this.mOwner.DataSource is IEnumerable && this.mDgvList.DataSource is DataView ? this.mItemRows[x] : x;
                        return this.Items.Any(y => SparkComboBoxDatasourceResolver.Equals(drv, y)) && !items.Any(y => SparkComboBoxDatasourceResolver.Equals(drv, y));
                    });
                    for (int i = delItems.Count() - 1; i >= 0; i--)
                    {
                        this.mSelectedItems.Remove(delItems.ElementAt(i));
                    }

                    IEnumerable<object> addItems = items.Where(x => x != null && !this.mSelectedItems.Any(y => SparkComboBoxDatasourceResolver.Equals(x, y)));
                    foreach (object item in addItems)
                    {
                        this.mSelectedItems.Add(item);
                    }
                }
                else
                {
                    // 单选
                    if (items != null && items.Any())
                    {
                        this.mSelectedItems.Clear();

                        object item = items.FirstOrDefault();
                        if (item != null) { this.mSelectedItems.Add(item); }
                    }
                }
            }

            this.mOwner.IsDroppedDown = false;

            IEnumerable<object> selectedItems = (this.MultiSelect ? this.mDgvList.Rows.Cast<DataGridViewRow>().Where(r => (r.Cells["dgvcChecked"] as DataGridViewCheckBoxCell).FormattedValue.Equals(true)) : this.mDgvList.SelectedRows.Cast<DataGridViewRow>()).Select(x => x.DataBoundItem);
            if (this.mOwner.DataSource is DataTable || this.mOwner.DataSource is DataView)
            {
                SetSelectedItems(selectedItems);
            }
            else if (this.mOwner.DataSource is IEnumerable)
            {
                if (this.mDgvList.DataSource is DataView ds)
                {
                    IEnumerable<object> objItems = selectedItems.Select(r => this.mItemRows.FirstOrDefault(x => SparkComboBoxDatasourceResolver.Equals(x.Value, r)).Key);
                    SetSelectedItems(objItems);
                }
                else if (this.mIsString)
                {
                    IEnumerable<string> objItems = selectedItems.Select(r => (r as BaseObject).Id);
                    SetSelectedItems(objItems);
                }
                else
                {
                    SetSelectedItems(selectedItems);
                }
            }

            this.SelectedChanged?.Invoke(this, new SparkDataGridComboBoxSelectedChangedEventArgs(this.mSelectedItems));
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.mOwner.IsDroppedDown = false;
        }

        private void DgvList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.mOwner.MultiSelect) { return; }
            this.mBtnOk.PerformClick();
        }

        private void DgvList_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (e.Column.Name != "dgvcChecked")
            {
                e.Column.ReadOnly = true;
            }
        }

        private void DgvList_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        internal void InitDataGridView()
        {
            if (this.mOwner.DataSource == null)
            {
                this.mDgvList.DataSource = null;
                return;
            }

            this.mIsBaseObject = false;
            this.mIsString = false;

            if (this.mOwner.DataSource is IEnumerable enumerable)
            {
                IEnumerable<object> objs = enumerable.Cast<object>();
                if (objs.Any())
                {
                    // 类型验证，数据源只允许同类型集合
                    IEnumerable<Type> types = objs.Select(obj => obj.GetType()).Distinct();
                    if (types.Count() > 1) { throw new Exception("不允许多种数据类型的项。"); }

                    if (objs.First() is BaseObject)
                    {
                        this.mIsBaseObject = true;
                    }
                    else if (objs.First() is string)
                    {
                        this.mIsBaseObject = true;
                        this.mIsString = true;
                    }
                }
            }

            if (this.mIsBaseObject)
            {
                this.mDgvList.Columns.Clear();
                this.mDgvList.AutoGenerateColumns = false;

                DataGridViewTextBoxColumn dgvTxtBoxColCode = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn dgvTxtBoxColName = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn dgvTxtBoxColMemo = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn dgvTxtBoxColPY = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn dgvTxtBoxColWB = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn dgvTxtBoxColZJ = new DataGridViewTextBoxColumn();
                // 
                // dgvTxtBoxColCode
                // 
                dgvTxtBoxColCode.DataPropertyName = "Id";
                dgvTxtBoxColCode.HeaderText = "编码";
                dgvTxtBoxColCode.Name = "dgvTxtBoxColCode";
                dgvTxtBoxColCode.ReadOnly = true;
                dgvTxtBoxColCode.Width = 100;
                // 
                // dgvTxtBoxColName
                // 
                dgvTxtBoxColName.DataPropertyName = "Name";
                dgvTxtBoxColName.HeaderText = "名称";
                dgvTxtBoxColName.Name = "dgvTxtBoxColName";
                dgvTxtBoxColName.ReadOnly = true;
                dgvTxtBoxColName.Width = 200;
                // 
                // dgvTxtBoxColMemo
                // 
                dgvTxtBoxColMemo.DataPropertyName = "Memo";
                dgvTxtBoxColMemo.HeaderText = "备注";
                dgvTxtBoxColMemo.Name = "dgvTxtBoxColMemo";
                dgvTxtBoxColMemo.ReadOnly = true;
                dgvTxtBoxColMemo.Width = 80;
                // 
                // dgvTxtBoxColPY
                // 
                dgvTxtBoxColPY.DataPropertyName = "SpellCode";
                dgvTxtBoxColPY.HeaderText = "拼音码";
                dgvTxtBoxColPY.Name = "dgvTxtBoxColPY";
                dgvTxtBoxColPY.ReadOnly = true;
                dgvTxtBoxColPY.Width = 100;
                // 
                // dgvTxtBoxColWB
                // 
                dgvTxtBoxColWB.DataPropertyName = "WBCode";
                dgvTxtBoxColWB.HeaderText = "五笔码";
                dgvTxtBoxColWB.Name = "dgvTxtBoxColWB";
                dgvTxtBoxColWB.ReadOnly = true;
                dgvTxtBoxColWB.Width = 100;
                // 
                // dgvTxtBoxColZJ
                // 
                dgvTxtBoxColZJ.DataPropertyName = "CustomCode";
                dgvTxtBoxColZJ.HeaderText = "助记码";
                dgvTxtBoxColZJ.Name = "dgvTxtBoxColZJ";
                dgvTxtBoxColZJ.ReadOnly = true;
                dgvTxtBoxColZJ.Width = 100;

                // 添加到表格列集合
                this.mDgvList.Columns.AddRange(new DataGridViewColumn[] {
                dgvTxtBoxColCode,
                dgvTxtBoxColName,
                dgvTxtBoxColMemo,
                dgvTxtBoxColPY,
                dgvTxtBoxColWB,
                dgvTxtBoxColZJ});

                // 多选时添加勾选列
                if (this.MultiSelect)
                {
                    SparkDataGridCheckBoxColumn dgvcChecked = new SparkDataGridCheckBoxColumn
                    {
                        Frozen = true,
                        HeaderText = "选择",
                        Name = "dgvcChecked",
                        ReadOnly = false,
                        Width = 38
                    };
                    this.mDgvList.Columns.Insert(0, dgvcChecked);
                }
            }
            else
            {
                if (this.mDgvList.Columns.Contains("dgvTxtBoxColCode"))
                {
                    this.mDgvList.Columns.Remove("dgvTxtBoxColCode");
                }

                if (this.mDgvList.Columns.Contains("dgvTxtBoxColName"))
                {
                    this.mDgvList.Columns.Remove("dgvTxtBoxColName");
                }

                if (this.mDgvList.Columns.Contains("dgvTxtBoxColMemo"))
                {
                    this.mDgvList.Columns.Remove("dgvTxtBoxColMemo");
                }

                if (this.mDgvList.Columns.Contains("dgvTxtBoxColPY"))
                {
                    this.mDgvList.Columns.Remove("dgvTxtBoxColPY");
                }

                if (this.mDgvList.Columns.Contains("dgvTxtBoxColWB"))
                {
                    this.mDgvList.Columns.Remove("dgvTxtBoxColWB");
                }

                if (this.mDgvList.Columns.Contains("dgvTxtBoxColZJ"))
                {
                    this.mDgvList.Columns.Remove("dgvTxtBoxColZJ");
                }

                this.mDgvList.AutoGenerateColumns = true;
            }
        }

    }

    /// <summary>
    /// 表格组合框勾选状态改变事件参数
    /// </summary>
    public class SparkDataGridComboBoxSelectedChangedEventArgs
    {
        /// <summary>
        /// 获取选择项绑定到的数据对象集合。
        /// </summary>
        public object[] SelectedItems { get; }

        /// <summary>
        /// 初始 <see cref="SparkDataGridComboBoxSelectedChangedEventArgs"/> 类型的新实例
        /// </summary>
        /// <param name="items">选择项绑定到的数据对象集合。</param>
        public SparkDataGridComboBoxSelectedChangedEventArgs(IEnumerable<object> items)
        {
            this.SelectedItems = items?.ToArray();
        }
    }

    /// <summary>
    /// 表格组合框勾选状态改变事件委托。
    /// </summary>
    /// <param name="sender">事件发送对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void SparkDataGridComboBoxSelectedChangedEventHandler(object sender, SparkDataGridComboBoxSelectedChangedEventArgs e);

    /// <summary>
    /// 表格组合框勾选状态改变前事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SparkDataGridComboBoxSelectingChangedEventHandler(object sender, CancelEventArgs e);
}