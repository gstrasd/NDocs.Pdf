using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Encoding;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    [PdfVersion("1.0")]
    [PdfVersion("1.1")]
    [PdfVersion("1.2")]
    [PdfVersion("1.3")]
    [PdfVersion("1.4")]
    [PdfVersion("1.5")]
    [PdfVersion("1.6")]
    public class String : IPdfType<string>, IPdfType, IEquatable<String>, IEquatable<string>, ICloneable, IComparable<String>, IComparable<string>, IComparable
    {
        private string _value;
        private string _stringValue;
        private byte[] _data;
        private int _hashCode;
        private readonly Type _type;
        private readonly StringEncodingType _encodingType;
        private readonly StringFormatType _formatType;
        private ObjectState _state;

        public String(string value, StringEncodingType encodingType, StringFormatType formatType)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            _encodingType = encodingType;
            _formatType = formatType;
            SetValue(value);
            _type = typeof(string);
            _state = ObjectState.New;
        }

        internal unsafe String(byte* data, long offset, long length, StringEncodingType encodingType, StringFormatType formatType)
        {
            _data = ArrayHelper.Copy(data, offset, length);
            _encodingType = encodingType;
            _formatType = formatType;
            var encoding = _formatType == StringFormatType.Literal ? (BaseEncoding)new LiteralStringEncoding() : (BaseEncoding)new HexadecimalStringEncoding();
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
            protected set
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

        public bool Equals(String other)
        {
            if ((object)other == null) return false;
            return ArrayHelper.Equals(_data, other._data);
        }

        public bool Equals(string other)
        {
            if ((object)other == null) return false;
            return _value.Length == other.Length && _value.Equals(other, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null: return false;
                case String otherString: return ArrayHelper.Equals(_data, otherString._data);
                case string @string: return _value.Length == @string.Length && _value.Equals(@string, StringComparison.Ordinal);
                default: return false;
            }
        }

        public object Clone()
        {
            return new String(_value, _encodingType, _formatType);
        }

        public int CompareTo(String other)
        {
            if ((object)other == null) return 1;
            return ArrayHelper.Compare(_data, other._data);
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
                case String otherString: return ArrayHelper.Compare(_data, otherString._data);
                case string @string: return System.String.Compare(_value, @string, StringComparison.Ordinal);
                default: throw new ArgumentException($"Object must either be of type {typeof(String).FullName} or {typeof(string).FullName}.");
            }
        }

        public static bool operator ==(String left, String right)
        {
            if ((object)left == null) return (object)right == null;
            if ((object)right == null) return false;
            return ArrayHelper.Equals(left._data, right._data);
        }

        public static bool operator !=(String left, String right)
        {
            if ((object)left == null) return (object)right != null;
            if ((object)right == null) return true;
            return !ArrayHelper.Equals(left._data, right._data);
        }

        public static implicit operator string(String obj)
        {
            return (string)obj?._value.Clone();
        }

        protected void SetValue(string value)
        {
            var encoding = _formatType == StringFormatType.Literal ? (BaseEncoding)new LiteralStringEncoding() : (BaseEncoding)new HexadecimalStringEncoding();
            _value = (string)value.Clone();
            //(_data, _stringValue) = encoding.GetBytesAndFormattedString(_value);
            _hashCode = _value.GetHashCode();
        }
    }
}
