namespace SparkControls.Controls
{
	/// <summary>
	/// 选中的对象
	/// </summary>
	public enum HitTestResult
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 关闭按钮
        /// </summary>
        CloseButton,
        /// <summary>
        /// 菜单
        /// </summary>
        MenuGlyph,
        /// <summary>
        /// TabPage
        /// </summary>
        TabPage,
    }

    /// <summary>
    /// TabPage 改变的方式
    /// </summary>
    public enum TabPageChangeAction
    {
        /// <summary>
        /// 添加
        /// </summary>
        Added,
        /// <summary>
        ///移除
        /// </summary>
        Removed,
        /// <summary>
        /// 改变
        /// </summary>
        //Changed,
        /// <summary>
        /// 选择改变
        /// </summary>
        SelectionChanged
    }

    /// <summary>
    /// 关闭按钮显示的模式
    /// </summary>
    public enum CloseButtonDisplayModes
    {
        /// <summary>
        /// 不显示
        /// </summary>
        None = 0,
        /// <summary>
        /// 选择或鼠标进去
        /// </summary>
        FocusOrMouseEnter,
        /// <summary>
        /// 一直
        /// </summary>
        Always
    }
}