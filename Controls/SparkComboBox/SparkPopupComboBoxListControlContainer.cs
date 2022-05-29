using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 组合框下拉列表控件容器。
    /// </summary>
    [ToolboxItem(false)]
    public class SparkPopupComboBoxListControlContainer : UserControl
    {
        /// <summary>
        /// 初始 <see cref="SparkPopupComboBoxListControlContainer" /> 类型的新实例。
        /// </summary>
        public SparkPopupComboBoxListControlContainer() : base()
        {
            AutoScroll = true;
            ResizeRedraw = true;
            BackColor = SystemColors.Window;
            BorderStyle = BorderStyle.FixedSingle;
            AutoScaleMode = AutoScaleMode.Inherit;
            MinimumSize = new Size(10, 10);
            MaximumSize = new Size(960, 720);
        }

        /// <summary>
        /// 处理 Windows 消息。
        /// </summary>
        /// <param name="m">Windows 消息。</param>
        protected override void WndProc(ref Message m)
        {
            if (Parent != null && (Parent as Popup).ProcessResizing(ref m))
            {
                return;
            }
            base.WndProc(ref m);
        }
    }
}