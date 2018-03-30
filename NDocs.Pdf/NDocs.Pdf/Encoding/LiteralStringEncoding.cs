using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Parsing;

namespace NDocs.Pdf.Encoding
{
    internal class LiteralStringEncoding : BaseEncoding
    {
        public override string EncodingName => "LiteralString";

        protected override unsafe int GetByteCountInternal(char* chars, int index, int count)
        {
            var byteCount = 2;
            var charRef = chars + index;
            var lastChar = chars + count;

            while (charRef < lastChar)
            {
                var character = *charRef++;
                if (character == '\n' || character == '\r' || character == '\t' || character == '\b' || character == '\f' || character == '(' || character == ')' || character == '\\') byteCount += 2;
                else if (character < ' ' || character > '~') byteCount += 4;
                else byteCount++;
            }

            return byteCount;
        }

        protected override int GetMaxByteCountInternal(int charCount)
        {
            return Math.Max(charCount, 0) * 4 + 2;
        }

        protected override unsafe void GetBytesInternal(char* chars, byte* bytes, int charCount, int byteCount)
        {
            var charRef = chars;
            var lastChar = chars + charCount;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var secondToLastByte = lastByte - 1;
            var thirdToLastByte = lastByte - 2;
            var fifthToLastByte = lastByte - 4;

            *byteRef++ = Ascii.LeftParenthesis;

            while (charRef < lastChar && byteRef < secondToLastByte)
            {
                var character = *charRef++;
                if (character > '\u00ff') throw new EncoderFallbackException();

                if (character == '\n')
                {
                    if (byteRef >= thirdToLastByte) break;
                    *byteRef++ = Ascii.Backslash;
                    *byteRef++ = Ascii.LowercaseN;
                }
                else if (character == '\r')
                {
                    if (byteRef >= thirdToLastByte) break;
                    *byteRef++ = Ascii.Backslash;
                    *byteRef++ = Ascii.LowercaseR;
                }
                else if (character == '\t')
                {
                    if (byteRef >= thirdToLastByte) break;
                    *byteRef++ = Ascii.Backslash;
                    *byteRef++ = Ascii.LowercaseT;
                }
                else if (character == '\b')
                {
                    if (byteRef >= thirdToLastByte) break;
                    *byteRef++ = Ascii.Backslash;
                    *byteRef++ = Ascii.LowercaseB;
                }
                else if (character == '\f')
                {
                    if (byteRef >= thirdToLastByte) break;
                    *byteRef++ = Ascii.Backslash;
                    *byteRef++ = Ascii.LowercaseF;
                }
                else if (character == '(' || character == ')' || character == '\\')
                {
                    if (byteRef >= thirdToLastByte) break;
                    *byteRef++ = Ascii.Backslash;
                    *byteRef++ = (byte)character;
                }
                else if (character < ' ' || character > '~')
                {
                    if (byteRef >= fifthToLastByte) break;
                    var octalDigit2 = (byte)(((character & 0b011000000) >> 6) + Ascii.Digit0);
                    var octalDigit1 = (byte)(((character & 0b111000) >> 3) + Ascii.Digit0);
                    var octalDigit0 = (byte)((character & 0b111) + Ascii.Digit0);
                    *byteRef++ = Ascii.Backslash;
                    *byteRef++ = octalDigit2;
                    *byteRef++ = octalDigit1;
                    *byteRef++ = octalDigit0;
                }
                else
                {
                    *byteRef++ = (byte)character;
                }
            }
            
            *byteRef = Ascii.RightParenthesis;
        }

        protected override unsafe int GetCharCountInternal(byte* bytes, int offset, int count)
        {
            var charCount = 0;
            var backslashFound = false;
            var octalDigitCount = 0;
            var endOfLineFound = false;
            var byteRef = bytes + offset;
            var lastByte = byteRef + count;

            while (byteRef < lastByte)
            {
                var @byte = *byteRef++;

                if (endOfLineFound)
                {
                    endOfLineFound = false;
                    charCount++;

                    if (@byte == Ascii.CarriageReturn || @byte == Ascii.LineFeed) continue;
                }
                else if (octalDigitCount > 0)
                {
                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit7)
                    {
                        octalDigitCount = (octalDigitCount + 1) % 3;
                        if (octalDigitCount == 0) charCount++;
                        continue;
                    }

                    charCount++;
                    octalDigitCount = 0;
                }

                if (@byte == Ascii.Backslash && !backslashFound)
                {
                    backslashFound = true;
                    continue;
                }

                if (backslashFound)
                {
                    backslashFound = false;
                    if (@byte == Ascii.CarriageReturn || @byte == Ascii.LineFeed) continue;

                    if (@byte == Ascii.LowercaseN || @byte == Ascii.LowercaseR || @byte == Ascii.LowercaseT || @byte == Ascii.LowercaseB || @byte == Ascii.LowercaseF || @byte == Ascii.LeftParenthesis || @byte == Ascii.RightParenthesis || @byte == Ascii.Backslash)
                    {
                        charCount++;
                        continue;
                    }

                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit7)
                    {
                        octalDigitCount = 1;
                        continue;
                    }
                }

                if (@byte == Ascii.CarriageReturn || @byte == Ascii.LineFeed)
                {
                    endOfLineFound = true;
                    continue;
                }

                charCount++;
            }

            if (octalDigitCount > 0) charCount++;

            return Math.Max(charCount - 2, 0);
        }

        protected override int GetMaxCharCountInternal(int byteCount)
        {
            return Math.Max(byteCount - 2, 0);
        }

        protected override unsafe void GetCharsInternal(byte* bytes, char* chars, int byteCount, int charCount)
        {
            var backslashFound = false;
            var octalDigitCount = 0;
            var octalEncodedByte = Ascii.Null;
            var endOfLineFound = false;
            var leftParenthesisCount = 0;
            var rightParenthesisCount = 0;
            var byteRef = bytes;
            var lastByte = bytes + byteCount;
            var charRef = chars;
            var lastChar = chars + charCount;

            if (*byteRef != Ascii.LeftParenthesis) throw new EncoderFallbackException();

            // TODO: This might overwrite the char pointer boundary since more than one character may be written with each pass. Some further checks are needed.
            while (byteRef < lastByte && charRef < lastChar)
            {
                var @byte = *byteRef++;

                if (endOfLineFound)
                {
                    endOfLineFound = false;
                    *charRef++ = (char)Ascii.LineFeed;

                    if (@byte == Ascii.CarriageReturn || @byte == Ascii.LineFeed) continue;
                }
                else if (octalDigitCount > 0)
                {
                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit7)
                    {
                        octalEncodedByte <<= 3;
                        octalEncodedByte |= (byte)(@byte - Ascii.Digit0);
                        octalDigitCount = (octalDigitCount + 1) % 3;
                        if (octalDigitCount == 0)
                        {
                            *charRef++ = (char)octalEncodedByte;
                            octalEncodedByte = Ascii.Null;
                        }
                        continue;
                    }

                    *charRef++ = (char)octalEncodedByte;
                    octalEncodedByte = Ascii.Null;
                    octalDigitCount = 0;
                }

                if (@byte == Ascii.Backslash && !backslashFound)
                {
                    backslashFound = true;
                    continue;
                }

                if (backslashFound)
                {
                    backslashFound = false;

                    if (@byte == Ascii.CarriageReturn || @byte == Ascii.LineFeed) continue;

                    switch (@byte)
                    {
                        case Ascii.LowercaseN:
                            *charRef++ = (char)Ascii.LineFeed;
                            continue;
                        case Ascii.LowercaseR:
                            *charRef++ = (char)Ascii.CarriageReturn;
                            continue;
                        case Ascii.LowercaseT:
                            *charRef++ = (char)Ascii.HorizontalTab;
                            continue;
                        case Ascii.LowercaseB:
                            *charRef++ = (char)Ascii.Backspace;
                            continue;
                        case Ascii.LowercaseF:
                            *charRef++ = (char)Ascii.FormFeed;
                            continue;
                        case Ascii.LeftParenthesis:
                            *charRef++ = (char)Ascii.LeftParenthesis;
                            continue;
                        case Ascii.RightParenthesis:
                            *charRef++ = (char)Ascii.RightParenthesis;
                            continue;
                        case Ascii.Backslash:
                            *charRef++ = (char)Ascii.Backslash;
                            continue;
                    }

                    if (@byte >= Ascii.Digit0 && @byte <= Ascii.Digit7)
                    {
                        octalDigitCount = 1;
                        octalEncodedByte = (byte)(@byte - Ascii.Digit0);
                        continue;
                    }
                }

                if (@byte == Ascii.CarriageReturn || @byte == Ascii.LineFeed)
                {
                    endOfLineFound = true;
                    continue;
                }

                *charRef++ = (char)@byte;

                if (@byte == Ascii.LeftParenthesis) leftParenthesisCount++;
                else if (@byte == Ascii.RightParenthesis) rightParenthesisCount++;
            }
            
            if (leftParenthesisCount != rightParenthesisCount) throw new EncoderFallbackException();
            if (*byteRef != Ascii.RightParenthesis) throw new EncoderFallbackException();
            if (octalDigitCount > 0) *charRef = (char)octalEncodedByte;
        }
    }
}