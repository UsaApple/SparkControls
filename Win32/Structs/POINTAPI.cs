using System.Drawing;
using System.Runtime.InteropServices;

namespace SparkControls.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
    public struct POINTAPI
    {
        public int x;
        public int y;
    }
}
