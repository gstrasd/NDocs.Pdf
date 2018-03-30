using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf
{
    public interface IPersistable
    {
        ObjectState State { get; }
        event EventHandler<ObjectState> StateChanged;
    }
}
