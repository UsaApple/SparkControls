using System.Collections.Generic;
using System.Drawing;

namespace SparkControls.Controls
{
	/// <summary>
	/// 时间日期元素
	/// </summary>
	public class DateTimeElement
    {
        #region 属性
        /// <summary>
        /// 元素内列数
        /// </summary>
        public int ColumnsCount { get; set; }

        /// <summary>
        /// 元素内行数
        /// </summary>
        public int RowsCount { get; set; }

        /// <summary>
        /// 元素单元大小
        /// </summary>
        public Size CellSize { get; set; }

        /// <summary>
        /// 日期时间单元元素集合
        /// </summary>
        public List<DateTimeCellElement> DtCellEles { get; } = new List<DateTimeCellElement>();
        #endregion

        #region 公共方法
        /// <summary>
        /// 绘制整个(一整块)元素
        /// </summary>
        /// <param name="g"></param>
        public void Paint(Graphics g)
        {
            if (DtCellEles == null || DtCellEles.Count == 0) return;

            foreach (var cell in DtCellEles)
            {
                if (!cell.Visible) continue;
                cell.Paint(g);
            }
        }
        #endregion
    }
}