using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Encoding;

namespace NDocs.Pdf.Parsing
{
    public class IndirectObject : IPdfType, IEquatable<IndirectObject>
    {
        private static readonly byte[] _beginObject = new FastAsciiEncoding().GetBytes("obj\n");        // TODO: This is ugly. Don't new up an instance of FastAscii every time
        private static readonly byte[] _endObject = new FastAsciiEncoding().GetBytes("\nendobj\n");
        private readonly IPdfType _value;

        public IndirectObject(int objectNumber, IPdfType value) : this(new ObjectIdentifier(objectNumber), value)
        {
        }

        public IndirectObject(int objectNumber, ushort generationNumber, IPdfType value) : this(new ObjectIdentifier(objectNumber, generationNumber), value)
        {
        }

        public IndirectObject(ObjectIdentifier objectIdentifier, IPdfType value)
        {
            ObjectIdentifier = objectIdentifier;
            _value = value;
            _value.StateChanged += (sender, state) =>
            {
                if (state == ObjectState.Updated) ObjectIdentifier.IncrementGeneration();
                StateChanged?.Invoke(this, state);
            };
        }

        public ObjectIdentifier ObjectIdentifier { get; }

        public object Value
        {
            get => _value.Value;
            set => _value.Value = value;
        }

        public Type Type => _value.Type;

        public ObjectState State => _value.State;

        public event EventHandler<ObjectState> StateChanged;

        public void Render(System.IO.Stream stream)
        {
            ObjectIdentifier.Render(stream);
            // TODO: Here's an idea: Create one big byte array that contains all of keywords and place it in an embedded resource. Use constants for the index and length
            stream.Write(_beginObject, 0, _beginObject.Length);         
            _value.Render(stream);
            stream.Write(_endObject, 0, _endObject.Length);
        }

        public override string ToString()
        {
            return $"{ObjectIdentifier} obj\n{_value}\nendobj\n";
        }

        public override int GetHashCode()
        {
            return ObjectIdentifier.GetHashCode();
        }

        public bool Equals(IndirectObject other)
        {
            if (other == null) return false;
            if (ObjectIdentifier != other.ObjectIdentifier) return false;
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            if (obj is IndirectObject indirectObject) return Equals(indirectObject);
            return false;
        }
    }
}
