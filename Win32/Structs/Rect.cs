using System.Drawing;
using System.Runtime.InteropServices;

namespace SparkControls.Win32
{
	/// <summary>
	/// 表示 Win32 中的矩形结构。
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public RECT(Rectangle rect)
		{
			this.left = rect.Left;
			this.right = rect.Right;
			this.top = rect.Top;
			this.bottom = rect.Bottom;
		}

		public Rectangle Rect
		{
			get
			{
				return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top);
			}
		}

		public static RECT FromXYWH(int x, int y, int width, int height)
		{
			return new RECT(x, y, x + width, y + height);
		}

		public static RECT FromRectangle(Rectangle rect)
		{
			return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
		}
	}
}
