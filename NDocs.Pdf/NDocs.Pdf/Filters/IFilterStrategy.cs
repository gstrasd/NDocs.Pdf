using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Filters
{
    internal interface IFilterStrategy
    {
        unsafe long GetMaxEncodedByteCount(byte* bytes, long byteCount);
        unsafe long EncodeBytes(byte* bytes, long byteCount, byte* encodedBytes);
        unsafe long GetMaxDecodedByteCount(byte* bytes, long byteCount);
        unsafe long DecodeBytes(byte* bytes, long byteCount, byte* decodedBytes);
    }
}