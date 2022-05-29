namespace SparkControls.Controls
{
    /// <summary>
    /// 表头信息
    /// </summary>
    internal struct SparkColumnSpanInfo
    {
        /// <summary>
        /// 列主标题
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// 位置，1:左，2中，3右
        /// </summary>
        public int Position { set; get; }

        /// <summary>
        /// 对应左行
        /// </summary>
        public int Left { set; get; }

        /// <summary>
        /// 对应右行
        /// </summary>
        public int Right { set; get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Text">文本</param>
        /// <param name="Position">位置</param>
        /// <param name="Left">对应左行</param>
        /// <param name="Right">对应右行</param>
        public SparkColumnSpanInfo(string Text, int Position, int Left, int Right)
        {
            this.Text = Text;
            this.Position = Position;
            this.Left = Left;
            this.Right = Right;
        }
    }
}