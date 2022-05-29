using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SparkControls.Controls
{
    /// <summary>
    /// <see cref="SparkListItem"/>的集合
    /// </summary>
    public class SparkListItemCollection : System.Collections.ObjectModel.Collection<SparkListItem>
    {
        #region 字段
        private readonly SparkList owner;
        #endregion

        #region 构造函数
        internal SparkListItemCollection(SparkList owner)
        {
            this.owner = owner;
        }
        #endregion

        public virtual void AddRange(SparkListItem[] items)
        {
            if (items == null || items.Length == 0)
            {
                return;
            }
            owner.BeginUpdate();
            foreach (var item in items)
            {
                base.Add(item);
            }
            owner.ResetCalcItem();
            owner.EndUpdate();
        }


        protected override void InsertItem(int index, SparkListItem item)
        {
            base.InsertItem(index, item);
            this.owner.ResetCalcItem();
            this.owner.Invalidate();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            this.owner.ResetCalcItem();
            this.owner.Invalidate();
        }

    }
}
