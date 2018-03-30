using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    public class Boolean : IPdfType<bool>, IPdfType, IEquatable<Boolean>, IEquatable<bool>, ICloneable, IComparable<Boolean>, IComparable<bool>, IComparable
    {
        private bool _value;
        private string _stringValue;
        private byte[] _data;
        private int _hashCode;
        private readonly Type _type;
        private ObjectState _state;

        public Boolean(bool value)
        {
            SetValue(value);
            _type = typeof(bool);
            _state = ObjectState.New;
        }

        internal unsafe Boolean(byte* data, long offset, long length)
        {
            _data = ArrayHelper.Copy(data, offset, length);
            _stringValue = System.Text.Encoding.ASCII.GetString(_data);

            switch (_stringValue.ToLower())
            {
                case "true":
                    _value = true;
                    _hashCode = 1;
                    break;
                case "false":
                    _value = false;
                    _hashCode = 0;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(data));
            }
            _type = typeof(bool);
            _state = ObjectState.Persisted;
        }

        public bool Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                SetValue(value);
                State = ObjectState.Updated;
            }
        }

        object IPdfType.Value
        {
            get => _value;
            set
            {
                if (!(value is bool)) throw new ArgumentOutOfRangeException(nameof(value));
                var boolValue = (bool) value;
                if (boolValue == _value) return;
                SetValue(boolValue);
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

        public bool Equals(Boolean other)
        {
            if ((object)other == null) return false;
            return _value == other._value;
        }

        public bool Equals(bool other)
        {
            return _value == other;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null: return false;
                case Boolean pdfBoolean: return _value == pdfBoolean._value;
                case bool boolean: return _value == boolean;
                default: return false;
            }
        }
        
        public object Clone()
        {
            return new Boolean(_value);
        }

        public int CompareTo(Boolean other)
        {
            if ((object) other == null) return 1;
            if (_value == other._value) return 0;
            if (!_value) return -1;
            return 1;
        }

        public int CompareTo(bool other)
        {
            if (_value == other) return 0;
            if (!_value) return -1;
            return 1;
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case Boolean pdfBoolean: return CompareTo(pdfBoolean);
                case bool boolean: return CompareTo(boolean);
                default: throw new ArgumentException($"Object must either be of type {typeof(Boolean).FullName} or {typeof(bool).FullName}.");
            }
        }

        public static bool operator ==(Boolean left, Boolean right)
        {
            if ((object) left == null) return (object) right == null;
            if ((object) right == null) return false;
            return left._value == right._value;
        }

        public static bool operator !=(Boolean left, Boolean right)
        {
            if ((object)left == null) return (object)right != null;
            if ((object)right == null) return true;
            return left._value != right._value;
        }

        public static implicit operator bool(Boolean obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj._value;
        }

        public static Boolean operator !(Boolean obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return new Boolean(!obj._value);
        }

        private void SetValue(bool value)
        {
            _value = value;
            _stringValue = value ? "True" : "False";
            _data = System.Text.Encoding.ASCII.GetBytes(_stringValue);
            
            _hashCode = value ? 1 : 0;
        }
    }
}
