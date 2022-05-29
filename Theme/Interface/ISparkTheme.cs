using SparkControls.Controls;

namespace SparkControls.Theme
{
    /// <summary>
    /// 主题接口，每个控件都需要继承此接口
    /// </summary>
    public interface ISparkTheme<out T> where T : SparkThemeBase
    {
        /// <summary>
        /// 主题
        /// </summary>
        T Theme { get; }
	}
}