using System;
using System.Collections;
using System.Collections.Generic;

namespace SparkControls.Controls
{
	public partial class SparkNavigationBar
	{
		/// <summary>
		/// NavigateBarPane集合
		/// </summary>
		public sealed class NavigateBarPanelCollection : IList<SparkNavigateBarPanel>, IList
		{
			private readonly SparkNavigationBar _owner;

			internal NavigateBarPanelCollection(SparkNavigationBar owner)
			{
				this._owner = owner;
			}

			#region IList<SparkNavigateBarPanel> Members

			public int IndexOf(SparkNavigateBarPanel item)
			{
				return this._owner._pages.IndexOf(item);
			}

			public void Insert(int index, SparkNavigateBarPanel item)
			{
				this._owner.Controls.Add(item);
				this._owner.Controls.SetChildIndex(item, index);
			}

			public void RemoveAt(int index)
			{
				this._owner.Controls.RemoveAt(index);
			}

			public SparkNavigateBarPanel this[int index]
			{
				get { return this._owner._pages[index]; }
				set
				{
					throw new NotSupportedException();
				}
			}

			#endregion

			#region ICollection<SparkNavigateBarPanel> Members

			public void Add(SparkNavigateBarPanel item)
			{
				this._owner.Controls.Add(item);
			}

			public void AddRange(SparkNavigateBarPanel[] items)
			{
				this._owner.Controls.AddRange(items);
			}

			public void Clear()
			{
				this._owner.Controls.Clear();
			}

			public bool Contains(SparkNavigateBarPanel item)
			{
				return this._owner._pages.Contains(item);
			}

			public void CopyTo(SparkNavigateBarPanel[] array, int arrayIndex)
			{
				this._owner._pages.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return this._owner._pages.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public bool Remove(SparkNavigateBarPanel item)
			{
				bool result = this._owner.Controls.Contains(item);
				this._owner.Controls.Remove(item);

				return result;
			}

			#endregion

			#region IEnumerable<SparkNavigateBarPanel> Members

			public IEnumerator<SparkNavigateBarPanel> GetEnumerator()
			{
				return this._owner._pages.GetEnumerator();
			}

			#endregion

			#region IList Members

			int IList.Add(object value)
			{
				this.Add((SparkNavigateBarPanel)(value));
				return this.Count - 1;
			}

			bool IList.Contains(object value)
			{
				return this.Contains((SparkNavigateBarPanel)(value));
			}

			int IList.IndexOf(object value)
			{
				return this.IndexOf((SparkNavigateBarPanel)(value));
			}

			void IList.Insert(int index, object value)
			{
				this.Insert(index, (SparkNavigateBarPanel)(value));
			}

			bool IList.IsFixedSize
			{
				get { return false; }
			}

			void IList.Remove(object value)
			{
				this.Remove((SparkNavigateBarPanel)(value));
			}

			void IList.RemoveAt(int index)
			{
				this.RemoveAt(index);
			}

			object IList.this[int index]
			{
				get { return this[index]; }
				set { this[index] = (SparkNavigateBarPanel)(value); }
			}

			#endregion

			#region ICollection Members

			void ICollection.CopyTo(Array array, int index)
			{
				this.CopyTo((SparkNavigateBarPanel[])(array), index);
			}

			bool ICollection.IsSynchronized
			{
				get { return false; }
			}

			object ICollection.SyncRoot
			{
				get { return this; }
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			#endregion
		}
	}
}