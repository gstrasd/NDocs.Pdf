using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf
{
    public partial class PdfDocument
    {
        public Version Version => Header.Version;
    }
}
