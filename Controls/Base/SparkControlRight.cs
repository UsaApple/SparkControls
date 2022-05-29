namespace SparkControls.Controls
{
    /// <summary>
    /// 表示控件的权限信息
    /// </summary>
    public class SparkControlRight
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public string RightId { get; set; }

        /// <summary>
        /// 没有权限时的显示模式
        /// </summary>
        public NoRightMode RightType { get; set; } = NoRightMode.Show;
    }

    /// <summary>
    /// 表示没有权限时的显示模式
    /// </summary>
    public enum NoRightMode
    {
        /// <summary>
        /// 显示并可用
        /// </summary>
        Show = 0,
        /// <summary>
        /// 可见但禁用
        /// </summary>
        Disable = 1,
        /// <summary>
        /// 不可见
        /// </summary>
        Invisible = 2
    }
}