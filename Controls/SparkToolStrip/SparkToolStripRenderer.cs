using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using SparkControls.Theme;

namespace SparkControls.Controls
{
	/// <summary>
	/// 对工具栏组件的绘制抽象类 <see cref="ToolStripRenderer"/> 的重新设计以实现对菜单的重绘。
	/// </summary>
	public class SparkToolStripRenderer : ToolStripRenderer
	{
		#region 常量
		private const int SPLIT_LINE_OFFSET = 3;
		private const int ARROW_WIDTH = 8;
		private const int ARROW_HEIGHT = 6;
		#endregion

		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public SparkToolStripRenderer(SparkToolStripTheme theme) : base()
		{
			this.Theme = theme;
			this.MenuCornerRadius = 0;
			this.ItemCornerRadius = 1;
			this.ShowMenuBackImage = true;
			this.MenuImageBackImage = null;
			this.MenuImageBackImageOpacity = 0.16f;
		}
		#endregion

		#region 属性
		/// <summary>
		/// 主题
		/// </summary>
		public SparkToolStripTheme Theme { get; private set; }

		/// <summary>
		/// 菜单区域圆角值
		/// </summary>
		public int MenuCornerRadius { get; set; }

		/// <summary>
		/// Item项的背景圆角值
		/// </summary>
		public int ItemCornerRadius { get; set; }

		/// <summary>
		/// 是否显示菜单图标栏的背景图片
		/// </summary>
		public bool ShowMenuBackImage { get; set; }

		/// <summary>
		/// 菜单图标栏的背景图片
		/// </summary>
		public Image MenuImageBackImage { get; set; }

		/// <summary>
		/// 菜单栏图标栏的背景图片透明度
		/// </summary>
		public float MenuImageBackImageOpacity { get; set; }
		#endregion

		#region 重写方法
		/// <summary>
		/// 绘制背景区域
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
		{
			GDIHelper.InitializeGraphics(e.Graphics);

			Rectangle rect = e.AffectedBounds;
			RoundRectangle roundRect = new RoundRectangle(rect, new CornerRadius(this.MenuCornerRadius));
			if (e.ToolStrip is ToolStripDropDown || e.ToolStrip is ContextMenuStrip)
			{
				this.CreateToolStripRegion(e.ToolStrip, roundRect);
				GDIHelper.FillPath(e.Graphics, roundRect, Theme.BackColor, Theme.BackColor);
			}
			else if (e.ToolStrip is SparkMenuStrip ms)
			{
				//填充渐变色
				GDIHelper.FillPath(e.Graphics, new RoundRectangle(rect, new CornerRadius(0)), Theme.BackColor1, Theme.BackColor2);
			}
			else if (e.ToolStrip is SparkToolStrip ts)
			{
				rect.Inflate(1, 1);
				//填充渐变色
				GDIHelper.FillPath(e.Graphics, new RoundRectangle(rect,
					new CornerRadius(0)), Theme.BackColor1, Theme.BackColor2);
			}
			else if (e.ToolStrip is SparkStatusStrip ss)
			{
				//填充渐变色
				GDIHelper.FillPath(e.Graphics, new RoundRectangle(rect,
					new CornerRadius(0)), Theme.BackColor1, Theme.BackColor2);
			}
		}

		/// <summary>
		/// 绘制项的背景
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
		{
			GDIHelper.InitializeGraphics(e.Graphics);
			Rectangle rect = e.AffectedBounds;
			rect.Width--; rect.Height--;

			if (e.ToolStrip is ToolStripDropDown)
			{
				GDIHelper.FillPath(e.Graphics, new RoundRectangle(rect, new CornerRadius(this.MenuCornerRadius,
					0, this.MenuCornerRadius, 0)), Theme.ImageMarginBackColor, Theme.ImageMarginBackColor);

				//绘制项的背景图片
				Image img = this.MenuImageBackImage;
				if (img != null && this.ShowMenuBackImage)
				{
					ImageAttributes imgAttributes = new ImageAttributes();
					GDIHelper.SetImageOpacity(imgAttributes, this.MenuImageBackImageOpacity);
					e.Graphics.DrawImage(img, new Rectangle(rect.X + 1, rect.Y + 2, img.Width, img.Height),
						0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttributes);
				}

				//绘制图片和项目之间间隔线
				Point p1 = new Point(rect.X + rect.Width, rect.Y + SPLIT_LINE_OFFSET);
				Point p2 = new Point(rect.X + rect.Width, rect.Bottom - SPLIT_LINE_OFFSET);
				using (Pen pen = new Pen(Theme.BorderColor))
				{
					e.Graphics.DrawLine(pen, p1, p2);
				}
			}
			else
			{
				base.OnRenderImageMargin(e);
			}
		}

		/// <summary>
		/// 绘制边框效果
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
		{
			GDIHelper.InitializeGraphics(e.Graphics);
			Rectangle rect = e.AffectedBounds;
			if (e.ToolStrip is ToolStripDropDown)
			{
				//阴影边框
				rect.Width--; rect.Height--;
				CornerRadius toolStripCornerRadius = new CornerRadius(this.MenuCornerRadius);
				RoundRectangle roundRect = new RoundRectangle(rect, toolStripCornerRadius);
				GDIHelper.DrawPathBorder(e.Graphics, roundRect, Theme.BorderColor);
			}
			else
			{
				base.OnRenderToolStripBorder(e);
			}
		}

		/// <summary>
		/// 绘制Item的状态背景样式
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
		{
			ToolStripItem item = e.Item;
			Rectangle bounds = new Rectangle(Point.Empty, item.Size);
			if (bounds.Width == 0 || bounds.Height == 0) return;

			GDIHelper.InitializeGraphics(e.Graphics);
			Rectangle rect = new Rectangle(1, -1, item.Width - 2, item.Height + 1);
			if (item.IsOnDropDown)
			{
				//这里不能添加Pressed的条件，因为ToolStripDropMenu展开子集内部其实调用了按下操作，所以Pressed为true
				if (item.Selected)
				{
					GDIHelper.FillRectangle(e.Graphics, rect, Theme.HighlightColor);
				}
			}
			else
			{
				if (item.Selected || item.Pressed)
				{
					GDIHelper.FillRectangle(e.Graphics, rect, Theme.HighlightColor);
				}
			}
		}

		/// <summary>
		/// 绘制按钮背景
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
		{
			GDIHelper.InitializeGraphics(e.Graphics);
			ToolStripButton item = e.Item as ToolStripButton;
			if (item.Tag != null && item.Tag.Equals("Hathaway"))
			{
				int temp = item.Width >= item.Height ? item.Height : item.Width;
				Rectangle rect = new Rectangle(0, 0, temp, temp);
				rect.Inflate(-1, -1);
				Blend blend = new Blend
				{
					Positions = new float[] { 0f, 0.5f, 1f },
					Factors = new float[] { 0.25f, 0.75f, 1f }
				};
				Color borderColor = item.Selected || item.Pressed ? Color.FromArgb(24, 116, 205) : Theme.BorderColor;
				float w = 1.0F;
				e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				if (item.Selected || item.Pressed)
				{
					w = 2.0F;
					Color c1 = Color.FromArgb(255, 226, 48);
					Color c2 = Color.FromArgb(255, 220, 102);
					Color c3 = Color.FromArgb(255, 220, 102);
					GDIHelper.DrawCrystalButton(e.Graphics, rect, c1, c2, c3, blend);
				}

				using (Pen p = new Pen(borderColor, w))
				{
					e.Graphics.DrawEllipse(p, rect);
				}
			}
			else
			{
				Rectangle rect = new Rectangle(1, 1, item.Width - 4, item.Height - 3);
				RoundRectangle roundRect = new RoundRectangle(rect, this.ItemCornerRadius);
				if (item.Selected || item.Pressed)
				{
					if (item.Pressed)
					{
						GDIHelper.FillRectangle(e.Graphics, roundRect, Theme.MouseDownBackColor);
					}
					else
					{
						GDIHelper.FillRectangle(e.Graphics, roundRect, Theme.HighlightColor);
					}
					GDIHelper.DrawPathBorder(e.Graphics, roundRect, Theme.BorderColor);
				}
				if (item.CheckState == CheckState.Checked)
				{
					//checked为true状态下
					GDIHelper.DrawPathBorder(e.Graphics, roundRect, Theme.SelectedBackColor);
				}
			}
		}

		/// <summary>
		/// 绘制下拉按钮背景
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
		{
			GDIHelper.InitializeGraphics(e.Graphics);
			Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
			RoundRectangle roundRect = new RoundRectangle(rect, this.ItemCornerRadius);
			if (e.Item.Selected || e.Item.Pressed)
			{
				GDIHelper.FillRectangle(e.Graphics, roundRect, Theme.HighlightColor);
				GDIHelper.DrawPathBorder(e.Graphics, roundRect, Theme.BorderColor);
			}
		}

		/// <summary>
		/// 绘制箭头
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
		{
			GDIHelper.InitializeGraphics(e.Graphics);
			Rectangle rect = e.ArrowRectangle;
			ArrowDirection direct = e.Direction;
			if (e.Item != null && e.Item.Owner != null &&
				(e.Item.Owner.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow ||
				e.Item.Owner.LayoutStyle == ToolStripLayoutStyle.Table))
			{
				direct = ArrowDirection.Right;
			}
			GDIHelper.DrawArrow(e.Graphics, direct, rect, this.GetArrowSize(direct));
		}

		/// <summary>
		/// 绘制溢出按钮背景
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
		{
			base.OnRenderOverflowButtonBackground(e);
			GDIHelper.InitializeGraphics(e.Graphics);
			Rectangle rect = e.Item.Bounds;
			rect = new Rectangle(0, 0, rect.Width, rect.Height);
			GDIHelper.DrawArrow(e.Graphics, ArrowDirection.Down, rect,
				this.GetArrowSize(ArrowDirection.Down, true));
		}

		/// <summary>
		///  绘制Item的图标
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
		{
			//绘制项目的图标，文本和图标的位置属性设置好像无效
			base.OnRenderItemImage(e);
		}

		/// <summary>
		/// 绘制item的文本
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			//修正ToolStripDropDown的子项文本不居中的问题
			if (e.ToolStrip is ToolStripOverflow)
			{
				//Overflow不需要修正
			}
			else if (e.ToolStrip is ToolStripDropDown || e.ToolStrip is ContextMenuStrip)
			{
				e.TextRectangle = new Rectangle(new Point(e.TextRectangle.X, e.TextRectangle.Y + e.Item.Padding.Vertical / 2), e.TextRectangle.Size);
			}

			if (e.Item is ToolStripMenuItem && (e.Item.Selected || e.Item.Pressed))
			{
				e.TextColor = e.Item.ForeColor;
			}

			this.OnBaseRenderItemText(e);
		}

		/// <summary>
		/// 绘制item的文本
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnBaseRenderItemText(ToolStripItemTextRenderEventArgs e)
		{
			base.OnRenderItemText(e);
		}

		/// <summary>
		/// 绘制item项的check状态
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
		{
			GDIHelper.InitializeGraphics(e.Graphics);
			Rectangle rect = e.ImageRectangle;

			if (e.ToolStrip is ToolStripDropDown)
			{
				bool showCheckMargin = false;
				if (e.ToolStrip is ContextMenuStrip cms)
				{
					rect = new Rectangle(rect.X - 1, rect.Y - 2, rect.Width + 2, rect.Height + 2);
					showCheckMargin = cms.ShowCheckMargin;
				}
				else
				{
					rect.Inflate(1, 1);
				}

				Tuple<Color, Color, Color> rltColor = this.GetCheckBoxColor(e.Item);
				GDIHelper.DrawCheckBox(e.Graphics, new RoundRectangle(rect, 1), rltColor.Item1, rltColor.Item2, 1);
				if (e.Item.Image == null || showCheckMargin)
				{
					GDIHelper.DrawCheckTick(e.Graphics, rect, rltColor.Item3);
				}
			}
			else
			{
				base.OnRenderItemCheck(e);
			}
		}

		/// <summary>
		/// 绘制分割线
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
		{
			Rectangle rect = e.Item.ContentRectangle;
			if (e.ToolStrip is ToolStripDropDown)
			{
				rect.X += e.ToolStrip.Padding.Left;
				rect.Width -= e.ToolStrip.Padding.Left;
			}

			this.DrawSeparatorLine(rect, e.Graphics, e.Vertical);
		}

		/// <summary>
		/// 渲染SplitButton
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
		{
			if (e.Item is ToolStripSplitButton item)
			{
				GDIHelper.InitializeGraphics(e.Graphics);
				Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
				RoundRectangle roundRect = new RoundRectangle(rect, this.ItemCornerRadius);

				Rectangle dropDownRect = item.DropDownButtonBounds;
				dropDownRect = new Rectangle(dropDownRect.X, dropDownRect.Y,
					dropDownRect.Width - 1, dropDownRect.Height - 1);

				if (item.Selected || item.Pressed)
				{
					GDIHelper.FillRectangle(e.Graphics, roundRect, Theme.HighlightColor);
					GDIHelper.DrawPathBorder(e.Graphics, roundRect, Theme.BorderColor);

					//绘制下拉框矩形
					GDIHelper.DrawPathBorder(e.Graphics, new RoundRectangle(
						dropDownRect, this.ItemCornerRadius), Theme.BorderColor);
				}
				//下拉框前面的按钮按下
				if (item.ButtonPressed)
				{
					Rectangle btnBounds = item.ButtonBounds;
					GDIHelper.FillRectangle(e.Graphics, new RoundRectangle(
						btnBounds, this.ItemCornerRadius), Theme.MouseDownBackColor);
				}

				ArrowDirection direct = ArrowDirection.Down;
				if (e.Item != null && e.Item.Owner != null &&
				e.Item.Owner.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow)
				{
					direct = ArrowDirection.Right;
				}
				GDIHelper.DrawArrow(e.Graphics, direct, dropDownRect, this.GetArrowSize(direct));
			}
			else
			{
				base.OnRenderSplitButtonBackground(e);
			}
		}
		#endregion

		#region 私有方法
		/// <summary>
		/// 获取箭头大小
		/// </summary>
		/// <param name="direct"></param>
		/// <param name="isOverFlowArrow"></param>
		/// <returns></returns>
		private Size GetArrowSize(ArrowDirection direct, bool isOverFlowArrow = false)
		{
			if (direct == ArrowDirection.Up || direct == ArrowDirection.Down)
			{
				if (isOverFlowArrow)
				{
					return new Size(ARROW_WIDTH + 2, ARROW_HEIGHT + 2);
				}
				else
				{
					return new Size(ARROW_WIDTH, ARROW_HEIGHT);
				}

			}
			if (isOverFlowArrow)
			{
				return new Size(ARROW_HEIGHT + 2, ARROW_WIDTH + 2);
			}
			else
			{
				return new Size(ARROW_HEIGHT, ARROW_WIDTH);
			}
		}

		/// <summary>
		/// 创建ToolStrip的工作区域
		/// </summary>
		/// <param name="toolStrip"></param>
		/// <param name="roundRect"></param>
		private void CreateToolStripRegion(ToolStrip toolStrip, RoundRectangle roundRect)
		{
			if (toolStrip.Region == null)
			{
				return;
			}

			//原始控件窗口区域不为空时修改区域
			using (GraphicsPath path = roundRect.ToGraphicsBezierPath())
			{
				Region region = new Region(path);
				path.Widen(new Pen(Theme.BorderColor));
				region.Union(path);

				if (toolStrip.Region != null)
				{
					toolStrip.Region.Dispose();
				}
				toolStrip.Region = region;
			}
		}

		/// <summary>
		/// 绘制分割线
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="g"></param>
		/// <param name="isVertical"></param>
		private void DrawSeparatorLine(Rectangle rect, Graphics g, bool isVertical)
		{
			Color c1 = Theme.BorderColor;
			Color c2 = Color.FromArgb(50, c1);
			using (LinearGradientBrush brush = new LinearGradientBrush(
				rect, c1, c2, isVertical ? 90 : 180))
			{
				brush.Blend = new Blend
				{
					Positions = new float[] { 0f, .2f, .5f, .8f, 1f },
					Factors = new float[] { 1f, .3f, 0f, .3f, 1f }
				};
				using (Pen pen = new Pen(brush))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					if (isVertical)
					{
						g.DrawLine(pen, rect.X, rect.Y + 1, rect.X, rect.Bottom - 1);
					}
					else
					{
						g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
					}
				}
			}
		}

		/// <summary>
		/// 获取checkbox的相关颜色
		/// </summary>
		/// <param name="tsi"></param>
		/// <returns></returns>
		private Tuple<Color, Color, Color> GetCheckBoxColor(ToolStripItem tsi)
		{
			Color backColor = Theme.CheckBoxTheme.SelectedBackColor;
			Color borderColor = Theme.CheckBoxTheme.SelectedBorderColor;
			Color tickColor = Theme.CheckBoxTheme.TickColor;
			if (tsi.Selected)
			{
				backColor = Theme.CheckBoxTheme.CombinedBackColor;
				borderColor = Theme.CheckBoxTheme.CombinedSelectedColor;
				tickColor = Theme.CheckBoxTheme.CombinedSelectedColor;
			}
			return Tuple.Create(backColor, borderColor, tickColor);
		}

		/// <summary>
		/// 渲染Check背景色(暂未使用)
		/// </summary>
		/// <param name="e"></param>
		private void RenderCheckBackground(ToolStripItemImageRenderEventArgs e)
		{
			Rectangle bounds = new Rectangle(e.ImageRectangle.Left - 2, 1,
				e.ImageRectangle.Width + 4, e.Item.Height - 2);
			Graphics g = e.Graphics;

			Color fill = e.Item.Selected ? Color.Red : Color.Red;
			fill = e.Item.Pressed ? Color.Red : fill;
			using (Brush b = new SolidBrush(fill))
			{
				g.FillRectangle(b, bounds);
			}

			using (Pen p = new Pen(Theme.BorderColor))
			{
				g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
			}
		}
		#endregion
	}
}