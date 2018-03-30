using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    internal class PdfReader
    {
        private PdfStream _pdfStream;

        public PdfReader(Stream stream)
        {
            _pdfStream = new PdfStream(stream);
        }

        // TODO: This is not thread safe because if two different versions of PDF documents are being read, the parser might use the wrong Name encoding. Somehow create a "reading session" or "document session"
        public PdfDocument Read()
        {
            // TODO: I don't like this injecting factory bullshit, find a better way
            var parser = new PdfParser(_pdfStream, new PdfScannerFactory());
            var header = parser.ReadHeader();
            
            // Ensure that the document being read is supported
            if (!VersionSelector.IsSupportedVersion(header.Version)) throw new VersionNotSupportedException(header.Version);

            // Register the appropriate encoding provider for this document version
            var encodingProvider = VersionSelector.GetVersionedInstance<EncodingProvider>(header.Version);
            System.Text.Encoding.RegisterProvider(encodingProvider);

            var indirectObject = parser.ReadIndirectObject(17L);
            var document = new PdfDocument
            {
                Header = header
            };

            return document;
        }
    }
}
