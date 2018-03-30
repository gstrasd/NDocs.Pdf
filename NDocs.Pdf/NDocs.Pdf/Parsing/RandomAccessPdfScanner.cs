using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    internal sealed unsafe class RandomAccessPdfScanner : IPdfScanner, IDisposable
    {
        private PdfStream _pdfStream;
        private GCHandle _handle;
        private byte* _origin;
        private bool _disposed;
        private long _position;

        private static readonly byte[] _header = System.Text.Encoding.ASCII.GetBytes("%PDF-");
        private static readonly byte[] _eof = System.Text.Encoding.ASCII.GetBytes("%%EOF");
        private static readonly byte[] _trailer = System.Text.Encoding.ASCII.GetBytes("trailer");
        private static readonly byte[] _beginDictionary = System.Text.Encoding.ASCII.GetBytes("<<");
        private static readonly byte[] _endDictionary = System.Text.Encoding.ASCII.GetBytes(">>");
        private static readonly byte[] _xref = System.Text.Encoding.ASCII.GetBytes("xref");
        private static readonly byte[] _startXref = System.Text.Encoding.ASCII.GetBytes("startxref");
        private static readonly byte[] _true = System.Text.Encoding.ASCII.GetBytes("true");
        private static readonly byte[] _false = System.Text.Encoding.ASCII.GetBytes("false");
        private static readonly byte[] _null = System.Text.Encoding.ASCII.GetBytes("null");
        private static readonly byte[] _obj = System.Text.Encoding.ASCII.GetBytes("obj");
        private static readonly byte[] _endobj = System.Text.Encoding.ASCII.GetBytes("endobj");
        private static readonly byte[] _stream = System.Text.Encoding.ASCII.GetBytes("stream");
        private static readonly byte[] _endStream = System.Text.Encoding.ASCII.GetBytes("endstream");
        private static readonly byte[] _length = System.Text.Encoding.ASCII.GetBytes("/Length");

        public RandomAccessPdfScanner(PdfStream pdfStream)
        {
            _pdfStream = pdfStream;
            _handle = GCHandle.Alloc(_pdfStream.Buffer, GCHandleType.Pinned);
            _origin = (byte*) _handle.AddrOfPinnedObject().ToPointer();
        }

        ~RandomAccessPdfScanner()
        {
            ReleaseUnmanagedResources();
        }

        public byte Current => *(_origin + _position);

        public IEnumerable<Token> ScanHeader()
        {
            EnsureNotDisposed();
            Token token;
            _position = 0;

            // Match %PDF-
            if (!Matches(_header)) throw new PdfFormatException();
            if (!Move(_header.LongLength)) throw new PdfFormatException();

            // Identify major version
            if (!TryScanMajorVersion(out token)) throw new PdfFormatException();
            yield return token;

            // Identify decimal point
            if (Current != Ascii.Period) throw new PdfFormatException();
            if (!Move(1L)) throw new PdfFormatException();

            // Identify minor version
            if (!TryScanMinorVersion(out token)) throw new PdfFormatException();
            yield return token;

            // Identify magic number
            SkipAll(Ascii.Whitespace);
            if (!TryScanMagicNumber(out token)) throw new PdfFormatException();
            yield return token;
        }

        public IEnumerable<Token> ScanTrailer()
        {
            EnsureNotDisposed();
            Token token;
            _position = _pdfStream.Length - 1;

            // Match trailer
            LastMatch(_trailer);
            if (!TryScanTrailer(out token)) throw new PdfFormatException();
            yield return token;

            // Scan dictionary
            SkipAll(Ascii.Whitespace);
            foreach (var t in ScanDictionary()) yield return t;

            // Scan startxref
            SkipAll(Ascii.Whitespace);
            if (!TryScanStartCrossReference(out token)) throw new PdfFormatException();
            yield return token;

            // Scan offset
            SkipAll(Ascii.Whitespace);
            if (!TryScanInteger(out token)) throw new PdfFormatException();
            yield return token;

            // Scan end of file
            SkipAll(Ascii.Whitespace);
            if (!TryScanEndOfFile(out token)) throw new PdfFormatException();
            yield return token;
        }

        public IEnumerable<Token> ScanCrossReference(long position)
        {
            EnsureNotDisposed();
            Token token;

            // Identify cross reference section
            if (!TryScanCrossReference(out token)) throw new PdfFormatException();
            yield return token;

            // Identify object number of first record
            SkipAll(Ascii.Whitespace);
            if (!TryScanObjectNumber(out token)) throw new PdfFormatException();
            yield return token;

            // Identify number of records
            if (Current != Ascii.Space || !Move(1L)) throw new PdfFormatException();
            if (!TryScanInteger(out token)) throw new PdfFormatException();
            var count = ParseInteger(token);
        }

        public IEnumerable<Token> ScanIndirectObject(long position)
        {
            Token token;
            if (position < 0 || position >= _pdfStream.Length) throw new ArgumentOutOfRangeException(nameof(position));
            _position = position;
            var matchName = true;
            var captureLength = false;
            var length = 0L;
            var dictionaryScanned = false;

            // Scan object number
            if (!TryScanObjectNumber(out token)) throw new PdfFormatException();
            yield return token;

            // Scan generation number
            SkipAll(Ascii.Whitespace);
            if (!TryScanGenerationNumber(out token)) throw new PdfFormatException();
            yield return token;

            // Scan begin object
            SkipAll(Ascii.Whitespace);
            if (!TryScanBeginObject(out token)) throw new PdfFormatException();
            yield return token;

            while (true)
            {
                SkipAll(Ascii.Whitespace);

                if (TryScanEndObject(out token))
                {
                    yield return token;
                    SkipAll(Ascii.Whitespace);
                    yield break;
                }
                if (!dictionaryScanned && TryScanBeginDictionary(out token))
                {
                    yield return token;
                    foreach (var t in ScanDictionary())
                    {
                        if (matchName && t.Classification == TokenClassification.Name && MatchesAt(_length, t.Index))
                        {
                            matchName = false;
                            captureLength = true;
                        }
                        else if (captureLength)
                        {
                            captureLength = false;
                            if (t.Classification == TokenClassification.Integer)
                            {
                                length = ParseInteger(t);
                            }
                        }
                        yield return t;
                    }
                    dictionaryScanned = true;
                }
                else if (dictionaryScanned)
                {
                    if (TryScanBeginStream(out token)) yield return token;
                    if (TryScanStream(length, out token)) yield return token;
                    if (!TryScanEndStream(out token)) throw new PdfFormatException();
                    yield return token;
                    SkipAll(Ascii.Whitespace);
                    if (!TryScanEndObject(out token)) throw new PdfFormatException();
                    yield return token;
                    SkipAll(Ascii.Whitespace);
                    yield break;
                }
                else if (TryScanName(out token)) yield return token;
                else if (TryScanNumeric(out token)) yield return token;
                else if (TryScanLiteralString(out token)) yield return token;
                else if (TryScanComment(out token)) yield return token;
                else if (TryScanObjectReference(out token)) yield return token;
                else if (TryScanNull(out token)) yield return token;
                else if (TryScanBooleanFalse(out token)) yield return token;
                else if (TryScanBooleanTrue(out token)) yield return token;
                else if (TryScanHexadecimalString(out token)) yield return token;
                else if (TryScanBeginArray(out token))
                {
                    yield return token;
                    foreach (var t in ScanArray()) yield return t;
                }
                else throw new PdfFormatException();
            }
        }

        private bool Matches(byte character)
        {
            return Current == character;
        }

        private bool Matches(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (_position + data.LongLength >= _pdfStream.Length) return false;

            var index = 0L;
            while (index < data.LongLength && *(_origin + _position + index) == data[index]) index++;
            return index == data.LongLength;
        }

        private bool MatchesAt(byte[] data, long position)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (position + data.LongLength >= _pdfStream.Length) return false;

            var index = 0L;
            while (index < data.LongLength && *(_origin + position + index) == data[index]) index++;
            return index == data.LongLength;
        }

        private bool Matches(Func<byte, bool> condition)
        {
            return condition(Current);
        }

        private bool Move(long distance)
        {
            if (distance == 0) return false;

            long position;

            try
            {
                position = checked(_position + distance);
            }
            catch (OverflowException)
            {
                return false;
            }

            if (position < 0 || position >= _pdfStream.Length) return false;

            _position = position;
            return true;
        }

        private bool Skip(byte data)
        {
            var position = _position;
            while (_position < _pdfStream.Length && *(_origin + _position) == data) _position++;

            return _position != position;
        }

        private bool SkipAll(byte[] data)
        {
            var position = _position;
            while (_position < _pdfStream.Length && data.Contains(*(_origin + _position))) _position++;

            return _position != position;
        }

        private bool SkipWhile(Func<byte, bool> condition)
        {
            var position = _position;
            while (_position < _pdfStream.Length && condition(*(_origin + _position))) _position++;

            return _position != position;
        }

        private bool NextAny(byte[] data)
        {
            var position = _position;
            while (_position < _pdfStream.Length && !data.Contains(*(_origin + _position))) _position++;

            return _position != position;
        }

        private bool NextMatch(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var position = _position;
            if (position + data.Length >= _pdfStream.Length) return false;
            
            var maxPosition = _pdfStream.Length - 1;
            var index = 0;

            while (position <= maxPosition)
            {
                if (*(_origin + position) != data[index])
                {
                    index = 0;
                }
                else if (++index == data.Length)
                {
                    _position = position - data.LongLength;
                    return true;
                }
                position++;
            }

            return false;
        }

        private bool LastMatch(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length > _pdfStream.Length) return false;

            var position = _pdfStream.Length - 1;
            var index = data.Length - 1;

            while (position >= 0L)
            {
                if (*(_origin + position) != data[index])
                {
                    index = data.Length - 1;
                }
                else if (--index < 0)
                {
                    _position = position;
                    return true;
                }
                position--;
            }

            return false;
        }

        private bool PreviousMatch(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length > _pdfStream.Length) return false;

            var position = _position;
            var index = data.Length - 1;

            while (position >= 0L)
            {
                if (*(_origin + position) != data[index])
                {
                    index = data.Length - 1;
                }
                else if (--index < 0)
                {
                    _position = position;
                    return true;
                }
                position--;
            }

            return false;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private IEnumerable<Token> ScanDictionary()
        {
            Token token;
            while (true)
            {
                SkipAll(Ascii.Whitespace);

                if (TryScanEndDictionary(out token))
                {
                    yield return token;
                    SkipAll(Ascii.Whitespace);
                    yield break;
                }
                else if (TryScanName(out token)) yield return token;
                else if (TryScanNumeric(out token)) yield return token;
                else if (TryScanLiteralString(out token)) yield return token;
                else if (TryScanComment(out token)) yield return token;
                else if (TryScanObjectReference(out token)) yield return token;
                else if (TryScanNull(out token)) yield return token;
                else if (TryScanBooleanFalse(out token)) yield return token;
                else if (TryScanBooleanTrue(out token)) yield return token;
                else if (TryScanBeginDictionary(out token))
                {
                    yield return token;
                    foreach (var t in ScanDictionary()) yield return t;
                }
                else if (TryScanHexadecimalString(out token)) yield return token;
                else if (TryScanBeginArray(out token))
                {
                    yield return token;
                    foreach (var t in ScanArray()) yield return t;
                }
                else throw new PdfFormatException();
            }
        }

        private IEnumerable<Token> ScanArray()
        {
            Token token;
            while (true)
            {
                SkipAll(Ascii.Whitespace);

                if (TryScanEndArray(out token))
                {
                    yield return token;
                    SkipAll(Ascii.Whitespace);
                    yield break;
                }
                else if (TryScanName(out token)) yield return token;
                else if (TryScanNumeric(out token)) yield return token;
                else if (TryScanLiteralString(out token)) yield return token;
                else if (TryScanComment(out token)) yield return token;
                else if (TryScanObjectReference(out token)) yield return token;
                else if (TryScanNull(out token)) yield return token;
                else if (TryScanBooleanFalse(out token)) yield return token;
                else if (TryScanBooleanTrue(out token)) yield return token;
                else if (TryScanBeginDictionary(out token))
                {
                    yield return token;
                    foreach (var t in ScanDictionary()) yield return t;
                }
                else if (TryScanHexadecimalString(out token)) yield return token;
                else if (TryScanBeginArray(out token))
                {
                    yield return token;
                    foreach (var t in ScanArray()) yield return t;
                }
                else throw new PdfFormatException();
            }
        }

        private bool TryScanEndOfFile(out Token token)
        {
            token = new Token(TokenClassification.EndOfFile, _position, _eof.LongLength);
            if (!Matches(_eof)) return false;
            Move(_eof.LongLength - 1L);
            Move(1L);
            return true;
        }

        private bool TryScanName(out Token token)
        {
            var startingPosition = _position;
            if (!Matches(Ascii.ForwardSlash))
            {
                token = Token.Undefined;
                return false;
            }
            if (!Move(1L)) throw new PdfFormatException();
            if (!NextAny(Ascii.WhitespaceAndDelimiters)) throw new PdfFormatException();
            token = new Token(TokenClassification.Name, startingPosition, _position - startingPosition);
            return true;
        }

        private bool TryScanNumeric(out Token token)
        {
            var startingPosition = _position;
            var real = false;
            var character = Current;

            if ((character == Ascii.MinusSign || character == Ascii.PlusSign) && !Move(1L)) throw new PdfFormatException();

            while (_position < _pdfStream.Length && (Ascii.Digit0 <= (character = Current) && character <= Ascii.Digit9 || character == Ascii.Period))
            {
                if (character == Ascii.Period) real = true;
                _position++;
            }

            if (_position == startingPosition)
            {
                token = Token.Undefined;
                return false;
            }

            token = new Token(real ? TokenClassification.Real : TokenClassification.Integer, startingPosition, _position - startingPosition);
            return true;
        }

        private bool TryScanHexadecimalString(out Token token)
        {
            if (!Matches(Ascii.LessThanSign))
            {
                token = Token.Undefined;
                return false;
            }

            var startingPosition = _position;
            if (!Move(1L)) throw new PdfFormatException();
            if (!SkipWhile(c => Ascii.Digit0 <= c && c <= Ascii.Digit9 || Ascii.A <= c && c <= Ascii.F || Ascii.LowercaseA <= c && c <= Ascii.LowercaseF)) throw new PdfFormatException();
            if (!Matches(Ascii.GreaterThanSign)) throw new PdfFormatException();
            if (!Move(1L)) throw new PdfFormatException();
            token = new Token(TokenClassification.HexadecimalString, startingPosition, _position - startingPosition);
            return true;
        }

        private bool TryScanLiteralString(out Token token)
        {
            if (!Matches(Ascii.LeftParenthesis))
            {
                token = Token.Undefined;
                return false;
            }

            var startingPosition = _position;
            if (!Move(1L)) throw new PdfFormatException();
            var lastCharacter = Ascii.LeftParenthesis;
            var character = Current;
            var depth = 0;

            while (_position < _pdfStream.Length && (character != Ascii.RightParenthesis || lastCharacter == Ascii.Backslash || depth > 0))
            {
                if (character == Ascii.LeftParenthesis && lastCharacter != Ascii.Backslash) depth++;
                else if (character == Ascii.RightParenthesis && lastCharacter != Ascii.Backslash) depth--;
             
                _position++;
                lastCharacter = character;
                character = Current;
            }

            if (!Move(1L)) throw new PdfFormatException();
            token = new Token(TokenClassification.LiteralString, startingPosition, _position - startingPosition);
            return true;
        }

        private bool TryScanComment(out Token token)
        {
            if (!Matches(Ascii.PercentSign))
            {
                token = Token.Undefined;
                return false;
            }

            var startingPosition = _position;
            if (!Move(1L)) throw new PdfFormatException();
            NextAny(Ascii.EndOfLine);
            token = new Token(TokenClassification.Comment, startingPosition, _position - startingPosition);
            return false;
        }

        private bool TryScanMagicNumber(out Token token)
        {
            var startingPosition = _position;
            token = new Token(TokenClassification.HeaderMagicNumber, startingPosition + 1, 4L);
            if (Current != Ascii.PercentSign) return false;
            if (!Move(5L)) throw new PdfFormatException();
            return true;
        }

        private bool TryScanStream(long length, out Token token)
        {
            if (length > 0)
            {
                if (!Move(length)) throw new PdfFormatException();
                SkipAll(Ascii.EndOfLine);
                token = new Token(TokenClassification.Stream, _position, length);
                return true;
            }

            // If length of stream is not known then scan buffer for the endstream token
            var startingPosition = _position;
            if (!NextMatch(_endStream)) throw new PdfFormatException();
            Move(-1L);
            if (Current == Ascii.LineFeed) Move(-1L);
            if (Current != Ascii.CarriageReturn) throw new PdfFormatException();
            token = new Token(TokenClassification.Stream, startingPosition, _position - startingPosition);
            SkipAll(Ascii.EndOfLine);
            return true;
        }

        private bool TryScanBeginStream(out Token token)
        {
            if (!TryScanToken(_stream, TokenClassification.BeginStream, out token)) return false;
            if (Current == Ascii.CarriageReturn && !(Move(1L))) throw new PdfFormatException();
            if (Current != Ascii.LineFeed || !Move(1L)) throw new PdfFormatException();
            return true;
        }

        private bool TryScanInteger(out Token token, TokenClassification tokenClassification = TokenClassification.Integer)
        {
            var startingPosition = _position;
            if (!SkipWhile(c => Ascii.Digit0 <= c && c <= Ascii.Digit9))
            {
                token = Token.Undefined;
                return false;
            }

            token = new Token(tokenClassification, startingPosition, _position - startingPosition);
            return true;
        }

        private bool TryScanObjectNumber(out Token token) => TryScanInteger(out token, TokenClassification.ObjectNumber);

        private bool TryScanGenerationNumber(out Token token) => TryScanInteger(out token, TokenClassification.GenerationNumber);

        private bool TryScanCrossReference(out Token token) => TryScanToken(_xref, TokenClassification.CrossReference, out token);

        private bool TryScanBeginObject(out Token token) => TryScanToken(_obj, TokenClassification.BeginObject, out token);

        private bool TryScanEndObject(out Token token) => TryScanToken(_endobj, TokenClassification.EndObject, out token);

        private bool TryScanMajorVersion(out Token token) => TryScanToken(c => Ascii.Digit0 <= c && c <= Ascii.Digit9, TokenClassification.HeaderMajorVersion, out token);

        private bool TryScanMinorVersion(out Token token) => TryScanToken(c => Ascii.Digit0 <= c && c <= Ascii.Digit9, TokenClassification.HeaderMinorVersion, out token);

        private bool TryScanTrailer(out Token token) => TryScanToken(_trailer, TokenClassification.Trailer, out token);

        private bool TryScanStartCrossReference(out Token token) => TryScanToken(_startXref, TokenClassification.StartCrossReference, out token);

        private bool TryScanObjectReference(out Token token) => TryScanToken(Ascii.R, TokenClassification.ObjectReference, out token);

        private bool TryScanNull(out Token token) => TryScanToken(_null, TokenClassification.Null, out token);

        private bool TryScanBooleanFalse(out Token token) => TryScanToken(_false, TokenClassification.BooleanFalse, out token);

        private bool TryScanBooleanTrue(out Token token) => TryScanToken(_true, TokenClassification.BooleanTrue, out token);

        private bool TryScanBeginDictionary(out Token token) => TryScanToken(_beginDictionary, TokenClassification.BeginDictionary, out token);

        private bool TryScanEndDictionary(out Token token) => TryScanToken(_endDictionary, TokenClassification.EndDictionary, out token);

        private bool TryScanEndStream(out Token token) => TryScanToken(_endStream, TokenClassification.EndStream, out token);

        private bool TryScanBeginArray(out Token token) => TryScanToken(Ascii.LeftSquareBracket, TokenClassification.BeginArray, out token);

        private bool TryScanEndArray(out Token token) => TryScanToken(Ascii.RightSquareBracket, TokenClassification.EndArray, out token);

        private bool TryScanToken(byte character, TokenClassification tokenClassification, out Token token)
        {
            token = new Token(tokenClassification, _position, 1L);
            if (Current != character) return false;
            if (!Move(1L)) throw new PdfFormatException();
            return true;
        }

        private bool TryScanToken(Func<byte, bool> condition, TokenClassification tokenClassification, out Token token)
        {
            token = new Token(tokenClassification, _position, 1L);
            if (!condition(Current)) return false;
            if (!Move(1L)) throw new PdfFormatException();
            return true;
        }

        private bool TryScanToken(byte[] data, TokenClassification tokenClassification, out Token token)
        {
            token = new Token(tokenClassification, _position, data.LongLength);
            if (!Matches(data)) return false;
            if (!Move(data.LongLength)) throw new PdfFormatException();
            return true;
        }

        private long ParseInteger(Token token)
        {
            var value = 0L;
            for (var index = 0; index < token.Length; index++)
            {
                value = value * 10 + _pdfStream.Buffer[token.Index + index] - Ascii.Digit0;
            }
            return value;
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
            if (_disposed) throw new ObjectDisposedException($"Operations cannot be performed on a disposed {nameof(RandomAccessPdfScanner)}.");
        }
    }
}