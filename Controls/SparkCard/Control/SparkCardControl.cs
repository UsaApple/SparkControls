using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SparkControls.Controls
{
    /// <summary>
    /// 卡片控件的基类。
    /// </summary>
    [Serializable]
    public abstract class SparkCardControl
    {

        /// <summary>
        /// 获取 xml 模板的属性值。
        /// </summary>
        /// <typeparam name="TReturn">返回值类型。</typeparam>
        /// <typeparam name="TConverter">转换器类型。</typeparam>
        /// <param name="element">xml 模板元素。</param>
        /// <param name="propertyName">属性名。</param>
        /// <param name="propertyValue">属性值。</param>
        /// <returns>成功标志，true：成功，false：失败。</returns>
        internal protected static bool GetPropertyValue<TReturn, TConverter>(XElement element, string propertyName, out TReturn propertyValue) where TConverter : TypeConverter, new()
        {
            propertyValue = default;
            if (element == null) return false;

            var attributeValue = element.Elements("Property").FirstOrDefault(p => p.GetAttributeValue("name") == propertyName)?.Value;
            if (!attributeValue.IsNullOrEmpty())
            {
                try
                {
                    propertyValue = (TReturn)new TConverter().ConvertFrom(attributeValue);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 重置控件尺寸时发生。
        /// </summary>
        public event ResizeEventHandler Resize;

        /// <summary>
        /// 绘制控件。
        /// </summary>
        /// <param name="g">绘图图面。</param>
        /// <param name="data">行数据源。</param>
        public abstract void Paint(Graphics g, DataRowView data);

        public abstract IDrawStyle CreateStyle(DataRowView dataRowView, IDrawStyle parent);

        /// <summary>
        /// 获取或设置控件标识。
        /// </summary>
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 获取或设置控件名称。
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 获取或设置控件类型。
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// 获取或设置控件的锚定方式。
        /// </summary>
        public virtual AnchorStyles Anchor { get; set; } = AnchorStyles.Top | AnchorStyles.Left;

        /// <summary>
        /// 获取或设置控件锚定到其容器上边缘的距离。
        /// </summary>
        public virtual int AnchorTop { get; protected set; } = 0;

        /// <summary>
        /// 获取或设置控件锚定到其容器下边缘的距离。
        /// </summary>
        public virtual int AnchorBottom { get; protected set; } = 0;

        /// <summary>
        /// 获取或设置控件锚定到其容器左边缘的距离。
        /// </summary>
        public virtual int AnchorLeft { get; protected set; } = 0;

        /// <summary>
        /// 获取或设置控件锚定到其容器右边缘的距离。
        /// </summary>
        public virtual int AnchorRight { get; protected set; } = 0;

        /// <summary>
        /// 获取或设置控件的停靠方式。
        /// </summary>
        public virtual DockStyle Dock { get; set; } = DockStyle.None;

        /// <summary>
        /// 获取或设置控件的横坐标。
        /// </summary>
        public virtual int X { get; set; }

        /// <summary>
        /// 获取或设置控件的纵坐标。
        /// </summary>
        public virtual int Y { get; set; }

        /// <summary>
        /// 获取或设置控件的长度。
        /// </summary>
        public virtual int Width { get; set; }

        /// <summary>
        /// 获取或设置控件的高度。
        /// </summary>
        public virtual int Height { get; set; }

        /// <summary>
        /// 获取或设置控件位置。
        /// </summary>
        public virtual Point Location
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
                SetAnchorDistance();
            }
        }

        /// <summary>
        /// 获取或设置控件尺寸。
        /// </summary>
        public virtual Size Size
        {
            get => new Size(Width, Height);
            set
            {
                Size oldSize = new Size(Width, Height);
                Width = value.Width;
                Height = value.Height;
                SetAnchorDistance();
                Resize?.Invoke(this, new ResizeEventArgs(oldSize, value));
            }
        }

        /// <summary>
        /// 获取或设置控件边距。
        /// </summary>
        public virtual Padding Margin { get; set; } = new Padding(3, 0, 3, 0);

        /// <summary>
        /// 获取控件的矩形范围。
        /// </summary>
        public virtual Rectangle Rectangle
        {
            get
            {
                return new Rectangle(new Point(this.Location.X + this.Parent.Location.X, this.Location.Y + this.Parent.Location.Y), this.Size);
            }
        }

        private SparkCardControl _parent = null;
        /// <summary>
        /// 获取控件的父控件。
        /// </summary>
        public virtual SparkCardControl Parent
        {
            get => _parent;
            private set
            {
                _parent = value;
                if (_parent != null)
                {
                    _parent.Resize += this.Parent_Resize;
                }
                SetAnchorDistance();
            }
        }

        /// <summary>
        /// 获取或设置控件的子控件。
        /// </summary>
        public virtual SparkCardControlCollection Children { get; set; }

        /// <summary>
        /// 初始 <see cref="SparkCardControl"/> 类型的新实例。
        /// </summary>
        protected SparkCardControl()
        {
            Children = new SparkCardControlCollection(this);
        }

        /// <summary>
        /// 获取位于指定坐标点的子控件。
        /// </summary>
        /// <param name="point">一个 Point，其中包含的坐标指定您要在何处查找控件。</param>
        /// <returns>位于指定坐标点的子控件。</returns>
        public SparkCardControl GetChildAtPoint(Point point)
        {
            if (Children != null && Children.Count > 0)
            {
                SparkCardControl ctrl = Children.FirstOrDefault(c => c.X < point.X && c.Y < point.Y && c.X + c.Width > point.X && c.Y + c.Height > point.Y);
                if (ctrl != null)
                {
                    if (ctrl.Children != null && ctrl.Children.Count > 0)
                    {
                        foreach (var c in ctrl.Children)
                        {
                            ctrl = c.GetChildAtPoint(point);
                            if (ctrl != null) { return ctrl; }
                        }
                    }
                    return ctrl;
                }
            }
            return null;
        }

        /// <summary>
        /// 计算控件的工作区位置。
        /// </summary>
        /// <returns>控件在工作区的位置。</returns>
        public Point Point2WorkArea()
        {
            return new Point(X2WorkArea(Location.X), Y2WorkArea(Location.Y));
        }

        /// <summary>
        /// 计算横向工作区坐标。
        /// </summary>
        /// <param name="x">横向相对坐标值。</param>
        /// <returns>横向工作区坐标。</returns>
        private int X2WorkArea(int x)
        {
            return Parent == null ? x : x + Parent.X2WorkArea(Parent.Location.X);
        }

        /// <summary>
        /// 计算纵向工作区坐标。
        /// </summary>
        /// <param name="y">纵向相对坐标值。</param>
        /// <returns>纵向工作区坐标。</returns>
        private int Y2WorkArea(int y)
        {
            return Parent == null ? y : y + Parent.Y2WorkArea(Parent.Location.Y);
        }

        /// <summary>
        /// 设置锚定距离。
        /// </summary>
        private void SetAnchorDistance()
        {
            if (Parent != null)
            {
                AnchorTop = this.Y;
                AnchorBottom = Parent.Height - this.Y - this.Height;
                AnchorLeft = this.X;
                AnchorRight = Parent.Width - this.X - this.Width;
            }
        }

        /// <summary>
        /// 父控件尺寸改变事件处理器。
        /// </summary>
        /// <param name="sender">事件发送对象。</param>
        /// <param name="e">事件数据对象。</param>
        private void Parent_Resize(object sender, ResizeEventArgs e)
        {
            // Dock
            if (Dock == DockStyle.Top || Dock == DockStyle.Bottom)
            {
                Width = Parent.Width;
                return;
            }
            if (Dock == DockStyle.Left || Dock == DockStyle.Right)
            {
                Height = Parent.Height;
                return;
            }

            // Anchor
            if ((Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right))
            {
                X = AnchorLeft;
                Width = Parent.Width >= AnchorLeft + AnchorRight ? Parent.Width - AnchorLeft - AnchorRight : 0;
            }
            else if ((Anchor & AnchorStyles.Left) == AnchorStyles.Left)
            {
                X = AnchorLeft;
            }
            else if ((Anchor & AnchorStyles.Left) == AnchorStyles.Right)
            {
                X = Parent.Width - AnchorRight - Width;
            }

            if ((Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom))
            {
                Y = AnchorTop;
                Height = Parent.Height >= AnchorTop + AnchorBottom ? Parent.Height - AnchorTop - AnchorBottom : 0;
            }
            else if ((Anchor & AnchorStyles.Top) == AnchorStyles.Top)
            {
                Y = AnchorTop;
            }
            else if ((Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom)
            {
                Y = Parent.Height - AnchorBottom - Height;
            }

            if (Anchor == AnchorStyles.None)
            {
                X += (e.NewSize.Width - e.OldSize.Width) / 2;
                Y += (e.NewSize.Height - e.OldSize.Height) / 2;
            }
        }



        /// <summary>
        /// 卡片控件集合类。
        /// </summary>
        [Serializable]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "<挂起>")]
        public sealed class SparkCardControlCollection : List<SparkCardControl>
        {
            /// <summary>
            /// 获取归属控件对象。
            /// </summary>
            public SparkCardControl Owner { get; }

            /// <summary>
            /// 初始 <see cref="SparkCardControlCollection"/> 类型的新实例。
            /// </summary>
            /// <param name="owner"></param>
            public SparkCardControlCollection(SparkCardControl owner)
            {
                Owner = owner;
            }

            /// <summary>
            /// 添加控件。
            /// </summary>
            /// <param name="control">将添加到集合中的控件实例。</param>
            public new void Add(SparkCardControl control)
            {
                if (control == null || this.Owner == control.Parent) { return; }

                if (control.Parent != null)
                {
                    control.Parent.Children.Remove(control);
                }

                control.Parent = Owner;
                base.Add(control);
            }

            /// <summary>
            /// 批量添加控件。
            /// </summary>
            /// <param name="controls">将添加到集合中的控件实例集合。</param>
            public new void AddRange(IEnumerable<SparkCardControl> controls)
            {
                foreach (SparkCardControl control in controls)
                {
                    this.Add(control);
                }
            }
        }
    }

    /// <summary>
    /// 卡片控件扩展方法类。
    /// </summary>
    public static class SparkCardControlExtension
    {
        /// <summary>
        /// 从指定的 xml 模板读取属性。
        /// </summary>
        /// <param name="cardControl">模板控件对象。</param>
        /// <param name="element">xml 模板元素。</param>
        internal static void FromXml(this SparkCardControl cardControl, XElement element)
        {
            if (cardControl != null)
            {
                if (SparkCardControl.GetPropertyValue<string, StringConverter>(element, "Name", out string name)) cardControl.Name = name;
                if (SparkCardControl.GetPropertyValue<AnchorStyles, EnumConverter<AnchorStyles>>(element, "Anchor", out AnchorStyles anchor)) cardControl.Anchor = anchor;
                if (SparkCardControl.GetPropertyValue<DockStyle, EnumConverter<DockStyle>>(element, "Dock", out DockStyle dock)) cardControl.Dock = dock;
                if (SparkCardControl.GetPropertyValue<Point, PointConverter>(element, "Location", out Point location)) cardControl.Location = location;
                if (SparkCardControl.GetPropertyValue<Size, SizeConverter>(element, "Size", out Size size)) cardControl.Size = size;
                if (SparkCardControl.GetPropertyValue<Padding, PaddingConverter>(element, "Margin", out Padding margin)) cardControl.Margin = margin;
            }
        }
    }

    /// <summary>
    /// 为重置控件尺寸事件提供数据。
    /// </summary>
    public class ResizeEventArgs
    {
        /// <summary>
        /// 获取原来的尺寸。
        /// </summary>
        public Size OldSize { get; private set; }

        /// <summary>
        /// 获取现在的尺寸。
        /// </summary>
        public Size NewSize { get; private set; }

        /// <summary>
        /// 初始 <see cref="ResizeEventArgs"/> 类型的新实例。
        /// </summary>
        /// <param name="oldSize">原来的尺寸。</param>
        /// <param name="newSize">现在的尺寸。</param>
        public ResizeEventArgs(Size oldSize, Size newSize)
        {
            OldSize = oldSize;
            NewSize = newSize;
        }
    }

    /// <summary>
    /// 表示将处理重置控件尺寸事件的方法。
    /// </summary>
    /// <param name="sender">事件发送对象。</param>
    /// <param name="e">事件数据对象。</param>
    public delegate void ResizeEventHandler(object sender, ResizeEventArgs e);
}