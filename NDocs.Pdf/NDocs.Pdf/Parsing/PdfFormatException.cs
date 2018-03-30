using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public class PdfFormatException : Exception
    {
        public PdfFormatException() : base()
        {
        }

        public PdfFormatException(string message) : base(message)
        {
        }

        public PdfFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
