using System;
using System.Collections;
using System.Collections.Generic;

namespace SparkControls.Controls
{
	public class SparkDataGridTreeNodeCollection : IList<SparkDataGridTreeNode>, IList
    {
        internal List<SparkDataGridTreeNode> _list;
        internal SparkDataGridTreeNode _owner;

        internal SparkDataGridTreeNodeCollection(SparkDataGridTreeNode owner)
        {
            this._owner = owner;
            this._list = new List<SparkDataGridTreeNode>();
        }

        #region Public Members

        public void Add(SparkDataGridTreeNode item)
        {
            item._grid = this._owner._grid;
            bool hadChildren = this._owner.HasChildren;
            item._owner = this;
            this._list.Add(item);
            this._owner.AddChildNode(item);
            if (!hadChildren && this._owner.IsSited)
            {
                this._owner._grid.InvalidateRow(this._owner.RowIndex);
            }
        }

        public SparkDataGridTreeNode Add(string text)
        {
            SparkDataGridTreeNode node = new SparkDataGridTreeNode();
            this.Add(node);
            node.Cells[0].Value = text;
            return node;
        }

        public SparkDataGridTreeNode Add(params object[] values)
        {
            SparkDataGridTreeNode node = new SparkDataGridTreeNode();
            this.Add(node);
            int cell = 0;
            if (values.Length > node.Cells.Count) throw new ArgumentOutOfRangeException("values");
            foreach (object o in values)
            {
                node.Cells[cell].Value = o;
                cell++;
            }
            return node;
        }

        public void Insert(int index, SparkDataGridTreeNode item)
        {
            item._grid = this._owner._grid;
            item._owner = this;
            this._list.Insert(index, item);
            this._owner.InsertChildNode(index, item);
        }

        public bool Remove(SparkDataGridTreeNode item)
        {
            this._owner.RemoveChildNode(item);
            item._grid = null;
            return this._list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            SparkDataGridTreeNode row = this._list[index];
            this._owner.RemoveChildNode(row);
            row._grid = null;
            this._list.RemoveAt(index);
        }

        public void Clear()
        {
            this._owner.ClearNodes();
            this._list.Clear();
        }

        public int IndexOf(SparkDataGridTreeNode item)
        {
            return this._list.IndexOf(item);
        }

        public SparkDataGridTreeNode this[int index]
        {
            get
            {
                return this._list[index];
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool Contains(SparkDataGridTreeNode item)
        {
            return this._list.Contains(item);
        }

        public void CopyTo(SparkDataGridTreeNode[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { return this._list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IList 接口

        void IList.Remove(object value)
        {
            this.Remove(value as SparkDataGridTreeNode);
        }
        
        int IList.Add(object value)
        {
            SparkDataGridTreeNode item = value as SparkDataGridTreeNode;
            this.Add(item);
            return item.Index;
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        int IList.IndexOf(object item)
        {
            return this.IndexOf(item as SparkDataGridTreeNode);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, value as SparkDataGridTreeNode);
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        bool IList.Contains(object value)
        {
            return this.Contains(value as SparkDataGridTreeNode);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
               
        #region IEnumerable<ExpandableRow> 接口

        public IEnumerator<SparkDataGridTreeNode> GetEnumerator()
        {
            return this._list.GetEnumerator();
        }

        #endregion
        
        #region IEnumerable 接口

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #endregion

        #region ICollection Members

        bool ICollection.IsSynchronized
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        object ICollection.SyncRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
