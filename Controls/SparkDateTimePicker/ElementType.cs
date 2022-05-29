using System.ComponentModel;

namespace SparkControls.Controls
{
    /// <summary>
    /// 元素类型
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        None = 0,
        /// <summary>
        /// 年
        /// </summary>
        [Description("年")]
        Year,
        /// <summary>
        /// 月
        /// </summary>
        [Description("月")]
        Month,
        /// <summary>
        /// 日
        /// </summary>
        [Description("日")]
        Day,
        /// <summary>
        /// 时
        /// </summary>
        [Description("时")]
        Hour,
        /// <summary>
        /// 分
        /// </summary>
        [Description("分")]
        Minute,
        /// <summary>
        /// 秒
        /// </summary>
        [Description("秒")]
        Second
    }
}