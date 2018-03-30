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
    public class HexadecimalStringEncodingTests
    {
        [TestCase("abc123", new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign })]
        [TestCase("Lorem ipsum", new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign })]
        public void CanEncodeByteArray(string input, byte[] expectedEncodedBytes)
        {
            var encoding = new HexadecimalStringEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase("abc123", new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign }, "<616263313233>")]
        //[TestCase("Lorem ipsum", new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign }, "<4C6F72656D20697073756D>")]
        //public void CanEncodeByteArrayAndFormatString(string input, byte[] expectedEncodedBytes, string expectedFormattedString)
        //{
        //    var encoding = new HexadecimalStringEncoding();
        //    var (actualEncodedBytes, actualFormattedString) = encoding.GetBytesAndFormattedString(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        //[TestCase("abc123", new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign }, "<616263313233>")]
        //[TestCase("Lorem ipsum", new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.LowercaseC, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.GreaterThanSign }, "<4c6f72656d20697073756d>")]
        //public void CanEncodeByteArrayAndFormatStringIntoLowercaseHexadecimal(string input, byte[] expectedEncodedBytes, string expectedFormattedString)
        //{
        //    var encoding = new HexadecimalStringEncoding(false);
        //    var (actualEncodedBytes, actualFormattedString) = encoding.GetBytesAndFormattedString(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign })]
        [TestCase(new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign })]
        public void CanEncodeByteArray(char[] input, byte[] expectedEncodedBytes)
        {
            var encoding = new HexadecimalStringEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign }, new[] { '<', '6', '1', '6', '2', '6', '3', '3', '1', '3', '2', '3', '3', '>' })]
        //[TestCase(new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign }, new[] { '<', '4', 'C', '6', 'F', '7', '2', '6', '5', '6', 'D', '2', '0', '6', '9', '7', '0', '7', '3', '7', '5', '6', 'D', '>' })]
        //public void CanEncodeByteArrayAndFormatCharArray(char[] input, byte[] expectedEncodedBytes, char[] expectedFormattedChars)
        //{
        //    var encoding = new HexadecimalStringEncoding();
        //    var (actualEncodedBytes, actualFormattedChars) = encoding.GetBytesAndFormattedChars(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedChars, actualFormattedChars);
        //}

        //[TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign }, new[] { '<', '6', '1', '6', '2', '6', '3', '3', '1', '3', '2', '3', '3', '>' })]
        //[TestCase(new[] { 'L', 'o', 'r', 'e', 'm', ' ', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.LowercaseC, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.GreaterThanSign }, new[] { '<', '4', 'c', '6', 'f', '7', '2', '6', '5', '6', 'd', '2', '0', '6', '9', '7', '0', '7', '3', '7', '5', '6', 'd', '>' })]
        //public void CanEncodeByteArrayAndFormatCharArrayIntoLowercaseHexadecimal(char[] input, byte[] expectedEncodedBytes, char[] expectedFormattedChars)
        //{
        //    var encoding = new HexadecimalStringEncoding(false);
        //    var (actualEncodedBytes, actualFormattedChars) = encoding.GetBytesAndFormattedChars(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedChars, actualFormattedChars);
        //}

        [TestCase(new [] {Ascii.LessThanSign, Ascii.GreaterThanSign}, "")]
        [TestCase(new[] { Ascii.Space, Ascii.LessThanSign, Ascii.Space, Ascii.GreaterThanSign, Ascii.Space }, "")]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign }, "abc123")]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.GreaterThanSign }, "@@@")]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Space, Ascii.GreaterThanSign }, "@@@")]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.GreaterThanSign }, "@@@")]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.LowercaseC, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.GreaterThanSign }, "Lorem ipsum")]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign }, "Lorem ipsum")]
        [TestCase(new[] { Ascii.Space, Ascii.LessThanSign, Ascii.HorizontalTab, Ascii.Digit4, Ascii.LowercaseC, Ascii.CarriageReturn, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.LineFeed, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.Space, Ascii.Space, Ascii.Null, Ascii.LowercaseD, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.FormFeed, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.GreaterThanSign }, "Lorem ipsum")]
        public void CanDecodeByteArray(byte[] input, string expectedDecodedString)
        {
            var encoding = new HexadecimalStringEncoding();
            var actualDecodedString = encoding.GetString(input);

            CollectionAssert.AreEqual(expectedDecodedString, actualDecodedString);
        }

        //[TestCase(new[] { Ascii.LessThanSign, Ascii.GreaterThanSign }, "", "<>")]
        //[TestCase(new[] { Ascii.Space, Ascii.LessThanSign, Ascii.Space, Ascii.GreaterThanSign, Ascii.Space }, "", "<>")]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign }, "abc123", "<616263313233>")]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.GreaterThanSign }, "@@", "<404>")]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.GreaterThanSign }, "@@@", "<40404>")]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Space, Ascii.GreaterThanSign }, "@@@", "<40404>")]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.GreaterThanSign }, "@@@", "<404040>")]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.LowercaseC, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign }, "Lorem ipsum", "<4c6f72656D20697073756D>")]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign }, "Lorem ipsum", "<4C6F72656D20697073756D>")]
        //[TestCase(new[] { Ascii.Space, Ascii.LessThanSign, Ascii.HorizontalTab, Ascii.Digit4, Ascii.LowercaseC, Ascii.CarriageReturn, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.LineFeed, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.Space, Ascii.Space, Ascii.Null, Ascii.LowercaseD, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.FormFeed, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.GreaterThanSign }, "Lorem ipsum", "<4c6f72656d20697073756d>")]
        //public void CanDecodeByteArrayAndFormatString(byte[] input, string expectedDecodedString, string expectedFormattedString)
        //{
        //    var encoding = new HexadecimalStringEncoding();
        //    var (actualDecodedString, actualFormattedString) = encoding.GetStringAndFormattedString(input);

        //    Assert.AreEqual(expectedDecodedString, actualDecodedString);
        //    Assert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [TestCase("!~`\x0100  ")]
        public void DetectsInvalidEncodingWhileEncodingByteArray(string input)
        {
            // TODO: Invalid test. This is really teasting FastAsciiEncoding - not HexadecimalEncoding
            var encoding = new HexadecimalStringEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[ExpectedException(typeof(EncoderFallbackException))]
        //[TestCase("!~`\x0100  ")]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingString(string input)
        //{
        //    // TODO: Invalid test. This is really teasting FastAsciiEncoding - not HexadecimalEncoding
        //    var encoding = new HexadecimalStringEncoding();
        //    var bytes = encoding.GetBytesAndFormattedString(input);
        //}

        [TestCase(new[] { '!', '~', '`', '\x0100' })]
        public void DetectsInvalidEncodingWhileEncodingByteArray(char[] input)
        {
            // TODO: Invalid test. This is really teasting FastAsciiEncoding - not HexadecimalEncoding
            var encoding = new HexadecimalStringEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[ExpectedException(typeof(EncoderFallbackException))]
        //[TestCase(new[] { '!', '~', '`', '\x0100' })]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingCharArray(char[] input)
        //{
        //    // TODO: Invalid test. This is really teasting FastAsciiEncoding - not HexadecimalEncoding
        //    var encoding = new HexadecimalStringEncoding();
        //    var bytes = encoding.GetBytesAndFormattedChars(input);
        //}

        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit1, Ascii.Colon, Ascii.GreaterThanSign })]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit1, Ascii.Null, Ascii.Colon, Ascii.GreaterThanSign })]
        public void DetectsInvalidEncodingWhileDecodingByteArray(byte[] input)
        {
            var encoding = new HexadecimalStringEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetString(input));
        }

        //[ExpectedException(typeof(EncoderFallbackException))]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit1, Ascii.Colon, Ascii.GreaterThanSign })]
        //[TestCase(new[] { Ascii.LessThanSign, Ascii.Digit1, Ascii.Null, Ascii.Colon, Ascii.GreaterThanSign })]
        //public void DetectsInvalidEncodingWhileDecodingByteArrayAndFormattingString(byte[] input)
        //{
        //    var encoding = new HexadecimalStringEncoding();
        //    var decodedString = encoding.GetStringAndFormattedString(input);
        //}
    }
}