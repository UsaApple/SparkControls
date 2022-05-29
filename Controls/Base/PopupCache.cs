using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	internal class PopupCache : IDisposable
    {
        private static List<ToolStripDropDown> list = new List<ToolStripDropDown>();
        
        public static void Add(ToolStripDropDown dropDown)
        {
            if (!list.Contains(dropDown))
            {
                list.Add(dropDown);
            }
        }

        public static void ClosePopup()
        {
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    try
                    {
                        if (!item.IsDisposed)
                        {
                            item.Hide();
                            item.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                list.Clear();
            }
        }

        public void Dispose()
        {
            list?.Clear();
            list = null;
        }
    }
}