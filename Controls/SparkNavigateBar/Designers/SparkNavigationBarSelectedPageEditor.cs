using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace SparkControls.Controls.Design
{
	public sealed class SparkNavigationBarSelectedPageEditor : ObjectSelectorEditor
    {
        protected override void FillTreeWithData(Selector selector, ITypeDescriptorContext context, IServiceProvider provider)
        {
            base.FillTreeWithData(selector, context, provider);
			if (context.Instance is SparkNavigationBar tabControl)
			{
				foreach (var page in tabControl.TabPages)
				{
					SelectorNode node = new SelectorNode(page.Name, page);
					selector.Nodes.Add(node);

					if (tabControl.SelectedTab == page)
					{
						selector.SelectedNode = node;
					}
				}
			}
		}
    }
}