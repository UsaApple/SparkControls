using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace SparkControls.Controls
{
    internal sealed class ShapeContainerDesignerHelper : IDisposable
    {
        private ShapeContainerCommandSet m_CommandSet;
        private bool m_Disposed;
        private DragManager m_DragManager;
        private EventManager m_EventManager;
        private bool m_InDrag;
        private IComponent m_RootComponent;
        private SelectionManager m_SelectionManager;
        private int m_ShapeContainerCounter;

        public ShapeContainerDesignerHelper(IComponent root)
        {
            this.m_RootComponent = root;
            this.m_ShapeContainerCounter = 1;
            this.m_EventManager = new EventManager(root);
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
                if (this.m_ShapeContainerCounter > 1)
                {
                    this.m_ShapeContainerCounter--;
                }
                else
                {
                    if (this.m_CommandSet != null)
                    {
                        if (this.m_CommandSet.InOperation)
                        {
                            this.m_CommandSet.ShouldDispose = true;
                        }
                        else
                        {
                            this.m_CommandSet.Dispose();
                        }
                    }
                    if (this.m_DragManager != null)
                    {
                        this.m_DragManager.Dispose();
                    }
                    if (this.m_SelectionManager != null)
                    {
                        this.m_SelectionManager.Dispose();
                    }
                    if (this.m_EventManager != null)
                    {
                        if (this.m_EventManager.InBehaviorSvcDrag)
                        {
                            this.m_EventManager.ShouldDispose = true;
                        }
                        else
                        {
                            this.m_EventManager.Dispose();
                        }
                    }
                    this.m_ShapeContainerCounter = 0;
                }
            }
            this.m_Disposed = true;
        }

        public void Initialize(ShapeContainerCommandSet cmdSet, DragManager dragMgr, SelectionManager selMgr)
        {
            this.m_CommandSet = cmdSet;
            this.m_CommandSet.Helper = this;
            this.m_DragManager = dragMgr;
            this.m_DragManager.Helper = this;
            this.m_SelectionManager = selMgr;
            this.m_SelectionManager.Helper = this;
        }

        public void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (e.LastTransaction)
            {
                this.Dispose();
            }
        }

        public DragManager DragManager
        {
            get
            {
                return this.m_DragManager;
            }
        }

        public bool InDrag
        {
            get
            {
                return this.m_InDrag;
            }
            set
            {
                this.m_InDrag = value;
            }
        }

        public SelectionManager SelectionManager
        {
            get
            {
                return this.m_SelectionManager;
            }
        }

        public ShapeContainerCommandSet ShapeContainerCommandSet
        {
            get
            {
                return this.m_CommandSet;
            }
        }

        public int ShapeContainerCounter
        {
            get
            {
                return this.m_ShapeContainerCounter;
            }
            set
            {
                this.m_ShapeContainerCounter = value;
            }
        }
    }
}