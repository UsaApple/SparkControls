using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.Serialization;

namespace SparkControls.Controls
{
    [Serializable]
    public sealed class LineShapeToolboxItem : ShapeToolboxItem
    {
        public LineShapeToolboxItem()
        {
        }

        public LineShapeToolboxItem(Type type)
        {
            base.Initialize(type);
        }

        public LineShapeToolboxItem(SerializationInfo info, StreamingContext context)
        {
            this.Deserialize(info, context);
        }

        protected override Shape CreateShape(Point loc, Size size, IDesignerHost host, IComponentChangeService cs)
        {
            SparkLine component = null;
            component = (SparkLine)host.CreateComponent(typeof(SparkLine));
            if (component != null)
            {
                cs.OnComponentChanging(component, null);
                component.X1 = loc.X;
                component.Y1 = loc.Y;
                component.X2 = loc.X + size.Width;
                component.Y2 = loc.Y + size.Height;
                component.Name = component.Site.Name;
                cs.OnComponentChanged(component, null, null, null);
            }
            return component;
        }

        public override void Initialize(Type type)
        {
            base.Initialize(type);
            this.DisplayName = typeof(SparkLine).Name;
        }
    }
}