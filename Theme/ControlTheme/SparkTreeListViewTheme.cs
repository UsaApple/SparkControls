using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Theme
{
    /// <summary>
    /// 树列表控件的样式
    /// </summary>
    public class SparkTreeListViewTheme : SparkEditTheme
    {
        private Color nodeSplitLineColor = SparkThemeConsts.TreeViewNodeSplitLineColor;

        /// <summary>
        /// 初始 <see cref="SparkTreeViewTheme"/> 类型的新实例。
        /// </summary>
        /// <param name="control">应用主题的控件。</param>
        public SparkTreeListViewTheme(Control control) : base(control)
        {
            CheckBoxTheme = new SparkCombineCheckBoxTheme(control);
        }

        /// <summary>
        /// 节点分割线颜色
        /// </summary>
        [DefaultValue(typeof(Color), SparkThemeConsts.TreeViewNodeSplitLineColorString)]
        [Description("节点分割线颜色")]
        public virtual Color NodeSplitLineColor
        {
            get => nodeSplitLineColor;
            set
            {
                if (nodeSplitLineColor != value)
                {
                    var oldValue = nodeSplitLineColor;
                    nodeSplitLineColor = value;
                    OnPropertyChanged(nameof(NodeSplitLineColor), oldValue, nodeSplitLineColor);
                }
            }
        }

        /// <summary>
        /// 复选框的主题
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("复选框的主题")]
        public SparkCombineCheckBoxTheme CheckBoxTheme { get; private set; }
    }
}