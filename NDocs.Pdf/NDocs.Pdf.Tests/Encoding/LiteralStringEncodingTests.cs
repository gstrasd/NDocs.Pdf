using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Encoding;
using NDocs.Pdf.Parsing;
using NUnit.Framework;

namespace NDocs.Pdf.Tests.Encoding
{
    [TestFixture]
    public class LiteralStringEncodingTests
    {
        [TestCase("", new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis })]
        [TestCase(" ", new[] { Ascii.LeftParenthesis, Ascii.Space, Ascii.RightParenthesis })]
        [TestCase("Lorem Ipsum", new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.I, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM, Ascii.RightParenthesis })]
        [TestCase("()", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.RightParenthesis })]
        [TestCase("\n\r\t\b\f()\\", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.Backslash, Ascii.LowercaseR, Ascii.Backslash, Ascii.LowercaseT, Ascii.Backslash, Ascii.LowercaseB, Ascii.Backslash, Ascii.LowercaseF, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.Backslash, Ascii.Backslash, Ascii.RightParenthesis })]
        [TestCase("\x0000", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.RightParenthesis })]
        [TestCase("A\x0000B", new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.B, Ascii.RightParenthesis })]
        [TestCase("©", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.RightParenthesis })]
        [TestCase("A©B", new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.B, Ascii.RightParenthesis })]
        public void CanEncodeByteArray(string input, byte[] expectedEncodedBytes)
        {
            var encoding = new LiteralStringEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }
        
        //[TestCase("", new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis }, "()")]
        //[TestCase(" ", new[] { Ascii.LeftParenthesis, Ascii.Space, Ascii.RightParenthesis }, "( )")]
        //[TestCase("Lorem Ipsum", new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.I, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM, Ascii.RightParenthesis }, "(Lorem Ipsum)")]
        //[TestCase("()", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.RightParenthesis }, @"(\(\))")]
        //[TestCase("\n\r\t\b\f()\\", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.Backslash, Ascii.LowercaseR, Ascii.Backslash, Ascii.LowercaseT, Ascii.Backslash, Ascii.LowercaseB, Ascii.Backslash, Ascii.LowercaseF, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.Backslash, Ascii.Backslash, Ascii.RightParenthesis }, @"(\n\r\t\b\f\(\)\\)")]
        //[TestCase("\x0000", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.RightParenthesis }, @"(\000)")]
        //[TestCase("A\x0000B", new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.B, Ascii.RightParenthesis }, @"(A\000B)")]
        //[TestCase("©", new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.RightParenthesis }, @"(\251)")]
        //[TestCase("A©B", new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.B, Ascii.RightParenthesis }, @"(A\251B)")]
        //public void CanEncodeByteArrayAndFormatString(string input, byte[] expectedEncodedBytes, string expectedFormattedString)
        //{
        //    var encoding = new LiteralStringEncoding();
        //    var (actualEncodedBytes, actualFormattedString) = encoding.GetBytesAndFormattedString(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [TestCase(new char[] { }, new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis })]
        [TestCase(new[] { ' ' }, new[] { Ascii.LeftParenthesis, Ascii.Space, Ascii.RightParenthesis })]
        [TestCase(new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'I', 'p', 's', 'u', 'm' }, new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.I, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM, Ascii.RightParenthesis })]
        [TestCase(new[] { '(', ')' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.RightParenthesis })]
        [TestCase(new[] { '\n', '\r', '\t', '\b', '\f', '(', ')', '\\' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.Backslash, Ascii.LowercaseR, Ascii.Backslash, Ascii.LowercaseT, Ascii.Backslash, Ascii.LowercaseB, Ascii.Backslash, Ascii.LowercaseF, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.Backslash, Ascii.Backslash, Ascii.RightParenthesis })]
        [TestCase(new[] { '\x0000' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.RightParenthesis })]
        [TestCase(new[] { 'A', '\x0000', 'B' }, new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.B, Ascii.RightParenthesis })]
        [TestCase(new[] { '©' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.RightParenthesis })]
        [TestCase(new[] { 'A', '\x00A9', 'B' }, new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.B, Ascii.RightParenthesis })]
        public void CanEncodeByteArray(char[] input, byte[] expectedEncodedBytes)
        {
            var encoding = new LiteralStringEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase(new char[] { }, new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis }, new[] { '(', ')' })]
        //[TestCase(new[] { ' ' }, new[] { Ascii.LeftParenthesis, Ascii.Space, Ascii.RightParenthesis }, new[] { '(', ' ', ')' })]
        //[TestCase(new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'I', 'p', 's', 'u', 'm' }, new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.I, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM, Ascii.RightParenthesis }, new[] { '(', 'L', 'o', 'r', 'e', 'm', ' ', 'I', 'p', 's', 'u', 'm', ')' })]
        //[TestCase(new[] { '(', ')' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.RightParenthesis }, new[] { '(', '\\', '(', '\\', ')', ')' })]
        //[TestCase(new[] { '\n', '\r', '\t', '\b', '\f', '(', ')', '\\' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.Backslash, Ascii.LowercaseR, Ascii.Backslash, Ascii.LowercaseT, Ascii.Backslash, Ascii.LowercaseB, Ascii.Backslash, Ascii.LowercaseF, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.Backslash, Ascii.Backslash, Ascii.RightParenthesis }, new[] { '(', '\\', 'n', '\\', 'r', '\\', 't', '\\', 'b', '\\', 'f', '\\', '(', '\\', ')', '\\', '\\', ')' })]
        //[TestCase(new[] { '\x0000' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.RightParenthesis }, new[] { '(', '\\', '0', '0', '0', ')' })]
        //[TestCase(new[] { 'A', '\x0000', 'B' }, new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit0, Ascii.Digit0, Ascii.Digit0, Ascii.B, Ascii.RightParenthesis }, new[] { '(', 'A', '\\', '0', '0', '0', 'B', ')' })]
        //[TestCase(new[] { '©' }, new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.RightParenthesis }, new[] { '(', '\\', '2', '5', '1', ')' })]
        //[TestCase(new[] { 'A', '\x00A9', 'B' }, new[] { Ascii.LeftParenthesis, Ascii.A, Ascii.Backslash, Ascii.Digit2, Ascii.Digit5, Ascii.Digit1, Ascii.B, Ascii.RightParenthesis }, new[] { '(', 'A', '\\', '2', '5', '1', 'B', ')' })]
        //public void CanEncodeByteArrayAndFormatCharArray(char[] input, byte[] expectedEncodedBytes, char[] expectedFormattedChars)
        //{
        //    var encoding = new LiteralStringEncoding();
        //    var (actualEncodedBytes, actualFormattedChars) = encoding.GetBytesAndFormattedChars(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedChars, actualFormattedChars);
        //}

        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis }, "")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.LeftParenthesis, Ascii.RightParenthesis, Ascii.RightParenthesis }, "()")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.RightParenthesis }, "(")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.RightParenthesis, Ascii.RightParenthesis }, "())")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.RightParenthesis }, "\x0004")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.X, Ascii.RightParenthesis }, "\x0004X")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.Digit0, Ascii.RightParenthesis }, " ")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.RightParenthesis }, " ")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.Digit0, Ascii.B, Ascii.RightParenthesis }, " B")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.CarriageReturn, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.LineFeed, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.CarriageReturn, Ascii.LineFeed, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.LineFeed, Ascii.CarriageReturn, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.Backslash, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.CarriageReturn, Ascii.Backslash, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit1, Ascii.Digit0, Ascii.Digit1, Ascii.Backslash, Ascii.Digit1, Ascii.Digit0, Ascii.Digit2, Ascii.Backslash, Ascii.Digit1, Ascii.Digit0, Ascii.Digit3, Ascii.RightParenthesis }, "ABC")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3, Ascii.RightParenthesis }, "abc123")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM, Ascii.RightParenthesis }, "Lorem.ipsum")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.RightParenthesis }, "\n")]
        [TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.Backslash, Ascii.LowercaseR, Ascii.Backslash, Ascii.LowercaseT, Ascii.Backslash, Ascii.LowercaseB, Ascii.Backslash, Ascii.LowercaseF, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.Backslash, Ascii.Backslash, Ascii.RightParenthesis }, "\n\r\t\b\f()\\")]
        public void CanDecodeByteArray(byte[] input, string expectedDecodedString)
        {
            var encoding = new LiteralStringEncoding();
            var actualDecodedString = encoding.GetString(input);

            CollectionAssert.AreEqual(expectedDecodedString, actualDecodedString);
        }

        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis }, "", "()")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.LeftParenthesis, Ascii.RightParenthesis, Ascii.RightParenthesis }, "()", "(())")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.RightParenthesis }, "(", @"(\()")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.RightParenthesis, Ascii.RightParenthesis }, "())", @"((\)))")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.RightParenthesis }, "\x0004", @"(\4)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.X, Ascii.RightParenthesis }, "\x0004X", @"(\4X)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.Digit0, Ascii.RightParenthesis }, " ", @"(\40)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.RightParenthesis }, " ", @"(\040)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit4, Ascii.Digit0, Ascii.B, Ascii.RightParenthesis }, " B", @"(\40B)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.CarriageReturn, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2", @"(LINE1\nLINE2)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.LineFeed, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2", @"(LINE1\nLINE2)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.CarriageReturn, Ascii.LineFeed, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2", @"(LINE1\nLINE2)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.LineFeed, Ascii.CarriageReturn, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2", @"(LINE1\nLINE2)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.I, Ascii.Backslash, Ascii.N, Ascii.E, Ascii.Digit1, Ascii.CarriageReturn, Ascii.Backslash, Ascii.L, Ascii.I, Ascii.N, Ascii.E, Ascii.Digit2, Ascii.RightParenthesis }, "LINE1\nLINE2", @"(LINE1\nLINE2)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.Digit1, Ascii.Digit0, Ascii.Digit1, Ascii.Backslash, Ascii.Digit1, Ascii.Digit0, Ascii.Digit2, Ascii.Backslash, Ascii.Digit1, Ascii.Digit0, Ascii.Digit3, Ascii.RightParenthesis }, "ABC", @"(\101\102\103)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3, Ascii.RightParenthesis }, "abc123", "(abc123)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM, Ascii.RightParenthesis }, "Lorem.ipsum", "(Lorem.ipsum)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.RightParenthesis }, "\n", @"(\n)")]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.Backslash, Ascii.LowercaseN, Ascii.Backslash, Ascii.LowercaseR, Ascii.Backslash, Ascii.LowercaseT, Ascii.Backslash, Ascii.LowercaseB, Ascii.Backslash, Ascii.LowercaseF, Ascii.Backslash, Ascii.LeftParenthesis, Ascii.Backslash, Ascii.RightParenthesis, Ascii.Backslash, Ascii.Backslash, Ascii.RightParenthesis }, "\n\r\t\b\f()\\", @"(\n\r\t\b\f\(\)\\)")]
        //public void CanDecodeByteArrayAndFormatString(byte[] input, string expectedDecodedString, string expectedFormattedString)
        //{
        //    var encoding = new LiteralStringEncoding();
        //    var (actualDecodedString, actualFormattedString) = encoding.GetStringAndFormattedString(input);

        //    Assert.AreEqual(expectedDecodedString, actualDecodedString);
        //    Assert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileEncodingByteArray(string input)
        {
            var encoding = new LiteralStringEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingString(string input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new LiteralStringEncoding();
        //    var bytes = encoding.GetBytesAndFormattedString(input);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileEncodingByteArray(char[] input)
        {
            var encoding = new LiteralStringEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingCharArray(char[] input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new LiteralStringEncoding();
        //    var bytes = encoding.GetBytesAndFormattedChars(input);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileDecodingByteArray(byte[] input)
        {
            var encoding = new LiteralStringEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetString(input));
        }

        //[ExpectedException(typeof(EncoderFallbackException))]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.LeftParenthesis, Ascii.RightParenthesis })]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis, Ascii.RightParenthesis })]
        //[TestCase(new[] { Ascii.LeftParenthesis, Ascii.RightParenthesis, Ascii.RightParenthesis, Ascii.RightParenthesis })]
        //public void DetectsInvalidEncodingWhileDecodingByteArrayAndFormattingString(byte[] input)
        //{
        //    var encoding = new LiteralStringEncoding();
        //    var decodedString = encoding.GetStringAndFormattedString(input);
        //}
    }
}