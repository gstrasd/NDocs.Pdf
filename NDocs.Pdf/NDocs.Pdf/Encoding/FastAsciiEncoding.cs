using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Encoding
{
    internal class FastAsciiEncoding : BaseEncoding
    {
        private readonly byte _replacementCharacter;

        public FastAsciiEncoding(char replacementCharacter = '?')
        {
            if (replacementCharacter > '\u00FF') throw new ArgumentOutOfRangeException(nameof(replacementCharacter));
            _replacementCharacter = (byte)replacementCharacter;
        }

        public override string EncodingName => "FastAscii";

        protected override unsafe int GetByteCountInternal(char* chars, int index, int count)
        {
            var charRef = chars + index;
            var lastChar = chars + count;
            var surrogateCount = 0;

            while (charRef < lastChar)
            {
                var character = *charRef++;
                if (character >= '\uD800' && character <= '\uDFFF') surrogateCount++;
            }

            return count - surrogateCount / 2;
        }

        protected override int GetMaxByteCountInternal(int charCount)
        {
            return Math.Max(charCount, 0);
        }

        protected override unsafe void GetBytesInternal(char* chars, byte* bytes, int charCount, int byteCount)
        {
            var highSurrogateFound = false;
            var charRef = chars;
            var lastChar = chars + charCount;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;

            while (charRef < lastChar && byteRef < lastByte)
            {
                var character = *charRef++;
                var highByte = (byte)((character & 0xFF00) >> 8);

                if (highSurrogateFound)
                {
                    highSurrogateFound = false;
                    if (highByte >= 0xD8 && highByte <= 0xDF) continue;
                }

                if (highByte == 0)
                {
                    *byteRef++ = (byte)character;
                }
                else
                {
                    highSurrogateFound = highByte >= 0xD8 && highByte <= 0xDF;
                    *byteRef++ = _replacementCharacter;
                }
            }
        }

        protected override unsafe int GetCharCountInternal(byte* bytes, int offset, int count)
        {
            return Math.Max(count, 0);
        }

        protected override int GetMaxCharCountInternal(int byteCount)
        {
            return Math.Max(byteCount, 0);
        }

        protected override unsafe void GetCharsInternal(byte* bytes, char* chars, int byteCount, int charCount)
        {
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var charRef = chars;
            var lastChar = chars + charCount;

            while (byteRef < lastByte && charRef < lastChar) *charRef++ = (char)*byteRef++;
        }
    }
}