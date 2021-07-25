using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EPlusActivities.API.Utils
{
    public class HashSetReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T x, T y) => object.ReferenceEquals(x, y);

        public int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
    }
}
