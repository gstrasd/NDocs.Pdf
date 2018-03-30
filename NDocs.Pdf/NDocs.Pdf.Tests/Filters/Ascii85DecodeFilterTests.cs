using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Encoding;
using NDocs.Pdf.Filters;
using NDocs.Pdf.Parsing;
using NUnit.Framework;

namespace NDocs.Pdf.Tests.Filters
{
    [TestFixture]
    public class Ascii85DecodeFilterTests
    {
        [TestCase(new byte[] { }, "~>")]
        [TestCase(new byte[] { 0x00 }, "z~>")]
        [TestCase(new byte[] { 0x00, 0x00 }, "z~>")]
        [TestCase(new byte[] { 0x00, 0x00, 0x00 }, "z~>")]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00 }, "z~>")]
        [TestCase(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }, "zz~>")]
        [TestCase(new byte[] { 0x12 }, "&c~>")]
        [TestCase(new byte[] { 0x12, 0x34 }, "&i9~>")]
        [TestCase(new byte[] { 0x12, 0x34, 0x56 }, "&i<V~>")]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78 }, "&i<X6~>")]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A }, "&i<X6RK~>")]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC }, "&i<X6R_/~>")]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE }, "&i<X6R_7J~>")]
        [TestCase(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 }, "&i<X6R_7MH~>")]
        public void CanEncodeByteArray(byte[] input, string expectedEncodedString)
        {
            var filter = new Filter(FilterType.Ascii85Decode);
            var encodeBytes = filter.EncodeBytes(input);
            var encoding = new FastAsciiEncoding();
            var actualEncodedString = encoding.GetString(encodeBytes);

            CollectionAssert.AreEqual(expectedEncodedString, actualEncodedString);
        }

        [TestCase("~>", new byte[] { })]
        [TestCase("z~>", new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [TestCase("&c~>", new byte[] { 0x12 })]
        [TestCase("&i9~>", new byte[] { 0x12, 0x34 })]
        [TestCase("&i<V~>", new byte[] { 0x12, 0x34, 0x56 })]
        [TestCase("&i<X6~>", new byte[] { 0x12, 0x34, 0x56, 0x78 })]
        [TestCase("&i<X6RK~>", new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A })]
        [TestCase("&i<X6R_/~>", new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC })]
        [TestCase("&i<X6R_7J~>", new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE })]
        [TestCase("&i<X6R_7MH~>", new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 })]
        public void CanDecodeByteArray(string input, byte[] expectedDecodedBytes)
        {
            var encoding = new FastAsciiEncoding();
            var inputBytes = encoding.GetBytes(input);
            var filter = new Filter(FilterType.Ascii85Decode);
            var actualDecodedBytes = filter.DecodeBytes(inputBytes);

            CollectionAssert.AreEqual(expectedDecodedBytes, actualDecodedBytes);
        }

        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit1, Ascii.Colon, Ascii.GreaterThanSign })]
        [TestCase(new[] { Ascii.LessThanSign, Ascii.Digit1, Ascii.Null, Ascii.Colon, Ascii.GreaterThanSign })]
        public void DetectsInvalidEncodingWhileDecodingByteArray(byte[] input)
        {
            throw new NotImplementedException();
        }
    }
}