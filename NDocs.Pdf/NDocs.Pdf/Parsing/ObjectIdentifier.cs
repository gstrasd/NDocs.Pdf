using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public struct ObjectIdentifier : IRenderable, IEquatable<ObjectIdentifier>, IComparable<ObjectIdentifier>
    {
        private string _stringValue;
        private byte[] _data;
        private int _hashCode;

        public ObjectIdentifier(int objectNumber, ushort generationNumber = 0)
        {
            if (objectNumber <= 0) throw new ArgumentOutOfRangeException(nameof(objectNumber), @"An object number must be a positive integer value.");
            ObjectNumber = objectNumber;
            GenerationNumber = generationNumber;
            _stringValue = $"{ObjectNumber} {GenerationNumber}";
            _data = System.Text.Encoding.ASCII.GetBytes(_stringValue);
            _hashCode = ((ObjectNumber & 0x00FFFFFF) << 8) | (GenerationNumber & 0x000000FF);
        }

        public int ObjectNumber { get; }

        public ushort GenerationNumber { get; private set; }

        public void Render(System.IO.Stream stream)
        {
            stream.Write(_data, 0, _data.Length);
            stream.WriteByte(Ascii.Space);
        }

        public override string ToString()
        {
            return _stringValue;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public bool Equals(ObjectIdentifier other)
        {
            return ObjectNumber == other.ObjectNumber && GenerationNumber == other.GenerationNumber;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectIdentifier identifier) return Equals(identifier);
            return false;
        }

        public int CompareTo(ObjectIdentifier other)
        {
            if (ObjectNumber < other.ObjectNumber) return -1;
            if (ObjectNumber > other.ObjectNumber) return 1;
            if (GenerationNumber < other.GenerationNumber) return -1;
            if (GenerationNumber > other.GenerationNumber) return 1;
            return 0;
        }

        public void IncrementGeneration()
        {
            unchecked
            {
                GenerationNumber++;
            }

            _stringValue = $"{ObjectNumber} {GenerationNumber}";
            _data = System.Text.Encoding.ASCII.GetBytes(_stringValue);
            _hashCode = ((ObjectNumber & 0x00FFFFFF) << 8) | (GenerationNumber & 0x000000FF);
        }

        public static bool operator ==(ObjectIdentifier left, ObjectIdentifier right)
        {
            return left.ObjectNumber == right.ObjectNumber && left.GenerationNumber == right.GenerationNumber;
        }

        public static bool operator !=(ObjectIdentifier left, ObjectIdentifier right)
        {
            return left.ObjectNumber != right.ObjectNumber || left.GenerationNumber != right.GenerationNumber;
        }
    }
}
