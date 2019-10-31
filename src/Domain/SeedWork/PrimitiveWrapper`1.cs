using System;
using System.Collections.Generic;

namespace Cds.DroidManagement.Domain.SeedWork
{
    public class PrimitiveWrapper<T> : IEquatable<PrimitiveWrapper<T>>
    {
        public T Value { get; }
        protected PrimitiveWrapper(T value)
        {
            Value = value;
        }
        
        public static explicit operator T(PrimitiveWrapper<T> primitiveWrapper) => primitiveWrapper.Value;

        public virtual bool Equals(PrimitiveWrapper<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((PrimitiveWrapper<T>) obj);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }
    }
}
