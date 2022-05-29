using System.Collections.Generic;

using SparkControls.Foundation;

namespace SparkControls.Controls
{
	internal class BaseObjectEqualityComparer : IEqualityComparer<BaseObject>
    {
        public bool Equals(BaseObject x, BaseObject y)
        {
            return x.Id == y.Id && x.Name == y.Name;
        }

        public int GetHashCode(BaseObject obj)
        {
            int hashCode = obj.Id.GetHashCode();
            if (hashCode != obj.Name.GetHashCode())
            {
                hashCode ^= obj.Name.GetHashCode();
            }
            return hashCode;
        }
    }
}