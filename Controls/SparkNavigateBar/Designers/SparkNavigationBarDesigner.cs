using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls.Design
{
	internal sealed class SparkNavigationBarDesigner : ParentControlDesigner, IDesignerUtilsClient
	{
		private DesignerVerbCollection _verbs;
		private readonly DesignerUtils _utils;
		private const string RemoveTabPageString = "ÒÆ³ýÏî";
		private const string AddTabPageString = "Ìí¼ÓÏî";
		private bool _settingSelection;

		public SparkNavigationBarDesigner()
		{
			this._utils = new DesignerUtils(this);
		}

		public override bool CanParent(Control control)
		{
			return (control is SparkNavigateBarPanel) && !(this.Control.Controls.Contains(control));
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this._verbs == null)
				{
					this._verbs = new DesignerVerbCollection()
					{
						new DesignerVerb(AddTabPageString, this.OnAddTab),
						new DesignerVerb(RemoveTabPageString, this.OnRemoveTab),
					};
				}
				return this._verbs;
			}
		}

		[Obsolete("",true)]
		public SparkNavigationBar TabControl => base.Control as SparkNavigationBar;

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			this._utils.SelectionService.SelectionChanged += this.OnSelectionChanged;

			//TabControl.NewTabButtonClick += OnAddTab;
			//TabControl.CloseButtonClick += OnRemoveTab;
			this.TabControl.SelectedTabChanged += this.OnSelectedTabChanged;
		}

		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			base.InitializeNewComponent(defaultValues);

			this.AddTabPage();
			this._utils.SetPropertyValueWithNotification(nameof(this.TabControl.SelectedTabIndex), 0);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//TabControl.NewTabButtonClick -= OnAddTab;
				//TabControl.CloseButtonClick -= OnRemoveTab;
				this.TabControl.SelectedTabChanged -= this.OnSelectedTabChanged;

				this._utils.SelectionService.SelectionChanged -= this.OnSelectionChanged;
			}

			base.Dispose(disposing);
		}

		protected override bool GetHitTest(Point point)
		{
			SparkNavigateToolStrip strip = this.TabControl.TabStrip;

			return strip.GetItemAt(strip.PointToClient(point)) != null;
		}

		private SparkNavigateBarPanel AddTabPage()
		{
			return this.AddTabPage(false);
		}

		private SparkNavigateBarPanel AddTabPage(bool suppressEvents)
		{
			return this._utils.ExecuteWithTransaction(AddTabPageString + ' ' + this.TabControl.Site.Name, delegate
			{
				SparkNavigateBarPanel page = this._utils.CreateComponent<SparkNavigateBarPanel>();
				PropertyDescriptor pagesDescriptor = this._utils.GetProperty("Controls");

				if (suppressEvents)
				{
					this.TabControl.TabPages.Add(page);

					this._utils.SetPropertyValue(nameof(this.TabControl.SelectedTab), page);
					this._utils.SetPropertyValue(page, nameof(this.TabControl.Text), page.Name);
				}
				else
				{
					this.RaiseComponentChanging(pagesDescriptor);

					page.Text = page.Name;
					this.TabControl.TabPages.Add(page);


					this._utils.SetPropertyValueWithNotification(nameof(this.TabControl.SelectedTab), page);
					//_utils.SetPropertyValueWithNotification(page, "Text", page.Name);

					this.RaiseComponentChanged(pagesDescriptor, null, null);
				}

				return page;
			});
		}

		private void RemoveTabPage(SparkNavigateBarPanel page)
		{
			this._utils.ExecuteWithTransaction(RemoveTabPageString, delegate
			{
				PropertyDescriptor pagesDescriptor = this._utils.GetProperty(nameof(this.TabControl.TabPages));

				this.RaiseComponentChanging(pagesDescriptor);
				this.TabControl.TabPages.Remove(page);
				this.RaiseComponentChanged(pagesDescriptor, null, null);

				this._utils.DesignerHost.DestroyComponent(page);
			});
		}

		private void OnAddTab(object sender, EventArgs e)
		{
			this.AddTabPage();
		}

		private void OnRemoveTab(object sender, EventArgs e)
		{
			this.RemoveTabPage(this.TabControl.SelectedTab);
		}

		private void OnRemoveTab(object sender, SparkNavigateBarPanelEventArgs e)
		{
			this.RemoveTabPage(e.Page);
		}

		private void OnSelectedTabChanged(object sender, EventArgs e)
		{
			if (this.TabControl.SelectedTab != null)
			{
				try
				{
					this._settingSelection = true;
					this._utils.SelectionService.SetSelectedComponents(new IComponent[]
					{
						this.TabControl.SelectedTab
					}, SelectionTypes.Replace);
				}
				finally
				{
					this._settingSelection = false;
				}
			}

			PropertyDescriptor descriptor = this._utils.GetProperty(nameof(this.TabControl.SelectedTab));
			if (descriptor != null)
			{
				this.RaiseComponentChanging(descriptor);
				this.RaiseComponentChanged(descriptor, null, null);
			}
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			foreach (object item in this._utils.SelectionService.GetSelectedComponents())
			{
				if (!this._settingSelection)
				{
					SparkNavigateBarPanel page = GetTabPageOfComponent(item);

					if ((page != null) && (page.Parent == this.TabControl))
					{
						this._utils.SetPropertyValueWithNotification(nameof(this.TabControl.SelectedTab), page);

						break;
					}
				}
			}
		}

		private static SparkNavigateBarPanel GetTabPageOfComponent(object component)
		{
			Control ctl = (component as Control);

			while (ctl != null)
			{
				SparkNavigateBarPanel page = (ctl as SparkNavigateBarPanel);

				if (page != null)
				{
					return page;
				}

				ctl = ctl.Parent;
			}

			return null;
		}

		#region IDesignerUtilsClient Members

		IComponent IDesignerUtilsClient.Component => this.Component;

		object IDesignerUtilsClient.GetService(Type type)
		{
			return this.GetService(type);
		}

		#endregion
	}
}