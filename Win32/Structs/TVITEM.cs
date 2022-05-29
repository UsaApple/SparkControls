using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SparkControls.Win32
{
    // Use a sequential structure layout to define TVITEM for the TreeView.
    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
    public struct TVITEM
    {
        public uint mask;
        public IntPtr hItem;
        public uint state;
        public uint stateMask;
        public IntPtr pszText;
        public int cchTextMax;
        public int iImage;
        public int iSelectedImage;
        public int cChildren;
        public IntPtr lParam;
    }
}
