using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    public class Array : IPdfType, IEnumerable<object>
    {
        List<object> _array = new List<object>(16);

        public void Add(object value)
        {
            _array.Add(value);
        }

        public IEnumerator<object> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ObjectState State { get; }
        public event EventHandler<ObjectState> StateChanged;
        public void Render(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public object Value { get; set; }
        public Type Type { get; }
    }
}