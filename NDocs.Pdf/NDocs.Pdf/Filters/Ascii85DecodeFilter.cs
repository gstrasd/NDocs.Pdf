using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Filters
{
    internal class Ascii85DecodeFilter : IFilterStrategy
    {
        private static readonly uint[] _base85Powers = { 1, 85, 7225, 614125, 52200625 };
        private static readonly uint[] _byteMasks = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };

        public unsafe long GetMaxEncodedByteCount(byte* bytes, long byteCount)
        {
            byteCount = Math.Max(byteCount, 0);
            if (byteCount == 0) return 2;

            var lastBlockCount = byteCount % 4;
            if (lastBlockCount == 0) return byteCount * 5 / 4 + 2;

            return byteCount * 5 / 4 + lastBlockCount + 2;
        }

        public unsafe long EncodeBytes(byte* bytes, long byteCount, byte* encodedBytes)
        {
            var encodedByteRef = encodedBytes;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var lastBlock = byteCount % 4;
            var lastBlockByte = bytes + byteCount - lastBlock;

            while (byteRef < lastBlockByte)
            {
                var block = ((uint)*byteRef++ << 24) + ((uint)*byteRef++ << 16) + ((uint)*byteRef++ << 8) + *byteRef++;

                if (block == 0)
                {
                    *encodedByteRef++ = Ascii.LowercaseZ;
                }
                else
                {
                    *encodedByteRef++ = (byte)((byte)(block / 52200625 % 85) + Ascii.ExclamationMark);
                    *encodedByteRef++ = (byte)((byte)(block / 614125 % 85) + Ascii.ExclamationMark);
                    *encodedByteRef++ = (byte)((byte)(block / 7225 % 85) + Ascii.ExclamationMark);
                    *encodedByteRef++ = (byte)((byte)(block / 85 % 85) + Ascii.ExclamationMark);
                    *encodedByteRef++ = (byte)((byte)(block % 85) + Ascii.ExclamationMark);
                }
            }

            if (lastBlock > 0)
            {
                uint block = 0;
                var position = 3;
                while (byteRef < lastByte)
                {
                    block += (uint)*byteRef++ << (8 * position--);
                }

                position = 4;
                while (lastBlock-- >= 0)
                {
                    *encodedByteRef++ = (byte)((byte)(block / _base85Powers[position--] % 85) + Ascii.ExclamationMark);
                }
            }

            *encodedByteRef++ = Ascii.Tilde;
            *encodedByteRef++ = Ascii.GreaterThanSign;

            return encodedByteRef - encodedBytes;
        }

        public unsafe long GetMaxDecodedByteCount(byte* bytes, long byteCount)
        {
            byteCount = Math.Max(byteCount - 2, 0);
            if (byteCount <= 0) return 0;

            var lastBlockCount = byteCount % 5;
            if (lastBlockCount == 0) return byteCount * 4 / 5;

            return (byteCount - lastBlockCount) * 4 / 5 + lastBlockCount - 1;
        }

        public unsafe long DecodeBytes(byte* bytes, long byteCount, byte* decodedBytes)
        {
            if (byteCount < 2) throw new FilterException();

            var trailer = bytes + byteCount - 2;
            if (*trailer++ != Ascii.Tilde || *trailer != Ascii.GreaterThanSign) throw new FilterException();

            byteCount -= 2;
            var lastByte = bytes + byteCount;
            var byteRef = bytes;
            var decodedByteRef = decodedBytes;
            uint block = 0;
            var position = 5;

            while (byteRef < lastByte)
            {
                var @byte = *byteRef++;

                if (Ascii.Whitespace.Contains(@byte)) continue;
                if (@byte == Ascii.LowercaseZ)
                {
                    if (position < 5) throw new FilterException();
                    *decodedByteRef++ = 0;
                    *decodedByteRef++ = 0;
                    *decodedByteRef++ = 0;
                    *decodedByteRef++ = 0;
                }
                else if (@byte < Ascii.ExclamationMark || @byte > Ascii.LowercaseU) throw new FilterException();
                else
                {
                    block += (uint)(@byte - Ascii.ExclamationMark) * _base85Powers[--position];
                    if (position == 0)
                    {
                        *decodedByteRef++ = (byte)((block & 0xFF000000) >> 24);
                        *decodedByteRef++ = (byte)((block & 0x00FF0000) >> 16);
                        *decodedByteRef++ = (byte)((block & 0x0000FF00) >> 8);
                        *decodedByteRef++ = (byte)(block & 0x000000FF);
                        block = 0;
                        position = 5;
                    }
                }
            }

            if (position < 5)
            {
                var lastBlockPosition = 3;
                while (lastBlockPosition >= position)
                {
                    *decodedByteRef++ = (byte)((block & _byteMasks[lastBlockPosition]) >> (lastBlockPosition-- * 8));
                }
            }

            return decodedByteRef - decodedBytes;
        }
    }
}