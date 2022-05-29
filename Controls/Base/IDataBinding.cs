namespace SparkControls.Controls
{
	/// <summary>
	/// 提供用于数据绑定的相关属性和方法
	/// </summary>
	public interface IDataBinding
	{
		/// <summary>
		/// 获取或设置控件绑定的字段名。
		/// </summary>
		string FieldName { get; set; }

		/// <summary>
		/// 获取或设置控件的值。
		/// </summary>
		object Value { get; set; }
	}

	/// <summary>
	/// 提供用于实际值/显示值双值绑定的相关属性和方法
	/// </summary>
	public interface IDualDataBinding : IDataBinding
	{
		/// <summary>
		/// 获取或设置控件显示值绑定的字段名。
		/// </summary>
		string DisplayFieldName { get; set; }

		/// <summary>
		/// 获取或设置控件的显示值。
		/// </summary>
		string DisplayValue { get; }
	}
}