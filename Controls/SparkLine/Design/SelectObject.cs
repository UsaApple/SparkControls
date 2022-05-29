using System;
using System.Collections.Generic;
using System.Drawing;

namespace SparkControls.Controls
{
    internal abstract class SelectObject : IDisposable
    {
        private bool m_Disposed = false;
        protected List<Rectangle> m_DrawBounds;
        protected bool m_IsPrimary;
        private Shape m_Shape;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_Disposed)
            {
                if (disposing)
                {
                    this.m_Shape = null;
                }
                if (this.m_DrawBounds != null)
                {
                    this.m_DrawBounds.Clear();
                }
            }
            this.m_Disposed = true;
        }

        public virtual bool StatesEquals(SelectObject selShape)
        {
            if (this.IsPrimary.Equals(selShape.IsPrimary))
            {
                return false;
            }
            return true;
        }

        public List<Rectangle> DrawBounds
        {
            get
            {
                return this.m_DrawBounds;
            }
        }

        public bool IsPrimary
        {
            get
            {
                return this.m_IsPrimary;
            }
        }

        public Shape Shape
        {
            get
            {
                return this.m_Shape;
            }
        }
    }
}