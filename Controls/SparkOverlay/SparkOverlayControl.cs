using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 遮罩层控件类。
	/// </summary>
	public partial class SparkOverlayControl : Control
	{
		// 是否使用透明
		private bool _transparentBG = true;
		// 控件透明度
		private int _alpha = 125;

		/// <summary>
		/// 获取创建控件句柄时所需要的创建参数
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x00000020; // 开启 WS_EX_TRANSPARENT，使控件支持透明
				return cp;
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值表示是否启用透明。
		/// </summary>
		[Category("Spark"), Description("获取或设置是否透启用明，true：开启；false：不开启，默认为 true。")]
		public bool TransparentBG
		{
			get => this._transparentBG;
			set
			{
				this._transparentBG = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// 获取或设置控件的透明度。
		/// </summary>
		[Category("Spark"), Description("获取或设置控件的透明度。")]
		public int Alpha
		{
			get => this._alpha;
			set
			{
				this._alpha = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// 创建 <see cref="SparkOverlayControl"/> 类型的新实例。
		/// </summary>
		public SparkOverlayControl() : this(125)
		{
		}

		/// <summary>
		/// 创建 <see cref="SparkOverlayControl"/> 类型的新实例。
		/// </summary>
		/// <param name="alpha">透明度。</param>
		public SparkOverlayControl(int alpha)
		{
			this.InitializeComponent();

			this.SetStyle(ControlStyles.Opaque, true);
			base.CreateControl();
			this._alpha = alpha;

			PictureBox picLoading = new PictureBox
			{
				Anchor = AnchorStyles.None,
				BackColor = Color.White,
				Image = Properties.Resources.loading,
				Name = "picLoading",
				Size = new Size(48, 48),
				SizeMode = PictureBoxSizeMode.AutoSize
			};
			picLoading.Location = new Point(this.Location.X + (this.Width - picLoading.Width) / 2, this.Location.Y + (this.Height - picLoading.Height) / 2);
			picLoading.Anchor = AnchorStyles.None;
			this.Controls.Add(picLoading);
		}

		/// <summary>
		/// 引发 Paint 事件。
		/// </summary>
		/// <param name="e">包含事件数据的 <see cref="PaintEventArgs"/>。</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			float controlWidth = this.Size.Width;
			float controlHeight = this.Size.Height;

			Pen borderPen;
			SolidBrush backColorBrush;
			if (this._transparentBG)
			{
				Color drawColor = Color.FromArgb(this._alpha, this.BackColor);
				borderPen = new Pen(drawColor, 0);
				backColorBrush = new SolidBrush(drawColor);
			}
			else
			{
				borderPen = new Pen(this.BackColor, 0);
				backColorBrush = new SolidBrush(this.BackColor);
			}

			e.Graphics.DrawRectangle(borderPen, 0, 0, controlWidth, controlHeight);
			e.Graphics.FillRectangle(backColorBrush, 0, 0, controlWidth, controlHeight);
		}
	}
}