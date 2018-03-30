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
    public class NameEncoding12Tests
    {
        [TestCase("", new[] { Ascii.ForwardSlash })]
        [TestCase("Name1", new[] { Ascii.ForwardSlash, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.Digit1 })]
        [TestCase("ASomewhatLongerName", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.S, Ascii.LowercaseO, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.LowercaseW, Ascii.LowercaseH, Ascii.LowercaseA, Ascii.LowercaseT, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseN, Ascii.LowercaseG, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE })]
        [TestCase("A;Name_With-Various***Characters?", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.Semicolon, Ascii.N, Ascii.LowercaseA, Ascii.LowercaseM, Ascii.LowercaseE, Ascii.Underscore, Ascii.W, Ascii.LowercaseI, Ascii.LowercaseT, Ascii.LowercaseH, Ascii.MinusSign, Ascii.V, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseI, Ascii.LowercaseO, Ascii.LowercaseU, Ascii.LowercaseS, Ascii.Asterisk, Ascii.Asterisk, Ascii.Asterisk, Ascii.C, Ascii.LowercaseH, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseA, Ascii.LowercaseC, Ascii.LowercaseT, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.LowercaseS, Ascii.QuestionMark })]
        [TestCase("1.2", new[] { Ascii.ForwardSlash, Ascii.Digit1, Ascii.Period, Ascii.Digit2 })]
        [TestCase("$$", new[] { Ascii.ForwardSlash, Ascii.DollarSign, Ascii.DollarSign })]
        [TestCase("@pattern", new[] { Ascii.ForwardSlash, Ascii.AtSign, Ascii.LowercaseP, Ascii.LowercaseA, Ascii.LowercaseT, Ascii.LowercaseT, Ascii.LowercaseE, Ascii.LowercaseR, Ascii.LowercaseN })]
        [TestCase(".notdef", new[] { Ascii.ForwardSlash, Ascii.Period, Ascii.LowercaseN, Ascii.LowercaseO, Ascii.LowercaseT, Ascii.LowercaseD, Ascii.LowercaseE, Ascii.LowercaseF })]
        [TestCase("#", new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3 })]
        [TestCase("A# # # B", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.B })]
        [TestCase("©", new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.A, Ascii.Digit9 })]
        [TestCase("Adobe Green", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.LowercaseD, Ascii.LowercaseO, Ascii.LowercaseB, Ascii.LowercaseE, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.G, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseE, Ascii.LowercaseN })]
        [TestCase("PANTONE 5757 CV", new[] { Ascii.ForwardSlash, Ascii.P, Ascii.A, Ascii.N, Ascii.T, Ascii.O, Ascii.N, Ascii.E, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.Digit5, Ascii.Digit7, Ascii.Digit5, Ascii.Digit7, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.C, Ascii.V })]
        [TestCase("The_Key_of_F#_Minor", new[] { Ascii.ForwardSlash , Ascii.T, Ascii.LowercaseH, Ascii.LowercaseE, Ascii.Underscore, Ascii.K, Ascii.LowercaseE, Ascii.LowercaseY, Ascii.Underscore, Ascii.LowercaseO, Ascii.LowercaseF, Ascii.Underscore, Ascii.F, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.Underscore, Ascii.M, Ascii.LowercaseI, Ascii.LowercaseN, Ascii.LowercaseO, Ascii.LowercaseR })]
        public void CanEncodeByteArray(string input, byte[] expectedEncodedBytes)
        {
            var encoding = new NameEncoding12();
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
        //[TestCase("#", new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3 }, "/#23")]
        //[TestCase("A# # # B", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.B }, "/A#23#20#23#20#23#20B")]
        //[TestCase("©", new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.A, Ascii.Digit9 }, "/#A9")]
        //[TestCase("Adobe Green", new[] { Ascii.ForwardSlash, Ascii.A, Ascii.LowercaseD, Ascii.LowercaseO, Ascii.LowercaseB, Ascii.LowercaseE, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.G, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseE, Ascii.LowercaseN }, "/Adobe#20Green")]
        //[TestCase("PANTONE 5757 CV", new[] { Ascii.ForwardSlash, Ascii.P, Ascii.A, Ascii.N, Ascii.T, Ascii.O, Ascii.N, Ascii.E, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.Digit5, Ascii.Digit7, Ascii.Digit5, Ascii.Digit7, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.C, Ascii.V }, "/PANTONE#205757#20CV")]
        //[TestCase("The_Key_of_F#_Minor", new[] { Ascii.ForwardSlash, Ascii.T, Ascii.LowercaseH, Ascii.LowercaseE, Ascii.Underscore, Ascii.K, Ascii.LowercaseE, Ascii.LowercaseY, Ascii.Underscore, Ascii.LowercaseO, Ascii.LowercaseF, Ascii.Underscore, Ascii.F, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.Underscore, Ascii.M, Ascii.LowercaseI, Ascii.LowercaseN, Ascii.LowercaseO, Ascii.LowercaseR }, "/The_Key_of_F#23_Minor")]
        //public void CanEncodeByteArrayAndFormatString(string input, byte[] expectedEncodedBytes, string expectedFormattedString)
        //{
        //    var encoding = new NameEncoding12();
        //    var (actualEncodedBytes, actualFormattedString) = encoding.GetBytesAndFormattedString(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 })]
        [TestCase(new[] { 'L', 'o', 'r', 'e', 'm', '.', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM })]
        [TestCase(new[] { '#' }, new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3 })]
        [TestCase(new[] { ' ' }, new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0 })]
        [TestCase(new[] { '§' }, new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.A, Ascii.Digit7 })]
        [TestCase(new[] { 'A', '#', ' ', '#', ' ', '#', ' ', 'B' }, new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.B })]
        public void CanEncodeByteArray(char[] input, byte[] expectedEncodedBytes)
        {
            var encoding = new NameEncoding12();
            var actualEncodedBytes = encoding.GetBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        //[TestCase(new[] { 'a', 'b', 'c', '1', '2', '3' }, new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, new[] { '/', 'a', 'b', 'c', '1', '2', '3' })]
        //[TestCase(new[] { 'L', 'o', 'r', 'e', 'm', '.', 'i', 'p', 's', 'u', 'm' }, new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, new[] { '/', 'L', 'o', 'r', 'e', 'm', '.', 'i', 'p', 's', 'u', 'm' })]
        //[TestCase(new[] { '#' }, new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3 }, new[] { '/', '#', '2', '3' })]
        //[TestCase(new[] { ' ' }, new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0 }, new[] { '/', '#', '2', '0' })]
        //[TestCase(new[] { '§' }, new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.A, Ascii.Digit7 }, new[] { '/', '#', 'A', '7' })]
        //[TestCase(new[] { 'A', '#', ' ', '#', ' ', '#', ' ', 'B' }, new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.B }, new[] { '/', 'A', '#', '2', '3', '#', '2', '0', '#', '2', '3', '#', '2', '0', '#', '2', '3', '#', '2', '0', 'B' })]
        //public void CanEncodeByteArrayAndFormatCharArray(char[] input, byte[] expectedEncodedBytes, char[] expectedFormattedChars)
        //{
        //    var encoding = new NameEncoding12();
        //    var (actualEncodedBytes, actualFormattedChars) = encoding.GetBytesAndFormattedChars(input);

        //    CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        //    CollectionAssert.AreEqual(expectedFormattedChars, actualFormattedChars);
        //}

        [TestCase(new[] { Ascii.ForwardSlash }, "")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, "abc123")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, "Lorem.ipsum")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.LowercaseP, Ascii.LowercaseA, Ascii.LowercaseI, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseD, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit8, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit9, Ascii.LowercaseP, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseN, Ascii.LowercaseT, Ascii.LowercaseH, Ascii.LowercaseE, Ascii.LowercaseS, Ascii.LowercaseE, Ascii.LowercaseS }, "paired()parentheses")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit4, Ascii.Digit2 }, "AB")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3 }, "#")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.B }, "A# # # B")]
        [TestCase(new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.A, Ascii.Digit9 }, "©")]
        public void CanDecodeByteArray(byte[] input, string expectedDecodedString)
        {
            var encoding = new NameEncoding12();
            var actualDecodedString = encoding.GetString(input);

            CollectionAssert.AreEqual(expectedDecodedString, actualDecodedString);
        }

        //[TestCase(new[] { Ascii.ForwardSlash }, "", "/")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, "abc123", "/abc123")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Period, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, "Lorem.ipsum", "/Lorem.ipsum")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.LowercaseP, Ascii.LowercaseA, Ascii.LowercaseI, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseD, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit8, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit9, Ascii.LowercaseP, Ascii.LowercaseA, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseN, Ascii.LowercaseT, Ascii.LowercaseH, Ascii.LowercaseE, Ascii.LowercaseS, Ascii.LowercaseE, Ascii.LowercaseS }, "paired()parentheses", "/paired#28#29parentheses")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit4, Ascii.Digit2 }, "AB", "/A#42")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3 }, "#", "/#23")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.A, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit3, Ascii.NumberSign, Ascii.Digit2, Ascii.Digit0, Ascii.B }, "A# # # B", "/A#23#20#23#20#23#20B")]
        //[TestCase(new[] { Ascii.ForwardSlash, Ascii.NumberSign, Ascii.A, Ascii.Digit9 }, "©", "/#A9")]
        //public void CanDecodeByteArrayAndFormatString(byte[] input, string expectedDecodedString, string expectedFormattedString)
        //{
        //    var encoding = new NameEncoding12();
        //    var (actualDecodedString, actualFormattedString) = encoding.GetStringAndFormattedString(input);

        //    Assert.AreEqual(expectedDecodedString, actualDecodedString);
        //    Assert.AreEqual(expectedFormattedString, actualFormattedString);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileEncodingByteArray(string input)
        {
            var encoding = new NameEncoding12();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingString(string input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new NameEncoding12();
        //    var bytes = encoding.GetBytesAndFormattedString(input);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileEncodingByteArray(char[] input)
        {
            var encoding = new NameEncoding12();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileEncodingByteArrayAndFormattingCharArray(char[] input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new NameEncoding12();
        //    var bytes = encoding.GetBytesAndFormattedChars(input);
        //}

        [Test]
        public void DetectsInvalidEncodingWhileDecodingByteArray(byte[] input)
        {
            var encoding = new NameEncoding12();
            Assert.Throws<EncoderFallbackException>(() => encoding.GetString(input));
        }

        //[Test]
        //[ExpectedException(typeof(EncoderFallbackException))]
        //public void DetectsInvalidEncodingWhileDecodingByteArrayAndFormattingString(byte[] input)
        //{
        //    throw new NotImplementedException();
        //    var encoding = new NameEncoding12();
        //    var decodedString = encoding.GetStringAndFormattedString(input);
        //}
    }
}