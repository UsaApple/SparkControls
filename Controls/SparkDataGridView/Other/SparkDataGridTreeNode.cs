using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class SparkDataGridTreeNode : DataGridViewRow
	{
		internal SparkDataGridTreeView _grid;
		internal SparkDataGridTreeNode _parent;
		internal SparkDataGridTreeNodeCollection _owner;
		internal bool IsExpanded;
		internal bool IsRoot;
		internal bool _isSited;
		internal bool _isFirstSibling;
		internal bool _isLastSibling;
		internal Image _image;
		internal int _imageIndex;
		private readonly Random rndSeed = new Random();
		public int UniqueValue = -1;
		private SparkDataGridTreeCell _treeCell;
		private SparkDataGridTreeNodeCollection childrenNodes;
		private int _index;
		private int _level;
		private bool childCellsCreated = false;
		private ISite site = null;
		private EventHandler disposed = null;

		internal SparkDataGridTreeNode(SparkDataGridTreeView owner) : this()
		{
			this._grid = owner;
			this.IsExpanded = true;
		}

		public SparkDataGridTreeNode()
		{
			this._index = -1;
			this._level = -1;
			this.IsExpanded = false;
			this.UniqueValue = this.rndSeed.Next();
			this._isSited = false;
			this._isFirstSibling = false;
			this._isLastSibling = false;
			this._imageIndex = -1;
		}

		public override object Clone()
		{
			SparkDataGridTreeNode r = (SparkDataGridTreeNode)base.Clone();
			r.UniqueValue = -1;
			r._level = this._level;
			r._grid = this._grid;
			r._parent = this.Parent;
			r._imageIndex = this._imageIndex;
			if (r._imageIndex == -1) r.Image = this.Image;
			r.IsExpanded = this.IsExpanded;
			return r;
		}

		protected internal virtual void UnSited()
		{
			SparkDataGridTreeCell cell;
			foreach (DataGridViewCell DGVcell in this.Cells)
			{
				cell = DGVcell as SparkDataGridTreeCell;
				if (cell != null) cell.UnSited();
			}
			this._isSited = false;
		}

		protected internal virtual void Sited()
		{
			this._isSited = true;
			this.childCellsCreated = true;
			Debug.Assert(this._grid != null);
			SparkDataGridTreeCell cell;
			foreach (DataGridViewCell DGVcell in this.Cells)
			{
				cell = DGVcell as SparkDataGridTreeCell;
				if (cell != null) cell.Sited();
			}
		}

		[System.ComponentModel.Description("Represents the index of this row in the Grid. Advanced usage."),
		 System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced),
		 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int RowIndex => base.Index;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Index
		{
			get
			{
				if (this._index == -1) this._index = this._owner.IndexOf(this);
				return this._index;
			}
			internal set => this._index = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ImageList ImageList
		{
			get
			{
				if (this._grid != null) return this._grid.ImageList;
				else return null;
			}
		}

		private bool ShouldSerializeImageIndex()
		{
			return (this._imageIndex != -1 && this._image == null);
		}

		[Category("Appearance"), Description("..."), DefaultValue(-1), TypeConverter(typeof(ImageIndexConverter)),
		 Editor("System.Windows.Forms.Design.ImageIndexEditor", typeof(UITypeEditor))]
		public int ImageIndex
		{
			get => this._imageIndex;
			set
			{
				this._imageIndex = value;
				if (this._imageIndex != -1) this._image = null;
				if (this._isSited)
				{
					this._treeCell.UpdateStyle();
					if (this.Displayed) this._grid.InvalidateRow(this.RowIndex);
				}
			}
		}

		private bool ShouldSerializeImage()
		{
			return (this._imageIndex == -1 && this._image != null);
		}

		public Image Image
		{
			get
			{
				if (this._image == null && this._imageIndex != -1)
				{
					if (this.ImageList != null && this._imageIndex < this.ImageList.Images.Count)
					{
						return this.ImageList.Images[this._imageIndex];
					}
					else return null;
				}
				else return this._image;
			}
			set
			{
				this._image = value;
				if (this._image != null) this._imageIndex = -1;
				if (this._isSited)
				{
					this._treeCell.UpdateStyle();
					if (this.Displayed) this._grid.InvalidateRow(this.RowIndex);
				}
			}
		}

		protected override DataGridViewCellCollection CreateCellsInstance()
		{
			DataGridViewCellCollection cells = base.CreateCellsInstance();
			cells.CollectionChanged += this.Cells_CollectionChanged;
			return cells;
		}

		private void Cells_CollectionChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
		{
			if (this._treeCell != null) return;
			if (e.Action == CollectionChangeAction.Add || e.Action == CollectionChangeAction.Refresh)
			{
				SparkDataGridTreeCell treeCell = null;
				if (e.Element == null)
				{
					foreach (DataGridViewCell cell in base.Cells)
					{
						if (cell.GetType().IsAssignableFrom(typeof(SparkDataGridTreeCell)))
						{
							treeCell = cell as SparkDataGridTreeCell;
							break;
						}
					}
				}
				else treeCell = e.Element as SparkDataGridTreeCell;
				if (treeCell != null) this._treeCell = treeCell;
			}
		}

		[Category("Data"), Description("The collection of root nodes in the treelist."),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		public SparkDataGridTreeNodeCollection Nodes
		{
			get
			{
				if (this.childrenNodes == null)
				{
					this.childrenNodes = new SparkDataGridTreeNodeCollection(this);
				}
				return this.childrenNodes;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DataGridViewCellCollection Cells
		{
			get
			{
				if (!this.childCellsCreated && this.DataGridView == null)
				{
					if (this._grid == null) return null;
					this.CreateCells(this._grid);
					this.childCellsCreated = true;
				}
				return base.Cells;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Level
		{
			get
			{
				if (this._level == -1)
				{
					int walk = 0;
					SparkDataGridTreeNode walkRow = this.Parent;
					while (walkRow != null)
					{
						walk++;
						walkRow = walkRow.Parent;
					}
					this._level = walk;
				}
				return this._level;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SparkDataGridTreeNode Parent => this._parent;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool HasChildren => this.childrenNodes != null && this.Nodes.Count != 0;

		[Browsable(false)]
		public bool IsSited => this._isSited;

		[Browsable(false)]
		public bool IsFirstSibling => this.Index == 0;

		[Browsable(false)]
		public bool IsLastSibling
		{
			get
			{
				SparkDataGridTreeNode parent = this.Parent;
				if (parent != null && parent.HasChildren)
				{
					return this.Index == parent.Nodes.Count - 1;
				}
				else return true;
			}
		}

		public virtual bool Collapse()
		{
			return this._grid.CollapseNode(this);
		}

		public virtual bool Expand()
		{
			if (this._grid != null) return this._grid.ExpandNode(this);
			else
			{
				this.IsExpanded = true;
				return true;
			}
		}

		protected internal virtual bool InsertChildNode(int index, SparkDataGridTreeNode node)
		{
			node._parent = this;
			node._grid = this._grid;
			if (this._grid != null) this.UpdateChildNodes(node);
			if ((this._isSited || this.IsRoot) && this.IsExpanded) this._grid.SiteNode(node);
			return true;
		}

		protected internal virtual bool InsertChildNodes(int index, params SparkDataGridTreeNode[] nodes)
		{
			foreach (SparkDataGridTreeNode node in nodes)
			{
				this.InsertChildNode(index, node);
			}
			return true;
		}

		protected internal virtual bool AddChildNode(SparkDataGridTreeNode node)
		{
			node._parent = this;
			node._grid = this._grid;
			if (this._grid != null) this.UpdateChildNodes(node);
			if ((this._isSited || this.IsRoot) && this.IsExpanded && !node._isSited) this._grid.SiteNode(node);
			return true;
		}

		protected internal virtual bool AddChildNodes(params SparkDataGridTreeNode[] nodes)
		{
			foreach (SparkDataGridTreeNode node in nodes)
			{
				this.AddChildNode(node);
			}
			return true;
		}

		protected internal virtual bool RemoveChildNode(SparkDataGridTreeNode node)
		{
			if ((this.IsRoot || this._isSited) && this.IsExpanded)
			{
				this._grid.UnSiteNode(node);
			}
			node._grid = null;
			node._parent = null;
			return true;
		}

		protected internal virtual bool ClearNodes()
		{
			if (this.HasChildren)
			{
				for (int i = this.Nodes.Count - 1; i >= 0; i--)
				{
					this.Nodes.RemoveAt(i);
				}
			}
			return true;
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public event EventHandler Disposed
		{
			add
			{
				this.disposed += value;
			}
			remove
			{
				this.disposed -= value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISite Site
		{
			get
			{
				return this.site;
			}
			set
			{
				this.site = value;
			}
		}

		private void UpdateChildNodes(SparkDataGridTreeNode node)
		{
			if (node.HasChildren)
			{
				foreach (SparkDataGridTreeNode childNode in node.Nodes)
				{
					childNode._grid = node._grid;
					this.UpdateChildNodes(childNode);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(36);
			sb.Append("TreeGridNode { Index=");
			sb.Append(this.RowIndex.ToString(System.Globalization.CultureInfo.CurrentCulture));
			sb.Append(" }");
			return sb.ToString();
		}
	}
}