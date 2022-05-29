using System;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;

using SparkControls.Expression;
using SparkControls.Foundation;

namespace SparkControls.Controls
{
	/// <summary>
	/// 表示字符串、表达式的组合类型。
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(StringExpressionConverter))]
	public class StringExpression : BaseExpression<string>
	{
		/// <summary>
		/// 初始 StringExpression 类型的新实例。
		/// </summary>
		public StringExpression()
		{

		}

		/// <summary>
		/// 初始 StringExpression 类型的新实例。
		/// </summary>
		/// <param name="value">固定值。</param>
		/// <param name="expression">表达式。</param>
		public StringExpression(string value, string expression)
		{
			Value = value;
			Expression = expression;
		}

		/// <summary>
		/// 固定值
		/// </summary>
		[Description("用于生成结果的固定值，优先级低于“Expression”属性。")]
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[TypeConverter(typeof(StringConverter))]
		public string Value { get; set; }

		/// <summary>
		/// 表达式
		/// </summary>
		[Description("用于生成结果的表达式，优先级高于“Value”属性。")]
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[TypeConverter(typeof(StringConverter))]
		public string Expression { get; set; }

		/// <summary>
		/// 计算表达式的值。
		/// </summary>
		/// <param name="dataSource">表示数据源的 DataRowView 实例。</param>
		/// <returns>表达式的计算值。</returns>
		public override string GetValue(DataRowView dataSource = null)
		{
			if (Expression.IsNullOrEmpty())
			{
				return dataSource == null ? Value : Converter.ReplaceSplit(Value, dataSource.Row);
			}

			try
			{
				// 数据源替换
				string expression = dataSource == null ? Expression : Converter.ReplaceSplit(Expression, dataSource.Row);
				// 表达式计算
				return new DCExpression(expression).Eval<string>();
			}
			catch (Exception e)
			{
				throw e;
			}
		}
	}
}