using System;
using System.Drawing;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// 显示在表格中的表格树。
	/// </summary>
	public class SparkDataGridTreeCell : DataGridViewTextBoxCell
    {
        private const int INDENT_WIDTH = 20;
        private const int INDENT_MARGIN = 5;
        private int glyphWidth;
        private int calculatedLeftPadding;
        internal bool IsSited;
        private Padding _previousPadding;
        private int _imageWidth = 0, _imageHeight = 0, _imageHeightOffset = 0;

        public SparkDataGridTreeCell()
        {
            glyphWidth = 15;
            calculatedLeftPadding = 0;
            this.IsSited = false;
        }

        public override object Clone()
        {
            SparkDataGridTreeCell c = (SparkDataGridTreeCell)base.Clone();

            c.glyphWidth = this.glyphWidth;
            c.calculatedLeftPadding = this.calculatedLeftPadding;

            return c;
        }
        public override Type EditType
        {
            get { return typeof(SparkDataGridTextBoxEditingControl); }
        }

        internal protected virtual void UnSited()
        {
            this.IsSited = false;
            this.Style.Padding = this._previousPadding;
        }

        internal protected virtual void Sited()
        {
            this.IsSited = true;
            this._previousPadding = this.Style.Padding;
            this.UpdateStyle();
        }

        internal protected virtual void UpdateStyle()
        {
            if (this.IsSited == false) return;
            int level = this.Level;
            Padding p = this._previousPadding;
            Size preferredSize;
            using (Graphics g = this.OwningNode._grid.CreateGraphics())
            {
                preferredSize = this.GetPreferredSize(g, this.InheritedStyle, this.RowIndex, new Size(0, 0));
            }
            Image image = this.OwningNode.Image;
            if (image != null)
            {
                _imageWidth = image.Width + 2;
                _imageHeight = image.Height + 2;
            }
            else
            {
                _imageWidth = glyphWidth;
                _imageHeight = 0;
            }
            if (preferredSize.Height < _imageHeight)
            {
                this.Style.Padding = new Padding(p.Left + (level * INDENT_WIDTH) + _imageWidth + INDENT_MARGIN,
                                                 p.Top + (_imageHeight / 2), p.Right, p.Bottom + (_imageHeight / 2));
                _imageHeightOffset = 2;// (_imageHeight - preferredSize.Height) / 2;
            }
            else
            {
                this.Style.Padding = new Padding(p.Left + (level * INDENT_WIDTH) + _imageWidth + INDENT_MARGIN,
                                                 p.Top, p.Right, p.Bottom);
            }
            calculatedLeftPadding = ((level - 1) * glyphWidth) + _imageWidth + INDENT_MARGIN;
        }

        public int Level
        {
            get
            {
                SparkDataGridTreeNode row = this.OwningNode;
                if (row != null) return row.Level;
                else return -1;
            }
        }

        protected virtual int GlyphMargin
        {
            get
            {
                return ((this.Level - 1) * INDENT_WIDTH) + INDENT_MARGIN;
            }
        }

        protected virtual int GlyphOffset
        {
            get
            {
                return ((this.Level - 1) * INDENT_WIDTH);
            }
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            SparkDataGridTreeNode node = this.OwningNode;
            if (node == null) return;
            Image image = node.Image;
            if (this._imageHeight == 0 && image != null) this.UpdateStyle();
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            Rectangle glyphRect = new Rectangle(cellBounds.X + this.GlyphMargin, cellBounds.Y, INDENT_WIDTH, cellBounds.Height - 1);
            if (image != null)
            {
                Point pp;
                if (_imageHeight > cellBounds.Height)
                    pp = new Point(glyphRect.X + this.glyphWidth, cellBounds.Y + _imageHeightOffset);
                else
                    pp = new Point(glyphRect.X + this.glyphWidth, (cellBounds.Height / 2 - _imageHeight / 2) + cellBounds.Y);
                System.Drawing.Drawing2D.GraphicsContainer gc = graphics.BeginContainer();
                {
                    graphics.SetClip(cellBounds);
                    graphics.DrawImageUnscaled(image, pp);
                }
                graphics.EndContainer(gc);
            }
            if (node._grid.ShowLines)
            {
                using (Pen linePen = new Pen(SystemBrushes.ControlDark, 1.0f))
                {
                    linePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    bool isLastSibling = node.IsLastSibling;
                    bool isFirstSibling = node.IsFirstSibling;
                    if (node.Level == 1)
                    {
                        if (isFirstSibling && isLastSibling)
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                        }
                        else if (isLastSibling)
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2);
                        }
                        else if (isFirstSibling)
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.X + 4, cellBounds.Bottom);
                        }
                        else
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Bottom);
                        }
                    }
                    else
                    {
                        if (isLastSibling)
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2);
                        }
                        else
                        {
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top + cellBounds.Height / 2, glyphRect.Right, cellBounds.Top + cellBounds.Height / 2);
                            graphics.DrawLine(linePen, glyphRect.X + 4, cellBounds.Top, glyphRect.X + 4, cellBounds.Bottom);
                        }
                        SparkDataGridTreeNode previousNode = node.Parent;
                        int horizontalStop = (glyphRect.X + 4) - INDENT_WIDTH;
                        while (!previousNode.IsRoot)
                        {
                            if (previousNode.HasChildren && !previousNode.IsLastSibling)
                            {
                                graphics.DrawLine(linePen, horizontalStop, cellBounds.Top, horizontalStop, cellBounds.Bottom);
                            }
                            previousNode = previousNode.Parent;
                            horizontalStop = horizontalStop - INDENT_WIDTH;
                        }
                    }
                }
            }
            if (node.HasChildren || node._grid.VirtualNodes)
            {
                if (node.IsExpanded)
                    node._grid.rOpen.DrawBackground(graphics, new Rectangle(glyphRect.X, glyphRect.Y + (glyphRect.Height / 2) - 4, 10, 10));
                else
                    node._grid.rClosed.DrawBackground(graphics, new Rectangle(glyphRect.X, glyphRect.Y + (glyphRect.Height / 2) - 4, 10, 10));
            }
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseUp(e);
            SparkDataGridTreeNode node = this.OwningNode;
            if (node != null)
                node._grid._inExpandCollapseMouseCapture = false;
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.Location.X > this.InheritedStyle.Padding.Left)
            {
                base.OnMouseDown(e);
            }
            else
            {
                SparkDataGridTreeNode node = this.OwningNode;
                if (node != null)
                {
                    node._grid._inExpandCollapseMouseCapture = true;
                    if (node.IsExpanded) node.Collapse();
                    else node.Expand();
                }
            }
        }

        public SparkDataGridTreeNode OwningNode
        {
            get { return base.OwningRow as SparkDataGridTreeNode; }
        }
    }
}