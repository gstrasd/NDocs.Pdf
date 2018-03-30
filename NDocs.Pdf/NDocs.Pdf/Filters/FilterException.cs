using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Filters
{
    public class FilterException : Exception
    {
        public FilterException() : base()
        {
        }

        public FilterException(string message) : base(message)
        {
        }

        public FilterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}