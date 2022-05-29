using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SparkControls.Controls.Helper;

namespace SparkControls.Controls
{
    /// <summary>
    /// <see cref="SparkList"/>的子项
    /// </summary>
    public class SparkListItem
    {
        private Image _selectedImage = null;
        /// <summary>
        /// 图标
        /// </summary>
        [DefaultValue(null)]
        public Image Image { get; set; } = null;

        /// <summary>
        /// 选中时,图标改为白色,只有透明的图标会转为白色,不是透明的图标不进行转化
        /// </summary>
        [Browsable(false)]
        internal Image SelectedImage
        {
            get
            {
                if (_selectedImage == null)
                {
                    ImageHelper.DoImageToWhite(this.Image, out _selectedImage);
                }
                return _selectedImage;
            }
        }

        /// <summary>
        /// tag信息
        /// </summary>
        [DefaultValue(null)]
        public object Tag { get; set; } = null;

        /// <summary>
        /// 文字
        /// </summary>
        [DefaultValue("")]
        public string Text { get; set; } = "";

        [Browsable(false)]
        internal DrawItemState State { get; set; } = DrawItemState.None;

        /// <summary>
        ///  获取树节点的界限。
        /// </summary>
        [Browsable(false)]
        public Rectangle Bounds { get; internal set; }

        /// <summary>
        /// 字体
        /// </summary>
        [DefaultValue(null)]
        public Font ItemFont { get; set; } = null;

       

        /// <summary>
        /// 获取一个值，用以指示树节点是否处于选定状态。
        /// </summary>
        [Browsable(false)]
        public bool IsSelected => this.State.HasFlag(DrawItemState.Selected);

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color ForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackColor { get; set; } = Color.Empty;


        internal RectangleF TextRectangle
        {
            get; set;
        }

        internal StringFormat StringFormat { get; set; } = null;

        internal RectangleF ImageRectangle
        {
            get; set;
        }

        /// <summary>
        /// <see cref="SparkListItem"/>的实例
        /// </summary>
        /// <param name="text"></param>
        public SparkListItem(string text)
        {
            Text = text;
        }

        /// <summary>
        /// <see cref="SparkListItem"/>的实例
        /// </summary>
        /// <param name="text"></param>
        /// <param name="image"></param>
        public SparkListItem(string text, Image image)
        {
            Text = text;
            Image = image;
        }




    }
}
