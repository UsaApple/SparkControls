using System;
using System.Windows.Forms;

namespace SparkControls.Controls
{
	/// <summary>
	/// CheckBox 状态改变事件的参数
	/// </summary>
	public class CheckBoxStateChangedEventArgs : EventArgs
    {
        private readonly CheckState _state;
        private readonly TreeNode _node;

        /// <summary>
        /// Check状态
        /// </summary>
        public CheckState State => _state;

        /// <summary>
        /// 当前节点
        /// </summary>
        public TreeNode Node => _node;

        /// <summary>
        /// CheckBox状态改变事件的参数的构造方法
        /// </summary>
        /// <param name="state">CheckBox的状态</param>
        /// <param name="node">当前改变状态的TreeNode</param>
        public CheckBoxStateChangedEventArgs(CheckState state, TreeNode node)
        {
            _state = state;
            _node = node;
        }
    }
}