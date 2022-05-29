using SparkControls.Controls;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表示可以设置可见状态。
    /// </summary>
    public interface IVisiblity
    {
        /// <summary>
        /// 获取或设置用于计算可见状态的表达式。
        /// </summary>
        BooleanExpression Visible { get; set; }
    }


    public interface IVisiblityBool
    {
        /// <summary>
        /// 获取或设置用于计算可见状态
        /// </summary>
        bool Visible { get; set; }
    }
}