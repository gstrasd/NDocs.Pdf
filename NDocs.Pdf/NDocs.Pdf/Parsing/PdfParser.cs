using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Types;
using Array = NDocs.Pdf.Types.Array;

namespace NDocs.Pdf.Parsing
{
    internal sealed unsafe class PdfParser : IDisposable
    {
        private PdfStream _pdfStream;
        private readonly IPdfScannerFactory _scannerFactory;
        private GCHandle _handle;
        private byte* _origin;
        private bool _disposed;

        public PdfParser(PdfStream pdfStream, IPdfScannerFactory scannerFactory)
        {
            _pdfStream = pdfStream;
            _scannerFactory = scannerFactory;
            _handle = GCHandle.Alloc(_pdfStream.Buffer, GCHandleType.Pinned);
            _origin = (byte*)_handle.AddrOfPinnedObject().ToPointer();
        }

        ~PdfParser()
        {
            ReleaseUnmanagedResources();
        }

        public Header ReadHeader()
        {
            EnsureNotDisposed();

            var scanner = _scannerFactory.GetScanner(_pdfStream);
            using (var enumerator = scanner.ScanHeader().GetEnumerator())
            {
                // major version
                enumerator.MoveNext();
                var majorVersion = *(_origin + enumerator.Current.Index) - Ascii.Digit0;

                // minor version
                enumerator.MoveNext();
                var minorVersion = *(_origin + enumerator.Current.Index) - Ascii.Digit0;

                // magic number
                enumerator.MoveNext();
                var magicNumber = BitConverter.ToUInt32(_pdfStream.Buffer, (int) enumerator.Current.Index);
                var header = new Header(majorVersion, minorVersion, magicNumber);

                return header;
            }
        }

        public Trailer ReadTrailer()
        {
            EnsureNotDisposed();

            var scanner = _scannerFactory.GetScanner(_pdfStream);

            using (var enumerator = scanner.ScanTrailer().GetEnumerator())
            {
                // trailer
                enumerator.MoveNext();
            }

            return null;
        }

        public IndirectObject ReadIndirectObject(long position)
        {
            EnsureNotDisposed();

            var scanner = _scannerFactory.GetScanner(_pdfStream);
            var enumerator = scanner.ScanIndirectObject(position).GetEnumerator();

            // Read object number
            EnsureMoveNext(enumerator);
            var objectNumber = ParseObjectNumber(enumerator.Current);

            // Read generation number
            EnsureMoveNext(enumerator);
            var generationNumber = ParseGenerationNumber(enumerator.Current);

            // Ensure proper token sequence
            EnsureMoveNext(enumerator);
            EnsureTokenClassification(enumerator.Current, TokenClassification.BeginObject);

            // Identify object type
            EnsureMoveNext(enumerator);

            IPdfType value;
            if (enumerator.Current.Classification == TokenClassification.BeginDictionary) value = ParseDictionary(enumerator);
            else if (enumerator.Current.Classification == TokenClassification.BeginArray) value = ParseArray(enumerator);
            else value = ParseObject(enumerator.Current);

            return new IndirectObject(objectNumber, generationNumber, value);
        }

        private Dictionary ParseDictionary(IEnumerator<Token> enumerator)
        {
            var dictionary = new Dictionary();

            EnsureMoveNext(enumerator);

            while (enumerator.Current.Classification != TokenClassification.EndDictionary)
            {
                // Read name
                var name = ParseName(enumerator.Current);

                // Read value
                EnsureMoveNext(enumerator);
                IPdfType value;
                if (enumerator.Current.Classification == TokenClassification.BeginDictionary) value = ParseDictionary(enumerator);
                //else if (enumerator.Current.Classification == TokenClassification.BeginArray) value = ParseArray(enumerator);
                else value = ParseObject(enumerator.Current);
                dictionary.Add(name, value);

                EnsureMoveNext(enumerator);
            }

            return dictionary;
        }

        private Array ParseArray(IEnumerator<Token> enumerator)
        {
            var array = new Array();

            EnsureMoveNext(enumerator);

            while (enumerator.Current.Classification != TokenClassification.EndArray)
            {
                // Read value
                EnsureMoveNext(enumerator);
                object value;
                if (enumerator.Current.Classification == TokenClassification.BeginDictionary) value = ParseDictionary(enumerator);
                else if (enumerator.Current.Classification == TokenClassification.BeginArray) value = ParseArray(enumerator);
                else value = ParseObject(enumerator.Current);
                array.Add(value);

                EnsureMoveNext(enumerator);
            }

            return array;
        }

        private IPdfType ParseObject(Token token)
        {
            switch (token.Classification)
            {
                case TokenClassification.Undefined: throw new NotImplementedException();
                case TokenClassification.HeaderMajorVersion: throw new NotImplementedException();
                case TokenClassification.HeaderMinorVersion: throw new NotImplementedException();
                case TokenClassification.HeaderMagicNumber: throw new NotImplementedException();
                case TokenClassification.Comment: throw new NotImplementedException();
                case TokenClassification.BooleanTrue: throw new NotImplementedException();
                case TokenClassification.BooleanFalse: throw new NotImplementedException();
                case TokenClassification.Integer: return ParseInteger(token);
                case TokenClassification.Real: throw new NotImplementedException();
                case TokenClassification.LiteralString: throw new NotImplementedException();
                case TokenClassification.HexadecimalString: throw new NotImplementedException();
                case TokenClassification.Name: return ParseName(token);
                case TokenClassification.BeginStream: throw new NotImplementedException();
                case TokenClassification.Stream: throw new NotImplementedException();
                case TokenClassification.EndStream: throw new NotImplementedException();
                case TokenClassification.FreeEntry: throw new NotImplementedException();
                case TokenClassification.NewEntry: throw new NotImplementedException();
                case TokenClassification.Null: throw new NotImplementedException();
                case TokenClassification.ObjectReference: throw new NotImplementedException();
                case TokenClassification.StartCrossReference: throw new NotImplementedException();
                case TokenClassification.CrossReference: throw new NotImplementedException();
                case TokenClassification.Trailer: throw new NotImplementedException();
                case TokenClassification.EndOfFile: throw new NotImplementedException();
                default: throw new PdfFormatException();        // TODO: set message to "Unexpected token : {token classification}", but convert classification name into the actual token string for clarity
            }
        }

        private Name ParseName(Token token)
        {
            EnsureTokenClassification(token, TokenClassification.Name);
            return new Name(_origin, token.Index, token.Length);
        }

        private Integer ParseInteger(Token token)
        {
            EnsureTokenClassification(token, TokenClassification.Integer);
            return new Integer(_origin, token.Index, token.Length);
        }

        private ushort ParseGenerationNumber(Token token)
        {
            EnsureTokenClassification(token, TokenClassification.GenerationNumber);
            var position = token.Index;
            var length = token.Length;
            ushort value = 0;
            for (var index = 0; index < length; index++)
            {
                value *= 10;
                value += *(_origin + position + index);     // TODO: Make sure this cannot create an overflow exception
                value -= Ascii.Digit0;
            }

            return value;
        }

        private int ParseObjectNumber(Token token)
        {
            EnsureTokenClassification(token, TokenClassification.ObjectNumber);
            var position = token.Index;
            var length = token.Length;
            var value = 0;
            for (var index = 0; index < length; index++)
            {
                value = value * 10 + *(_origin + position + index) - Ascii.Digit0;
            }

            return value;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            if (_disposed) return;
            if (_handle.IsAllocated) _handle.Free();
            _origin = null;
            _disposed = true;
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException($"Operations cannot be performed on a disposed {nameof(PdfParser)}.");
        }

        private void EnsureMoveNext(IEnumerator<Token> enumerator)
        {
            if (!enumerator.MoveNext()) throw new PdfFormatException();
        }

        private void EnsureTokenClassification(Token token, TokenClassification tokenClassification)
        {
            if (token.Classification != tokenClassification) throw new PdfFormatException();
        }
    }
}
