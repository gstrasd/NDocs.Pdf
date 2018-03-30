using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf
{
    public partial class PdfDocument
    {
        internal PdfDocument() { }
        internal Header Header { get; set; }
        internal Trailer Trailer { get; set; }
    }
}
