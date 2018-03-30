using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Filters;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Types
{
    public class Stream : IPdfType<byte[]>, IPdfType
    {
        private readonly byte[] _value;
        private readonly byte[] _data;
        private readonly Type _type;
        private readonly int _hashCode;
        private readonly Dictionary _properties;

        public Stream(byte[] value)
        {
            _properties = new Dictionary();
        }

        internal Stream(byte[] value, Dictionary properties)
        {
            _properties = properties;
        }

        public byte[] Value => _value;
        //object IPdfType.Value => _value;
        object IPdfType.Value { get; set; }
        Type IPdfType.Type => _type;

        public long Length => _properties.Get<long>("Length");

        public FilterType[] Filters
        {
            get
            {
                var value = _properties["Filter"];
                if (value.Type == typeof(Name))
                {
                    return new FilterType[0];
                }
                if (value.Type == typeof(Array))
                {
                    return new FilterType[0];
                }
                return new FilterType[0];
            }
        }

        byte[] IPdfType<byte[]>.Value { get; set; }
        public ObjectState State { get; }
        public event EventHandler<ObjectState> StateChanged;
        public void Render(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}