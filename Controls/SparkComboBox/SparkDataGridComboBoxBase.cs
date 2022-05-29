using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SparkControls.Foundation;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表格组合框的基类。
    /// </summary>
    [ToolboxItem(false)]
    public class SparkDataGridComboBoxBase : SparkPopupComboBox, IMessageFilter
    {
        internal readonly SparkDataGridComboBoxListControl mDataGridComboBoxListControl;
        private bool mDataGridComboBoxListControlInited = false;

        #region 事件

        /// <summary>
        /// 选择项发生改变时发生。
        /// </summary>
        public event SparkDataGridComboBoxSelectedChangedEventHandler SelectedChanged;

        #endregion

        #region 属性

        private bool mMultiSelect = false;
        /// <summary>
        /// 获取或设置一个值，该值指示是否可以选择多行。
        /// </summary>
        internal bool MultiSelect { get { return mMultiSelect; } set { mDataGridComboBoxListControl.MultiSelect = mMultiSelect = value; } }

        private object mDataSource = null;
        /// <summary>
        /// 获取或设置控件的数据源。
        /// </summary>
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new object DataSource
        {
            get
            {
                if (mDataSource == null && this.Items.Count > 0)
                {
                    mDataSource = this.Items;
                    OnDataSourceChanged(EventArgs.Empty);
                }
                return mDataSource;
            }
            set
            {
                if (this.QueryableDataModel == false)
                {
                    this.Text = string.Empty;
                    this.SelectedIndex = -1;
                }
                else
                {
                    base.mSelectedIndex = -1;
                }

                this.mDataSource = value;
                this.Items.Clear();
                if (value != null)
                {
                    this.Items.AddRange(SparkComboBoxDatasourceResolver.Convert2ItemCollection(value).ToArray());
                }
                this.mDataGridComboBoxListControl.SynchronizeDataSource();

                this.OnDataSourceChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// 设置或获取Guid,用于保存样式文件的名称（需要唯一值）
        /// </summary>
        [Category("Spark"), Description("设置或获取Guid,用于保存样式文件的名称（需要唯一值）")]
        [DefaultValue("")]
        public string Guid
        {
            get => mDataGridComboBoxListControl.Guid;
            set => mDataGridComboBoxListControl.Guid = value;
        }

        #region 动态查询需要的属性
        /// <summary>
        /// 获取或设置对应数据库的连接名。
        /// </summary>
        [Category("Spark")]
        [Description("对应数据库的连接名(连串配置文件中定义)。")]
        [DefaultValue(null)]
        public virtual string ConnectionName { get; set; } = null;

        /// <summary>
        /// 获取或设置用于获取数据源的 SQL 系统代码。
        /// </summary>
        [Category("Spark")]
        [Description("用于获取数据源的 SQL 系统代码。")]
        [DefaultValue("")]
        public virtual string DataSourceSystemCode { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置用于获取数据源的 SQL 脚本名称。
        /// </summary>
        [Category("Spark")]
        [Description("用于获取数据源的 SQL 脚本名称。")]
        [DefaultValue("")]
        public virtual string DataSourceSqlId { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置用于 SQL 脚本的参数
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public virtual Dictionary<string, object> DataSourceParameters { get; set; }

        /// <summary>
        /// 用于获取数据源的 SQL 语句,{0}表示输入的内容，可以配置在语句中。优先级小于 DataSourceSqlId
        /// </summary>
        [Category("Spark")]
        [Description("用于获取数据源的 SQL 语句,{0}表示输入的内容，可以配置在语句中。优先级小于 DataSourceSqlId")]
        [DefaultValue("")]
        public virtual string SqlString { get; set; } = "";

        /// <summary>
        /// 使用DataSourceSqlId或SqlString查询数据时，false表示查询全部数据（默认）；true表示动态查询，返回前100行数据。（针对大数据请使用动态查询）
        /// </summary>
        [Category("Spark")]
        [Description("使用DataSourceSqlId或SqlString查询数据时，false表示查询全部数据（默认）；true表示动态查询，返回前100行数据。（针对大数据请使用动态查询）")]
        [DefaultValue(false)]
        public virtual bool QueryableDataModel { get; set; } = false;
        #endregion

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始 <see cref="SparkDataGridComboBox" /> 类型的新实例。
        /// </summary>
        public SparkDataGridComboBoxBase() : base()
        {
            mDataGridComboBoxListControl = new SparkDataGridComboBoxListControl(this)
            {
                Dock = DockStyle.Fill,
                MultiSelect = MultiSelect,
                Padding = new Padding(0, 0, 0, 12)
            };

            mDataGridComboBoxListControl.SelecingChanged += (sender, e) =>
            {
                OnSelectingItemChanged(e);
            };

            mDataGridComboBoxListControl.SelectedChanged += (sender, e) =>
            {
                UpdateText();
                OnSelectedChanged(e);
            };

            DropDownControl = mDataGridComboBoxListControl;
            PopupDropDown.Size = mDataGridComboBoxListControl.Size;
            PopupDropDown.AutoClose = false;
            bool isFirstOpenPopup = false;
            PopupDropDown.Opening += (sender, e) =>
            {
                //第一次打开的时候，如果是动态查询，需要查询一次数据源，不然打开的是空的下拉界面
                if (isFirstOpenPopup == false && this.QueryableDataModel == true && this.DataSource == null)
                {
                    isFirstOpenPopup = true;
                    this.QueryDataSource("", true, true);
                }
                if (this.QueryableDataModel == false)
                {
                    this.mDataGridComboBoxListControl.SynchronizeDataSource();
                }
            };
            PopupDropDown.Closed += (sender, e) =>
            {
                this.IsDroppedDown = false;
            };
            KeyPress += (sender, e) =>
            {
                //回车 和 Esc
                if (e.KeyChar == 13 || e.KeyChar == 27) { return; }
                if (this.SelectedItem != null)
                {
                    this.SelectedItem = null;
                }
                this.IsDroppedDown = true;
            };
            MouseClick += (sender, e) =>
            {
                if (this.RectangleToScreen(ButtonRect).Contains(MousePosition))
                {
                    mDataGridComboBoxListControl.mDgvList.Focus();
                }
            };
            TextChanged += (sender, e) =>
            {
                if (this.IsDroppedDown)
                {
                    mDataGridComboBoxListControl.FilterDataSource(this.Text);
                    mTextBox.Focus();
                }
            };

            ItemsChanged += (sender, e) =>
            {
                if (e.Action == ItemsAction.Clear)
                {
                    mDataGridComboBoxListControlInited = false;
                    mDataGridComboBoxListControl.ClearSelectedItems();
                }
                else if (e.Action == ItemsAction.Remove)
                {
                    if (this.Items.Count < 1)
                    {
                        mDataGridComboBoxListControlInited = false;
                        mDataGridComboBoxListControl.ClearSelectedItems();
                    }
                    else if (!e.ChangedItems.Contains(this.SelectedItem))
                    {
                        mDataGridComboBoxListControl.ClearSelectedItems();
                    }
                }
                else if (!mDataGridComboBoxListControlInited && this.Items.Count > 0)
                {
                    mDataGridComboBoxListControl.InitDataGridView();
                    mDataGridComboBoxListControlInited = true;
                }
            };
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 引发 SelectedChanged 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected virtual void OnSelectedChanged(SparkDataGridComboBoxSelectedChangedEventArgs e)
        {
            SelectedChanged?.Invoke(this, e);
            OnSelectedItemChanged(EventArgs.Empty);
            OnSelectedIndexChanged(EventArgs.Empty);
            OnSelectedValueChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 释放由 <see cref="SparkDataGridComboBox"/> 所使用的资源。
        /// </summary>
        /// <param name="disposing">如果应该释放托管资源为 true，否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                mDataGridComboBoxListControl?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 重写KeyDown事件，回车健实现Tab健功能，切换焦点
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                if (PopupDropDown?.Visible == true)
                {
                    mDataGridComboBoxListControl?.OnKeyDown(e);
                    if (e.KeyCode == Keys.Enter && IsEnterToTab)
                    {
                        e.Handled = true;
                    }
                }
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// 重写ShowDropDown方法
        /// </summary>
        protected override void ShowDropDown()
        {
            // 引发事件
            OnDropDown(EventArgs.Empty);
            if (PopupDropDown != null && !PopupDropDown.Visible)
            {
                PopupDropDown.Show(this);
            }
        }

        /// <summary>
        /// 更新控件的文本
        /// </summary>
        protected virtual void UpdateText()
        {
            mTextBox.Text = SelectedText;
            mTextBox.SelectionStart = (mTextBox.Text ?? "").Length;
        }

        #endregion

        #region IMessageFilter 成员  

        /// <summary>
        /// 在调度消息之前将其筛选出来。
        /// </summary>
        /// <param name="m">要调度的消息。</param>
        /// <returns>如果筛选消息并禁止消息被调度，则为 true；如果允许消息继续到达下一个筛选器或控件，则为 false。</returns>
        public override bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 522 && mTextBox.Focused)
            {
                // 禁用鼠标滚轮
                return true;
            }
            else
            {
                return base.PreFilterMessage(ref m);
            }
        }

        #endregion

        #region 查询数据源

        /// <summary>
        /// 根据动态查询的配置,查询数据源
        /// </summary>
        /// <param name="key">筛选的字符或选中项目的ID</param>
        /// <param name="isFirst">是否第一次筛选,true第一次,false不是第一次</param>
        /// <param name="isInitializeOrSetValue">是否是初始化或设置SelectedValue,true是,false不是</param>
        public virtual void QueryDataSource(string key, bool isFirst, bool isInitializeOrSetValue)
        {
            var sqlId = this.DataSourceSqlId;
            var sqlSysId = this.DataSourceSystemCode;
            var sqlString = this.SqlString;
            var sqlConnectName = this.ConnectionName;

            if (!sqlId.IsNullOrEmpty() && !sqlSysId.IsNullOrEmpty())
            {//动态查询，需要把语句 获取，要拼接语句进行查询
                var resp = DAL.DbAccessFactory.Create().GetSqlString(new DTO.DbInputSection()
                {
                    GroupName = sqlSysId,
                    Id = sqlId,
                });
                if (resp?.Content != null)
                {
                    sqlString = resp.Content.Content;
                    sqlConnectName = resp.Content.DbName;
                }
            }
            //动态查询的方式
            if (this.QueryableDataModel == true)
            {
                if (!sqlString.IsNullOrEmpty() && !sqlConnectName.IsNullOrEmpty())
                {
                    //语句怎么拼接呢，一个是用全局参数的替换，一个用DataSourceParameters属性来替换，还有一个是{0}表示当要筛选的字符
                    //如果下拉框选中了一个项之后,第一次初始化时,需要查询数据源,把选中的想查询出来赋值到DataSource里面.(否则无法赋值了)
                    //每次第一次打开下拉框的时候,要查询100行数据 + 选中的数据,之后只查询100行的数据,选中的数据不用管
                    var dbInput = new DTO.DbInput()
                    {
                        CommandText = sqlString,
                        ConnectName = sqlConnectName,
                    };
                    dbInput.InParameters = Spark.DAL.DbInputExtensions.ToParameters(ParamObject.MergePara(this.DataSourceParameters));
                    if (isInitializeOrSetValue)
                    {
                        //初始化的话,把选中的项目查询出来
                        if (!this.FieldName.IsNullOrEmpty())
                        {
                            if (this.DataSource is DataTable dt)
                            {
                                if (!dt.HasColumn(this.ValueMember)) return;
                                if (dt.Rows.Cast<DataRow>().Any(a => $"{a[this.ValueMember]}" == key)) return;
                            }
                            //查询选中的项目 及 其他数据100条
                            sqlString = DoSqlString(sqlString, key, this.FieldName, 101);
                            dbInput.CommandText = sqlString;
                            var resp = DAL.DbAccessFactory.Create().QueryToTable(dbInput);
                            this.DataSource = resp.Content;
                        }
                    }
                    else
                    {
                        if (isFirst)
                        {//第一次筛选把选中的项目查询出来 + 100条其他数据
                            sqlString = DoSqlString(sqlString, key, this.FieldName, 101);
                            dbInput.CommandText = sqlString;
                            var resp = DAL.DbAccessFactory.Create().QueryToTable(dbInput);
                            this.DataSource = resp.Content;
                        }
                        else
                        {
                            //查询100条其他数据
                            sqlString = DoSqlString(sqlString, key, "", 100);
                            dbInput.CommandText = sqlString;
                            var resp = DAL.DbAccessFactory.Create().QueryToTable(dbInput);
                            this.DataSource = resp.Content;
                        }
                    }
                }
            }
            else
            {
                if (sqlString.Contains("{0}"))
                {
                    sqlString = sqlString.Replace("{0}", "");
                }
                if (!sqlString.IsNullOrEmpty() && !sqlConnectName.IsNullOrEmpty())
                {
                    var dict = ParamObject.MergePara(this.DataSourceParameters);
                    var dbInput = new DTO.DbInput()
                    {
                        CommandText = sqlString,
                        ConnectName = sqlConnectName,
                        InParameters = Spark.DAL.DbInputExtensions.ToParameters(dict),
                    };
                    var resp = DAL.DbAccessFactory.Create().QueryToTable(dbInput);
                    this.DataSource = resp.Content;
                }
            }
        }

        private string DoSqlString(string sqlString, string key, string columnName, int rowNum = 100)
        { //seelct * from t where xxxx order by x
          //拼接 seelct * from t where ((1==0 and (xxxx))) or [FieldName=key] or rownum<=101 order by x
          //或   seelct * from t where (xxxx) and rownum<=101 order by x
          //找到最后的where 和 group by | order by
            int whereIndex = sqlString.LastIndexOf(" where ", StringComparison.OrdinalIgnoreCase);
            int groupByIndex = sqlString.LastIndexOf(" group ", StringComparison.OrdinalIgnoreCase);
            int orderByIndex = sqlString.LastIndexOf(" order ", StringComparison.OrdinalIgnoreCase);
            if (whereIndex > 0)
            {
                int endIndex = 0;
                if (groupByIndex > 0 && orderByIndex > 0) endIndex = Math.Min(groupByIndex, orderByIndex);
                else endIndex = Math.Max(groupByIndex, orderByIndex);
                string whereString = "";
                if (endIndex > 0)
                {
                    whereString = sqlString.Substring(whereIndex + 7, endIndex - whereIndex - 7);
                }
                else
                {
                    whereString = sqlString.Substring(whereIndex + 7);
                }
                string joinWhereString = "";
                if (columnName.IsNullOrEmpty() || key.IsNullOrEmpty())
                {
                    joinWhereString = $"({whereString}) and rownum<={rowNum}";
                    if (joinWhereString.Contains("{0}"))
                    {
                        joinWhereString = joinWhereString.Replace("{0}", key);
                    }
                }
                else
                {
                    joinWhereString = $"(1=0 and ({whereString})) or {columnName}='{key}' or rownum<={rowNum}";
                    if (joinWhereString.Contains("{0}"))
                    {
                        joinWhereString = joinWhereString.Replace("{0}", "");
                    }
                }

                string ret = sqlString.Replace(whereString, joinWhereString);

                return ret;
            }
            return sqlString;
        }

        /// <summary>
        /// 查询数据源，当QueryableDataModel=false时，且配置了ConnectionName和SqlString 或 DataSourceSystemCode和DataSourceSqlId，进行数据源查询，并初始化
        /// </summary>
        public virtual void LoadDataSource()
        {
            QueryDataSource("", true, false);
        }
        #endregion
    }
}