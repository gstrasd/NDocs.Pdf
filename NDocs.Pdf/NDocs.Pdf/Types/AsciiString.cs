using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Types
{
    [PdfVersion("1.7")]
    public class AsciiString : String
    {
        public AsciiString(string value) : base(value, StringEncodingType.Ascii, StringFormatType.Literal)
        {
        }

        internal unsafe AsciiString(byte* value, long offset, long length) : base(value, offset, length, StringEncodingType.Ascii, StringFormatType.Literal)
        {
        }
    }
}