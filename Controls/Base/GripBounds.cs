using System.Drawing;

namespace System.Windows.Forms
{
	/// <summary>
	/// 表示手柄范围。
	/// </summary>
	public struct GripBounds
	{
		private const int GripSize = 6;
		private const int CornerGripSize = GripSize << 1;

		/// <summary>
		/// 初始 <see cref="GripBounds"/> 类型的新实例。
		/// </summary>
		/// <param name="clientRectangle">对象范围。</param>
		public GripBounds(Rectangle clientRectangle)
		{
			this.ClientRectangle = clientRectangle;
		}

		/// <summary>
		/// 对象范围。
		/// </summary>
		public Rectangle ClientRectangle
		{
			get; private set;
		}

		/// <summary>
		/// 上
		/// </summary>
		public Rectangle Top
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.Height = GripSize;
				return rect;
			}
		}

		/// <summary>
		/// 右上
		/// </summary>
		public Rectangle TopRight
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		/// <summary>
		/// 右
		/// </summary>
		public Rectangle Right
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.X = rect.Right - GripSize + 1;
				rect.Width = GripSize;
				return rect;
			}
		}

		/// <summary>
		/// 右下
		/// </summary>
		public Rectangle BottomRight
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.Y = rect.Bottom - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				rect.X = rect.Width - CornerGripSize + 1;
				rect.Width = CornerGripSize;
				return rect;
			}
		}

		/// <summary>
		/// 下
		/// </summary>
		public Rectangle Bottom
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.Y = rect.Bottom - GripSize + 1;
				rect.Height = GripSize;
				return rect;
			}
		}

		/// <summary>
		/// 左下
		/// </summary>
		public Rectangle BottomLeft
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.Width = CornerGripSize;
				rect.Y = rect.Height - CornerGripSize + 1;
				rect.Height = CornerGripSize;
				return rect;
			}
		}

		/// <summary>
		/// 左
		/// </summary>
		public Rectangle Left
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.Width = GripSize;
				return rect;
			}
		}

		/// <summary>
		/// 左上
		/// </summary>
		public Rectangle TopLeft
		{
			get
			{
				Rectangle rect = ClientRectangle;
				rect.Width = CornerGripSize;
				rect.Height = CornerGripSize;
				return rect;
			}
		}
	}
}