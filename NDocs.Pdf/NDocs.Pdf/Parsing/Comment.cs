using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public class Comment
    {
        public Comment(byte[] comment)
        {
        }

        public override string ToString()
        {
            return "%" + base.ToString();
        }
    }
}