using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Encoding;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    public class Name : IPdfType<string>, IPdfType, IEquatable<Name>, IEquatable<string>, ICloneable, IComparable<Name>, IComparable<string>, IComparable
    {
        private string _value;
        private string _stringValue;
        private byte[] _data;
        private int _hashCode;
        private readonly Type _type;
        private ObjectState _state;

        public Name(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            SetValue(name);
            _type = typeof(string);
            _state = ObjectState.New;
        }

        internal unsafe Name(byte* data, long offset, long length)
        {
            _data = ArrayHelper.Copy(data, offset, length);
            var encoding = new NameEncoding12();
            //(_value, _stringValue) = encoding.GetStringAndFormattedString(_data);
            _hashCode = _value.GetHashCode();
            _type = typeof(string);
            _state = ObjectState.Persisted;
        }

        public string Value
        {
            get => _value;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length == _value.Length && value.Equals(_value, StringComparison.Ordinal)) return;
                SetValue(value);
                State = ObjectState.Updated;
            } 
        }

        object IPdfType.Value
        {
            get => _value;
            set
            {
                if (!(value is string)) throw new ArgumentOutOfRangeException(nameof(value));
                var stringValue = (string)value;
                if (stringValue == null) throw new ArgumentNullException(nameof(value));
                if (stringValue.Length == _value.Length && stringValue.Equals(_value, StringComparison.Ordinal)) return;
                SetValue(stringValue);
                State = ObjectState.Updated;
            }
        }

        Type IPdfType.Type => _type;

        public ObjectState State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    StateChanged?.Invoke(this, _state);
                }
            }
        }

        public event EventHandler<ObjectState> StateChanged;

        public void Render(System.IO.Stream stream)
        {
            stream.Write(_data, 0, _data.Length);
            State = ObjectState.Persisted;
        }

        public override string ToString()
        {
            return _stringValue;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(Name other)
        {
            if ((object)other == null) return false;
            return _value.Length == other._value.Length && _value.Equals(other._value, StringComparison.Ordinal);
        }

        public bool Equals(string other)
        {
            if ((object) other == null) return false;
            return _value.Length == other.Length && _value.Equals(other, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null: return false;
                case Name other: return _value.Length == other._value.Length && _value.Equals(other._value, StringComparison.Ordinal);
                case string @string: return _value.Length == @string.Length && _value.Equals(@string, StringComparison.Ordinal);
                default: return false;
            }
        }

        public object Clone()
        {
            return new Name(_value);
        }

        public int CompareTo(Name other)
        {
            if ((object)other == null) return 1;
            return System.String.Compare(_value, other._value, StringComparison.Ordinal);
        }

        public int CompareTo(string other)
        {
            if ((object)other == null) return 1;
            return System.String.Compare(_value, other, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case Name otherName: return System.String.Compare(_value, otherName._value, StringComparison.Ordinal);
                case string otherString: return System.String.Compare(_value, otherString, StringComparison.Ordinal);
                default: throw new ArgumentException($"Object must either be of type {typeof(Name).FullName} or {typeof(string).FullName}.");
            }
        }

        public static bool operator ==(Name left, Name right)
        {
            if ((object)left == null) return (object)right == null;
            if ((object)right == null) return false;
            return left._value.Length == right._value.Length && left._value.Equals(right._value, StringComparison.Ordinal);
        }

        public static bool operator !=(Name left, Name right)
        {
            if ((object)left == null) return (object)right != null;
            if ((object)right == null) return true;
            return left._value.Length != right._value.Length || !left._value.Equals(right._value, StringComparison.Ordinal);
        }

        public static implicit operator string(Name obj)
        {
            return (string)obj?._value.Clone();
        }

        private void SetValue(string name)
        {
            var encoding = new NameEncoding12();
            _value = (string)name.Clone();
            //(_data, _stringValue) = encoding.GetBytesAndFormattedString(name);
            _hashCode = _value.GetHashCode();
        }
    }
}