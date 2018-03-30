using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Filters
{
    internal class AsciiHexDecodeFilter : IFilterStrategy
    {
        public AsciiHexDecodeFilter(bool encodeHexadecimalToUpperCase = true)
        {
            EncodeHexadecimalToUpperCase = encodeHexadecimalToUpperCase;
        }

        public bool EncodeHexadecimalToUpperCase { get; }

        public unsafe long GetMaxEncodedByteCount(byte* bytes, long byteCount)
        {
            return Math.Max(byteCount, 0) * 2 + 1;
        }

        public unsafe long EncodeBytes(byte* bytes, long byteCount, byte* encodedBytes)
        {
            var alphabeticOffset = EncodeHexadecimalToUpperCase ? 7 : 39;
            var byteRef = bytes;
            var lastByte = bytes + byteCount - 1;
            var encodedByteRef = encodedBytes;

            while (byteRef <= lastByte)
            {
                var @byte = *byteRef++;

                var upper = (byte)(((@byte & 0xF0) >> 4) + Ascii.Digit0);
                if (upper > Ascii.Digit9) upper = (byte)(upper + alphabeticOffset);
                *encodedByteRef++ = upper;

                var lower = (byte)((@byte & 0x0F) + Ascii.Digit0);
                if (lower > Ascii.Digit9) lower = (byte)(lower + alphabeticOffset);
                *encodedByteRef++ = lower;
            }

            *encodedByteRef++ = Ascii.GreaterThanSign;
            return encodedByteRef - encodedBytes;
        }

        public unsafe long GetMaxDecodedByteCount(byte* bytes, long byteCount)
        {
            return Math.Max((byteCount - 1) / 2, 0);
        }

        public unsafe long DecodeBytes(byte* bytes, long byteCount, byte* decodedBytes)
        {
            if (byteCount < 1) throw new FilterException();

            var decodedByte = (byte)0x00;
            var upperNibbleFound = false;
            var byteRef = bytes;
            var lastByte = bytes + byteCount - 1;
            var decodedByteRef = decodedBytes;

            if (*lastByte != Ascii.GreaterThanSign) throw new FilterException();

            while (byteRef < lastByte)
            {
                var @byte = *byteRef++;

                if (Ascii.Whitespace.Contains(@byte)) continue;

                if (!upperNibbleFound)
                {
                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit9)
                    {
                        decodedByte = (byte)((@byte - Ascii.Digit0) << 4);
                    }
                    else if (@byte >= Ascii.A && @byte <= Ascii.F)
                    {
                        decodedByte = (byte)((@byte - Ascii.Digit7) << 4);
                    }
                    else if (@byte >= Ascii.LowercaseA && @byte <= Ascii.LowercaseF)
                    {
                        decodedByte = (byte)((@byte - Ascii.W) << 4);
                    }
                    else
                    {
                        throw new FilterException();
                    }

                    upperNibbleFound = true;
                    continue;
                }

                upperNibbleFound = false;

                if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit9)
                {
                    decodedByte |= (byte)(@byte - Ascii.Digit0);
                }
                else if (@byte >= Ascii.A && @byte <= Ascii.F)
                {
                    decodedByte |= (byte)(@byte - Ascii.Digit7);
                }
                else if (@byte >= Ascii.LowercaseA && @byte <= Ascii.LowercaseF)
                {
                    decodedByte |= (byte)(@byte - Ascii.W);
                }
                else
                {
                    throw new FilterException();
                }

                *decodedByteRef++ = decodedByte;
            }

            if (upperNibbleFound)
            {
                *decodedByteRef++ = decodedByte;
            }

            return decodedByteRef - decodedBytes;
        }
    }
}