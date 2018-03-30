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
    public class FastAsciiEncodingTests
    {
        [TestCase("abc123", new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 })]
        [TestCase("Lorem ipsum", new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM })]
        public void CanEncodeByteArray(string input, byte[] expectedEncodedBytes)
        {
            var encoding = new FastAsciiEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase("abc123", new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, "abc123")]
        //[TestCase("Lorem ipsum", new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, "Lorem ipsum")]
        //public void CanEncodeByteArrayAndFormatString(string input, byte[] expectedEncodedBytes, string expectedFormattedString)
        //{
        //    var encoding = new FastAsciiEncoding();
        //    var (actualEncodedBytes, actualFormattedString) = encoding.GetBytesAndFormattedString(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 })]
        [TestCase(new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM })]
        public void CanEncodeByteArray(char[] input, byte[] expectedEncodedBytes)
        {
            var encoding = new FastAsciiEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, new[] { 'a', 'b', 'c', '1', '2', '3' })]
        //[TestCase(new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'i', 'p', 's', 'u', 'm' })]
        //public void CanEncodeByteArrayAndFormatCharArray(char[] input, byte[] expectedEncodedBytes, char[] expectedFormattedChars)
        //{
        //    var encoding = new FastAsciiEncoding();
        //    var (actualEncodedBytes, actualFormattedChars) = encoding.GetBytesAndFormattedChars(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedChars, actualFormattedChars);
        //}

        [TestCase(new byte[] { }, "")]
        [TestCase(new[] { Ascii.Space }, " ")]
        [TestCase(new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, "abc123")]
        [TestCase(new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, "Lorem ipsum")]
        [TestCase(new[] { Ascii.Null, Ascii.HorizontalTab, Ascii.LineFeed, Ascii.FormFeed, Ascii.CarriageReturn, Ascii.Space, Ascii.LeftParenthesis, Ascii.RightParenthesis, Ascii.LessThanSign, Ascii.GreaterThanSign, Ascii.LeftSquareBracket, Ascii.RightSquareBracket, Ascii.LeftCurlyBracket, Ascii.RightCurlyBracket, Ascii.ForwardSlash, Ascii.PercentSign }, "\0\t\n\f\r ()<>[]{}/%")]
        public void CanDecodeByteArray(byte[] input, string expectedDecodedString)
        {
            var encoding = new FastAsciiEncoding();
            var actualDecodedString = encoding.GetString(input);

            CollectionAssert.AreEqual(expectedDecodedString, actualDecodedString);
        }

        //[TestCase(new byte[] { }, "", "")]
        //[TestCase(new[] { Ascii.Space }, " ", " ")]
        //[TestCase(new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, "abc123", "abc123")]
        //[TestCase(new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, "Lorem ipsum", "Lorem ipsum")]
        //[TestCase(new[] { Ascii.Null, Ascii.HorizontalTab, Ascii.LineFeed, Ascii.FormFeed, Ascii.CarriageReturn, Ascii.Space, Ascii.LeftParenthesis, Ascii.RightParenthesis, Ascii.LessThanSign, Ascii.GreaterThanSign, Ascii.LeftSquareBracket, Ascii.RightSquareBracket, Ascii.LeftCurlyBracket, Ascii.RightCurlyBracket, Ascii.ForwardSlash, Ascii.PercentSign }, "\0\t\n\f\r ()<>[]{}/%", "\0\t\n\f\r ()<>[]{}/%")]
        //public void CanDecodeByteArrayAndFormatString(byte[] input, string expectedDecodedString, string expectedFormattedString)
        //{
        //    var encoding = new FastAsciiEncoding();
        //    var (actualDecodedString, actualFormattedString) = encoding.GetStringAndFormattedString(input);

        //    Assert.AreEqual(expectedDecodedString, actualDecodedString);
        //    Assert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [TestCase("𐐷")]
        [TestCase("\x0200")]
        [TestCase("!~`\x0100  ")]
        public void DetectsInvalidEncodingWhileEncodingByteArray(string input)
        {
            var encoding = new FastAsciiEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[ExpectedException(typeof(EncoderFallbackException))]
        //[TestCase("𐐷")]
        //[TestCase("\x0200")]
        //[TestCase("!~`\x0100  ")]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingString(string input)
        //{
        //    var encoding = new FastAsciiEncoding();
        //    var bytes = encoding.GetBytesAndFormattedString(input);
        //}

        [TestCase(new[] { '\uD801', '\uDC37' })]
        [TestCase(new[] { '\x0200' })]
        [TestCase(new[] { '!', '~', '`', '\x0100' })]
        public void DetectsInvalidEncodingWhileEncodingByteArray(char[] input)
        {
            var encoding = new FastAsciiEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[ExpectedException(typeof(EncoderFallbackException))]
        //[TestCase(new[] { '\uD801', '\uDC37' })]
        //[TestCase(new[] { '\x0200' })]
        //[TestCase(new[] { '!', '~', '`', '\x0100' })]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingCharArray(char[] input)
        //{
        //    var encoding = new FastAsciiEncoding();
        //    var bytes = encoding.GetBytesAndFormattedChars(input);
        //}
    }
}
