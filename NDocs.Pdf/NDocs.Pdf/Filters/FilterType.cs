using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Filters
{
    public enum FilterType
    {
        AsciiHexDecode,
        Ascii85Decode,
        LzwDecode,
        FlateDecode,
        RunLengthDecode,
        CcittFaxDecode,
        Jbig2Decode,
        DctDecode,
        JpxDecode,
        Crypt
    }
}
