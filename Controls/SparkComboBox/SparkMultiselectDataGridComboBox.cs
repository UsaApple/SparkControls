using System;
using System.Collections;
using System.Collections.Generic;
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
    public class SparkMultiselectDataGridComboBox : SparkDataGridComboBoxBase
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
                this.SelectedText = value.Split(',');
                if ((SelectedItem == null || !SelectedItem.Any()) && this.DropDownStyle != ComboBoxStyle.DropDownList)
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
        public new IEnumerable<object> SelectedItem
        {
            get { return mDataGridComboBoxListControl.GetSelectedItems(); }
            set
            {
                var objValue = value?.Where(a => a != null);
                if (DataSource != null && objValue != null && objValue.Any())
                {
                    SelectedIndex = objValue.Select(v => SparkComboBoxDatasourceResolver.GetItemIndex(DataSource, v));
                }
                else if (SelectedIndex != null && SelectedIndex.Any() && (objValue == null || !objValue.Any()))
                {
                    SelectedIndex = null;
                }
            }
        }

        /// <summary>
        /// 获取或设置 ComboBox 当前选定项的索引。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new IEnumerable<int> SelectedIndex
        {
            get
            {
                return SelectedItem.Select(v => SparkComboBoxDatasourceResolver.GetItemIndex(DataSource, v));
            }
            set
            {
                var obj = value?.Where(a => a >= 0);
                if (!SparkComboBoxDatasourceResolver.Equals(SelectedIndex, obj))
                {
                    this.mDataGridComboBoxListControl.SelectRows(obj?.ToArray());
                    this.UpdateText();
                    //直接设置SelectedIndex或SelectedItem或SelectValue需要触发事件
                    OnSelectedChanged(new SparkDataGridComboBoxSelectedChangedEventArgs(this.SelectedItem));
                }
            }
        }

        /// <summary>
        /// 获取或设置 ComboBox 当前选定的项由 DisplayMember 指定的成员属性的值。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new IEnumerable<string> SelectedText
        {
            get
            {
                return SelectedItem.Select(i => SparkComboBoxDatasourceResolver.GetMemberValue(i, DisplayMember)?.ToString());
            }
            set
            {
                var obj = value?.Where(a => !a.IsNullOrEmpty());
                if (DataSource != null && obj != null && obj.Any())
                {
                    var objs = obj.Select(v => SparkComboBoxDatasourceResolver.GetItemByMemeberValue(DataSource, DisplayMember, v));
                    if (objs == null || objs.Count() == 0)
                    {
                        SelectedItem = null;
                    }
                    else if (!SparkComboBoxDatasourceResolver.Equals<object>(SelectedItem, objs))
                    {
                        SelectedItem = objs;
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
        public new IEnumerable<object> SelectedValue
        {
            get
            {
                return SelectedItem.Select(i => SparkComboBoxDatasourceResolver.GetMemberValue(i, ValueMember));
            }
            set
            {
                var objValue = value?.Where(a => a != null && !a.ToString().IsNullOrEmpty());
                if (DataSource != null && objValue != null && objValue.Any())
                {
                    var objs = objValue.Select(v => SparkComboBoxDatasourceResolver.GetItemByMemeberValue(DataSource, ValueMember, v));
                    if (!SparkComboBoxDatasourceResolver.Equals<object>(SelectedItem, objs))
                    {
                        SelectedItem = objs;
                    }
                }
                else if (SelectedItem != null)
                {
                    SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// 多选模式下，获取或设置 ComboBox 当前选定的BaseObject项。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new IEnumerable<BaseObject> SelectedObject
        {
            get
            {
                if (SelectedItem != null)
                {
                    return SelectedItem.Select(i => (i is BaseObject obj) ? obj : null).Where(i => i != null);
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
        public SparkMultiselectDataGridComboBox() : base()
        {
            MultiSelect = true;
            DataSourceChanged += (sneder, e) => SelectedIndex = null;
        }

        #endregion

        /// <summary>
        /// 更新控件的文本
        /// </summary>
        protected override void UpdateText()
        {
            mTextBox.Text = string.Join(",", SelectedText.ToArray());
            mTextBox.SelectionStart = (mTextBox.Text ?? "").Length;
        }


        #region IDualDataBinding 接口成员

        /// <summary>
        /// 获取或设置控件的实际值。
        /// </summary>
        [Browsable(false)]
        public override object Value
        {
            get => this.SelectedValue;
            set => this.SelectedValue = value is IEnumerable ie ? ie.Cast<object>() : (new object[] { value });
        }

        /// <summary>
        /// 获取或设置控件的显示值。
        /// </summary>
        [Browsable(false)]
        public override string DisplayValue => string.Join(",", this.SelectedText.ToArray());

        #endregion
    }
}