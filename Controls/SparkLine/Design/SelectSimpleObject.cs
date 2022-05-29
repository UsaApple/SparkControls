using System.Collections.Generic;
using System.Drawing;

namespace SparkControls.Controls
{
    internal sealed class SelectSimpleObject : SelectObject
    {
        private Rectangle m_Bounds;

        public SelectSimpleObject(Rectangle bounds, bool primary)
        {
            base.m_DrawBounds = new List<Rectangle>(4);
            this.m_Bounds = bounds;
            base.m_IsPrimary = primary;
        }

        public override bool StatesEquals(SelectObject selShape)
        {
            SelectSimpleObject obj2 = (SelectSimpleObject)selShape;
            return (!this.Bounds.Equals(obj2.Bounds) || base.StatesEquals(selShape));
        }

        public Rectangle Bounds
        {
            get
            {
                return this.m_Bounds;
            }
        }
    }
}