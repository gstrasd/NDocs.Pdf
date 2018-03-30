using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    public class Integer : IPdfType<int>, IPdfType, IEquatable<Integer>, IEquatable<int>, ICloneable, IComparable<Integer>, IComparable<int>, IComparable
    {
        private readonly int _value;
        private readonly string _stringValue;
        private readonly byte[] _data;
        private readonly Type _type;
        private readonly int _hashCode;

        public Integer(int value)
        {
            _value = value;
            _stringValue = _value.ToString(CultureInfo.InvariantCulture);
            _data = System.Text.Encoding.ASCII.GetBytes(_stringValue);
            _type = typeof(int);
            _hashCode = _value;
        }

        internal unsafe Integer(byte* data, long offset, long length)
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
            var value = 0L;
            for (var index = 0; index < length; index++)
            {
                var character = *(data + offset + index);
                if (character < Ascii.Digit0 || character > Ascii.Digit9) throw new ArgumentOutOfRangeException(nameof(data), $"Character at position {offset + index} is not a number.");

                try
                {
                    value = value * 10 + (character - Ascii.Digit0);
                }
                catch (OverflowException)
                {
                    throw new ArgumentOutOfRangeException(nameof(data), "Data indicates a value beyond the minumum or maximum range allowed for a 32-bit integer.");
                }
            }

            if (firstByte == Ascii.MinusSign) value = -value;
            if (value < Int32.MinValue) throw new ArgumentOutOfRangeException(nameof(data), "Data indicates a value less than the minumum value for 32-bit integer.");
            if (value > Int32.MaxValue) throw new ArgumentOutOfRangeException(nameof(data), "Data indicates a value greater than the maximum value for 32-bit integer.");
            _value = Convert.ToInt32(value);
            _type = typeof(int);
            _hashCode = _value;
        }

        //public int Value => _value;
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

        public bool Equals(Integer other)
        {
            if ((object)other == null) return false;
            return ArrayHelper.Equals(_data, other._data);
        }

        public bool Equals(int other)
        {
            return _value == other;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null: return false;
                case Integer other: return ArrayHelper.Equals(_data, other._data);
                case int @int: return _value == @int;
                default: return false;
            }
        }

        public object Clone()
        {
            return new Integer(_value);
        }

        public int CompareTo(Integer other)
        {
            if ((object)other == null) return 1;
            return ArrayHelper.Compare(_data, other._data);
        }

        public int CompareTo(int other)
        {
            return _value.CompareTo(other);
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case Integer other: return CompareTo(other);
                case int @int: return _value.CompareTo(@int);
                default: throw new ArgumentException($"Object must either be of type {typeof(Integer).FullName} or {typeof(int).FullName}.");
            }
        }

        public static bool operator ==(Integer left, Integer right)
        {
            if ((object)left == null) return (object)right == null;
            if ((object)right == null) return false;
            return ArrayHelper.Equals(left._data, right._data);
        }

        public static bool operator !=(Integer left, Integer right)
        {
            if ((object)left == null) return (object)right != null;
            if ((object)right == null) return true;
            return !ArrayHelper.Equals(left._data, right._data);
        }

        public static implicit operator int(Integer obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj._value;
        }

        public static Integer operator -(Integer obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return new Integer(-obj._value);
        }

        int IPdfType<int>.Value { get; set; }
        public ObjectState State { get; }
        public event EventHandler<ObjectState> StateChanged;
    }
}
