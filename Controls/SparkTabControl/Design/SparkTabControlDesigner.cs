using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SparkControls.Controls.Design
{
    /// <summary>
    /// 选项卡控件设计器。
    /// </summary>
    public class SparkTabControlDesigner : ParentControlDesigner
    {
        #region 私有变量

        private IComponentChangeService changeService;

        #endregion

        #region 属性
        /// <summary>
        /// 选中的规则
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                return this.Control != null && this.Control.Dock == DockStyle.Fill ? SelectionRules.Visible : base.SelectionRules;
            }
        }

        /// <summary>
        /// 设计时的谓词
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (base.Verbs.Count == 4)
                {
                    bool fill = this.Control.Dock == DockStyle.Fill;
                    base.Verbs[0].Enabled = !fill;
                    base.Verbs[1].Enabled = fill;
                    base.Verbs[3].Enabled = this.Control.Items.Count != 0;
                }
                return base.Verbs;
            }
        }

        /// <summary>
        /// 当前控件
        /// </summary>
        public new virtual SparkTabControl Control
        {
            get
            {
                return base.Control as SparkTabControl;
            }
        }


        /// <summary>
        /// 获取容器集合
        /// </summary>
        public override ICollection AssociatedComponents
        {
            get
            {
                return this.Control.Items;
            }
        }
        #endregion

        #region 初始化和释放
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="component"></param>
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);
            //Design services
            this.changeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));


            this.changeService.ComponentRemoved += this.ChangeService_ComponentRemoved;
            this.Control.TabPageSelectionChanged += this.Control_TabPageSelectionChanged;
            //Bind design events
            //changeService.ComponentRemoving += new ComponentEventHandler(OnRemoving);
            this.Verbs.Add(new DesignerVerb("在父容器停靠", new EventHandler(this.OnDockFill)));
            this.Verbs.Add(new DesignerVerb("取消在父容器停靠", new EventHandler(this.OnUnDockFill)));
            this.Verbs.Add(new DesignerVerb("添加选项卡", new EventHandler(this.OnAddTabPage)));
            this.Verbs.Add(new DesignerVerb("移除选项卡", new EventHandler(this.OnRemoveTabPage)));
        }

        private void Control_TabPageSelectionChanged(SparkTabPageChangedEventArgs e)
        {
            try
            {
                if (e.ChangeAction == TabPageChangeAction.SelectionChanged)
                {
                    this.changeService.OnComponentChanged(this.Control, null, null, null);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Message");
            }
        }

        private void ChangeService_ComponentRemoved(object sender, ComponentEventArgs e)
        {

        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.changeService != null)
            {
                this.changeService.ComponentRemoving -= new ComponentEventHandler(this.OnRemoving);

            }
            if (this.Control != null)
            {
                this.Control.TabPageSelectionChanged -= this.Control_TabPageSelectionChanged;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region 私有方法
        private void OnRemoving(object sender, ComponentEventArgs e)
        {
            if (e.Component is SparkTabPage page)
            {
                this.RemoveTabPage(page);
            }
            //IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));
            ////Removing a button
            //if (e.Component is SparkTabPage)
            //{
            //    SparkTabPage itm = e.Component as SparkTabPage;
            //    if (itm != null && Control.Items.Contains(itm))
            //    {
            //        //changeService.OnComponentChanging(Control, null);
            //        Control.RemoveTab(itm);
            //        //changeService.OnComponentChanged(Control, null, null, null);
            //        return;
            //    }
            //}

            //if (e.Component is SparkTabControl)
            //{
            //    for (int i = Control.Items.Count - 1; i >= 0; i--)
            //    {
            //        SparkTabPage itm = Control.Items[i];
            //        //changeService.OnComponentChanging(Control, null);
            //        Control.RemoveTab(itm);
            //        host.DestroyComponent(itm);
            //        //changeService.OnComponentChanged(Control, null, null, null);
            //    }
            //}

        }

        private void OnAddTabPage(object sender, EventArgs e)
        {
            IDesignerHost host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
            DesignerTransaction transaction = host.CreateTransaction("Add TabPage");
            MemberDescriptor member = TypeDescriptor.GetProperties(base.Component)["Controls"];
            //var oldTabs = this.Control.Items;
            //RaiseComponentChanging(TypeDescriptor.GetProperties(this.Control)["Items"]);
            SparkTabPage itm = OnCreateComponent(host);
            //changeService.OnComponentChanging(Control, null);
            base.RaiseComponentChanging(member);
            this.Control.AddTab(itm);
            itm.Text = itm.Name;
            this.Control.SelectItem(itm);
            transaction.Commit();
            base.RaiseComponentChanged(member, null, null);
            //changeService.OnComponentChanged(Control, null, null, null);
            //RaiseComponentChanged(TypeDescriptor.GetProperties(this.Control)["Items"], oldTabs, this.Control.Items);
        }

        private void OnRemoveTabPage(object sender, EventArgs e)
        {
            //SparkTabControl component = (SparkTabControl)base.Component;
            this.RemoveTabPage(this.Control.SelectedItem);

            ((ISelectionService)this.GetService(typeof(ISelectionService))).SetSelectedComponents(new IComponent[]
            {
                this.Control
            }, SelectionTypes.Auto);
        }

        private void RemoveTabPage(SparkTabPage page)
        {
            if ((this.Control != null) && (this.Control.Items.Count != 0) && this.Control.Items.Contains(page))
            {
                MemberDescriptor member = TypeDescriptor.GetProperties(base.Component)["Controls"];
                IDesignerHost service = (IDesignerHost)this.GetService(typeof(IDesignerHost));
                if (service != null)
                {
                    DesignerTransaction transaction = null;
                    try
                    {
                        try
                        {
                            transaction = service.CreateTransaction("Remove TabPage");
                            base.RaiseComponentChanging(member);
                        }
                        catch (CheckoutException exception)
                        {
                            if (exception != CheckoutException.Canceled)
                            {
                                throw exception;
                            }
                            return;
                        }
                        this.Control.RemoveTab(page);
                        service.DestroyComponent(page);
                        base.RaiseComponentChanged(member, null, this.Control.Controls);
                    }
                    finally
                    {
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        private void OnDockFill(object sender, EventArgs e)
        {
            this.Control.Dock = DockStyle.Fill;
        }

        private void OnUnDockFill(object sender, EventArgs e)
        {
            this.Control.Dock = DockStyle.None;
        }
        #endregion

        #region 重写事件
        protected virtual SparkTabPage OnCreateComponent(IDesignerHost host)
        {
            return (SparkTabPage)host.CreateComponent(typeof(SparkTabPage));
        }

        /// <summary>
        ///命中关闭按钮和菜单按钮返回true，否则返回false
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override bool GetHitTest(Point point)
        {
            HitTestResult result = this.Control.HitTest(point);
            if (result == HitTestResult.CloseButton || result == HitTestResult.MenuGlyph)
                return true;

            return false;
        }

        /// <summary>
        /// 属性过滤事件
        /// </summary>
        /// <param name="properties"></param>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            properties.Remove("UseVisualStyleBackColor");
            properties.Remove("DockPadding");
            properties.Remove("DrawGrid");
            properties.Remove("Margin");
            properties.Remove("Padding");
            properties.Remove("BorderStyle");
            properties.Remove("ForeColor");
            properties.Remove("BackgroundImage");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("GridSize");
            properties.Remove("ImeMode");
        }

        /// <summary>
        /// 消息监听事件
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x201:
                    base.WndProc(ref m);
                    Point pt = this.Control.PointToClient(Cursor.Position);
                    SparkTabPage itm = this.Control.GetTabPageByPoint(pt);
                    if (itm != null)
                    {
                        this.Control.SelectedItem = itm;
                        ArrayList selection = new ArrayList() { itm };
                        ISelectionService selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
                        selectionService.SetSelectedComponents(selection);
                    }
                    break;
                case 0x114:
                case 0x115:
                    base.BehaviorService.Invalidate(base.BehaviorService.ControlRectInAdornerWindow(this.Control));
                    base.WndProc(ref m);
                    return;

                case 0x84:
                    base.WndProc(ref m);
                    if (((int)m.Result) != -1)
                    {
                        break;
                    }
                    m.Result = (IntPtr)1;
                    return;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        #endregion
    }
}