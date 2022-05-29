using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表格组合框。
    /// </summary>
    [ToolboxItem(true), ToolboxBitmap(typeof(ComboBox))]
    public class SparkDataGridComboBox : SparkDataGridComboBoxBase
    {
        #region 属性

        /// <summary>
        /// 获取或设置控件关联的文本。
        /// </summary>
        [Category("Spark"), Description("控件关联的文本。")]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return mTextBox.Text;
            }
            set
            {
                this.SelectedText = value;
                if (SelectedItem == null && this.DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    if (this.mTextBox.Text != value)
                    {
                        this.mTextBox.Text = value;
                        this.Invalidate(true);
                        OnTextChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 获取或设置 ComboBox 当前选定的项。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public override object SelectedItem
        {
            get { return mDataGridComboBoxListControl?.GetSelectedItems().FirstOrDefault(); }
            set
            {
                if (DataSource != null && value != null)
                {
                    SelectedIndex = SparkComboBoxDatasourceResolver.GetItemIndex(DataSource, value);
                }
                else
                {
                    SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// 获取或设置 ComboBox 当前选定项的索引。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public override int SelectedIndex
        {
            get
            {
                if (SelectedItem == null) { return -1; }
                return SparkComboBoxDatasourceResolver.GetItemIndex(DataSource, SelectedItem);
            }
            set
            {
                if (SelectedIndex != value)
                {
                    CancelEventArgs cancelArgs = new CancelEventArgs();
                    OnSelectingItemChanged(cancelArgs);
                    if (cancelArgs.Cancel)
                    {
                        return;
                    }
                    if (value >= 0)
                    {
                        this.mDataGridComboBoxListControl.SelectRows(value);
                        this.UpdateText();
                    }
                    else
                    {
                        this.mDataGridComboBoxListControl.ClearSelectedItems();
                        this.UpdateText();
                    }
                    var args = new SparkDataGridComboBoxSelectedChangedEventArgs(new object[]
                    {
                        this.SelectedItem
                    });
                    OnSelectedChanged(args);
                }
            }
        }

        /// <summary>
        /// 获取或设置 ComboBox 当前选定的项由 DisplayMember 指定的成员属性的值。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public override string SelectedText
        {
            get
            {
                if (SelectedItem == null) { return string.Empty; }
                return SparkComboBoxDatasourceResolver.GetMemberValue(SelectedItem, DisplayMember)?.ToString();
            }
            set
            {
                if (DataSource != null && !value.IsNullOrEmpty())
                {
                    var item = SparkComboBoxDatasourceResolver.GetItemByMemeberValue(DataSource, DisplayMember, value);
                    if (item == null)
                    {
                        SelectedItem = null;
                    }
                    else if (SelectedItem != item)
                    {
                        SelectedItem = item;
                    }
                }
                else if (SelectedItem != null)
                {
                    SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// 获取或设置 ComboBox 当前选定的项由 ValueMember 指定的成员属性的值。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public override object SelectedValue
        {
            get
            {
                if (SelectedItem == null) { return null; }
                return SparkComboBoxDatasourceResolver.GetMemberValue(SelectedItem, ValueMember);
            }
            set
            {
                QueryDataSource(value);//动态查询,现在只能通过SelectedValue来定位
                if (DataSource != null && value != null)
                {
                    var item = SparkComboBoxDatasourceResolver.GetItemByMemeberValue(DataSource, ValueMember, value);
                    if (item == null)
                    {
                        SelectedItem = null;
                    }
                    else if (SelectedItem != item)
                    {
                        SelectedItem = item;
                    }
                }
                else if (SelectedItem != null)
                {
                    SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// 单选模式下，获取或设置 ComboBox 当前选定的BaseObject项。
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override BaseObject SelectedObject
        {
            get
            {
                if (SelectedItem is BaseObject obj)
                {
                    return obj;
                }
                return null;
            }
            set
            {
                SelectedItem = value;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始 <see cref="SparkDataGridComboBox" /> 类型的新实例。
        /// </summary>
        public SparkDataGridComboBox() : base()
        {
            MultiSelect = false;
        }

        #endregion

        protected override void ClearItemInit()
        {
            if (this.QueryableDataModel == true)
            {
                base.mSelectedIndex = -1;
            }
            else
            {
                if (this.SelectedIndex != -1)
                {
                    this.SelectedIndex = -1;
                }
                if (!this.Text.IsNullOrEmpty())
                {
                    this.Text = "";
                }
            }
        }
        #region 动态查询
        private void QueryDataSource(object value)
        {
            if (value == null) return;
            if (this.QueryableDataModel == true)
            {
                base.QueryDataSource(value?.ToString(), false, true);
            }
        }
        #endregion
    }
}