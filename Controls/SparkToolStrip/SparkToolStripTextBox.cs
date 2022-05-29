using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls
{
	/// <summary>
	/// 工具栏文本框控件。
	/// </summary>
	[DesignerCategory("code")]
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip |
        ToolStripItemDesignerAvailability.MenuStrip |
        ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class SparkToolStripTextBox : ToolStripControlHost
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkToolStripTextBox() : base(CreateCtrlInstance())
        {
        }

        /// <summary>
        /// 有参构造方法
        /// </summary>
        /// <param name="name"></param>
        public SparkToolStripTextBox(string name) : this()
        {
            Name = name;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 承载指定的控件
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Spark"), Description("自定义文本框设置")]
        public SparkTextBox TextBox
        {
            get { return Control as SparkTextBox; }
        }

        /// <summary>
        /// 默认大小
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 23);
            }
        }

        /// <summary>
        /// 默认边距
        /// </summary>
        protected override Padding DefaultMargin
        {
            get
            {
                return IsOnDropDown ? new Padding(1) : new Padding(1, 0, 1, 0);
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 检索适合控件的矩形区域的大小
        /// </summary>
        /// <param name="constrainingSize"></param>
        /// <returns></returns>
        public override Size GetPreferredSize(Size constrainingSize)
        {
            return TextBox.Size;
        }

        /// <summary>
        /// 创建 <see cref="SparkTextBox"/>。
        /// </summary>
        /// <returns></returns>
        private static Control CreateCtrlInstance()
        {
            SparkTextBox textBox = new SparkTextBox
            {
                Text = string.Empty
            };
            return textBox;
        }
        #endregion
    }
}