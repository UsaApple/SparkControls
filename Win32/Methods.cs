using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SparkControls.Win32
{
	/// <summary>
	/// 系统 API 方法类。
	/// </summary>
	public static class NativeMethods
	{
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DragDetect(IntPtr hWnd, Point pt);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetFocus();

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern uint SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, ref Rectangle lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern UInt32 SendMessage(IntPtr hWnd, UInt32 Msg,
		  UInt32 wParam, UInt32 lParam);

		[DllImport("User32", CharSet = CharSet.Auto)]
		public static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg,
			UInt32 wParam, ref TVITEM lParam);

		[DllImport("User32", CharSet = CharSet.Auto)]
		public static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg,
			UInt32 wParam, ref TVHITTESTINFO lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, string lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern int ShowWindow(IntPtr hWnd, short cmdShow);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int X, int Y, int Width, int Height, FlagsSetWindowPos flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowLong(IntPtr hWnd, int Index);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SetWindowLong(IntPtr hWnd, int Index, int Value);


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		//*********************************
		// FxCop bug, suppress the message
		//*********************************
		[SuppressMessage("Microsoft.Portability", "CA1901:PInvokeDeclarationsShouldBePortable", MessageId = "0")]
		public static extern IntPtr WindowFromPoint(Point point);

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
		public static extern int GetCurrentThreadId();

		public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(Win32.HookType code, HookProc func, IntPtr hInstance, int threadID);

		[DllImport("user32.dll")]
		public static extern int UnhookWindowsHookEx(IntPtr hhook);

		[DllImport("user32.dll")]
		public static extern IntPtr CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// 获取窗体的工作区域
		/// </summary>
		[DllImport("user32.dll")]
		public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

		[DllImport("user32.dll")]
		public static extern IntPtr BeginPaint(IntPtr hWnd, ref Paint ps);

		[DllImport("user32.dll")]
		public static extern bool EndPaint(IntPtr hWnd, ref Paint ps);

		/// <summary>
		/// 获取ComboBox的控件信息
		/// </summary>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref ComboBoxInfo info);

		/// <summary>
		/// 获取指定窗口控件的系统信息
		/// </summary>
		/// <returns></returns>
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetWindowDC(IntPtr handle);

		/// <summary>
		/// 释放设备上下文环境
		/// </summary>
		/// <returns></returns>
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDC);

		/// <summary>
		/// 设置窗口激活
		/// </summary>
		/// <param name="handle"></param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern IntPtr SetActiveWindow(IntPtr handle);

		/// <summary>
		/// 获取前台窗口的句柄
		/// </summary>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("User32.dll", EntryPoint = "GetKeyState")]
		public static extern int GetKeyState(int nVirtKey);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetCursorPos(out POINT pt);

		[DllImport("user32.dll ")]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		public static extern uint GetClassLongPtr32(HandleRef hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		public static extern IntPtr GetClassLongPtr64(HandleRef hWnd, int nIndex);

		public static IntPtr GetClassLongPtr(HandleRef hWnd, int nIndex)
		{
			if (IntPtr.Size > 4)
				return GetClassLongPtr64(hWnd, nIndex);
			else
				return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
		}

		[DllImport("user32.dll", EntryPoint = "SetClassLong", CharSet = CharSet.Auto)]
		public static extern IntPtr SetClassLongPtr32(HandleRef hwnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetClassLongPtr", CharSet = CharSet.Auto)]
		public static extern IntPtr SetClassLongPtr64(HandleRef hwnd, int nIndex, IntPtr dwNewLong);

		public static IntPtr SetClassLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 4)
			{
				return SetClassLongPtr32(hWnd, nIndex, dwNewLong);
			}
			return SetClassLongPtr64(hWnd, nIndex, dwNewLong);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool IsWindow(HandleRef hWnd);
	}
}