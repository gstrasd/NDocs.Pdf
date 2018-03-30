using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public struct CrossReference
    {
        public static readonly CrossReference Head = new CrossReference(0, UInt16.MaxValue, 0, false);

        public CrossReference(int objectNumber, ushort generationNumber, long offset, bool inUse)
        {
            Id = new ObjectIdentifier(objectNumber, generationNumber);

            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), @"The offset must be a positive integer value.");
            Offset = offset;
            InUse = inUse;
        }

        public ObjectIdentifier Id { get; }
        public long Offset { get; }
        public bool InUse { get; }

        public override string ToString()
        {
            return String.Concat($"{Offset:0000000000} {Id.GenerationNumber:00000} ", InUse ? "n" : "f", "\r\n");
        }
    }
}