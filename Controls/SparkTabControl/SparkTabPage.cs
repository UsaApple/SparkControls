using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SparkControls.Controls.Design;

namespace SparkControls.Controls
{
    /// <summary>
    /// TabPage
    /// </summary>
    [ToolboxItem(false)]
    [DefaultProperty("Text")]
    [DefaultEvent("TextChanged")]
    [Designer(typeof(SparkTabPageDesigner))]
    public class SparkTabPage : SparkPanel
    {
        #region 事件

        /// <summary>
        /// Text改变事件
        /// </summary>
        public new event TabPageTextChangedEventHandler TextChanged;

        #endregion

        #region 变量

        private readonly SparkTabControl owner = null;
        private readonly Image image = null;

        private RectangleF stripRect = Rectangle.Empty;
        private bool canClose = true;
        private bool selected = false;
        private bool visible = true;
        private bool isDrawn = false;
        private string text = string.Empty;

        #endregion

        #region 属性

        /// <summary>
        /// 大小
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        [DefaultValue(true)]
        public new bool Visible
        {
            get { return this.visible; }
            set
            {
                if (this.visible == value)
                    return;
                this.visible = value;
            }
        }

        /// <summary>
        /// 大小
        /// </summary>
        [Browsable(false)]
        internal RectangleF StripRect
        {
            get { return this.stripRect; }
            set { this.stripRect = value; }
        }

        /// <summary>
        /// 关闭按钮的大小
        /// </summary>
        [Browsable(false)]
        internal Rectangle CloseButtonRect
        {
            get
            {
                return new Rectangle((int)this.stripRect.Right - 5 - 16,
                                      (int)this.stripRect.Y + (int)(this.stripRect.Height - 16) / 2,
                                      16,
                                      16);
            }
        }

        /// <summary>
        /// 是否绘制
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        internal bool IsDrawn
        {
            get { return this.isDrawn; }
            set
            {
                if (this.isDrawn == value)
                    return;
                this.isDrawn = value;
            }
        }

        /// <summary>
        /// 标题图标
        /// </summary>
        [Localizable(true)]
        [DefaultValue(null)]
        public Image Image
        {
            get; set;
        } = null;


        /// <summary>
        /// 选中选项卡的图标
        /// </summary>
        [Localizable(true)]
        [DefaultValue(null)]
        public Image SelectedImage
        {
            get; set;
        } = null;

        /// <summary>
        /// 是否绘制关闭按钮
        /// </summary>
        [DefaultValue(true)]
        public bool IsDrawClose
        {
            get { return this.canClose; }
            set { this.canClose = value; }
        }

        /// <summary>
        /// 标题文本
        /// </summary>
        [Browsable(true)]
        public override string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                if (this.text == value)
                    return;
                string oldText = this.text;
                this.text = value;
                this.OnTextChanged(oldText, this.text);
            }
        }

        /// <summary>
        /// 是否选择
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        public bool Selected
        {
            get => this.selected;
            set
            {
                if (this.selected == value)
                    return;

                this.selected = value;
            }
        }

        /// <summary>
        ///  获取或设置哪些控件边框停靠到其父控件并确定控件如何随其父级一起调整大小。
        /// </summary>
        [Browsable(false)]
        public new DockStyle Dock
        {
            get
            {
                return DockStyle.Fill;
            }
        }

        /// <summary>
        /// 是否固定选项卡，true固定，false不固定，默认为false
        /// </summary>
        [DefaultValue(false)]
        [Browsable(true)]
        public bool IsFixed { get; set; } = false;

        #endregion

        #region 构造

        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkTabPage() : this(string.Empty)
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="text"></param>
        public SparkTabPage(string text)
        {
            this.SetStyle(ControlStyles.ResizeRedraw |          //调整大小时重绘
                          ControlStyles.DoubleBuffer |          //双缓冲
                          ControlStyles.OptimizedDoubleBuffer | //双缓冲
                          ControlStyles.AllPaintingInWmPaint |  //禁止擦除背景
                          ControlStyles.UserPaint |
                          ControlStyles.ContainerControl, true
            );

            this.Selected = false;
            this.Visible = true;
            this.Text = text;
            base.Dock = DockStyle.Fill;
            this.BorderStyle = BorderStyle.None;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Handles proper disposition of the tab page control.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this.image != null)
                    this.image.Dispose();
            }
        }

        #endregion

        #region ShouldSerialize

        public bool ShouldSerializeIsDrawn()
        {
            return false;
        }

        public bool ShouldSerializeDock()
        {
            return false;
        }

        public bool ShouldSerializeControls()
        {
            return this.Controls != null && this.Controls.Count > 0;
        }

        public bool ShouldSerializeVisible()
        {
            return true;
        }

        #endregion

        #region 私有方法

        private void UpdateText(string caption, Control displayControl)
        {
            //if (displayControl != null && displayControl is ICaptionSupport)
            //{
            //    ICaptionSupport capControl = displayControl as ICaptionSupport;
            //    Text = capControl.Caption;
            //}
            //else if (caption.Length <= 0 && displayControl != null)
            //{
            //    Text = displayControl.Text;
            //}
            //else if (caption != null)
            //{
            //    Text = caption;
            //}
            //else
            //{
            //    Text = string.Empty;
            //}
        }

        #endregion

        public void Assign(SparkTabPage item)
        {
            this.Visible = item.Visible;
            this.Text = item.Text;
            this.IsDrawClose = item.IsDrawClose;
            this.Tag = item.Tag;
        }

        /// <summary>
        /// Text修改事件
        /// </summary>
        protected internal virtual void OnTextChanged(string oldText, string newText)
        {
            TextChanged?.Invoke(this, new SparkTabPageTextChangedEventArgs(oldText, newText));
        }

        /// <summary>
        /// Return a string representation of page.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Text;
        }
    }
}