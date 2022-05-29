using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 自定义按钮单击事件委托。
    /// </summary>
    /// <param name="sender">事件发送对象。</param>
    /// <param name="e">包含事件数据的 <see cref="TitleButtonClickEventArgs"/>。</param>
    public delegate void TitleButtonClickEventHandler(object sender, TitleButtonClickEventArgs e);

    /// <summary>
    /// 鼠标命中测试事件委托。
    /// </summary>
    /// <param name="sender">事件发送对象。</param>
    /// <param name="e">包含事件数据的 <see cref="MouseEventArgs"/>。</param>
    internal delegate bool TitleHitTestEventHandler(object sender, MouseEventArgs e);

    /// <summary>
    /// 自定义按钮单击事件参数类。
    /// </summary>
    public class TitleButtonClickEventArgs : EventArgs
    {
        public object Key { get; internal set; }
        public TitleItemAction TitleAction { get; internal set; }
        public SparkTitleBarItem Item { get; set; }
        public TitleButtonClickEventArgs()
        {
        }

        public TitleButtonClickEventArgs(object key, TitleItemAction titleAction, SparkTitleBarItem item)
        {
            Key = key;
            TitleAction = titleAction;
            Item = item;
        }
    }
}