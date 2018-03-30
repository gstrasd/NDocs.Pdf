using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    public class Null : IPdfType<object>, IPdfType, IEquatable<Null>, IEquatable<object>, IComparable<Null>, IComparable<object>, IComparable
    {
        public static readonly Null Default = new Null();
        private const string _stringValue = "null";
        private static readonly byte[] _data = System.Text.Encoding.ASCII.GetBytes(_stringValue);
        private static readonly Type _type = typeof(object);

        private Null() { }

        public object Value
        {
            get => null;
            set { }
        }

        object IPdfType.Value
        {
            get => null;
            set { }
        }

        Type IPdfType.Type => _type;

        public ObjectState State => ObjectState.Persisted;

        public event EventHandler<ObjectState> StateChanged;

        public void Render(System.IO.Stream stream)
        {
            stream.Write(_data, 0, _data.Length);
        }

        public override string ToString()
        {
            return _stringValue;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(Null other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Null other: return true;
                default: return obj == null;
            }
        }

        public int CompareTo(Null other)
        {
            return other == null ? 1 : 0;
        }

        public int CompareTo(object obj)
        {
            switch (obj)
            {
                case Null other: return 0;
                default: return obj == null ? 1 : -1;
            }
        }

        public static bool operator ==(Null left, Null right)
        {
            if ((object)left == null) return (object)right == null;
            return (object) right != null;
        }

        public static bool operator !=(Null left, Null right)
        {
            if ((object)left == null) return (object)right != null;
            return (object)right == null;
        }
    }
}