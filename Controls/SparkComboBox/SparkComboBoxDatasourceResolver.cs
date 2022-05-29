using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SparkControls.Controls
{
	/// <summary>
	/// ComboBox 数据源解析器。
	/// </summary>
	public static class SparkComboBoxDatasourceResolver
	{
		/// <summary>
		/// 获取数据源项指定名称的成员属性的值。
		/// </summary>
		/// <param name="sourceItem">数据源项。</param>
		/// <param name="memberName">属性名称。</param>
		/// <returns>属性值。</returns>
		public static object GetMemberValue(object sourceItem, string memberName)
		{
			if (sourceItem == null) { throw new ArgumentNullException(nameof(sourceItem), "值不能为空。"); }

			// 字符串
			if (sourceItem is string || memberName.IsNullOrEmpty())
			{
				return sourceItem.ToString();
			}

			// DataTable、DataView
			DataRow dr = null;
			if (sourceItem is DataRowView)
			{
				dr = (sourceItem as DataRowView).Row;
			}
			else if (sourceItem is DataRow)
			{
				dr = sourceItem as DataRow;
			}
			if (dr != null)
			{
				if (dr.Table.Columns.Contains(memberName))
				{
					return dr[memberName];
				}
				else
				{
					throw new ArgumentException("指定的成员名称不存在。", nameof(memberName));
				}
			}

			// IEnumerable
			PropertyInfo pi = sourceItem.GetType().GetProperty(memberName);
			if (pi != null)
			{
				return pi.GetValue(sourceItem, null);
			}

			return sourceItem.ToString();
		}

		/// <summary>
		/// 获取指定数据源项的索引号。
		/// </summary>
		/// <param name="dataSource">数据源对象。</param>
		/// <param name="sourceItem">数据源项。</param>
		/// <returns>当前数据源项的索引号。</returns>
		public static int GetItemIndex(object dataSource, object sourceItem)
		{
			if (dataSource == null) { throw new ArgumentNullException(nameof(dataSource), "值不能为空。"); }
			if (sourceItem == null) { throw new ArgumentNullException(nameof(sourceItem), "值不能为空。"); }

			DataRow dr = null;
			if (sourceItem is DataRow)
			{
				dr = sourceItem as DataRow;
			}
			else if (sourceItem is DataRowView)
			{
				dr = (sourceItem as DataRowView).Row;
			}

			if (dataSource is DataTable dt)
			{
				return dt.Rows.IndexOf(dr);
			}
			if (dataSource is DataView dv)
			{
				return dv.Table.Rows.IndexOf(dr);
			}
			if (dataSource is IEnumerable enumerable)
			{
				return Array.IndexOf(enumerable.Cast<object>().ToArray(), sourceItem);
			}
			throw new NotSupportedException("不支持的数据源类型。");
		}

		/// <summary>
		/// 获取数据源中指定成员属性值的项。
		/// </summary>
		/// <param name="dataSource">数据源对象。</param>
		/// <param name="memberName">属性名称。</param>
		/// <param name="memberValue">属性值。</param>
		/// <returns>数据源项。</returns>
		public static object GetItemByMemeberValue(object dataSource, string memberName, object memberValue)
		{
			if (dataSource == null) { throw new ArgumentNullException(nameof(dataSource), "值不能为空。"); }
			if (!memberName.IsNullOrEmpty())
			{
				if (dataSource is DataTable dt)
				{
					if (dt.Columns.Contains(memberName))
					{
						return dt.AsEnumerable().FirstOrDefault(dr => dr[memberName].Equals(memberValue));
					}
					else
					{
						throw new ArgumentException("指定的列名不存在。", nameof(memberName));
					}
				}
				if (dataSource is DataView dv)
				{
					DataTable dvt = dv.Table;
					if (dvt.Columns.Contains(memberName))
					{
						return dvt.AsEnumerable().FirstOrDefault(dr => dr[memberName].Equals(memberValue));
					}
					else
					{
						throw new ArgumentException("指定的列名不存在。", nameof(memberName));
					}
				}
			}
			if (dataSource is IEnumerable enumerable)
			{
				if (memberName.IsNullOrEmpty())
				{
					return enumerable.Cast<object>().FirstOrDefault(o => o?.ToString() == memberValue?.ToString());
				}
				else
				{
					return enumerable.Cast<object>().FirstOrDefault(o => GetMemberValue(o, memberName)?.Equals(memberValue) == true);
				}
			}
			throw new NotSupportedException("不支持的数据源类型。");
		}

		/// <summary>
		/// 获取数据源中指定索引号的项。
		/// </summary>
		/// <param name="dataSource">数据源对象。</param>
		/// <param name="indices">索引集合。</param>
		/// <returns>数据源项的集合。</returns>
		public static IEnumerable<object> GetItemByIndex(object dataSource, params int[] indices)
		{
			if (dataSource == null) { throw new ArgumentNullException(nameof(dataSource), "值不能为空。"); }
			if (indices != null && indices.Length > 0)
			{
				if (dataSource is DataTable dt)
				{
					return indices.Select(i =>
					{
						if (i >= 0 && i < dt.Rows.Count)
						{
							dt.DefaultView.RowFilter = string.Empty;
							return dt.DefaultView[i];
						}
						else
						{
							return null;
						}
					}).Where(row => row != null);
				}
				if (dataSource is DataView dv)
				{
					return indices.Select(i =>
					{
						if (i >= 0 && i < dv.Count)
						{
							return dv[i];
						}
						else
						{
							return null;
						}
					}).Where(row => row != null);
				}
				if (dataSource is IEnumerable enumerable)
				{
					var objs = enumerable.Cast<object>();
					return indices.Select(i =>
					{
						if (i >= 0 && i < objs.Count())
						{
							return objs.ElementAt(i);
						}
						else
						{
							return null;
						}
					}).Where(obj => obj != null);
				}
				throw new NotSupportedException("不支持的数据源类型。");
			}
			return null;
		}

		/// <summary>
		/// 将指定数据源转换为项的集合。
		/// </summary>
		/// <param name="dataSource">数据源对象。</param>
		/// <returns>数据源项的集合。</returns>
		public static IEnumerable<object> Convert2ItemCollection(object dataSource)
		{
			if (dataSource == null) { throw new ArgumentNullException(nameof(dataSource), "值不能为空。"); }

			if (dataSource is DataTable dt)
			{
				return dt.AsEnumerable();
			}
			if (dataSource is DataView dv)
			{
				return dv.Cast<DataRowView>();
			}
			if (dataSource is IEnumerable enumerable)
			{
				return enumerable.Cast<object>();
			}
			throw new NotSupportedException("不支持的数据源类型。");
		}

		/// <summary>
		/// 获取数据源包含的项的数量。
		/// </summary>
		/// <param name="dataSource">数据源对象。</param>
		/// <returns>数据源包含的项的数量。</returns>
		public static int GetItemCount(object dataSource)
		{
			return Convert2ItemCollection(dataSource).Count();
		}

		/// <summary>
		/// 确定指定的集合是否被视为相等。
		/// </summary>
		/// <param name="source">源集合。</param>
		/// <param name="target">目标集合。</param>
		/// <returns>如果对象被视为相等，则返回 true，否则返回 false。如果 source 和 target 均为 null，返回 true。</returns>
		public static bool Equals<T>(IEnumerable<T> source, IEnumerable<T> target)
		{
			if (source == null && target == null) return true;
			else if (source != null && target == null) return false;
			else if (source == null && target != null) return false;
			else if (source.Count() != target.Count()) return false;
			else return !source.Except(target).Any();
		}

		/// <summary>
		/// 确定指定的对象是否被视为相等。
		/// </summary>
		/// <param name="source">源对象。</param>
		/// <param name="target">目标对象。</param>
		/// <returns>如果对象被视为相等，则返回 true，否则返回 false。如果 source 和 target 均为 null，返回 true。</returns>
		public static new bool Equals(object source, object target)
		{
			if (source is DataRowView drvSource && target is DataRowView drvTarget)
			{
				return Enumerable.SequenceEqual(drvSource.Row.ItemArray, drvTarget.Row.ItemArray);
			}
			if (source is DataRow drSource && target is DataRow drTarget)
			{
				return Enumerable.SequenceEqual(drSource.ItemArray, drTarget.ItemArray);
			}
			if (source is IEnumerable eSource && target is IEnumerable eTarget)
			{
				return Enumerable.SequenceEqual(eSource.Cast<object>(), eTarget.Cast<object>());
			}
			return object.Equals(source, target);
		}
	}
}