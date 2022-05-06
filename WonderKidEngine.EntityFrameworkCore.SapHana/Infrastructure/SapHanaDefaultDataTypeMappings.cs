// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;

namespace WonderKidEngine.EntityFrameworkCore.SapHana.Infrastructure
{
    public class SapHanaDefaultDataTypeMappings
    {
        public SapHanaDefaultDataTypeMappings()
        {
        }

        protected SapHanaDefaultDataTypeMappings(SapHanaDefaultDataTypeMappings copyFrom)
        {
            ClrBoolean = copyFrom.ClrBoolean;
            ClrDateTime = copyFrom.ClrDateTime;
            ClrDateTimeOffset = copyFrom.ClrDateTimeOffset;
            ClrTimeSpan = copyFrom.ClrTimeSpan;
            ClrTimeOnlyPrecision = copyFrom.ClrTimeOnlyPrecision;
        }

        public virtual SapHanaBooleanType ClrBoolean { get; private set; }
        public virtual SapHanaDateTimeType ClrDateTime { get; private set; }
        public virtual SapHanaDateTimeType ClrDateTimeOffset { get; private set; }
        public virtual SapHanaTimeSpanType ClrTimeSpan { get; private set; }
        public virtual int ClrTimeOnlyPrecision { get; private set; } = -1;

        public virtual SapHanaDefaultDataTypeMappings WithClrBoolean(SapHanaBooleanType SapHanaBooleanType)
        {
            var clone = Clone();
            clone.ClrBoolean = SapHanaBooleanType;
            return clone;
        }

        public virtual SapHanaDefaultDataTypeMappings WithClrDateTime(SapHanaDateTimeType SapHanaDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTime = SapHanaDateTimeType;
            return clone;
        }

        public virtual SapHanaDefaultDataTypeMappings WithClrDateTimeOffset(SapHanaDateTimeType SapHanaDateTimeType)
        {
            var clone = Clone();
            clone.ClrDateTimeOffset = SapHanaDateTimeType;
            return clone;
        }

        // TODO: Remove Time6, add optional precision parameter for Time types.
        public virtual SapHanaDefaultDataTypeMappings WithClrTimeSpan(SapHanaTimeSpanType SapHanaTimeSpanType)
        {
            var clone = Clone();
            clone.ClrTimeSpan = SapHanaTimeSpanType;
            return clone;
        }

        /// <summary>
        /// Set the default precision for `TimeOnly` CLR type mapping to a SapHana TIME type.
        /// Set <paramref name="precision"/> to <see langword="null"/>, to use the highest supported precision.
        /// Otherwise, set <paramref name="precision"/> to a valid value between `0` and `6`.
        /// </summary>
        /// <param name="precision">The precision used for the SapHana TIME type.</param>
        /// <returns>The same instance, to allow chained method calls.</returns>
        public virtual SapHanaDefaultDataTypeMappings WithClrTimeOnly(int? precision = null)
        {
            if (precision is < 0 or > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(precision));
            }

            var clone = Clone();
            clone.ClrTimeOnlyPrecision = precision ?? -1;
            return clone;
        }

        protected virtual SapHanaDefaultDataTypeMappings Clone() => new SapHanaDefaultDataTypeMappings(this);

        protected virtual bool Equals(SapHanaDefaultDataTypeMappings other)
        {
            return ClrBoolean == other.ClrBoolean &&
                   ClrDateTime == other.ClrDateTime &&
                   ClrDateTimeOffset == other.ClrDateTimeOffset &&
                   ClrTimeSpan == other.ClrTimeSpan &&
                   ClrTimeOnlyPrecision == other.ClrTimeOnlyPrecision;
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

            return Equals((SapHanaDefaultDataTypeMappings)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)ClrBoolean;
                hashCode = (hashCode * 397) ^ (int)ClrDateTime;
                hashCode = (hashCode * 397) ^ (int)ClrDateTimeOffset;
                hashCode = (hashCode * 397) ^ (int)ClrTimeSpan;
                return hashCode;
            }
        }
    }

    public enum SapHanaBooleanType
    {
        /// <summary>
        /// TODO
        /// </summary>
        None = -1, // TODO: Remove in EF Core 5; see SapHanaTypeMappingTest.Bool_with_SapHanaBooleanType_None_maps_to_null()

        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        TinyInt1 = 1,

        /// <summary>
        /// TODO
        /// </summary>
        Bit1 = 2
    }

    public enum SapHanaDateTimeType
    {
        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        DateTime = 1,

        /// <summary>
        /// TODO
        /// </summary>
        DateTime6 = 2,

        /// <summary>
        /// TODO
        /// </summary>
        Timestamp6 = 3,

        /// <summary>
        /// TODO
        /// </summary>
        Timestamp = 4,
    }

    public enum SapHanaTimeSpanType
    {
        /// <summary>
        /// TODO
        /// </summary>
        Default = 0,

        /// <summary>
        /// TODO
        /// </summary>
        Time = 1,

        /// <summary>
        /// TODO
        /// </summary>
        Time6 = 2,
    }
}
