using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream = System.IO.Stream;

namespace NDocs.Pdf.Parsing
{
    internal sealed class PdfStream
    {
        public PdfStream(Stream stream)
        {
            Buffer = new byte[stream.Length];
            stream.Read(Buffer, 0, Convert.ToInt32(stream.Length));
            Length = Buffer.LongLength;
        }

        public long Length { get; }
        public byte[] Buffer { get; }

        //public bool Next(Expression expression)
        //{
        //    EnsureNotDisposed();
        //    if (expression == null) throw new ArgumentNullException(nameof(expression));

        //    var current = Position;
        //    var offset = 0L;
        //    var maxOffset = Length - Position - 1;

        //    while (offset <= maxOffset)
        //    {
        //        if (expression.Matches(*(_origin + Position + offset)))
        //        {
        //            Position += offset;
        //            return true;
        //        }
        //        offset++;
        //    }

        //    Position = current;
        //    return false;
        //}

        //public bool Matches(byte[] data)
        //{
        //    EnsureNotDisposed();
        //    if (data == null) throw new ArgumentNullException(nameof(data));
            
        //    if (Position + data.Length >= Length) return false;

        //    var offset = 0;
        //    while (offset < data.Length)
        //    {
        //        if (*(_origin + Position + offset) != data[offset]) return false;
        //        offset++;
        //    }

        //    return true;
        //}

        //public Integer ReadInteger()
        //{
        //    var character = *(_origin + Position);
        //    if (character < Ascii.Digit0 || character > Ascii.Digit9) throw new InvalidDataException("The character at the current buffer position does not represent an integer.");

        //    var value = 0L;
        //    var offset = 0L;
        //    var maxOffset = Length - Position - 1;

        //    while (offset <= maxOffset)
        //    {
        //        character = *(_origin + Position + offset);
        //        if (character < Ascii.Digit0 || character > Ascii.Digit9) break;
        //        value = value * 10 + character - Ascii.Digit0;
        //        offset++;
        //    }
           
        //    return new Integer(value);
        //}
    }
}
