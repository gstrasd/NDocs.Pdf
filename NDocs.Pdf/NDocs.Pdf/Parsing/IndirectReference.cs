using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public class IndirectReference : IPdfType, IEquatable<IndirectReference>
    {
        private readonly IndirectObject _indirectObject;

        public IndirectReference(IndirectObject indirectObject)
        {
            _indirectObject = indirectObject;
            _indirectObject.StateChanged += (sender, state) =>
            {
                if (state == ObjectState.Updated) ObjectIdentifier.IncrementGeneration();
                StateChanged?.Invoke(this, state);
            };
        }

        public ObjectIdentifier ObjectIdentifier => _indirectObject.ObjectIdentifier;

        public object Value
        {
            get => _indirectObject.Value;
            set => _indirectObject.Value = value;
        }

        public Type Type => _indirectObject.Type;

        public ObjectState State => _indirectObject.State;

        public event EventHandler<ObjectState> StateChanged;

        public void Render(System.IO.Stream stream)
        {
            ObjectIdentifier.Render(stream);
            stream.WriteByte(Ascii.R);
            stream.WriteByte(Ascii.Space);
        }

        public override string ToString()
        {
            return $"{ObjectIdentifier} R ";
        }

        public override int GetHashCode()
        {
            return ObjectIdentifier.GetHashCode();
        }

        public bool Equals(IndirectReference other)
        {
            if (other == null) return false;
            return _indirectObject.Equals(other._indirectObject);
        }

        public override bool Equals(object obj)
        {
            if (obj is IndirectReference indirectReference) return Equals(indirectReference);
            return false;
        }
    }
}
