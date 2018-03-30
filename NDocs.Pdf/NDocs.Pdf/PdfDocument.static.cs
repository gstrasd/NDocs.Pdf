using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf
{
    public partial class PdfDocument
    {
        public static PdfDocument Load(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var reader = new PdfReader(stream);
            var document = reader.Read();

          

            return document;
        }
    }
}
