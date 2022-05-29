using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SparkControls.Controls
{
    public interface IDrawStyle
    {
        /// <summary>
        /// 获取控件的矩形范围。
        /// </summary>
        Rectangle Rectangle { get;  }
        Padding Margin { get; set; }

        bool Visible { get; set; }

        Size Size { get; set; }

        Point Location { get; set; }

        void Paint(Graphics g);

        void Init(SparkCardControl trtCardControl, DataRowView dataRowView, IDrawStyle parent);

        IEnumerable<IDrawStyle> Children { get; }

        IDrawStyle Parent { get; }
    }
}
