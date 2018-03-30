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
    public class NameEncodingTests
    {
        [TestCase("", new[] { Ascii.ForwardSlash })]
        [TestCase("Name1", new[] { Ascii.ForwardSlash, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.Digit1 })]
        [TestCase("ASomewhatLongerName", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.S, Ascii.LowercaseO, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.LowercaseW, Ascii.LowercaseH, Ascii.LowercaseA, Ascii.LowercaseT, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseN, Ascii.LowercaseG, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE })]
        [TestCase("A;Name_With-Various***Characters?", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.Semicolon, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.Underscore, Ascii.W, Ascii.LowercaseI, Ascii.LowercaseT, Ascii.LowercaseH, Ascii.MinusSign, Ascii.V, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseI, Ascii.LowercaseO, Ascii.LowercaseU, Ascii.LowercaseS, Ascii.Asterisk, Ascii.Asterisk, Ascii.Asterisk, Ascii.C, Ascii.LowercaseH, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseA, Ascii.LowercaseC, Ascii.LowercaseT, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.LowercaseS, Ascii.QuestionMark })]
        [TestCase("1.2", new[] { Ascii.ForwardSlash, Ascii.Digit1, Ascii.Period, Ascii.Digit2 })]
        [TestCase("$$", new[] { Ascii.ForwardSlash, Ascii.DollarSign, Ascii.DollarSign })]
        [TestCase("@pattern", new[] { Ascii.ForwardSlash, Ascii.AtSign, Ascii.LowercaseP, Ascii.LowercaseA, Ascii.LowercaseT, Ascii.LowercaseT, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.LowercaseN })]
        [TestCase(".notdef", new[] { Ascii.ForwardSlash, Ascii.Period, Ascii.LowercaseN, Ascii.LowercaseO, Ascii.LowercaseT, Ascii.LowercaseD, Ascii.LowercaseE, Ascii.LowercaseF })]
        public void CanEncodeByteArray(string input, byte[] expectedEncodedBytes)
        {
            var encoding = new NameEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase("", new[] { Ascii.ForwardSlash }, "/")]
        //[TestCase("Name1", new[] { Ascii.ForwardSlash, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.Digit1 }, "/Name1")]
        //[TestCase("ASomewhatLongerName", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.S, Ascii.LowercaseO, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.LowercaseW, Ascii.LowercaseH, Ascii.LowercaseA, Ascii.LowercaseT, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseN, Ascii.LowercaseG, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE }, "/ASomewhatLongerName")]
        //[TestCase("A;Name_With-Various***Characters?", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.Semicolon, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.Underscore, Ascii.W, Ascii.LowercaseI, Ascii.LowercaseT, Ascii.LowercaseH, Ascii.MinusSign, Ascii.V, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseI, Ascii.LowercaseO, Ascii.LowercaseU, Ascii.LowercaseS, Ascii.Asterisk, Ascii.Asterisk, Ascii.Asterisk, Ascii.C, Ascii.LowercaseH, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseA, Ascii.LowercaseC, Ascii.LowercaseT, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.LowercaseS, Ascii.QuestionMark }, "/A;Name_With-Various***Characters?")]
        //[TestCase("1.2", new[] { Ascii.ForwardSlash, Ascii.Digit1, Ascii.Period, Ascii.Digit2 }, "/1.2")]
        //[TestCase("$$", new[] { Ascii.ForwardSlash, Ascii.DollarSign, Ascii.DollarSign }, "/$$")]
        //[TestCase("@pattern", new[] { Ascii.ForwardSlash, Ascii.AtSign, Ascii.LowercaseP, Ascii.LowercaseA, Ascii.LowercaseT, Ascii.LowercaseT, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.LowercaseN }, "/@pattern")]
        //[TestCase(".notdef", new[] { Ascii.ForwardSlash, Ascii.Period, Ascii.LowercaseN, Ascii.LowercaseO, Ascii.LowercaseT, Ascii.LowercaseD, Ascii.LowercaseE, Ascii.LowercaseF }, "/.notdef")]
        //public void CanEncodeByteArrayAndFormatString(string input, byte[] expectedEncodedBytes, string expectedFormattedString)
        //{
        //    var encoding = new NameEncoding();
        //    var (actualEncodedBytes, actualFormattedString) = encoding.GetBytesAndFormattedString(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 })]
        [TestCase(new[] { 'L', 'o', 'r', 'e', 'm', '.', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM })]
        public void CanEncodeByteArray(char[] input, byte[] expectedEncodedBytes)
        {
            var encoding = new NameEncoding();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, new[] { '/', 'a', 'b', 'c', '1', '2', '3' })]
        //[TestCase(new[] { 'L', 'o', 'r', 'e', 'm', '.', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, new[] { '/', 'L', 'o', 'r', 'e', 'm', '.', 'i', 'p', 's', 'u', 'm' })]
        //public void CanEncodeByteArrayAndFormatCharArray(char[] input, byte[] expectedEncodedBytes, char[] expectedFormattedChars)
        //{
        //    var encoding = new NameEncoding();
        //    var (actualEncodedBytes, actualFormattedChars) = encoding.GetBytesAndFormattedChars(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedChars, actualFormattedChars);
        //}

        [TestCase(new[] { Ascii.ForwardSlash }, "")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, "abc123")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, "Lorem.ipsum")]
        public void CanDecodeByteArray(byte[] input, string expectedDecodedString)
        {
            var encoding = new NameEncoding();
            var actualDecodedString = encoding.GetString(input);

            CollectionAssert.AreEqual(expectedDecodedString, actualDecodedString);
        }

        //[TestCase(new[] { Ascii.ForwardSlash }, "", "/")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, "abc123", "/abc123")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, "Lorem.ipsum", "/Lorem.ipsum")]
        //public void CanDecodeByteArrayAndFormatString(byte[] input, string expectedDecodedString, string expectedFormattedString)
        //{
        //    var encoding = new NameEncoding();
        //    var (actualDecodedString, actualFormattedString) = encoding.GetStringAndFormattedString(input);

        //    Assert.AreEqual(expectedDecodedString, actualDecodedString);
        //    Assert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileEncodingByteArray(string input)
        {
            var encoding = new NameEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingString(string input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new NameEncoding();
        //    var bytes = encoding.GetBytesAndFormattedString(input);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileEncodingByteArray(char[] input)
        {
            var encoding = new NameEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingCharArray(char[] input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new NameEncoding();
        //    var bytes = encoding.GetBytesAndFormattedChars(input);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileDecodingByteArray(byte[] input)
        {
            var encoding = new NameEncoding();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetString(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileDecodingByteArrayAndFormattingString(byte[] input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new NameEncoding();
        //    var decodedString = encoding.GetStringAndFormattedString(input);
        //}
    }
}