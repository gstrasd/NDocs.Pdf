using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Filters
{
    internal class CryptFilter : IFilterStrategy
    {
        public unsafe long GetMaxEncodedByteCount(byte* bytes, long byteCount)
        {
            throw new NotImplementedException();
        }

        public unsafe long EncodeBytes(byte* bytes, long byteCount, byte* encodedBytes)
        {
            throw new NotImplementedException();
        }

        public unsafe long GetMaxDecodedByteCount(byte* bytes, long byteCount)
        {
            throw new NotImplementedException();
        }

        public unsafe long DecodeBytes(byte* bytes, long byteCount, byte* decodedBytes)
        {
            throw new NotImplementedException();
        }
    }
}