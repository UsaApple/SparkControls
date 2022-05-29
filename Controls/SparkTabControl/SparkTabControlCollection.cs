using System;
using System.ComponentModel;
using System.Linq;

namespace SparkControls.Controls
{
    /// <summary>
    /// <see cref="SparkTabControl"/> 集合类。
    /// </summary>
    [ToolboxItem(false)]
    public class SparkTabControlCollection : CollectionWithEvents
    {
        #region 私有变量

        /// <summary>
        /// 集合改变事件
        /// </summary>
        [Browsable(false)]
        public event CollectionChangeEventHandler CollectionChanged;

        private int lockUpdate;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造方法
        /// </summary>
        public SparkTabControlCollection()
        {
            lockUpdate = 0;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 根据索引返回标签页
        /// </summary>
        /// <param name="index">从 0 开始的索引号。</param>
        /// <returns></returns>
        public SparkTabPage this[int index]
        {
            get
            {
                if (index < 0 || List.Count - 1 < index)
                    return null;

                return (SparkTabPage)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public SparkTabPage this[string name]
        {
            get
            {
                if (this.Contains(name))
                {
                    return this.List.Cast<SparkTabPage>().FirstOrDefault(a => a.Name == name);
                }
                return null;
            }
        }

        /// <summary>
        /// 绘制的个数
        /// </summary>
        [Browsable(false)]
        public virtual int DrawnCount
        {
            get
            {
                if (this.Count == 0) return 0;
                return this.List.Cast<SparkTabPage>().Count(a => a.IsDrawn);
                //for (int n = 0; n < count; n++)
                //{
                //    if (this[n].IsDrawn)
                //        res++;
                //}
                //return res;
            }
        }

        /// <summary>
        /// 最后一个标签页
        /// </summary>
        public virtual SparkTabPage LastVisible
        {
            get
            {
                //for (int n = Count - 1; n > 0; n--)
                //{
                //    if (this[n].Visible)
                //        return this[n];
                //}
                //return null;
                return this.List.Cast<SparkTabPage>().LastOrDefault(a => a.Visible);
            }
        }

        /// <summary>
        /// 第一个标签页
        /// </summary>
        public virtual SparkTabPage FirstVisible
        {
            get
            {
                //for (int n = 0; n < Count; n++)
                //{
                //    if (this[n].Visible)
                //        return this[n];
                //}
                //return null;
                return this.List.Cast<SparkTabPage>().FirstOrDefault(a => a.Visible);
            }
        }

        /// <summary>
        /// 显示的个数
        /// </summary>
        [Browsable(false)]
        public virtual int VisibleCount
        {
            get
            {
                if (Count == 0) return 0;
                return this.List.Cast<SparkTabPage>().Count(a => a.Visible);
                //for (int n = 0; n < count; n++)
                //{
                //    if (this[n].Visible)
                //        res++;
                //}
                //return res;
            }
        }

        /// <summary>
        /// 获取固定项的数量
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        public virtual int FixedCount
        {
            get
            {
                if (this.Count == 0) return 0;
                return this.List.Cast<SparkTabPage>().Count(a => a.IsFixed);
            }
        }
        #endregion

        #region 重写事件
        /// <summary>
        /// 插入对象到容器中
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void OnInsertComplete(int index, object item)
        {
            SparkTabPage itm = item as SparkTabPage;
            itm.TextChanged += OnItem_Changed;
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
        }

        /// <summary>
        /// 移除容器中的对象
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void OnRemove(int index, object item)
        {
            base.OnRemove(index, item);
            SparkTabPage itm = item as SparkTabPage;
            itm.TextChanged -= OnItem_Changed;
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
        }

        /// <summary>
        /// 清空容器
        /// </summary>
        protected override void OnClear()
        {
            if (Count == 0) return;
            BeginUpdate();
            try
            {
                for (int n = Count - 1; n >= 0; n--)
                {
                    RemoveAt(n);
                }
            }
            finally
            {
                EndUpdate();
            }
        }
        #endregion

        #region protected方法
        /// <summary>
        /// Text改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnItem_Changed(object sender, SparkTabPageTextChangedEventArgs e)
        {
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void BeginUpdate()
        {
            lockUpdate++;
        }

        /// <summary>
        /// 结束更新
        /// </summary>
        protected virtual void EndUpdate()
        {
            if (--lockUpdate == 0)
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <summary>
        /// 从index位置开始获取下一个显示的TabPage，包含index
        /// </summary>
        /// <param name="index">从Index位置开始</param>
        /// <returns></returns>
        protected internal SparkTabPage NextTabPageVisible(int index)
        {
            if (this.Count > index)
            {
                for (var i = index; i < Count; i++)
                {
                    if (this[i].Visible)
                    {
                        return this[i];
                    }
                }
                return PreviousTabPageVisible(index);
            }
            else
            {
                return this.LastVisible;
            }
        }

        /// <summary>
        /// 从index位置开始获取上一个显示的TabPage,不包含index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected internal SparkTabPage PreviousTabPageVisible(int index)
        {
            if (this.Count > index)
            {
                for (var i = index - 1; i >= 0; i--)
                {
                    if (this[i].Visible)
                    {
                        return this[i];
                    }
                }
                return this.FirstVisible;
            }
            else
            {
                return this.LastVisible;
            }
        }

        internal void UnSelect()
        {
            var list = this.List.Cast<SparkTabPage>().Where(a => a.Selected);
            foreach (var item in list)
            {
                item.Selected = false;
                item.Hide();
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="items"></param>
        public virtual void AddRange(params SparkTabPage[] items)
        {
            BeginUpdate();
            try
            {
                foreach (SparkTabPage item in items)
                {
                    List.Add(item);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        public virtual void Assign(SparkTabControlCollection collection)
        {
            BeginUpdate();
            try
            {
                Clear();
                for (int n = 0; n < collection.Count; n++)
                {
                    SparkTabPage item = collection[n];
                    SparkTabPage newItem = new SparkTabPage();
                    newItem.Assign(item);
                    Add(newItem);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// 添加标签页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int Add(SparkTabPage item)
        {
            int res = this.IndexOf(item);
            if (res == -1) res = List.Add(item);
            return res;
        }

        /// <summary>
        /// 移除标签页
        /// </summary>
        /// <param name="item"></param>
        public virtual void Remove(SparkTabPage item)
        {
            if (List.Contains(item))
            {
                List.Remove(item);
            }
        }

        /// <summary>
        /// 移动标签页的位置
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual SparkTabPage MoveTo(int newIndex, SparkTabPage item)
        {
            try
            {
                int currentIndex = List.IndexOf(item);
                if (currentIndex == newIndex) return item;
                if (currentIndex >= 0 && this.Count > newIndex)
                {
                    RemoveAt(currentIndex);
                    Insert(newIndex, item);
                    return item;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// 返回标签页在集合中的索引，不存在返回-1
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int IndexOf(SparkTabPage item)
        {
            return List.IndexOf(item);
        }

        /// <summary>
        /// 判断标签页是否在集合中
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Contains(SparkTabPage item)
        {
            return List.Contains(item);
        }

        /// <summary>
        /// 插入标签页到集合指定的索引中
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public virtual void Insert(int index, SparkTabPage item)
        {
            if (Contains(item)) return;
            try
            {
                List.Insert(index, item);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 根据标签页名称判断标签页是否在集合中
        /// </summary>
        /// <param name="name">标签页名称</param>
        /// <returns></returns>
        public virtual bool Contains(string name)
        {
            return this.List.Cast<SparkTabPage>().Any(a => a.Name == name);
        }
        #endregion
    }
}