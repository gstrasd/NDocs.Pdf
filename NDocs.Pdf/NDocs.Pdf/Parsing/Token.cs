using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Token
    {
        internal static readonly unsafe Token Undefined = new Token(TokenClassification.Undefined, 0L, 0L);

        public Token(TokenClassification classification, long index, long length)
        {
            Classification = classification;
            Index = index;
            Length = length;
        }

        public TokenClassification Classification { get; }
        public long Index { get; }
        public long Length { get; }

        public override string ToString()
        {
            return $"Classification: {Classification}, Index: {Index}, Length: {Length}";
        }
    }
}