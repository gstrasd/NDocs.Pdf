using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Types;

namespace NDocs.Pdf.Parsing
{
    public class Trailer : Dictionary
    {
        public long CrossReferenceOffset { get; set; }
    }
}