using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    internal class PdfScannerFactory : IPdfScannerFactory
    {
        public IPdfScanner GetScanner(PdfStream pdfStream)
        {
            return new RandomAccessPdfScanner(pdfStream);
        }
    }
}
