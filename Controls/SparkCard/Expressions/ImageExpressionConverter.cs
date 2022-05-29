﻿using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
	/// <summary>
	/// ImageExpression 对象的类型转换器实现。
	/// </summary>
	public class ImageExpressionConverter : TypeConverter
    {
        /// <summary>
        /// 返回该转换器是否可以使用指定上下文将给定类型的对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <param name="sourceType">一个 Type，表示要转换的类型。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// 返回此转换器能否使用指定上下文将对象转换为指定类型。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <param name="destinationType">一个 Type，表示你希望转换为的类型。</param>
        /// <returns>如果该转换器能够执行转换，则为 true；否则为 false。</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            if (destinationType == typeof(InstanceDescriptor)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定对象转换为此转换器的类型。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <param name="culture">要用作当前区域性的 CultureInfo。</param>
        /// <param name="value">要转换的 Object。</param>
        /// <returns>一个 Object，它表示转换后的值。</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string s)) return base.ConvertFrom(context, culture, value);

            string[] ps = s.Split(new char[] { char.Parse(",") });
            if (ps.Length != 3)
            {
                throw new ArgumentException($"Failed to parse Text({s}) expected text in the format \"Value,Expression.\"");
            }

            //解析字符串并实例化对象
            return new ImageExpression(Converter.FromBase64(ps[2]), ps[1].DecodeFromBase64(), ps[0].DecodeFromBase64());
        }

        /// <summary>
        /// 使用指定的上下文和区域性信息将给定值对象转换为指定的类型。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <param name="culture">CultureInfo。 如果传递 null，则采用当前区域性。</param>
        /// <param name="value">要转换的 Object。</param>
        /// <param name="destinationType">value 参数要转换成的 Type。</param>
        /// <returns>一个 Object，它表示转换后的值。</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // 将对象转换为字符串
            if ((destinationType == typeof(string)) && (value is ImageExpression ie1))
            {
                return $"{ie1.Expression.EncodeToBase64()},{ie1.File.EncodeToBase64()},{Converter.ToBase64(ie1.Value)}";
            }

            // 生成设计时的构造器代码
            if (destinationType == typeof(InstanceDescriptor) && value is ImageExpression ie2)
            {
                ConstructorInfo ctor = typeof(ImageExpression).GetConstructor(new Type[] { typeof(Image), typeof(string), typeof(string) });
                return new InstanceDescriptor(ctor, new object[] { ie2.Value, ie2.File, ie2.Expression });
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// 给定该对象的一组属性值，使用指定上下文创建与此 TypeConverter 相关联的类型的实例。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <param name="propertyValues">新属性值的 IDictionary。</param>
        /// <returns>如果表示给定 IDictionary，则为 Object，或如果无法创建对象，则为 null。 此方法始终返回 null。</returns>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            Image pv1 = (Image)propertyValues["Value"];
            string pv2 = (string)propertyValues["File"];
            string pv3 = (string)propertyValues["Expression"];
            return new ImageExpression(pv1, pv2, pv3);
        }

        /// <summary>
        /// 使用指定的上下文返回值参数指定的数组类型的属性的集合。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <param name="value">一个 Object ，它指定要为其获取属性的数组类型。</param>
        /// <param name="attributes">用作筛选器的 Attribute 类型数组。</param>
        /// <returns>具有为此数据类型公开的属性的 PropertyDescriptorCollection；或者如果没有属性，则为 null。</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, System.Attribute[] attributes)
        {
            if (value is ImageExpression)
                return TypeDescriptor.GetProperties(value, attributes);

            return base.GetProperties(context, value, attributes);
        }

        /// <summary>
        /// 返回有关更改该对象上的某个值是否需要使用指定的上下文调用 CreateInstance(IDictionary) 以创建新值的情况。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <returns>如果更改此对象的属性需要调用 CreateInstance(IDictionary) 来创建新值，则为 true；否则为 false。</returns>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// 使用指定的上下文返回该对象是否支持属性。
        /// </summary>
        /// <param name="context">一个 ITypeDescriptorContext，用于提供格式上下文。</param>
        /// <returns>如果应调用 GetProperties(Object) 来查找此对象的属性，则为 true；否则为 false。</returns>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}