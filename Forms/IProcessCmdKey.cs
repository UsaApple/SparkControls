using System.Collections;

namespace SparkControls.Forms
{
	/// <summary>
	/// ProcessCmdKey事件的接口
	/// </summary>
	public interface IProcessCmdKey
	{
		/// <summary>
		/// ProcessCmdKey 事件
		/// </summary>
		event ProcessCmdKeyEventHandler ProcessCmdKey;

		/// <summary>
		/// 快捷键的集合
		/// </summary>
		Hashtable Shortcuts { get; }

		/// <summary>
		/// 是否注册 ProcessCmdKey 事件
		/// </summary>
		bool IsRegisteredProcessCmdKey { get; }
	}
}