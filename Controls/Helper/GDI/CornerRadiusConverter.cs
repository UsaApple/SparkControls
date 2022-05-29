using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace SparkControls.Controls
{
	/// <summary>
	/// 将 <see cref="CornerRadius"/> 从一种数据类型转换为另一种数据类型。
	/// </summary>
	public class CornerRadiusConverter : TypeConverter
	{
		/// <summary>
		/// 返回该转换器是否可以使用指定上下文将给定类型的对象转换为此转换器的类型。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <param name="sourceType">表示要转换的类型。</param>
		/// <returns>true：可以转换，false：不可以转换。</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// 返回该转换器是否可以使用指定上下文将给对象转换为指定类型。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <param name="destinationType">表示要转换的目标类型。</param>
		/// <returns>true：可以转换，false：不可以转换。</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// 使用指定上下文和区域信息将给定对象转换为此转换器的类型。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <param name="culture">区域信息。</param>
		/// <param name="value">要转换的对象。</param>
		/// <returns>此转换器类型的实例。</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string text)
			{
				string text2 = text.Trim();
				if (text2.Length == 0)
				{
					return null;
				}
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				char c = culture.TextInfo.ListSeparator[0];
				string[] array = text2.Split(c);
				int[] array2 = new int[array.Length];
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = (int)converter.ConvertFromString(context, culture, array[i]);
				}
				if (array2.Length == 1)
				{
					return new CornerRadius(array2[0]);
				}
				if (array2.Length == 4)
				{
					return new CornerRadius(array2[0], array2[1], array2[2], array2[3]);
				}
				throw new ArgumentException("文本转换失败。");
			}
			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// 使用指定上下文和区域信息将给定值转换为指定的类型。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <param name="culture">区域信息。</param>
		/// <param name="value">要转换的对象。</param>
		/// <param name="destinationType">目标类型。</param>
		/// <returns>目标类型的实例。</returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (value is CornerRadius)
			{
				if (destinationType == typeof(string))
				{
					CornerRadius cornerRadius = (CornerRadius)value;
					if (culture == null)
					{
						culture = CultureInfo.CurrentCulture;
					}
					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
					string[] array = new string[4];
					int num = 0;
					array[num++] = converter.ConvertToString(context, culture, cornerRadius.TopLeft);
					array[num++] = converter.ConvertToString(context, culture, cornerRadius.TopRight);
					array[num++] = converter.ConvertToString(context, culture, cornerRadius.BottomLeft);
					array[num++] = converter.ConvertToString(context, culture, cornerRadius.BottomRight);
					return string.Join(separator, array);
				}
				if (destinationType == typeof(InstanceDescriptor))
				{
					CornerRadius cornerRadius = (CornerRadius)value;
					ConstructorInfo constructor = typeof(CornerRadius).GetConstructor(new Type[4]
					{
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(int)
					});
					if (constructor != null)
					{
						return new InstanceDescriptor(constructor, new object[4]
						{
							cornerRadius.TopLeft,
							cornerRadius.TopRight,
							cornerRadius.BottomLeft,
							cornerRadius.BottomRight
						});
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// 使用指定上下文根据给定的属性值创建对象。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <param name="propertyValues">属性字典。</param>
		/// <returns>此转换器类型的实例。</returns>
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			object obj1 = propertyValues["TopLeft"];
			object obj2 = propertyValues["TopRight"];
			object obj3 = propertyValues["BottomLeft"];
			object obj4 = propertyValues["BottomRight"];
			if (obj1 == null || obj2 == null || obj3 == null || obj4 == null || !(obj1 is int) || !(obj2 is int) || !(obj3 is int) || !(obj4 is int))
			{
				throw new ArgumentException("无效的属性信息。");
			}
			return new CornerRadius((int)obj1, (int)obj2, (int)obj3, (int)obj4);
		}

		/// <summary>
		/// 返回更改此对象的值是否要求调用 TypeConverter.CreateInstance 方法来创建新值。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <returns>true：需要调用，false：不需调用。</returns>
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// 返回属性集合。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <param name="value">要获取属性值的对象。</param>
		/// <param name="attributes">用作筛选器的 Attribute 数组。</param>
		/// <returns>PropertyDescriptor 对象集合。</returns>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, System.Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(CornerRadius), attributes);
			return properties.Sort(new string[4]
			{
				"TopLeft",
				"TopRight",
				"BottomLeft",
				"BottomRight"
			});
		}

		/// <summary>
		/// 返回此对象是否支持属性。
		/// </summary>
		/// <param name="context">格式上下文。</param>
		/// <returns>true：支持，false：不支持。</returns>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}