using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SparkControls.Controls
{
    public sealed class ShapeCollection : IList, IDisposable
    {
        private bool m_Disposed;
        private ShapeContainer m_OwnerShapeContainer;
        private List<Shape> m_Shapes = new List<Shape>();
        private object m_SyncRoot;

        [EditorBrowsable(EditorBrowsableState.Always)]
        public ShapeCollection(ShapeContainer owner)
        {
            this.m_OwnerShapeContainer = owner;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Add(Shape value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            ShapeContainer parent = value.Parent;
            if ((this.Owner != null) && this.Owner.Equals(parent))
            {
                if (!this.m_Shapes.Contains(value))
                {
                    throw new ArgumentException("ExceptionCollectionMethodAdd");
                }
                value.SendToBack();
            }
            else
            {
                if (parent != null)
                {
                    parent.Shapes.Remove(value);
                }
                if (!this.m_Shapes.Contains(value))
                {
                    this.m_Shapes.Add(value);
                    value.SetParent(this.m_OwnerShapeContainer);
                }
            }
        }

        int IList.Add(object value)
        {
            if(value is Shape shape)
			{
                Add(shape);
                return 1;
            }
            return 0;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void AddRange(Shape[] shapes)
        {
            if (shapes == null)
            {
                throw new ArgumentNullException("shapes");
            }
            foreach (Shape shape in shapes)
            {
                this.Add(shape);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Clear()
        {
            if (this.m_OwnerShapeContainer != null)
            {
                foreach (Shape shape in this.m_Shapes)
                {
                    shape.SetParent(null);
                }
            }
            this.m_Shapes.Clear();
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool Contains(Shape value)
        {
            return this.m_Shapes.Contains(value);
        }

        bool IList.Contains(object value)
        {
            return this.Contains((Shape)value);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool ContainsKey(string key)
        {
            return this.IsValidIndex(this.IndexOfKey(key));
        }

        void ICollection.CopyTo(Array array, int index)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void CopyTo(Shape[] array, int index)
        {
            this.m_Shapes.CopyTo(array, index);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        private void Dispose(bool disposing)
        {
            if (!this.m_Disposed && disposing)
            {
                foreach (Shape shape in this.m_Shapes)
                {
                    shape.Dispose();
                }
                this.m_Shapes.Clear();
                this.m_Shapes = null;
            }
            this.m_Disposed = true;
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public int GetChildIndex(Shape child)
        {
            return this.GetChildIndex(child, true);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public int GetChildIndex(Shape child, bool throwException)
        {
            int index = this.m_Shapes.IndexOf(child);
            if ((index == -1) && throwException)
            {
                throw new ArgumentException("The child shape is not in the ShapeCollection.");
            }
            return index;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this.m_Shapes.GetEnumerator();
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public int IndexOf(Shape value)
        {
            return this.m_Shapes.IndexOf(value);
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((Shape)value);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public int IndexOfKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                int num3 = this.Count - 1;
                for (int i = 0; i <= num3; i++)
                {
                    if (Utility.SafeStringEquals(this.m_Shapes[i].Name, key, true))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal void Insert(int index, Shape value)
        {
            if ((index < 0) || (index > this.m_Shapes.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (index == this.m_Shapes.Count)
            {
                this.Add(value);
            }
            else
            {
                this.Add(value);
                this.SetChildIndex(value, index);
            }
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (Shape)value);
        }

        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < this.Count));
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void Remove(Shape value)
        {
            if (this.m_Shapes.Contains(value) && this.m_Shapes.Remove(value))
            {
                value.SetParent(null);
            }
        }

        void IList.Remove(object value)
        {
            this.Remove((Shape)value);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void RemoveAt(int index)
        {
            Shape shape = this.m_Shapes[index];
            this.m_Shapes.RemoveAt(index);
            shape.SetParent(null);
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public void SetChildIndex(Shape child, int newIndex)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child");
            }
            if (newIndex < -1)
            {
                throw new ArgumentOutOfRangeException("newIndex");
            }
            int index = this.IndexOf(child);
            if (index < 0)
            {
                throw new ArgumentException("The child shape is not in the ShapeCollection.");
            }
            if (index != newIndex)
            {
                if ((newIndex < 0) || (newIndex >= this.m_Shapes.Count))
                {
                    this.m_Shapes.Remove(child);
                    this.m_Shapes.Add(child);
                }
                else
                {
                    this.m_Shapes.Remove(child);
                    if (newIndex == this.m_Shapes.Count)
                    {
                        this.m_Shapes.Add(child);
                    }
                    else
                    {
                        this.m_Shapes.Insert(newIndex, child);
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public int Count
        {
            get
            {
                return this.m_Shapes.Count;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        private bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        private bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public object this[int index]
        {
            get
            {
                return this.m_Shapes[index];
            }
            set
            {
                Shape shape = (Shape)value;
                if (shape == null)
                {
                    throw new ArgumentException("Item is not a Shape or is Nothing");
                }
                this.m_Shapes[index] = shape;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public ShapeContainer Owner
        {
            get
            {
                if ((this.m_OwnerShapeContainer != null) && !this.m_OwnerShapeContainer.IsDisposed)
                {
                    return this.m_OwnerShapeContainer;
                }
                return null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        private object SyncRoot
        {
            get
            {
                if (this.m_SyncRoot == null)
                {
                    Interlocked.CompareExchange(ref this.m_SyncRoot, RuntimeHelpers.GetObjectValue(new object()), null);
                }
                return this.m_SyncRoot;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        object ICollection.SyncRoot
        {
            get
            {
                if (this.m_SyncRoot == null)
                {
                    Interlocked.CompareExchange(ref this.m_SyncRoot, RuntimeHelpers.GetObjectValue(new object()), null);
                }
                return this.m_SyncRoot;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }
    }
}