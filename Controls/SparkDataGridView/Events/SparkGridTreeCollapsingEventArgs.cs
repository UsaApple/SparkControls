using System.ComponentModel;

namespace SparkControls.Controls
{
	public class SparkGridTreeCollapsingEventArgs : CancelEventArgs
    {
        private readonly SparkDataGridTreeNode _node;

        private SparkGridTreeCollapsingEventArgs() { }

        public SparkGridTreeCollapsingEventArgs(SparkDataGridTreeNode node) : base()
        {
            this._node = node;
        }

		public SparkDataGridTreeNode Node => _node;
	}
}