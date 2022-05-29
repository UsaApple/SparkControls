using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace SparkControls.Controls
{
    internal sealed class DragManager : IDisposable
    {
        private Adorner m_Adorner;
        private BehaviorService m_BehaviorService;
        private IDesignerHost m_DesignerHost;
        private bool m_Disposed;
        private Control m_DragControl;
        private DragGlyph m_Glyph;
        private ShapeContainerDesignerHelper m_Helper;
        private bool m_InDrag;
        private ISite m_Site;

        public DragManager(ISite site)
        {
            this.m_Site = site;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_Disposed && disposing)
            {
                if (this.m_DragControl != null)
                {
                    if (this.DragBehavior != null)
                    {
                        DragBehavior dragBehavior = this.DragBehavior;
                        this.m_DragControl.GiveFeedback -= new GiveFeedbackEventHandler(dragBehavior.GiveFeedback);
                        DragBehavior behavior2 = this.DragBehavior;
                        this.m_DragControl.QueryContinueDrag -= new QueryContinueDragEventHandler(behavior2.QueryContinueDrag);
                    }
                    this.m_DragControl.Dispose();
                    this.m_DragControl = null;
                }
                if ((this.m_BehaviorService != null) && this.m_BehaviorService.Adorners.Contains(this.m_Adorner))
                {
                    this.m_BehaviorService.Adorners.Remove(this.m_Adorner);
                }
                this.m_Glyph = null;
                this.m_Adorner = null;
                this.m_Helper = null;
            }
            this.m_Disposed = true;
        }

        public void DoDragDrop(ShapeContainerDesigner sourceDesigner, List<Shape> comps, HitFlag hit, Point loc, bool addSelection)
        {
            IDesignerHost designerHost = this.DesignerHost;
            if ((designerHost != null) && !this.m_InDrag)
            {
                this.m_InDrag = true;
                Size screenSize = DesignerUtility.GetScreenSize();
                DesignerTransaction transaction = null;
                DragObject data = null;
                Cursor current = Cursor.Current;
                try
                {
                    if (hit == HitFlag.HitMove)
                    {
                        if (comps.Count > 1)
                        {
                            transaction = designerHost.CreateTransaction($"Move {comps.Count} shapes");
                        }
                        else
                        {
                            transaction = designerHost.CreateTransaction($"Move {comps[0].Name}");
                        }
                    }
                    else if (comps.Count > 1)
                    {
                        transaction = designerHost.CreateTransaction($"Resize {comps.Count} shapes");
                    }
                    else
                    {
                        transaction = designerHost.CreateTransaction($"Resize {comps[0].Name}");
                    }
                    this.Helper.SelectionManager.HideAdorner();
                    data = new DragObject(sourceDesigner, comps, this.DragGlyph, hit, loc, screenSize, addSelection);
                    ((DragBehavior)this.DragGlyph.Behavior).Source = sourceDesigner.ShapeContainer;
                    this.BehaviorService.Adorners.Add(this.Adorner);
                    data.StatusbarCommand = this.Helper.ShapeContainerCommandSet.StatusBarCommand;
                    data.GiveFeedback(null);
                    this.DragControl.DoDragDrop(data, DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Scroll);
                }
                finally
                {
                    if (data != null)
                    {
                        if (data.Result == DesignerUtility.DragResult.None)
                        {
                            if (hit == HitFlag.HitMove)
                            {
                                data.CancelDrag();
                            }
                            else
                            {
                                data.Drop(Control.MousePosition.X, Control.MousePosition.Y);
                            }
                        }
                        else if (data.Result == DesignerUtility.DragResult.Cancel)
                        {
                            data.CancelDrag();
                        }
                        data.Dispose();
                    }
                    if (this.BehaviorService.Adorners.Contains(this.Adorner))
                    {
                        this.BehaviorService.Adorners.Remove(this.Adorner);
                    }
                    this.Helper.SelectionManager.Refresh();
                    this.Helper.SelectionManager.ShowAdorner();
                    Cursor.Current = current;
                    this.m_InDrag = false;
                    if (transaction != null)
                    {
                        transaction.Commit();
                        ((IDisposable)transaction).Dispose();
                    }
                }
            }
        }

        private Adorner Adorner
        {
            get
            {
                if (this.m_Adorner == null)
                {
                    this.m_Adorner = new Adorner();
                    this.m_Adorner.Glyphs.Add(this.DragGlyph);
                    this.DragGlyph.Adorenr = this.m_Adorner;
                }
                return this.m_Adorner;
            }
        }

        private BehaviorService BehaviorService
        {
            get
            {
                if (this.m_BehaviorService == null)
                {
                    this.m_BehaviorService = (BehaviorService)this.Site.GetService(typeof(BehaviorService));
                }
                return this.m_BehaviorService;
            }
        }

        private IDesignerHost DesignerHost
        {
            get
            {
                if (this.m_DesignerHost == null)
                {
                    this.m_DesignerHost = (IDesignerHost)this.Site.GetService(typeof(IDesignerHost));
                }
                return this.m_DesignerHost;
            }
        }

        private DragBehavior DragBehavior
        {
            get
            {
                return (DragBehavior)this.DragGlyph.Behavior;
            }
        }

        private Control DragControl
        {
            get
            {
                if (this.m_DragControl == null)
                {
                    this.m_DragControl = new Control();
                    DragBehavior dragBehavior = this.DragBehavior;
                    this.m_DragControl.GiveFeedback += new GiveFeedbackEventHandler(dragBehavior.GiveFeedback);
                    DragBehavior behavior2 = this.DragBehavior;
                    this.m_DragControl.QueryContinueDrag += new QueryContinueDragEventHandler(behavior2.QueryContinueDrag);
                }
                return this.m_DragControl;
            }
        }

        private DragGlyph DragGlyph
        {
            get
            {
                if (this.m_Glyph == null)
                {
                    this.m_Glyph = new DragGlyph(this.BehaviorService, (Control)this.DesignerHost.RootComponent);
                }
                return this.m_Glyph;
            }
        }

        internal ShapeContainerDesignerHelper Helper
        {
            get
            {
                return this.m_Helper;
            }
            set
            {
                this.m_Helper = value;
            }
        }

        private ISite Site
        {
            get
            {
                return this.m_Site;
            }
        }
    }
}