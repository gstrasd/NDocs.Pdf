using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public class CrossReferenceTable : List<CrossReferenceSection>
    {
        public IEnumerable<CrossReference> CrossReferences => this.SelectMany(section => section);

        public override string ToString()
        {
            var value = new StringBuilder();
            value.Append("xref\r\n");
            ForEach(crs => value.Append(crs));

            return value.ToString();
        }
    }
}