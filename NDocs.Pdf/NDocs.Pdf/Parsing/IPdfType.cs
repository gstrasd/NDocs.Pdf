using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public interface IPdfType<T>
    {
        T Value { get; set; }
    }

    public interface IPdfType : IPersistable, IRenderable
    {
        object Value { get; set; }
        Type Type { get; }
    }
}
