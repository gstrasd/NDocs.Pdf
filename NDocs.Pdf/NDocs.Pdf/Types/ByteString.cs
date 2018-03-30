using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Encoding;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    [PdfVersion("1.7")]
    public class ByteString : String, IPdfType<byte[]>
    {
        private byte[] _value;

        public unsafe ByteString(byte[] value) : base(new FastAsciiEncoding().GetString(value), StringEncodingType.Unknown, StringFormatType.Hexadecimal)
        {
            fixed (byte* byteRef = value)
            {
                _value = ArrayHelper.Copy(byteRef, 0, value.Length);
            }
        }

        internal unsafe ByteString(byte* data, long offset, long length) : base(data, offset, length, StringEncodingType.Unknown, StringFormatType.Hexadecimal)
        {
            _value = new FastAsciiEncoding().GetBytes(Value);
        }

        unsafe byte[] IPdfType<byte[]>.Value
        {
            get => _value;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (ArrayHelper.Equals(_value, value)) return;
                fixed (byte* byteRef = value)
                {
                    _value = ArrayHelper.Copy(byteRef, 0, value.Length);
                }

                var stringValue = new FastAsciiEncoding().GetString(value);
                SetValue(stringValue);
                State = ObjectState.Updated;
            }
        }
    }
}