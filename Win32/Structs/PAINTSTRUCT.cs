using System;
using System.Runtime.InteropServices;

namespace SparkControls.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Paint
	{
		public IntPtr hdc;
		public int fErase;
		public RECT rcPaint;
		public int fRestore;
		public int fIncUpdate;
		public int Reserved1;
		public int Reserved2;
		public int Reserved3;
		public int Reserved4;
		public int Reserved5;
		public int Reserved6;
		public int Reserved7;
		public int Reserved8;
	}
}