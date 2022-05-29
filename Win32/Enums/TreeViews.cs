using System;

namespace SparkControls.Win32
{
    public class TreeViews
    {
        public const UInt32 TV_FIRST = 4352;

        public const UInt32 TVSIL_NORMAL = 0;
        public const UInt32 TVSIL_STATE = 2;

        public const UInt32 TVM_SETIMAGELIST = TV_FIRST + 9;
        public const UInt32 TVM_GETNEXTITEM = TV_FIRST + 10;
        public const UInt32 TVM_GETITEM = TV_FIRST + 12;
        public const UInt32 TVM_SETITEM = TV_FIRST + 13;
        public const UInt32 TVM_HITTEST = TV_FIRST + 17;

        public const UInt32 TVIF_HANDLE = 16;
        public const UInt32 TVIF_STATE = 8;

        public const UInt32 TVIS_STATEIMAGEMASK = 61440;

        public const UInt32 TVGN_ROOT = 0;

        //TVHITTESTINFO.flags flags
        public const int TVHT_ONITEMSTATEICON = 64;
    }
}