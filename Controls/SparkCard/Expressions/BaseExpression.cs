using System;
using System.Data;

namespace SparkControls.Controls
{
    /// <summary>
    /// 表达式组合类型的基类。
    /// </summary>
    /// <typeparam name="T">组合类型的基础类型。</typeparam>
    [Serializable]
    public abstract class BaseExpression<T> : System.MarshalByRefObject
    {
        [NonSerialized]
        private DataRowView mDataSource = null;
        private T mEvaluatedValue = default;

        /// <summary>
        /// 计算表达式的值。
        /// </summary>
        /// <param name="dataSource">表示数据源的 DataRowView 实例。</param>
        /// <returns>表达式的计算值。</returns>
        public abstract T GetValue(DataRowView dataSource);

        /// <summary>
        /// 获取表达式指定数据源的计算值。
        /// </summary>
        /// <param name="dataSource">表示数据源的 DataRowView 实例。</param>
        /// <returns>表达式的计算值。</returns>
        public virtual T this[DataRowView dataSource]
        {
            get
            {
                if (dataSource == null || dataSource == mDataSource)
                {
                    return mEvaluatedValue;
                }
                else
                {
                    mDataSource = dataSource;
                    return mEvaluatedValue = GetValue(dataSource);
                }
            }
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}