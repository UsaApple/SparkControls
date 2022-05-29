using System.ComponentModel;

namespace SparkControls.Controls
{
    internal sealed class LineShapeAccessibleObject : ShapeAccessibleObject
    {
        public new SparkLine Owner
        {
            get
            {
                return (SparkLine)base.Owner;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always)]
        public LineShapeAccessibleObject(SparkLine ownerObject) : base(ownerObject)
        {
        }
    }
}