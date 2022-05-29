using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
    /// <summary>
    /// 多选组合框。
    /// </summary>
    [ToolboxItem(true), ToolboxBitmap(typeof(ComboBox))]
    public partial class SparkMultiselectComboBox : SparkPopupComboBox
    {
        private readonly SparkMultiselectComboBoxListControl mCheckBoxComboBoxListControl;

        #region 事件

        /// <summary>
        /// 选项勾选状态改变时发生。
        /// </summary>
        public event ItemCheckEventHandler ItemCheck;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置控件的数据源。
        /// </summary>
        public new object DataSource
        {
            get => base.DataSource;
            set
            {
                this.Text = string.Empty;
                this.SelectedIndex = null;

                base.DataSource = value;
                this.mCheckBoxComboBoxListControl.SynchronizeControlsWithComboBoxItems();
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
            get { return mCheckBoxComboBoxListControl.CheckedItems.Cast<object>(); }
            set
            {
                this.mCheckBoxComboBoxListControl.SynchronizeControlsWithComboBoxItems();
                this.mCheckBoxComboBoxListControl.Items.Cast<object>().ForEach((o, i) => { this.mCheckBoxComboBoxListControl.SetItemChecked(i, false); });
                if (value == null) return ;
                var selectedIndices = mCheckBoxComboBoxListControl.Items.Cast<object>().Select((o, i) => value.Contains(o) ? i : -1).ToList().Where(i => i >= 0);
                selectedIndices.ForEach(i => mCheckBoxComboBoxListControl.SetItemChecked(i, true));

                base.OnSelectedItemChanged(EventArgs.Empty);
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
                return mCheckBoxComboBoxListControl.SelectedIndices.Cast<int>();
            }
            set
            {
                this.mCheckBoxComboBoxListControl.SynchronizeControlsWithComboBoxItems();
                this.mCheckBoxComboBoxListControl.Items.Cast<object>().ForEach((o, i) => { this.mCheckBoxComboBoxListControl.SetItemChecked(i, false); });

                value.ForEach(i =>
                {
                    if (i >= 0 && i <= mCheckBoxComboBoxListControl.Items.Count)
                    {
                        mCheckBoxComboBoxListControl.SetItemChecked(i, true);
                    }
                });
                base.OnSelectedIndexChanged(EventArgs.Empty);
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
                foreach (var item in SelectedItem)
                {
                    if (!DisplayMember.IsNullOrEmpty())
                    {
                        yield return SparkComboBoxDatasourceResolver.GetMemberValue(item, DisplayMember)?.ToString();
                    }
                    else
                    {
                        yield return mCheckBoxComboBoxListControl.GetItemText(item);
                    }
                }
            }
            set
            {
                this.mCheckBoxComboBoxListControl.SynchronizeControlsWithComboBoxItems();
                this.mCheckBoxComboBoxListControl.Items.Cast<object>().ForEach((o, i) => { this.mCheckBoxComboBoxListControl.SetItemChecked(i, false); });

                var selectedIndices = mCheckBoxComboBoxListControl.Items.Cast<object>().Select((o, i) =>
                {
                    object v = DisplayMember.IsNullOrEmpty() ? mCheckBoxComboBoxListControl.GetItemText(o) : SparkComboBoxDatasourceResolver.GetMemberValue(o, DisplayMember);
                    return value.Contains(v) ? i : -1;
                }).ToList().Where(i => i >= 0);
                selectedIndices.ForEach(i => mCheckBoxComboBoxListControl.SetItemChecked(i, true));

                base.OnTextChanged(EventArgs.Empty);
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
                foreach (var item in SelectedItem)
                {
                    if (!ValueMember.IsNullOrEmpty())
                    {
                        yield return SparkComboBoxDatasourceResolver.GetMemberValue(item, ValueMember);
                    }
                    else
                    {
                        yield return mCheckBoxComboBoxListControl.GetItemText(item);
                    }
                }
            }
            set
            {
                this.mCheckBoxComboBoxListControl.SynchronizeControlsWithComboBoxItems();
                this.mCheckBoxComboBoxListControl.Items.Cast<object>().ForEach((o, i) => { this.mCheckBoxComboBoxListControl.SetItemChecked(i, false); });
                if (value == null) return;
                var selectedIndices = mCheckBoxComboBoxListControl.Items.Cast<object>().Select((o, i) =>
                {
                    object v = ValueMember.IsNullOrEmpty() ? mCheckBoxComboBoxListControl.GetItemText(o) : SparkComboBoxDatasourceResolver.GetMemberValue(o, ValueMember);
                    return value.Contains(v) ? i : -1;
                }).ToList().Where(i => i >= 0);
                selectedIndices.ForEach(i => mCheckBoxComboBoxListControl.SetItemChecked(i, true));

                base.OnSelectedValueChanged(EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// 多选控件，禁用此属性
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new IEnumerable<BaseObject> SelectedObject
        {
            get
            {
                var item = SelectedItem;
                if (item != null && item.FirstOrDefault() is BaseObject)
                {
                    return item.Select(a => a as BaseObject).ToArray();
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
        /// 初始 <see cref="SparkMultiselectComboBox" /> 类型的新实例。
        /// </summary>
        public SparkMultiselectComboBox() : base()
        {
            // 初始化组件
            InitializeComponent();

            // 创建下拉控件
            mCheckBoxComboBoxListControl = new SparkMultiselectComboBoxListControl(this) { Dock = DockStyle.Fill };
            mCheckBoxComboBoxListControl.ItemCheck += (sender, e) => { this.OnItemCheck(e); };
            mCheckBoxComboBoxListControl.VisibleChanged += (sender, e) => { this.SetPopupSize(); };

            // 创建下拉框容器
            SparkPopupComboBoxListControlContainer container = new SparkPopupComboBoxListControlContainer
            {
                MaximumSize = mCheckBoxComboBoxListControl.MaximumSize,
                MinimumSize = mCheckBoxComboBoxListControl.MinimumSize,
                Size = mCheckBoxComboBoxListControl.Size
            };
            container.Controls.Add(mCheckBoxComboBoxListControl);

            // 设置 Popup 控件
            DropDownControl = container;
            PopupDropDown.Resizable = false;
            PopupDropDown.AutoClose = false;
            // 绑定事件
            //MouseClick += (sender, e) =>
            //{
            //    if (this.RectangleToScreen(ButtonRect).Contains(MousePosition))
            //    {
            //        this.IsDroppedDown = true;
            //    }
            //};
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 处理 Windows 消息。
        /// </summary>
        /// <param name="m">Windows 消息。</param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x7b)
            {
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 引发 Resize 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnResize(EventArgs e)
        {
            if (PopupDropDown != null)
            {
                PopupDropDown.Size = new Size(Width, DropDownControl == null ? 15 : DropDownControl.Height);
            }
            base.OnResize(e);
        }

        /// <summary>
        /// 引发 KeyDown 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// 引发 KeyPress 事件。
        /// </summary>
        /// <param name="e">事件参数。</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// 引发 CheckBoxCheckedChanged 事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="e">事件参数。</param>
        protected virtual void OnItemCheck(ItemCheckEventArgs e)
        {
            var item = mCheckBoxComboBoxListControl.Items[e.Index];
            string text = DisplayMember.IsNullOrEmpty() ? mCheckBoxComboBoxListControl.GetItemText(item) : SparkComboBoxDatasourceResolver.GetMemberValue(item, DisplayMember)?.ToString();

            List<string> texts = SelectedText.ToList();
            if (e.NewValue == CheckState.Checked)
            {
                if (!texts.Contains(text))
                {
                    texts.Add(text);
                }
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (texts.Contains(text))
                {
                    texts.Remove(text);
                }
            }
            Text = string.Join(",", texts.ToArray());
            ItemCheck?.Invoke(this, e);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 释放由 <see cref="SparkMultiselectComboBox"/> 所使用的资源。
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            mCheckBoxComboBoxListControl.Dispose();
        }

        /// <summary>
        /// 根据value值设置选择项
        /// </summary>
        /// <param name="values"></param>
        public void SetItemByValue(params object[] values)
        {
            this.SelectedValue = values;
        }

        /// <summary>
        /// 根据text值设置选择项
        /// </summary>
        public void SetItemByText(params string[] texts)
        {
            this.SelectedText = texts;
        }

        /// <summary>
        /// 根据text值设置选择项
        /// </summary>
        public void SetItemByIndex(params int[] indexs)
        {
            this.SelectedIndex = indexs;
        }

        /// <summary>
        /// 选中全部项
        /// </summary>
        public void SelectAllItem()
        {
            int[] arrs = new int[this.Items.Count];
            SetItemByIndex(arrs.Select((item, index) => index).ToArray());
        }
        #endregion

        #region 私有方法

        // 设置 Popup 大小。
        private void SetPopupSize()
        {
            PopupDropDown.Size = new Size(Width, mCheckBoxComboBoxListControl.Items.Cast<object>().Select((o, i) => i < MaxDropDownItems ? mCheckBoxComboBoxListControl.GetItemHeight(i) : 0).Sum(i => i) + 4);
        }

        #endregion

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