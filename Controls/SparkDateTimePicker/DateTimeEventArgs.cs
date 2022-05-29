using System;

namespace SparkControls.Controls
{
    /// <summary>
    /// 日期时间选择参数
    /// </summary>
    public sealed class DateTimeEventArgs : EventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dt"></param>
        public DateTimeEventArgs(DateTime dt)
        {
            SelectValue = dt;
        }

        /// <summary>
        /// 选择的日期时间
        /// </summary>
        public DateTime SelectValue { get; private set; }
    }
}