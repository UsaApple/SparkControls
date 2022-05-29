using System;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 遮罩层命令类。
	/// </summary>
	public class SparkOverlayCommand
	{
		// 遮罩控件
		private readonly Control _control = null;
		// 蒙板控件
		private SparkOverlayControl _overlayControl = null;

		/// <summary>
		/// 创建 <see cref="SparkOverlayCommand"/> 类型的新实例。
		/// </summary>
		/// <param name="control">需要遮罩的控件。</param>
		public SparkOverlayCommand(Control control)
		{
			this._control = control ?? throw new ArgumentNullException(nameof(control), "遮罩对象不能为空！");
		}

		/// <summary>
		/// 显示遮罩层
		/// </summary>
		/// <param name="alpha">蒙板透明度，默认为125。</param>
		public void Show(int alpha = 125)
		{
			this.Show(SystemColors.Control, alpha);
		}

		/// <summary>
		/// 显示遮罩层
		/// </summary>
		/// <param name="backColor">蒙版背景色。</param>
		/// <param name="alpha">蒙板透明度，默认为125。</param>
		public void Show(Color backColor, int alpha = 125)
		{
			try
			{
				if (this._control.InvokeRequired)
				{
					this._control.Invoke(new Action(() => this.Show(backColor, alpha)));
				}
				else
				{
					if (this._overlayControl == null)
					{
						this._overlayControl = new SparkOverlayControl(alpha) { BackColor = backColor };
						this._control.Controls.Add(this._overlayControl);
						this._overlayControl.Dock = DockStyle.Fill;
						this._overlayControl.BringToFront();
					}
					this._overlayControl.Enabled = true;
					this._overlayControl.Visible = true;
				}
			}
			catch { }
		}

		/// <summary>
		/// 隐藏遮罩层
		/// </summary>
		public void Hide()
		{
			try
			{
				if (this._control.InvokeRequired)
				{
					this._control.Invoke(new Action(() => this.Hide()));
				}
				else
				{
					if (this._overlayControl != null)
					{
						this._overlayControl.Visible = false;
						this._overlayControl.Enabled = false;
					}
				}
			}
			catch { }
		}
	}
}