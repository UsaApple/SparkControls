using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
	/// <summary>
	/// 控件扩展功能类。
	/// </summary>
	public static class Extension
	{
		/// <summary>
		/// 获取控件数据。
		/// </summary>
		/// <typeparam name="T">返回数据的类型。</typeparam>
		/// <param name="control">要获取数据的控件。</param>
		/// <returns>获取到的数据。</returns>
		public static T GetData<T>(this Control control) where T : class, new()
		{
			if (control == null) { return default; }

			if (typeof(T) == typeof(DataTable))
			{
				DataTable dt = new DataTable();
				control.GetData(ref dt);
				return (T)Converter.ChangeType(dt, typeof(T));
			}
			else
			{
				T o = new T();
				control.GetData(ref o);
				return o;
			}
		}

		/// <summary>
		/// 获取控件数据。
		/// </summary>
		/// <typeparam name="T">返回数据的类型。</typeparam>
		/// <param name="control">要获取数据的控件。</param>
		/// <param name="obj">用于存储数据的对象。</param>
		public static void GetData<T>(this Control control, ref T obj) where T : class, new()
		{
			if (control == null) { return; }

			object GetBindingValue(IDataBinding binding, bool joinCollection = true)
			{
				if (binding == null) { throw new ArgumentNullException(nameof(binding), "值不能为空。"); }
				if (binding.Value == null) { return null; }

				if (binding.Value.GetType() != typeof(string) && binding.Value is IEnumerable values)
				{
					// 多选项的处理
					return string.Join(",", values.Cast<object>().Where(x => x != null).Select(x => x.ToString()));
				}
				else
				{
					return binding.Value;
				}
			}

			try
			{
				List<IDataBinding> bindings = FindBindingControl(control).Where(b => !b.FieldName.IsNullOrEmpty()).ToList();
				if (typeof(T) == typeof(DataTable))
				{
					DataTable dt = obj == null ? new DataTable() : obj as DataTable;
					foreach (var binding in bindings)
					{
						if (!dt.HasColumn(binding.FieldName))
						{
							dt.Columns.Add(new DataColumn(binding.FieldName, binding.Value is IEnumerable ? typeof(string) : binding.Value.GetType()));
						}
						if (binding is IDualDataBinding dualBinding && !dualBinding.DisplayFieldName.IsNullOrEmpty() && !dt.HasColumn(dualBinding.DisplayFieldName))
						{
							dt.Columns.Add(new DataColumn(dualBinding.DisplayFieldName, dualBinding.DisplayValue is IEnumerable ? typeof(string) : dualBinding.DisplayValue.GetType()));
						}
					}

					DataRow dr = dt.NewRow();
					foreach (IDataBinding binding in bindings)
					{
						// RadioButton 需要特殊处理，未勾选的跳过
						if (binding is SparkRadioButton radio && !radio.Checked) { continue; }

						dr[binding.FieldName] = GetBindingValue(binding);
						if (binding is IDualDataBinding dualBinding && !dualBinding.DisplayFieldName.IsNullOrEmpty())
						{
							dr[dualBinding.DisplayFieldName] = dualBinding.DisplayValue;
						}
					}
					dt.Rows.Add(dr);
				}
				else
				{
					T o = obj ?? new T();
					if (o is EntityBase entity)
					{
						foreach (IDataBinding binding in bindings)
						{
							// RadioButton 需要特殊处理，未勾选的跳过
							if (binding is SparkRadioButton radio && !radio.Checked) { continue; }

							if (entity.HasProperty(binding.FieldName))
							{
								entity.SetValue(binding.FieldName, binding.Value);
							}

							if (binding is IDualDataBinding dualBinding && entity.HasProperty(dualBinding.DisplayFieldName))
							{
								entity.SetValue(dualBinding.DisplayFieldName, dualBinding.DisplayValue);
							}
						}
					}
					else
					{
						var properties = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);
						foreach (IDataBinding binding in bindings)
						{
							// RadioButton 需要特殊处理，未勾选的跳过
							if (binding is SparkRadioButton radio && !radio.Checked) { continue; }

							PropertyInfo property = properties.FirstOrDefault(p => p.Name == binding.FieldName);
							if (property != null)
							{
								var joinCollection = binding.Value is IEnumerable values && binding.Value.GetType() != typeof(string) && property.PropertyType == typeof(string);
								property.SetValue(o, Converter.ChangeType(GetBindingValue(binding, joinCollection), property.PropertyType), null);
							}

							if (binding is IDualDataBinding dualBinding)
							{
								property = properties.FirstOrDefault(p => p.Name == dualBinding.DisplayFieldName);
								if (property != null)
								{
									property.SetValue(o, Converter.ChangeType(dualBinding.DisplayValue, property.PropertyType), null);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Comm.Logger.WriteErr(e.ToString());
			}
		}

		/// <summary>
		/// 设置控件数据。
		/// </summary>
		/// <param name="control">要设置数据的控件。</param>
		/// <param name="source">要设置的数据源。</param>
		public static void SetData(this Control control, object source)
		{
			if (control == null || source == null) { return; }
			List<IDataBinding> bindings = FindBindingControl(control).Where(b => !b.FieldName.IsNullOrEmpty()).ToList();

			/**
			 * 给控件赋值。
			 */
			void SetBindingValue(IDataBinding binding, object value)
			{
				if (binding == null) { return; }
				if (value == null) { binding.Value = value; return; }

				if (value.GetType().IsEnum == true)
				{
					// 枚举存储数字的处理
					binding.Value = value;
				}
				else if (value is string str)
				{
					// 多选项的处理
					string[] strs = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
					if (binding.Value is IList<string>)
					{
						binding.Value = new List<string>(strs);
					}
					else if (binding.Value is Array || binding.Value is IEnumerable<string>)
					{
						binding.Value = strs;
					}
					else
					{
						binding.Value = value;
					}
				}
				else
				{
					binding.Value = value;
				}
			}

			/**
			 * 用实体对象给控件赋值。
			 */
			void SetDataWithEntity(object obj)
			{
				if (obj is EntityBase entity)
				{
					foreach (IDataBinding binding in bindings)
					{
						if (entity.HasProperty(binding.FieldName))
						{
							object value = entity.GetValue(binding.FieldName);
							SetBindingValue(binding, value);
						}
					}
				}
				else if (obj != null)
				{
					var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);
					foreach (IDataBinding binding in bindings)
					{
						PropertyInfo property = properties.FirstOrDefault(p => p.Name == binding.FieldName);
						if (property != null)
						{
							object value = property.GetValue(obj, null);
							SetBindingValue(binding, value);
						}
					}
				}
			}

			try
			{
				if (source is DataTable dt && dt.HasRow())
				{
					foreach (IDataBinding binding in bindings)
					{
						if (dt.HasColumn(binding.FieldName))
						{
							SetBindingValue(binding, dt.Rows[0][binding.FieldName]);
						}
					}
				}
				else if (source is DataView dv && dv.Count > 0)
				{
					foreach (IDataBinding binding in bindings)
					{
						if (dv.Table.HasColumn(binding.FieldName))
						{
							SetBindingValue(binding, dv[0][binding.FieldName]);
						}
					}
				}
				else if (source is IEnumerable ie)
				{
					IEnumerable<object> objs = ie.Cast<object>();
					foreach (object obj in objs)
					{
						SetDataWithEntity(obj);
					}
				}
				else
				{
					SetDataWithEntity(source);
				}
			}
			catch (Exception e)
			{
				Comm.Logger.WriteErr(e.ToString());
			}
		}

		/// <summary>
		/// 清空控件数据。
		/// </summary>
		/// <param name="control">要清空数据的控件。</param>
		public static void ClearData(this Control control)
		{
			if (control == null) { return; }

			List<IDataBinding> bindings = FindBindingControl(control).Where(b => !b.FieldName.IsNullOrEmpty()).ToList();
			foreach (IDataBinding binding in bindings)
			{
				binding.Value = null;
			}
		}

		/// <summary>
		/// 通过模态对话框的方式弹出。
		/// </summary>
		/// <param name="control">要弹出的控件。</param>
		/// <param name="title">窗体标题。</param>
		/// <param name="icon">窗体图标。</param>
		/// <returns>对话框返回值。</returns>
		public static DialogResult ShowDialog(this Control control, string title = null, Icon icon = null)
		{
			if (control == null) { return DialogResult.None; }
			if (title.IsNullOrEmpty())
			{
				title = control.Text.IsNullOrEmpty() ? control.Name : control.Text; ;
			}
			control.Dock = DockStyle.Fill;

			Form form = new Form()
			{
				Text = title,
				Size = control.Size,
				MaximizeBox = false,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				StartPosition = FormStartPosition.CenterScreen,
			};
			if (icon != null) { form.Icon = icon; }
			form.Controls.Add(control);
			return form.ShowDialog();
		}

		/// <summary>
		/// 递归查找数据绑定控件。
		/// </summary>
		/// <param name="ctrl">要查找的控件对象。</param>
		/// <returns>数据绑定控件列表。</returns>
		private static List<IDataBinding> FindBindingControl(Control ctrl)
		{
			List<IDataBinding> ctrls = new List<IDataBinding>();
			if (ctrl == null) return ctrls;

			if (ctrl is IDataBinding binding)
			{
				ctrls.Add(binding);
			}
			else if (ctrl.Controls.Count > 0)
			{
				foreach (Control c in ctrl.Controls)
				{
					if (c is IDataBinding cBinding)
					{
						ctrls.Add(cBinding);
					}
					else
					{
						List<IDataBinding> cBindings = FindBindingControl(c);
						if (cBindings.Count > 0)
						{
							ctrls.AddRange(cBindings);
						}
					}
				}
			}
			return ctrls;
		}
	}
}