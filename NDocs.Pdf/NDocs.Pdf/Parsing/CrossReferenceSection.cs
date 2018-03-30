using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public class CrossReferenceSection : List<CrossReference>
    {
        public CrossReferenceSection()
        {
        }

        public CrossReferenceSection(IEnumerable<CrossReference> crossReferences) : base(crossReferences)
        {
        }

        public CrossReferenceSection(params CrossReference[] crossReferences) : base(crossReferences)
        {
        }

        public int ObjectNumber => Count > 0 ? this[0].Id.ObjectNumber : CrossReference.Head.Id.ObjectNumber;

        public override string ToString()
        {
            var value = new StringBuilder();
            value.Append($"{ObjectNumber} {Count}\r\n");
            ForEach(cr => value.Append(cr));

            return value.ToString();
        }
    }
}