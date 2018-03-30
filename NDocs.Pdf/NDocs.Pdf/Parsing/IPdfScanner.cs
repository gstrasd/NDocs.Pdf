using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    internal interface IPdfScanner
    {
        IEnumerable<Token> ScanHeader();
        IEnumerable<Token> ScanTrailer();
        IEnumerable<Token> ScanCrossReference(long position);
        IEnumerable<Token> ScanIndirectObject(long position);
    }
}
