using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    /// <summary>
    /// 自定义下拉框的组合框。
    /// </summary>
    [ToolboxItem(false)]
    [Description("Displays an editable text box with a drop-down list of permitted values.")]
    public abstract partial class SparkPopupComboBox : SparkComboBox
    {
        #region 构造函数

        /// <summary>
        /// 初始类型 <see cref="SparkPopupComboBox"/> 的新实例。
        /// </summary>
        public SparkPopupComboBox()
        {
            InitializeComponent();

            this.DropDownStyle = ComboBoxStyle.DropDown;
            this.DropDownWidth = 0;
            this.DropDownHeight = 0;
            this.IntegralHeight = false;

        }

        /// <summary>
        /// 初始类型 <see cref="SparkPopupComboBox"/> 的新实例。
        /// </summary>
        /// <param name="container">组件容器。</param>
        public SparkPopupComboBox(IContainer container) : this()
        {
            container.Add(this);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取组合框下拉部分的宽度。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownWidth
        {
            get { return base.DropDownWidth; }
            private set { base.DropDownWidth = value; }
        }

        /// <summary>
        /// 获取组合框下拉部分的高度。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownHeight
        {
            get { return base.DropDownHeight; }
            private set { base.DropDownHeight = value; }
        }

        /// <summary>
        /// 获取一个值，该值指示空间是否应调整大小以避免只显示项的局部。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IntegralHeight
        {
            get { return base.IntegralHeight; }
            private set { base.IntegralHeight = value; }
        }

        /// <summary>
        /// 获取组合框的样式。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public new ComboBoxStyle DropDownStyle
        {
            get { return base.DropDownStyle; }
            private set { base.DropDownStyle = value; }
        }

        private Control mDropDownControl;
        /// <summary>
        /// 获取或设置组合框绑定到的下拉控件。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        public Control DropDownControl
        {
            get
            {
                return mDropDownControl;
            }
            set
            {
                if (mDropDownControl != value)
                {
                    mDropDownControl = value;
                    PopupDropDown = new Popup(value);
                }
            }
        }

        #endregion

        #region 重写方法

        /// <summary>
        /// 处理 Windows 消息。
        /// </summary>
        /// <param name="m">Windows 消息。</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Popup.NativeMethods.WM_REFLECT + Popup.NativeMethods.WM_COMMAND && Popup.NativeMethods.HIWORD(m.WParam) == Popup.NativeMethods.CBN_DROPDOWN)
            {
                TimeSpan TimeSpan = DateTime.Now.Subtract(PopupDropDown.LastClosedTimeStamp);
                if (TimeSpan.TotalMilliseconds > 500) this.IsDroppedDown = true;
                return;
            }
            base.WndProc(ref m);
        }

        #endregion
    }
}