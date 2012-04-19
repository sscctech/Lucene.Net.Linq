﻿using System;
using Lucene.Net.Linq.Search;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Lucene.Net.Linq.Util
{
    internal static class NumericRangeUtils
    {
        internal static NumericRangeQuery CreateNumericRangeQuery(string fieldName, ValueType lowerBound, ValueType upperBound, RangeType lowerRange, RangeType upperRange)
        {
            if (lowerBound == null && upperBound == null)
            {
                throw new ArgumentException("lowerBound and upperBound may not both be null.");
            }

            if (lowerBound == null)
            {
                lowerBound = (ValueType) upperBound.GetType().GetField("MinValue").GetValue(null);
            }
            else if (upperBound == null)
            {
                upperBound = (ValueType) lowerBound.GetType().GetField("MaxValue").GetValue(null);
            }

            if (lowerBound.GetType() != upperBound.GetType())
            {
                throw new ArgumentException("Cannot compare different value types " + lowerBound.GetType() + " and " + upperBound.GetType());
            }

            lowerBound = ToNumericFieldValue(lowerBound);
            upperBound = ToNumericFieldValue(upperBound);

            var minInclusive = lowerRange == RangeType.Inclusive;
            var maxInclusive = upperRange == RangeType.Inclusive;

            if (lowerBound is int)
            {
                
                return NumericRangeQuery.NewIntRange(fieldName, (int)lowerBound, (int)upperBound, minInclusive, maxInclusive);
            }
            if (lowerBound is long)
            {
                return NumericRangeQuery.NewLongRange(fieldName, (long)lowerBound, (long)upperBound, minInclusive, maxInclusive);
            }
            if (lowerBound is float)
            {
                return NumericRangeQuery.NewFloatRange(fieldName, (float)lowerBound, (float)upperBound, minInclusive, maxInclusive);
            }
            if (lowerBound is double)
            {
                return NumericRangeQuery.NewDoubleRange(fieldName, (double)lowerBound, (double)upperBound, minInclusive, maxInclusive);
            }

            throw new NotSupportedException("Unsupported numeric range type " + lowerBound.GetType()); 
        }

        /// <summary>
        /// Converts supported value types such as DateTime to an underlying ValueType that is supported by
        /// <c ref="NumericRangeQuery"/>.
        /// </summary>
        internal static ValueType ToNumericFieldValue(this ValueType value)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToUniversalTime().Ticks;
            }
            if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).Ticks;
            }
            if (value is bool)
            {
                return ((bool)value) ? 1 : 0;
            }

            return value;
        }

        internal static string ToPrefixCoded(this ValueType value)
        {
            value = ToNumericFieldValue(value);
            
            if (value is int)
            {
                return NumericUtils.IntToPrefixCoded((int)value);
            }
            if (value is long)
            {
                return NumericUtils.LongToPrefixCoded((long)value);
            }
            if (value is double)
            {
                return NumericUtils.DoubleToPrefixCoded((double)value);
            }
            if (value is float)
            {
                return NumericUtils.FloatToPrefixCoded((float)value);
            }

            throw new NotSupportedException("ValueType " + value.GetType() + " not supported.");
        }
    }
}