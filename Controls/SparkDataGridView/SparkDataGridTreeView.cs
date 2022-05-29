using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SparkControls.Controls
{
    /// <summary>
    /// 树表格
    /// </summary>
    [ToolboxBitmap(typeof(DataGridView)), Description("树表格")]
    public class SparkDataGridTreeView : SparkDataGridView
    {
        #region 字段
        private readonly SparkDataGridTreeNode _root;
        private SparkDataGridTreeColumn _expandableColumn;
        private bool _inExpandCollapse = false;
        private Control hideScrollBarControl;

        internal ImageList _imageList;
        internal bool _inExpandCollapseMouseCapture = false;
        internal VisualStyleRenderer rOpen = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
        internal VisualStyleRenderer rClosed = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
        #endregion

        #region 自定义事件
        /// <summary>
        /// 节点正展开事件
        /// </summary>
        public event SparkGridTreeExpandingEventHandler NodeExpanding = null;
        /// <summary>
        /// 节点展开完成事件
        /// </summary>
        public event SparkGridTreeExpandedEventHandler NodeExpanded = null;
        /// <summary>
        /// 节点正收缩事件
        /// </summary>
        public event SparkGridTreeCollapsingEventHandler NodeCollapsing = null;
        /// <summary>
        /// 节点正收缩完成事件
        /// </summary>
        public event SparkGridTreeCollapsedEventHandler NodeCollapsed = null;
        #endregion

        #region 构造器
        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkDataGridTreeView()
        {
            this.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.RowTemplate = new SparkDataGridTreeNode() as DataGridViewRow;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this._root = new SparkDataGridTreeNode(this)
            {
                IsRoot = true
            };

            base.Rows.CollectionChanged += delegate (object sender, CollectionChangeEventArgs e) { };
        }
        #endregion

        #region 属性
        /// <summary>
        /// 绑定数据源(不支持)
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new object DataSource
        {
            get { return null; }
            set { throw new NotSupportedException("The TreeGridView does not support databinding"); }
        }

        /// <summary>
        /// 显示数据的数据源中的列表或表的名称(不支持)
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new object DataMember
        {
            get { return null; }
            set { throw new NotSupportedException("The TreeGridView does not support databinding"); }
        }

        /// <summary>
        /// 虚模式(不支持,若节点需始终显示为可展开的，请使用NodeExpanding事件添加节点
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new bool VirtualMode
        {
            get { return false; }
            set { throw new NotSupportedException("The TreeGridView does not support virtual mode"); }
        }

        /// <summary>
        /// 控件中的所有行
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new DataGridViewRowCollection Rows
        {
            get { return base.Rows; }
        }

        /// <summary>
        /// 获取或设置一行，该行表示控件中所有行的模板
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new DataGridViewRow RowTemplate
        {
            get { return base.RowTemplate; }
            set { base.RowTemplate = value; }
        }

        /// <summary>
        /// 树节点集合
        /// </summary>
        [Category("Spark"), Description("树节点集合"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public SparkDataGridTreeNodeCollection Nodes
        {
            get
            {
                return this._root.Nodes;
            }
        }

        /// <summary>
        /// 获取当前行
        /// </summary>
        public new SparkDataGridTreeNode CurrentRow
        {
            get
            {
                return base.CurrentRow as SparkDataGridTreeNode;
            }
        }

        /// <summary>
        /// 节点始终显示为可展开的
        /// </summary>
        [Category("Spark"), Description("节点是否始终显示为可展开的"), DefaultValue(false)]
        public bool VirtualNodes { get; set; } = false;

        /// <summary>
        /// 是否抑制双击以解决复选框快速双击的问题(有副作用)
        /// </summary>
        [Category("Spark"), Description("是否抑制双击以解决复选框快速双击的问题(有副作用)"), DefaultValue(false)]
        public bool SuppressDbClick { get; set; } = false;

        /// <summary>
        /// 获取当前表格树节点
        /// </summary>
        public SparkDataGridTreeNode CurrentNode
        {
            get
            {
                return this.CurrentRow;
            }
        }

        private bool _showLines = true;
        /// <summary>
        /// 是否显示树层级结构线
        /// </summary>
        [Category("Spark"), Description("是否显示树层级结构线"), DefaultValue(true)]
        public bool ShowLines
        {
            get { return this._showLines; }
            set
            {
                if (value != this._showLines)
                {
                    this._showLines = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 设置或获取图片集合
        /// </summary>
        [Category("Spark"), Description("设置或获取图片集合")]
        public ImageList ImageList
        {
            get { return this._imageList; }
            set { this._imageList = value; }
        }

        /// <summary>
        /// 行数
        /// </summary>
        public new int RowCount
        {
            get { return this.Nodes.Count; }
            set
            {
                for (int i = 0; i < value; i++) this.Nodes.Add(new SparkDataGridTreeNode());
            }
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 返回指定行的树结构
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        [Description("返回指定行的树结构")]
        public SparkDataGridTreeNode GetNodeForRow(DataGridViewRow row)
        {
            return row as SparkDataGridTreeNode;
        }

        /// <summary>
        /// 返回指定行索引的树结构
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Description("返回指定行索引的树结构")]
        public SparkDataGridTreeNode GetNodeForRow(int index)
        {
            return GetNodeForRow(base.Rows[index]);
        }
        #endregion

        #region 内部方法
        private void CellCanEdited()
        {
            if (!this.CurrentCell.Displayed)
            {
                this.FirstDisplayedScrollingRowIndex = this.CurrentCellAddress.Y;
            }
            this.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.BeginEdit(true);
        }

        protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
        {
            base.OnRowsAdded(e);
            int count = e.RowCount - 1;
            SparkDataGridTreeNode row;
            while (count >= 0)
            {
                row = base.Rows[e.RowIndex + count] as SparkDataGridTreeNode;
                if (row != null) row.Sited();
                count--;
            }
        }

        internal protected void UnSiteAll()
        {
            this.UnSiteNode(this._root);
        }

        internal protected virtual void UnSiteNode(SparkDataGridTreeNode node)
        {
            if (node.IsSited || node.IsRoot)
            {
                foreach (SparkDataGridTreeNode childNode in node.Nodes)
                {
                    this.UnSiteNode(childNode);
                }
                if (!node.IsRoot)
                {
                    base.Rows.Remove(node);
                    node.UnSited();
                }
            }
        }

        internal protected virtual bool CollapseNode(SparkDataGridTreeNode node)
        {
            if (node.IsExpanded)
            {
                SparkGridTreeCollapsingEventArgs exp = new SparkGridTreeCollapsingEventArgs(node);
                this.OnNodeCollapsing(exp);
                if (!exp.Cancel)
                {
                    this.LockVerticalScrollBarUpdate(true);
                    this.SuspendLayout();
                    _inExpandCollapse = true;
                    node.IsExpanded = false;
                    foreach (SparkDataGridTreeNode childNode in node.Nodes)
                    {
                        Debug.Assert(childNode.RowIndex != -1, "Row is NOT in the grid.");
                        this.UnSiteNode(childNode);
                    }
                    SparkGridTreeCollapsedEventArgs exped = new SparkGridTreeCollapsedEventArgs(node);
                    this.OnNodeCollapsed(exped);
                    _inExpandCollapse = false;
                    this.LockVerticalScrollBarUpdate(false);
                    this.ResumeLayout(true);
                    this.InvalidateCell(node.Cells[0]);
                }
                return !exp.Cancel;
            }
            else return false;
        }

        internal protected virtual void SiteNode(SparkDataGridTreeNode node)
        {
            int rowIndex = -1;
            SparkDataGridTreeNode currentRow;
            node._grid = this;
            if (node.Parent != null && node.Parent.IsRoot == false)
            {
                Debug.Assert(node.Parent != null && node.Parent.IsExpanded == true);
                if (node.Index > 0)
                {
                    currentRow = node.Parent.Nodes[node.Index - 1];
                }
                else
                {
                    currentRow = node.Parent;
                }
            }
            else
            {
                if (node.Index > 0) currentRow = node.Parent.Nodes[node.Index - 1];
                else currentRow = null;
            }
            if (currentRow != null)
            {
                while (currentRow.Level >= node.Level)
                {
                    if (currentRow.RowIndex < base.Rows.Count - 1)
                    {
                        currentRow = base.Rows[currentRow.RowIndex + 1] as SparkDataGridTreeNode;
                        Debug.Assert(currentRow != null);
                    }
                    else break;

                }
                if (currentRow == node.Parent) rowIndex = currentRow.RowIndex + 1;
                else if (currentRow.Level < node.Level) rowIndex = currentRow.RowIndex;
                else rowIndex = currentRow.RowIndex + 1;
            }
            else rowIndex = 0;
            Debug.Assert(rowIndex != -1);
            this.SiteNode(node, rowIndex);
            Debug.Assert(node.IsSited);
            if (node.IsExpanded)
            {
                foreach (SparkDataGridTreeNode childNode in node.Nodes)
                {
                    this.SiteNode(childNode);
                }
            }
        }

        internal protected virtual void SiteNode(SparkDataGridTreeNode node, int index)
        {
            if (index < base.Rows.Count)
            {
                base.Rows.Insert(index, node);
            }
            else base.Rows.Add(node);
        }

        internal protected virtual bool ExpandNode(SparkDataGridTreeNode node)
        {
            if (!node.IsExpanded || this.VirtualNodes)
            {
                SparkGridTreeExpandingEventArgs exp = new SparkGridTreeExpandingEventArgs(node);
                this.OnNodeExpanding(exp);
                if (!exp.Cancel)
                {
                    this.LockVerticalScrollBarUpdate(true);
                    this.SuspendLayout();
                    _inExpandCollapse = true;
                    node.IsExpanded = true;
                    foreach (SparkDataGridTreeNode childNode in node.Nodes)
                    {
                        Debug.Assert(childNode.RowIndex == -1, "Row is already in the grid.");
                        this.SiteNode(childNode);
                    }
                    SparkGridTreeExpandedEventArgs exped = new SparkGridTreeExpandedEventArgs(node);
                    this.OnNodeExpanded(exped);
                    _inExpandCollapse = false;
                    this.LockVerticalScrollBarUpdate(false);
                    this.ResumeLayout(true);
                    this.InvalidateCell(node.Cells[0]);
                }
                return !exp.Cancel;
            }
            else return false;
        }

        protected override void WndProc(ref Message m)
        {
            //Suppress WM_LBUTTONDBLCLK
            if (SuppressDbClick && m.Msg == 0x203)
            {
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //    if (!e.Handled)
        //    {
        //        if (e.KeyCode == Keys.F2 && this.CurrentCellAddress.X > -1 && this.CurrentCellAddress.Y > -1)
        //        {
        //            CellCanEdited();
        //        }
        //        else if (e.KeyCode == Keys.Enter && !this.IsCurrentCellInEditMode)
        //        {
        //            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        //            this.CurrentCell.OwningRow.Selected = true;
        //        }
        //    }
        //}

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            //CellCanEdited();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this._inExpandCollapseMouseCapture = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this._inExpandCollapseMouseCapture) base.OnMouseMove(e);
        }

        protected virtual void OnNodeExpanding(SparkGridTreeExpandingEventArgs e)
        {
            NodeExpanding?.Invoke(this, e);
        }

        protected virtual void OnNodeExpanded(SparkGridTreeExpandedEventArgs e)
        {
            NodeExpanded?.Invoke(this, e);
        }

        protected virtual void OnNodeCollapsing(SparkGridTreeCollapsingEventArgs e)
        {
            NodeCollapsing?.Invoke(this, e);
        }

        protected virtual void OnNodeCollapsed(SparkGridTreeCollapsedEventArgs e)
        {
            NodeCollapsed?.Invoke(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(Disposing);
            this.UnSiteAll();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            hideScrollBarControl = new Control();
            hideScrollBarControl.Visible = false;
            hideScrollBarControl.Enabled = false;
            hideScrollBarControl.TabStop = false;
            this.Controls.Add(hideScrollBarControl);
        }

        protected override void OnRowEnter(DataGridViewCellEventArgs e)
        {
            base.OnRowEnter(e);
            if (this.SelectionMode == DataGridViewSelectionMode.CellSelect ||
                (this.SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
                base.Rows[e.RowIndex].Selected == false))
            {
                this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                base.Rows[e.RowIndex].Selected = true;
            }
        }

        private void LockVerticalScrollBarUpdate(bool lockUpdate)
        {
            if (!this._inExpandCollapse)
            {
                if (lockUpdate)
                {
                    this.VerticalScrollBar.Parent = hideScrollBarControl;
                }
                else
                {
                    this.VerticalScrollBar.Parent = this;
                }
            }
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            if (typeof(SparkDataGridTreeColumn).IsAssignableFrom(e.Column.GetType()))
            {
                if (_expandableColumn == null)
                {
                    _expandableColumn = (SparkDataGridTreeColumn)e.Column;
                }
            }
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
            base.OnColumnAdded(e);
        }
        #endregion
    }
}