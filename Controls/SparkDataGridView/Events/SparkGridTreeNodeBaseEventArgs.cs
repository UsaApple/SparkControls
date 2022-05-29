namespace SparkControls.Controls
{
	public class SparkGridTreeNodeBaseEventArgs
    {
        private readonly SparkDataGridTreeNode _node;

        public SparkGridTreeNodeBaseEventArgs(SparkDataGridTreeNode node)
        {
            this._node = node;
        }

		public SparkDataGridTreeNode Node => _node;
	}
}