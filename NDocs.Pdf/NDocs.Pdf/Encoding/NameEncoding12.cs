using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Encoding
{
    [PdfVersion("1.2")]
    [PdfVersion("1.3")]
    [PdfVersion("1.4")]
    [PdfVersion("1.5")]
    [PdfVersion("1.6")]
    [PdfVersion("1.7")]
    internal class NameEncoding12 : NameEncoding
    {
        protected override unsafe int GetByteCountInternal(char* chars, int index, int count)
        {
            var byteCount = 1;
            var charRef = chars + index;
            var lastChar = chars + count;

            while (charRef < lastChar)
            {
                var character = *charRef++;
                if (character < '\u0021' || character == '\u0023' || character > '\u007E') byteCount += 3;
                else byteCount++;
            }

            return byteCount;
        }

        protected override int GetMaxByteCountInternal(int charCount)
        {
            return Math.Max(charCount, 0) * 3 + 1;
        }

        protected override unsafe void GetBytesInternal(char* chars, byte* bytes, int charCount, int byteCount)
        {
            var charRef = chars;
            var lastChar = chars + charCount;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var thirdToLastByte = lastByte - 3;

            if (byteCount > 0) *byteRef++ = Ascii.ForwardSlash;

            while (charRef < lastChar && byteRef < lastByte)
            {
                var character = *charRef++;
                if (character > '\u00FF') throw new EncoderFallbackException();

                var @byte = (byte)character;
                if (@byte == Ascii.Null) throw new EncoderFallbackException();
                if (@byte == Ascii.NumberSign || @byte < Ascii.ExclamationMark || @byte > Ascii.Tilde)
                {
                    if (byteRef >= thirdToLastByte) break;
                    *byteRef++ = Ascii.NumberSign;

                    var upper = (byte)(((@byte & 0xF0) >> 4) + Ascii.Digit0);
                    if (upper > Ascii.Digit9) upper = (byte)(upper + 7);
                    *byteRef++ = upper;

                    var lower = (byte)((@byte & 0x0F) + Ascii.Digit0);
                    if (lower > Ascii.Digit9) lower = (byte)(lower + 7);
                    *byteRef++ = lower;
                }
                else
                {
                    *byteRef++ = @byte;
                }
            }
        }

        protected override unsafe int GetCharCountInternal(byte* bytes, int offset, int count)
        {
            var charCount = Math.Max(count - 1, 0);
            var byteRef = bytes + offset;
            var lastByte = byteRef + count;

            while (byteRef < lastByte)
            {
                if (*byteRef++ == Ascii.NumberSign) charCount -= 2;
            }

            return Math.Max(charCount, 0);
        }

        protected override unsafe void GetCharsInternal(byte* bytes, char* chars, int byteCount, int charCount)
        {
            var escapedByte = Ascii.Null;
            var hashFound = false;
            var upperNibbleFound = false;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var charRef = chars;
            var lastChar = chars + charCount;

            if (byteCount == 0 || *byteRef++ != Ascii.ForwardSlash) throw new EncoderFallbackException();

            while (byteRef < lastByte && charRef < lastChar)
            {
                var @byte = *byteRef++;
                if (@byte == Ascii.Null) throw new EncoderFallbackException();
                if (@byte == Ascii.NumberSign)
                {
                    if (hashFound || upperNibbleFound) throw new EncoderFallbackException();
                    hashFound = true;
                    escapedByte = Ascii.Null;
                    continue;
                }

                if (hashFound)
                {
                    hashFound = false;
                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit9)
                    {
                        escapedByte = (byte)((@byte - Ascii.Digit0) << 4);
                    }
                    else if (@byte >= Ascii.A && @byte <= Ascii.F)
                    {
                        escapedByte = (byte)((@byte - Ascii.Digit7) << 4);
                    }
                    else if (@byte >= Ascii.LowercaseA && @byte <= Ascii.LowercaseF)
                    {
                        escapedByte = (byte)((@byte - Ascii.W) << 4);
                    }
                    else
                    {
                        throw new EncoderFallbackException();
                    }

                    upperNibbleFound = true;
                    continue;
                }

                if (upperNibbleFound)
                {
                    upperNibbleFound = false;
                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit9)
                    {
                        escapedByte |= (byte)(@byte - Ascii.Digit0);
                    }
                    else if (@byte >= Ascii.A && @byte <= Ascii.F)
                    {
                        escapedByte |= (byte)(@byte - Ascii.Digit7);
                    }
                    else if (@byte >= Ascii.LowercaseA && @byte <= Ascii.LowercaseF)
                    {
                        escapedByte |= (byte)(@byte - Ascii.W);
                    }
                    else
                    {
                        throw new EncoderFallbackException();
                    }

                    *charRef++ = (char)escapedByte;
                    continue;
                }

                var character = (char)@byte;
                *charRef++ = character;
            }
        }
    }
}
