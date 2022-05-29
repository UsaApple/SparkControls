using System.ComponentModel;
using System.Drawing;

namespace SparkControls.Controls
{
    /// <summary>
    /// 矩形的圆角半径。
    /// </summary>
    public class CornerRadius
    {
        /// <summary>
        /// 获取一个 CornerRadius 实例，该实例的圆角值为零。
        /// </summary>
        public static CornerRadius Empty { get; } = new CornerRadius(0f);

        #region Initializes

        /// <summary>
        /// 初始 <see cref="CornerRadius"/> 类型的新实例。
        /// </summary>
        /// <param name="radius">圆角半径。</param>
        public CornerRadius(float radius) : this(radius, radius, radius, radius)
        {

        }

        /// <summary>
        /// 初始 <see cref="CornerRadius"/> 类型的新实例。
        /// </summary>
        /// <param name="topLeft">圆角半角 - 左上。</param>
        /// <param name="topRight">圆角半角 - 有伤。</param>
        /// <param name="bottomLeft">圆角半角 - 左下。</param>
        /// <param name="bottomRight">圆角半角 - 右上。</param>
        public CornerRadius(float topLeft, float topRight, float bottomLeft, float bottomRight)
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomLeft = bottomLeft;
            this.BottomRight = bottomRight;
        }

        #endregion

        #region Fields

        /// <summary>
        /// 左上角圆角半径
        /// </summary>
        [DefaultValue(0)]
        public float TopLeft { get; set; }

        /// <summary>
        /// 右上角圆角半径
        /// </summary>
        [DefaultValue(0)]
        public float TopRight { get; set; }

        /// <summary>
        /// 左下角圆角半径
        /// </summary>
        [DefaultValue(0)]
        public float BottomLeft { get; set; }

        /// <summary>
        /// 右下角圆角半径
        /// </summary>
        [DefaultValue(0)]
        public float BottomRight { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 指示指定的 CornerRadius 是否为空。
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return TopLeft == 0 && TopRight == 0 && BottomLeft == 0 && BottomRight == 0;
        }

        #endregion


        //public static bool operator ==(CornerRadius left, CornerRadius right)
        //{
        //    return left.TopLeft == right.TopLeft &&
        //        left.TopRight == right.TopRight &&
        //        left.BottomLeft == right.BottomLeft &&
        //        left.BottomRight == right.BottomRight;
        //}

        //public static bool operator !=(CornerRadius left, CornerRadius right)
        //{
        //    return !(left.TopLeft == right.TopLeft &&
        //        left.TopRight == right.TopRight &&
        //        left.BottomLeft == right.BottomLeft &&
        //        left.BottomRight == right.BottomRight);
        //}
    }
}