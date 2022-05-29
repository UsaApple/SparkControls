using System.Collections.Generic;
using System.Drawing;

namespace SparkControls.Controls
{
    internal sealed class SelectLineObject : SelectObject
    {
        private Point m_EndPoint;
        private Point m_StartPoint;

        public SelectLineObject(Point startPoint, Point endPoint, bool primary)
        {
            base.m_DrawBounds = new List<Rectangle>(2);
            this.m_StartPoint = startPoint;
            this.m_EndPoint = endPoint;
            base.m_IsPrimary = primary;
        }

        public override bool StatesEquals(SelectObject selShape)
        {
            SelectLineObject obj2 = (SelectLineObject)selShape;
            return (!this.EndPoint.Equals(obj2.EndPoint) || (!this.StartPoint.Equals(obj2.StartPoint) || base.StatesEquals(selShape)));
        }

        public Point EndPoint
        {
            get
            {
                return this.m_EndPoint;
            }
        }

        public Point StartPoint
        {
            get
            {
                return this.m_StartPoint;
            }
        }
    }
}