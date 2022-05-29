using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace SparkControls.Controls
{
    /// <summary>
    /// 工具栏,按钮所在的容器
    /// </summary>
    [ToolboxItem(false)]
    public class SparkNavigateToolStrip : SparkToolStrip
    {
        #region 字段
        private static readonly object EventSelectedTabChanged = new object();
        private SparkNavigationBarStripButton _selectedTab;
        private int _selectedIndex;
        private bool _clearingTabs;
        private int _tabCount = 0;
        private readonly SparkNavigationBarStripScrollButton _scrollLeft =
           new SparkNavigationBarStripScrollButton(TabStripScrollDirection.Left);

        private readonly SparkNavigationBarStripScrollButton _scrollRight =
            new SparkNavigationBarStripScrollButton(TabStripScrollDirection.Right);

        private SparkTabStripLayoutEngine _layout;
        #endregion

        #region 事件
        public event EventHandler SelectedTabChanged
        {
            add { this.Events.AddHandler(EventSelectedTabChanged, value); }
            remove { this.Events.RemoveHandler(EventSelectedTabChanged, value); }
        }
        #endregion

        #region 属性
        [DefaultValue(null)]
        public SparkNavigationBarStripButton SelectedTab
        {
            get { return this._selectedTab; }
            set { this.SetSelectedTab(value, false); }
        }

        [Browsable(false)]
        [DefaultValue(-1)]
        public int SelectedTabIndex
        {
            get { return this._selectedIndex; }
            set
            {
                if (value == -1)
                {
                    this.SelectedTab = null;
                }
                else
                {
                    int index = 0;

                    foreach (SparkNavigationBarStripButton item in this.ItemsOfType<SparkNavigationBarStripButton>())
                    {
                        if (index == value)
                        {
                            this.SelectedTab = item;
                            return;
                        }

                        ++index;
                    }

                    //throw new ArgumentOutOfRangeException(
                    //    string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    //    "SelectedTabIndex的值超出索引范围", value));
                }
            }
        }

        internal int TabCount
        {
            get { return this._tabCount; }
            private set
            {
                if (value < 0)
                {
                    throw new InvalidOperationException();
                }

                this._tabCount = value;
            }
        }

        internal SparkNavigationBarStripScrollButton ScrollNearButton
        {
            get { return this._scrollLeft; }
        }

        internal SparkNavigationBarStripScrollButton ScrollFarButton
        {
            get { return this._scrollRight; }
        }


        [Browsable(false)]
        public override LayoutEngine LayoutEngine
        {
            get
            {
                return base.LayoutEngine;
                if (this._layout == null)
                {
                    this._layout = new SparkTabStripLayoutEngine(this);
                }

                return this._layout;
            }
        }
        #endregion

        public SparkNavigateToolStrip() : base()
        {
            this.Renderer = new SparkNavigateBarToolStripRenderer(this.Theme);
        }

        #region internal 方法

        internal IEnumerable<T> ItemsOfType<T>() where T : ToolStripItem
        {
            foreach (ToolStripItem item in this.Items)
            {
                T t = (item as T);

                if (t != null)
                {
                    yield return t;
                }
            }
        }

        internal void RemoveAllTabs()
        {
            try
            {
                this._clearingTabs = true;
                this.Items.Clear();
            }
            finally
            {
                this._clearingTabs = false;
                this._tabCount = 0;
            }
            this.Refresh();
        }

        protected internal virtual void OnRefreshItemsByAlignment()
        {
            switch (this.Dock)
            {
                case DockStyle.Top:
                case DockStyle.Bottom:
                    this.SetItemTextImageRelation(TextImageRelation.ImageBeforeText);
                    break;
                case DockStyle.Left:
                case DockStyle.Right:
                    this.SetItemTextImageRelation(TextImageRelation.ImageAboveText);
                    break;
            }
        }

        protected internal virtual void OnRefreshItemsByDisplay(ToolStripItemDisplayStyle displayStyle)
        {
            this.SetItemDisplay(displayStyle);
        }

        internal void CheckItem(bool isCheck)
        {
            if (this.SelectedTab != null)
            {
                this.SelectedTab.Checked = isCheck;
            }
        }

        public void LayoutItem(ToolStripItem item, Point location, Size size)
        {
            this.SetItemLocation(item, location);
            item.Size = size;
        }
        #endregion

        #region 重写protected
        protected override Padding DefaultPadding
        {
            get { return new Padding(0, 0, 0, 0); } //0, 2, 2, 4
        }

        protected override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick)
        {
            return new SparkNavigationBarStripButton(text, image, onClick);
        }

        protected override void OnItemAdded(ToolStripItemEventArgs e)
        {
            if (this._clearingTabs)
            {
                base.OnItemAdded(e);
                return;
            }


            if (e.Item is SparkNavigationBarStripButton)
            {
                ++this.TabCount;
                if (this._layout != null) this._layout.ScrollDirection = TabStripScrollDirection.Right;
            }

            base.OnItemAdded(e);

            this.Refresh();
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (this._layout != null)
            {
                if (e.ClickedItem == this._scrollLeft)
                {
                    this._layout.ScrollDirection = TabStripScrollDirection.Left;
                    //this.PerformLayout();
                    this.Refresh();
                    return;
                }

                if (e.ClickedItem == this._scrollRight)
                {
                    this._layout.ScrollDirection = TabStripScrollDirection.Right;
                    //this.PerformLayout();
                    this.Refresh();
                    return;
                }
            }
            base.OnItemClicked(e);
        }

        protected override void OnItemRemoved(ToolStripItemEventArgs e)
        {
            if (this._clearingTabs)
            {
                base.OnItemAdded(e);
                return;
            }

            if (e.Item is SparkNavigationBarStripButton)
            {
                --this.TabCount;
            }

            if ((e.Item == this.SelectedTab))
            {
                int newIndex = this.SelectedTabIndex;

                if (newIndex >= this.TabCount)
                {
                    newIndex = this.TabCount - 1;
                }

                this.SelectedTabIndex = newIndex;
            }

            base.OnItemRemoved(e);
        }


        #endregion

        #region protected 虚方法
        protected virtual void OnSelectedTabChanged(EventArgs e)
        {
            EventHandler handler = this.Events[EventSelectedTabChanged] as EventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region 私有方法

        private void CheckItem(SparkNavigationBarStripButton newItem, SparkNavigationBarStripButton oldItem)
        {
            if (oldItem != null)
            {
                oldItem.Checked = false;
            }
            if (newItem != null)
            {
                newItem.Checked = true;
            }
        }

        private void SetSelectedTab(SparkNavigationBarStripButton value, bool force)
        {
            if (force || (this._selectedTab != value))
            {
                if (!force)
                {
                    //var e = new CancelEventArgs();
                    //OnSelectedTabChanging(e);
                    //if (e.Cancel)
                    //{
                    //    return;
                    //}
                }
                //if (this._selectedTab != null) this._selectedTab.ForeColor = Color.White;
                this._selectedIndex = -1;
                this.CheckItem(value, this._selectedTab);
                this._selectedTab = value;
                //if (value != null) value.ForeColor = Color.Black;
                int index = 0;

                foreach (SparkNavigationBarStripButton tab in this.ItemsOfType<SparkNavigationBarStripButton>())
                {
                    if (tab == this._selectedTab)
                    {
                        this._selectedIndex = index;
                    }

                    tab.Checked = (tab == this._selectedTab);
                    ++index;
                }

                //this.PerformLayout();
                this.Refresh();
                this.OnSelectedTabChanged(EventArgs.Empty);
            }
        }


        private void SetItemTextImageRelation(TextImageRelation relation)
        {
            foreach (SparkNavigationBarStripButton tab in this.ItemsOfType<SparkNavigationBarStripButton>())
            {
                tab.TextImageRelation = relation;
            }
        }

        private void SetItemDisplay(ToolStripItemDisplayStyle displayStyle)
        {
            foreach (SparkNavigationBarStripButton tab in this.ItemsOfType<SparkNavigationBarStripButton>())
            {
                tab.DisplayStyle = displayStyle;
            }
        }
        #endregion
    }
}