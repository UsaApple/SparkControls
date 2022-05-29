using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 选项卡窗口控件。
	/// </summary>
	[ToolboxBitmap(typeof(TabControl))]
    public class SparkTabFormControl : SparkTabControl
    {
        #region 属性
        /// <summary>
        /// 获取或设置一个值，该值指示选项卡在其中对齐的控件区域。
        /// </summary>
        [Browsable(false)]
        [DefaultValue(TabAlignment.Top)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override TabAlignment Alignment
		{
			get => TabAlignment.Top;
			set => base.Alignment = TabAlignment.Top;
		}

		/// <summary>
		/// 获取或设置一个值，该值指示是否将控件的元素对齐以支持使用从右向左的字体的区域设置。
		/// </summary>
		[Browsable(false)]
        [DefaultValue(RightToLeft.No)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override RightToLeft RightToLeft
        {
            get => RightToLeft.No;
            set => base.RightToLeft = RightToLeft.No;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否绘制标题栏。
        /// </summary>
        [Browsable(false)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool IsDrawTitle
        {
            get => true;
            set => this.TitleBarDraw.IsDrawTitle = true;
        }

        /// <summary>
        ///  获取或设置一个值，该值指示是否在窗体的标题栏中显示“最小化”按钮。
        /// </summary>
        [DefaultValue(true)]
        [Category("Spark"), Description("是否显示最小化按钮。")]
        public override bool MinimizeBox
        {
            get => this.TitleBarDraw.MinimizeBox;
            set => this.TitleBarDraw.MinimizeBox = value;
        }

        /// <summary>
        /// 获取或设置一个值，该值指示是否在窗体的标题栏中显示“最大化”按钮。
        /// </summary>
        [DefaultValue(true)]
        [Category("Spark"), Description("是否显示最大化按钮。")]
        public override bool MaximizeBox
        {
            get => this.TitleBarDraw.MaximizeBox;
            set => this.TitleBarDraw.MaximizeBox = value;
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkTabFormControl()
        {
            Alignment = TabAlignment.Top;
            IsDrawTitle = true;
            this.TitleBarDraw.IsMouseMove = true;
        }
        #endregion
    }
}