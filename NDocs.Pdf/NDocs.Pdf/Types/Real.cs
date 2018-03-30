using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    public class Real : IPdfType<float>, IPdfType, IEquatable<Real>, IEquatable<float>, ICloneable, IComparable<Real>, IComparable<float>, IComparable
    {
        private readonly float _value;
        private readonly string _stringValue;
        private readonly byte[] _data;
        private readonly Type _type;
        private readonly int _hashCode;

        public Real(float value)
        {
            _value = value;
            _stringValue = _value.ToString(CultureInfo.InvariantCulture);
            _data = System.Text.Encoding.ASCII.GetBytes(_stringValue);
            _type = typeof(float);
            _hashCode = _value.GetHashCode();
        }

        internal unsafe Real(byte* data, long offset, long length)
        {
            _data = ArrayHelper.Copy(data, offset, length);
            _stringValue = System.Text.Encoding.ASCII.GetString(_data);

            // Adjust index if this number is signed
            var firstByte = *(data + offset);
            if (firstByte == Ascii.MinusSign || firstByte == Ascii.PlusSign)
            {
                offset++;
                length--;
            }

            // Calculate value
            var value = 0D;
            for (var index = 0; index < length; index++)
            {
                // TODO: Consider decimal point to properly calculate real value
                var character = *(data + offset + index);
                if (character < Ascii.Digit0 || character > Ascii.Digit9) throw new ArgumentOutOfRangeException(nameof(data), $"Character at position {offset + index} is not a number.");

                try
                {
                    value = value * 10 + (character - Ascii.Digit0);
                }
                catch (OverflowException)
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Data indicates a value beyond the minumum or maximum range allowed for a 32-bit floating-point number.");
                }
            }

            if (firstByte == Ascii.MinusSign) value = -value;
            if (value < Single.MinValue) throw new ArgumentOutOfRangeException(nameof(data), "Data indicates a value less than the minumum value for 32-bit floating-point number.");
            if (value > Single.MaxValue) throw new ArgumentOutOfRangeException(nameof(data), "Data indicates a value greater than the maximum value for 32-bit floating-point number.");
            _value = Convert.ToSingle(value);
            _type = typeof(int);
            _hashCode = _value.GetHashCode();
        }

        public float Value => _value;
        //object IPdfType.Value => _value;
        object IPdfType.Value { get; set; }
        Type IPdfType.Type => _type;

        public override string ToString()
        {
            return _stringValue;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public void Render(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Real other)
        {
            if ((object)other == null) return false;
            return ArrayHelper.Equals(_data, other._data);
        }

        public bool Equals(float other)
        {
            return _value == other;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null: return false;
                case Real other: return ArrayHelper.Equals(_data, other._data);
                case float @float: return _value == @float;
                default: return false;
            }
        }

        public object Clone()
        {
            return new Real(_value);
        }

        public int CompareTo(Real other)
        {
            if ((object)other == null) return 1;
            return ArrayHelper.Compare(_data, other._data);
        }

        public int CompareTo(float other)
        {
            return _value.CompareTo(other);
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case Real other: return CompareTo(other);
                case float @float: return _value.CompareTo(@float);
                default: throw new ArgumentException($"Object must either be of type {typeof(Real).FullName} or {typeof(float).FullName}.");
            }
        }

        public static bool operator ==(Real left, Real right)
        {
            if ((object)left == null) return (object)right == null;
            if ((object)right == null) return false;
            return ArrayHelper.Equals(left._data, right._data);
        }

        public static bool operator !=(Real left, Real right)
        {
            if ((object)left == null) return (object)right != null;
            if ((object)right == null) return true;
            return !ArrayHelper.Equals(left._data, right._data);
        }

        public static implicit operator float(Real obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj._value;
        }

        public static Real operator -(Real obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return new Real(-obj._value);
        }

        float IPdfType<float>.Value { get; set; }
        public ObjectState State { get; }
        public event EventHandler<ObjectState> StateChanged;
    }
}
