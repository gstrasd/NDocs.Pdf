using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDocs.Pdf.Filters;
using NDocs.Pdf.Parsing;
using NUnit.Framework;

namespace NDocs.Pdf.Tests.Filters
{
    [TestFixture]
    public class AsciiHexDecodeFilterTests
    {
        [TestCase(new byte[] { }, new[] { Ascii.GreaterThanSign })]
        [TestCase(new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 }, new[] { Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign })]
        [TestCase(new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM }, new[] { Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign })]
        public void CanEncodeByteArray(byte[] input, byte[] expectedEncodedBytes)
        {
            var filter = new Filter(FilterType.AsciiHexDecode);
            var actualEncodedBytes = filter.EncodeBytes(input);

            CollectionAssert.AreEqual(expectedEncodedBytes, actualEncodedBytes);
        }

        [TestCase(new[] { Ascii.GreaterThanSign }, new byte[] { })]
        [TestCase(new[] { Ascii.Digit6, Ascii.Digit1, Ascii.Digit6, Ascii.Digit2, Ascii.Digit6, Ascii.Digit3, Ascii.Digit3, Ascii.Digit1, Ascii.Digit3, Ascii.Digit2, Ascii.Digit3, Ascii.Digit3, Ascii.GreaterThanSign }, new[] { Ascii.LowercaseA, Ascii.LowercaseB, Ascii.LowercaseC, Ascii.Digit1, Ascii.Digit2, Ascii.Digit3 })]
        [TestCase(new[] { Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.GreaterThanSign }, new[] { Ascii.AtSign, Ascii.AtSign, Ascii.AtSign })]
        [TestCase(new[] { Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Space, Ascii.GreaterThanSign }, new[] { Ascii.AtSign, Ascii.AtSign, Ascii.AtSign })]
        [TestCase(new[] { Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.Digit4, Ascii.Digit0, Ascii.GreaterThanSign }, new[] { Ascii.AtSign, Ascii.AtSign, Ascii.AtSign })]
        [TestCase(new[] { Ascii.Digit4, Ascii.LowercaseC, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.GreaterThanSign }, new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM })]
        [TestCase(new[] { Ascii.Digit4, Ascii.C, Ascii.Digit6, Ascii.F, Ascii.Digit7, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.D, Ascii.GreaterThanSign }, new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM })]
        [TestCase(new[] { Ascii.HorizontalTab, Ascii.Digit4, Ascii.LowercaseC, Ascii.CarriageReturn, Ascii.Digit6, Ascii.LowercaseF, Ascii.Digit7, Ascii.LineFeed, Ascii.Digit2, Ascii.Digit6, Ascii.Digit5, Ascii.Digit6, Ascii.Space, Ascii.Space, Ascii.Null, Ascii.LowercaseD, Ascii.Digit2, Ascii.Digit0, Ascii.Digit6, Ascii.Digit9, Ascii.Digit7, Ascii.Digit0, Ascii.FormFeed, Ascii.Digit7, Ascii.Digit3, Ascii.Digit7, Ascii.Digit5, Ascii.Digit6, Ascii.LowercaseD, Ascii.GreaterThanSign }, new[] { Ascii.L, Ascii.LowercaseO, Ascii.LowercaseR, Ascii.LowercaseE, Ascii.LowercaseM, Ascii.Space, Ascii.LowercaseI, Ascii.LowercaseP, Ascii.LowercaseS, Ascii.LowercaseU, Ascii.LowercaseM })]
        public void CanDecodeByteArray(byte[] input, byte[] expectedDecodedBytes)
        {
            var filter = new Filter(FilterType.AsciiHexDecode);
            var actualDecodedBytes = filter.DecodeBytes(input);

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