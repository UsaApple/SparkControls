using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Foundation;
using SparkControls.Win32;

namespace SparkControls.Controls
{
    /// <summary>
    /// 组合框控件。
    /// </summary>
    [ToolboxBitmap(typeof(ComboBox))]
    [ToolboxItem(true)]
    public class SparkComboBox : SparkComboBoxBase, IMessageFilter
    {
        /// <summary>
        /// 初始 <see cref="SparkComboBox"/> 类型的新实例。
        /// </summary>
        public SparkComboBox()
        {
            Application.AddMessageFilter(this);
            this.ValueMember = "Id";
            this.DisplayMember = "Name";
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否将回车键转化为 Tab 键。
        /// </summary>
        [Category("Spark"), Description("如果为 true 则回车按键触发 Tab 键。")]
        [DefaultValue(true)]
        public bool IsEnterToTab { get; set; } = true;

        /// <summary>
        /// 单选模式下，获取或设置当前选定项对应的 <see cref="BaseObject"/> 实例。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public virtual BaseObject SelectedObject
        {
            get
            {
                if (this.SelectedItem is BaseObject obj)
                {
                    return obj;
                }
                return null;
            }
            set => this.SelectedItem = value;
        }

        /// <summary>
        /// 添加项到集合。
        /// </summary>
        /// <typeparam name="T">项的类型。</typeparam>
        /// <param name="item">要添加到集合的项。</param>
        public virtual void AddItem<T>(T item) where T : BaseObject
        {
            if (item == null) { throw new ArgumentNullException(nameof(item), "值不能为空。"); }
            this.Items.Add(item);
        }

        /// <summary>
        /// 添加项到集合。
        /// </summary>
        /// <typeparam name="T">项的类型。</typeparam>
        /// <param name="items">要添加到集合的项。</param>
        public virtual void AddItems<T>(List<T> items) where T : BaseObject
        {
            if (items == null) { throw new ArgumentNullException(nameof(items), "值不能为空。"); }
            this.Items.AddRange(items.ToArray());
        }

        /// <summary>
        /// 添加项到集合。
        /// </summary>
        /// <param name="items">要添加到集合的项。</param>
        public virtual void AddItems(ArrayList items)
        {
            if (items == null) { throw new ArgumentNullException(nameof(items), "值不能为空。"); }
            this.Items.AddRange(items.ToArray());
        }

        /// <summary>
        /// 从集合中移除指定的项。
        /// </summary>
        /// <typeparam name="T">项的类型。</typeparam>
        /// <param name="index">插入项从零开始的索引位置。</param>
        /// <param name="item">要插入的项。</param>
        public virtual void InsertItem<T>(int index, T item) where T : BaseObject
        {
            this.Items.Insert(index, item);
        }

        /// <summary>
        /// 从集合中移除所有的项。
        /// </summary>
        public virtual void ClearItem()
        {
            this.Items.Clear();
            ClearItemInit();
        }

        /// <summary>
        /// 从集合中移除指定的项。
        /// </summary>
        /// <typeparam name="T">项的类型。</typeparam>
        /// <param name="item">要移除的项。</param>
        public virtual void RemoveItem<T>(T item) where T : BaseObject
        {
            if (this.Items.Contains(item))
            {
                this.Items.Remove(item);
            }
        }

        /// <summary>
        /// 获取集合中项的列表。
        /// </summary>
        /// <typeparam name="T">项的类型。</typeparam>
        /// <returns>下拉项的列表。</returns>
        public virtual List<T> GetItems<T>() where T : BaseObject
        {
            List<T> items = new List<T>();
            foreach (var item in this.Items)
            {
                if (item is T t)
                {
                    items.Add(t);
                }
            }
            return items;
        }

        /// <summary>
        /// 在调度消息之前将其筛选出来。
        /// </summary>
        /// <param name="m">要调度的消息。</param>
        /// <returns>如果筛选消息并禁止消息被调度，则为 true；如果允许消息继续到达下一个筛选器或控件，则为 false。</returns>
        public virtual bool PreFilterMessage(ref Message m)
        {
            if (PopupDropDown.Visible && !PopupDropDown.AutoClose)
            {
                switch (m.Msg)
                {
                    case (int)Msgs.WM_LBUTTONDOWN:
                    case (int)Msgs.WM_RBUTTONDOWN:
                    case (int)Msgs.WM_MBUTTONDOWN:
                    case (int)Msgs.WM_NCLBUTTONDOWN:
                    case (int)Msgs.WM_NCRBUTTONDOWN:
                    case (int)Msgs.WM_NCMBUTTONDOWN:
                        //int i = unchecked((int)(long)m.LParam);
                        //short x = (short)(i & 0xFFFF);
                        //short y = (short)((i >> 16) & 0xffff);
                        var pt = PopupDropDown.PointToClient(Cursor.Position);

                        /*
                        // 把相对于一个窗口的坐标空间的一组点映射成相对于另一窗口的坐标空 的一组点。
                        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
                        [ResourceExposure(ResourceScope.None)]
                        private static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref Point pt, int cPoints);

                        //根据坐标获取窗口句柄
                        [DllImport("user32.dll")]
                        private static extern IntPtr WindowFromPoint(Point Point);
                        */
                        // 点击 Popup 不做处理
                        //var handle = WindowFromPoint(pt);
                        //if (handle == PopupDropDown.Handle || handle == PopupDropDown.Content.Handle)
                        //{
                        //    return false;
                        //}
                        // 转换坐标
                        //MapWindowPoints(m.HWnd, PopupDropDown.Handle, ref pt, 1);

                        // 用户点击 Popup 外部
                        if (!PopupDropDown.ClientRectangle.Contains(pt))
                        {
                            // 确定鼠标点击坐标是否在下拉控件中，如果不在则关闭下拉控件。
                            if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
                            {
                                this.IsDroppedDown = false;
                            }
                        }
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// 重写KeyDown事件，回车健实现Tab健功能，切换焦点
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                if (e.KeyCode == (Keys.Enter | Keys.Shift))
                {
                    SendKeys.SendWait("+{Tab}");
                }
                else if (e.KeyCode == Keys.Enter && IsEnterToTab)
                {
                    SendKeys.SendWait("{Tab}");
                }
            }
        }

        /// <summary>
        /// 处理命令键
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape || keyData == Keys.Tab)
            {
                this.IsDroppedDown = false;
            }
            else if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                if (keyData == Keys.Down)
                {
                    var count = GetItemsCount();
                    if (count > 0)
                    {
                        int index = this.SelectedIndex + 1;
                        if (index >= 0 && index < count)
                        {
                            this.SelectedIndex = index;
                        }
                    }
                    return true;
                }
                else if (keyData == Keys.Up)
                {
                    var count = GetItemsCount();
                    if (count > 0)
                    {
                        int index = this.SelectedIndex - 1;
                        if (index >= 0 && index < count)
                        {
                            this.SelectedIndex = index;
                        }
                    }
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// 释放由 <see cref="SparkDataGridComboBox"/> 所使用的资源。
        /// </summary>
        /// <param name="disposing">如果应该释放托管资源为 true，否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            Application.RemoveMessageFilter(this);
            PopupDropDown?.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 获取选项个数
        /// </summary>
        /// <returns></returns>
        private int GetItemsCount()
        {
            if (this.DataSource != null)
            {
                try
                {
                    return SparkComboBoxDatasourceResolver.GetItemCount(this.DataSource);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else if (this.Items != null)
            {
                return this.Items.Count;
            }
            return 0;
        }
    }
}