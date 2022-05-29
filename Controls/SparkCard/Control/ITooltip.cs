using SparkControls.Controls;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表示可以显示提示信息。
    /// </summary>
    public interface ITooltip
    {
        /// <summary>
        /// 获取或设置用于计算提示信息的表达式。
        /// </summary>
        StringExpression TooltipExpression { get; set; }
    }

    public interface ITooltipString
    {
        /// <summary>
        /// 获取或设置用于计算提示信息
        /// </summary>
        string Tooltip { get; set; }
    }
}