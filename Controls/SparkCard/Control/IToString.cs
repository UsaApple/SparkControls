namespace SparkControls.Controls
{
	/// <summary>
	/// 提供转换为字符串表示的方法。
	/// </summary>
	public interface IToString
	{
		/// <summary>
		/// 转换为等效的字符串表示形式。
		/// </summary>
		/// <param name="datasource">数据源对象。</param>
		/// <returns>对象的字符串表示形式。</returns>
		string ToString(object datasource);
	}
}