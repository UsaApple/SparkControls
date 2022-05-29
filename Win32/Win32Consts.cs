using System;

namespace SparkControls.Win32
{
    /// <summary>
    /// Win32 常量。
    /// </summary>
    public class Win32Consts
    {
        /// <summary>
        /// 表示非托管的真值。
        /// </summary>
        public static readonly IntPtr TRUE = new IntPtr(1);

        /// <summary>
        /// 表示非托管的假值。
        /// </summary>
        public static readonly IntPtr FALSE = IntPtr.Zero;

        /// <summary>
        /// The wm activate
        /// </summary>
        public const int WM_ACTIVATE = 0x006;

        /// <summary>
        /// 发此消息给应用程序哪个窗口是激活的，哪个是非激活的 
        /// </summary>
        public const int WM_ACTIVATEAPP = 0x01C;

        /// <summary>
        /// The wm ncactivate
        /// </summary>
        public const int WM_NCACTIVATE = 0x086;

        /// <summary>
        /// The wa inactive
        /// </summary>
        public const int WA_INACTIVE = 0;

        /// <summary>
        /// The wm mouseactivate
        /// </summary>
        public const int WM_MOUSEACTIVATE = 0x21;

        /// <summary>
        /// The ma noactivate
        /// </summary>
        public const int MA_NOACTIVATE = 3;

        /// <summary>
        /// 还原  
        /// </summary>
        public const int SC_RESTORE = 0xF120;
        /// <summary>
        /// 移动
        /// </summary>
        public const int SC_MOVE = 0xF010;
        /// <summary>
        /// 大小
        /// </summary>
        public const int SC_SIZE = 0xF000;
        /// <summary>
        /// 最小化
        /// </summary>
        public const int SC_MINIMIZE = 0xF020;
        /// <summary>
        /// 最大化
        /// </summary>
        public const int SC_MAXIMIZE = 0xF030;
        /// <summary>
        /// 最大化2
        /// </summary>
        public const int SC_MAXIMIZE2 = 0xF032;
        /// <summary>
        /// 关闭
        /// </summary>
        public const int SC_CLOSE = 0xF060;

        public const int PRF_CLIENT = 0x00000004;

        public const int TV_FIRST = 0x1100;
        public const int TVM_SETBKCOLOR = TV_FIRST + 29;
        public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        public const int TVS_EX_DOUBLEBUFFER = 0x0004;

    }
}