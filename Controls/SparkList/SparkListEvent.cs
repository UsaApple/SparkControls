using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 节点点击事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SparkListMouseClickEventHandler(object sender, SparkListMouseClickEventArgs e);

    /// <summary>
    ///  提供有关 ItemMouseClick
    /// </summary>
    public class SparkListMouseClickEventArgs : MouseEventArgs
    {
        /// <summary>
        /// 点击的TreeNodeEx节点
        /// </summary>
        public SparkListItem Item { get; }

        /// <summary>
        /// <see cref="SparkListMouseClickEventArgs"/>的新实例
        /// </summary>
        /// <param name="item"></param>
        /// <param name="button"></param>
        /// <param name="clicks"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="delta"></param>
        public SparkListMouseClickEventArgs(SparkListItem item, MouseButtons button, int clicks, int x, int y, int delta) : base(button, clicks, x, y, delta)
        {
            Item = item;
        }
    }

    /// <summary>
    /// 提供给BeforeSelect
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SparkListCancelEventHandler(object sender, SparkListCancelEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class SparkListCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        /// TreeViewExCancelEventArgs的新实例
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cancel"></param>
        public SparkListCancelEventArgs(SparkListItem item, bool cancel)
        {
            Item = item;
        }

        /// <summary>
        /// 选中的节点。
        /// </summary>
        public SparkListItem Item { get; }
    }

    /// <summary>
    /// 提供给AfterSelect
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void SparkListEventHandler(object sender, SparkListEventArgs e);

    /// <summary>
    /// AfterSelect事件的数据
    /// </summary>
    public class SparkListEventArgs : EventArgs
    {

        /// <summary>
        /// <see cref="SparkListEventArgs"/>的新实例
        /// </summary>
        /// <param name="item"></param>
        public SparkListEventArgs(SparkListItem item)
        {
            this.Item = item;
        }

        /// <summary>
        ///  选中的节点。
        /// </summary>
        public SparkListItem Item { get; }

    }


}
