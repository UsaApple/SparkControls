using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SparkControls.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
    public struct TVHITTESTINFO
    {
        public POINTAPI pt;
        public int flags;
        public IntPtr hItem;
    }
}
