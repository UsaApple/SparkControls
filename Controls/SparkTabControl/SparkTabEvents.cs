using System;
using System.ComponentModel;

namespace SparkControls.Controls
{
    /// <summary>
    /// Text改变事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void TabPageTextChangedEventHandler(object sender, SparkTabPageTextChangedEventArgs args);

    /// <summary>
    /// TabPage改变事件
    /// </summary>
    /// <param name="e"></param>
    public delegate void TabPageChangedEventHandler(SparkTabPageChangedEventArgs e);

    /// <summary>
    /// 在选项卡索引SelectedIndex属性更改后发生。
    /// </summary>
    /// <param name="e"></param>
    public delegate void TabPageSelectedIndexEventHandler(TabPageSelectedIndexEventArgs e);

    /// <summary>
    /// 选项卡选择前事件
    /// </summary>
    /// <param name="e"></param>
    public delegate void TabPageBeforeSelectEventHandler(TabPageBeforeSelectEventArgs e);
    /// <summary>
    /// TabPageSelectedIndexEventHandler事件的参数
    /// </summary>
    public class TabPageSelectedIndexEventArgs
    {
        public TabPageSelectedIndexEventArgs(SparkTabPage item, int index)
        {
            Item = item;
            Index = index;
        }

        /// <summary>
        /// 当前选项卡
        /// </summary>
        public SparkTabPage Item { get; }

        /// <summary>
        /// 当前选项卡的索引
        /// </summary>
        public int Index { get; }
    }

    /// <summary>
    /// TabPage关闭前事件
    /// </summary>
    /// <param name="e"></param>
    public delegate void TabPageClosingEventHandler(TabPageClosingEventArgs e);

    /// <summary>
    /// Text改变事件的参数
    /// </summary>
    public class SparkTabPageTextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        public SparkTabPageTextChangedEventArgs(object oldText, object newText)
        {
            this.OldText = oldText;
            this.NewText = newText;
        }
        /// <summary>
        /// 改变前的Text值
        /// </summary>
        public object OldText { get; }

        /// <summary>
        /// 改变后的Text值
        /// </summary>
        public object NewText { get; }
    }

    /// <summary>
    /// TabPage关闭前事件的参数
    /// </summary>
    public class TabPageClosingEventArgs : EventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="item"></param>
        public TabPageClosingEventArgs(SparkTabPage item)
        {
            this.Item = item;
        }

        /// <summary>
        /// 当前TabPage
        /// </summary>
        public SparkTabPage Item { get; set; }

        /// <summary>
        /// 是否取消关闭
        /// </summary>
        public bool Cancel { get; set; } = false;

    }

    /// <summary>
    /// TabPage改变事件的参数
    /// </summary>
    public class SparkTabPageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="item"></param>
        /// <param name="action"></param>
        public SparkTabPageChangedEventArgs(SparkTabPage item, TabPageChangeAction action)
        {
            this.Item = item;
            this.ChangeAction = action;
        }

        /// <summary>
        /// 当前的TabPage
        /// </summary>
        public SparkTabPage Item { get; }

        /// <summary>
        /// 改变的方式
        /// </summary>
        public TabPageChangeAction ChangeAction { get; }
    }

    /// <summary>
    /// TabPage选择前事件的参数
    /// </summary>
    public class TabPageBeforeSelectEventArgs : CancelEventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="item"></param>
        public TabPageBeforeSelectEventArgs(SparkTabPage item)
        {
            this.Item = item;
        }

        /// <summary>
        /// 当前TabPage
        /// </summary>
        public SparkTabPage Item { get; set; }
    }

    /// <summary>
    /// Represents the method that will handle the event that has no data.
    /// </summary>
    public delegate void CollectionClear();

    /// <summary>
    /// Represents the method that will handle the event that has item data.
    /// </summary>
    public delegate void CollectionChange(int index, object value);
}