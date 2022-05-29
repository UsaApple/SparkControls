namespace SparkControls.Controls
{
	/// <summary>
	/// 树节点双击事件的触发动作
	/// </summary>
	[System.Flags]
	public enum TreeViewDoubleClickAction
    {
        /// <summary>
        /// 触发事件，但没有默认动作
        /// </summary>
        None = 0,
        /// <summary>
        /// 有复选框，选中或取消
        /// </summary>
        Check = 2,
        /// <summary>
        /// 展开或伸缩
        /// </summary>
        Expand = 4,
        /// <summary>
        /// 同时触发Check和Expand动作
        /// </summary>
        Both = 8
    }
}