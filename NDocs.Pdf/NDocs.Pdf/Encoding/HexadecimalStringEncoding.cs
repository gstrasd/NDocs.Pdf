using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Encoding
{
    internal class HexadecimalStringEncoding : BaseEncoding
    {
        public HexadecimalStringEncoding(bool encodeHexadecimalToUpperCase = true)
        {
            EncodeHexadecimalToUpperCase = encodeHexadecimalToUpperCase;
        }

        public override string EncodingName => "HexadecimalString";

        public bool EncodeHexadecimalToUpperCase { get; }

        protected override unsafe int GetByteCountInternal(char* chars, int index, int count)
        {
            return Math.Max(count, 0) * 2 + 2;
        }

        protected override int GetMaxByteCountInternal(int charCount)
        {
            return Math.Max(charCount, 0) * 2 + 2;
        }

        protected override unsafe void GetBytesInternal(char* chars, byte* bytes, int charCount, int byteCount)
        {
            var alphabeticOffset = EncodeHexadecimalToUpperCase ? 7 : 39;
            var charRef = chars;
            var lastChar = chars + charCount;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var secondToLastByte = lastByte - 1;

            if (byteRef < lastByte) *byteRef++ = Ascii.LessThanSign;

            while (charRef < lastChar && byteRef < secondToLastByte)
            {
                var character = *charRef++;
                if (character > '\u00ff') throw new EncoderFallbackException();

                var @byte = (byte)character;
                var upper = (byte)(((@byte & 0xF0) >> 4) + Ascii.Digit0);
                if (upper > Ascii.Digit9) upper = (byte)(upper + alphabeticOffset);
                *byteRef++ = upper;

                var lower = (byte)((@byte & 0x0F) + Ascii.Digit0);
                if (lower > Ascii.Digit9) lower = (byte)(lower + alphabeticOffset);
                *byteRef++ = lower;
            }

            if (byteRef < lastByte) *byteRef = Ascii.GreaterThanSign;
        }

        protected override unsafe int GetCharCountInternal(byte* bytes, int offset, int count)
        {
            var charCount = 0;
            var byteRef = bytes + offset;
            var lastByte = byteRef + count;

            while (byteRef < lastByte)
            {
                var @byte = *byteRef++;
                if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit9 || @byte >= Ascii.A && @byte <= Ascii.F || @byte >= Ascii.LowercaseA && @byte <= Ascii.LowercaseF) charCount++;
            }

            return charCount % 2 == 0 ? charCount / 2 : (charCount + 1) / 2;
        }

        protected override int GetMaxCharCountInternal(int byteCount)
        {
            return Math.Max((byteCount - 2) / 2, 0);
        }

        protected override unsafe void GetCharsInternal(byte* bytes, char* chars, int byteCount, int charCount)
        {
            var character = Ascii.Null;
            var beginStringFound = false;
            var endStringFound = false;
            var upperNibbleFound = false;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var charRef = chars;
            var lastChar = chars + charCount;

            while (byteRef < lastByte && charRef < lastChar)
            {
                var @byte = *byteRef++;

                if (Ascii.Whitespace.Contains(@byte)) continue;

                if (@byte == Ascii.LessThanSign)
                {
                    if (beginStringFound) throw new EncoderFallbackException();
                    beginStringFound = true;
                    continue;
                }

                if (@byte == Ascii.GreaterThanSign)
                {
                    if (!beginStringFound) throw new EncoderFallbackException();
                    if (endStringFound) throw new EncoderFallbackException();
                    endStringFound = true;
                    if (upperNibbleFound)
                    {
                        *charRef++ = (char)character;
                        upperNibbleFound = false;
                    }
                    continue;
                }

                if (!upperNibbleFound)
                {
                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit9)
                    {
                        character = (byte)((@byte - Ascii.Digit0) << 4);
                    }
                    else if (@byte >= Ascii.A && @byte <= Ascii.F)
                    {
                        character = (byte)((@byte - Ascii.Digit7) << 4);
                    }
                    else if (@byte >= Ascii.LowercaseA && @byte <= Ascii.LowercaseF)
                    {
                        character = (byte)((@byte - Ascii.W) << 4);
                    }
                    else
                    {
                        throw new EncoderFallbackException();
                    }

                    upperNibbleFound = true;
                    continue;
                }

                upperNibbleFound = false;

                if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit9)
                {
                    character |= (byte)(@byte - Ascii.Digit0);
                }
                else if (@byte >= Ascii.A && @byte <= Ascii.F)
                {
                    character |= (byte)(@byte - Ascii.Digit7);
                }
                else if (@byte >= Ascii.LowercaseA && @byte <= Ascii.LowercaseF)
                {
                    character |= (byte)(@byte - Ascii.W);
                }
                else
                {
                    throw new EncoderFallbackException();
                }

                *charRef++ = (char)character;
            }

            if (!endStringFound) throw new EncoderFallbackException();
        }
    }
}