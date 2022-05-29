using System.ComponentModel;

namespace SparkControls.Controls
{
	public class SparkGridTreeExpandingEventArgs : CancelEventArgs
    {
        private readonly SparkDataGridTreeNode _node;

        private SparkGridTreeExpandingEventArgs() { }

        public SparkGridTreeExpandingEventArgs(SparkDataGridTreeNode node) : base()
        {
            this._node = node;
        }

		public SparkDataGridTreeNode Node => _node;
	}
}