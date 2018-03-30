using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Filters
{
    internal sealed class Filter
    {
        private readonly IList<IFilterStrategy> _encodeFilters;
        private readonly IList<IFilterStrategy> _decodeFilters;

        public Filter(params FilterType[] filters)
        {
            if (filters == null) throw new ArgumentNullException(nameof(filters));

            _encodeFilters = filters.Select<FilterType, IFilterStrategy>(type =>
            {
                switch (type)
                {
                    case FilterType.AsciiHexDecode: return new AsciiHexDecodeFilter();
                    case FilterType.Ascii85Decode: return new Ascii85DecodeFilter();
                    case FilterType.LzwDecode: return new LzwDecodeFilter();
                    case FilterType.FlateDecode: return new FlateDecodeFilter();
                    case FilterType.RunLengthDecode: return new RunLengthDecodeFilter();
                    case FilterType.CcittFaxDecode: return new CcittFaxDecodeFilter();
                    case FilterType.Jbig2Decode: return new Jbig2DecodeFilter();
                    case FilterType.DctDecode: return new DctDecodeFilter();
                    case FilterType.JpxDecode: return new JpxDecodeFilter();
                    case FilterType.Crypt: return new CryptFilter();
                    default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }).ToList();
            if (!_encodeFilters.Any()) throw new ArgumentException(nameof(filters));
            _decodeFilters = _encodeFilters.Reverse().ToList();
        }

        public unsafe byte[] EncodeBytes(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var sourceBytes = bytes;
            var sourceCount = bytes.LongLength;
            foreach (var filter in _encodeFilters)
            {
                fixed (byte* sourceRef = sourceBytes)
                {
                    var maxEncodedCount = filter.GetMaxEncodedByteCount(sourceRef, sourceCount);
                    var encodedBytes = new byte[maxEncodedCount];

                    fixed (byte* destinationRef = encodedBytes)
                    {
                        sourceCount = filter.EncodeBytes(sourceRef, sourceCount, destinationRef);
                        sourceBytes = encodedBytes;
                    }
                }
            }

            if (sourceCount < sourceBytes.LongLength)
            {
                fixed (byte* sourceRef = sourceBytes)
                {
                    sourceBytes = ArrayHelper.Copy(sourceRef, 0, sourceCount);
                }
            }

            return sourceBytes;
        }

        public unsafe byte[] DecodeBytes(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var sourceBytes = bytes;
            var sourceCount = bytes.LongLength;
            foreach (var filter in _decodeFilters)
            {
                fixed (byte* sourceRef = sourceBytes)
                {
                    var maxEncodedCount = filter.GetMaxDecodedByteCount(sourceRef, sourceCount);
                    var decodedBytes = new byte[maxEncodedCount];

                    fixed (byte* destinationRef = decodedBytes)
                    {
                        sourceCount = filter.DecodeBytes(sourceRef, sourceCount, destinationRef);
                        sourceBytes = decodedBytes;
                    }
                }
            }

            if (sourceCount < sourceBytes.LongLength)
            {
                fixed (byte* sourceRef = sourceBytes)
                {
                    sourceBytes = ArrayHelper.Copy(sourceRef, 0, sourceCount);
                }
            }

            return sourceBytes;
        }
    }
}
