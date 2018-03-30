using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Encoding
{
    [PdfVersion("1.0")]
    [PdfVersion("1.1")]
    internal class PdfEncodingProvider10 : EncodingProvider
    {
        public override System.Text.Encoding GetEncoding(string name)
        {
            switch (name)
            {
                //case "Name": return new NameEncoding10();
                case "LiteralString": return null;
                case "HexadecimalString": return null;
                default: return null;
            }
        }

        public override System.Text.Encoding GetEncoding(int codepage)
        {
            return null;
        }
    }

    [PdfVersion("1.2")]
    [PdfVersion("1.3")]
    [PdfVersion("1.4")]
    [PdfVersion("1.5")]
    [PdfVersion("1.6")]
    [PdfVersion("1.7")]
    internal class PdfEncodingProvider12 : EncodingProvider
    {
        public override System.Text.Encoding GetEncoding(string name)
        {
            switch (name)
            {
                //case "Name": return new NameEncoding12();
                case "LiteralString": return null;
                case "HexadecimalString": return null;
                default: return null;
            }
        }

        public override System.Text.Encoding GetEncoding(int codepage)
        {
            return null;
        }
    }
}
