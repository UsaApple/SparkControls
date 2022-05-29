using System;

namespace SparkControls.Controls
{
    /// <summary>
    /// 数据项参数
    /// </summary>
    public sealed class DataItemEventArgs : EventArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataType"></param>
        public DataItemEventArgs(string dataType)
        {
            DataType = dataType;
        }

        /// <summary>
        /// 数据项的类型
        /// </summary>
        public string DataType { get; private set; }
    }
}