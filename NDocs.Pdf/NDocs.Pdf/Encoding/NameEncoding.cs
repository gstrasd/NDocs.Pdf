using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Encoding
{
    [PdfVersion("1.0")]
    [PdfVersion("1.1")]
    internal class NameEncoding : BaseEncoding
    {
        public override string EncodingName => "Name";

        protected override unsafe int GetByteCountInternal(char* chars, int index, int count)
        {
            return Math.Max(count, 0) + 1;
        }

        protected override int GetMaxByteCountInternal(int charCount)
        {
            return Math.Max(charCount, 0) + 1;
        }

        protected override unsafe void GetBytesInternal(char* chars, byte* bytes, int charCount, int byteCount)
        {
            var charRef = chars;
            var lastChar = chars + charCount;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;

            if (byteCount > 0) *byteRef++ = Ascii.ForwardSlash;

            while (charRef < lastChar && byteRef < lastByte)
            {
                var character = *charRef++;
                if (character > '\u007E') throw new EncoderFallbackException();

                var @byte = (byte)character;
                if (Ascii.Whitespace.Contains(@byte) || Ascii.Delimiters.Contains(@byte)) throw new EncoderFallbackException();

                *byteRef++ = @byte;
            }
        }

        protected override unsafe int GetCharCountInternal(byte* bytes, int offset, int count)
        {
            return Math.Max(count - 1, 0);
        }

        protected override int GetMaxCharCountInternal(int byteCount)
        {
            return Math.Max(byteCount - 1, 0);
        }

        protected override unsafe void GetCharsInternal(byte* bytes, char* chars, int byteCount, int charCount)
        {
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var charRef = chars;
            var lastChar = chars + charCount;

            if (byteCount == 0 || *byteRef++ != Ascii.ForwardSlash) throw new EncoderFallbackException();

            while (byteRef < lastByte && charRef < lastChar)
            {
                var @byte = *byteRef++;
                if (@byte > Ascii.Delete || Ascii.Whitespace.Contains(@byte) || Ascii.Delimiters.Contains(@byte)) throw new EncoderFallbackException();

                *charRef++ = (char)@byte;
            }
        }
    }
}
