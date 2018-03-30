using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    internal interface IPdfScannerFactory
    {
        IPdfScanner GetScanner(PdfStream pdfStream);
    }
}
