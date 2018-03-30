using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NDocs.Pdf.Tests
{
    [TestFixture]
    public class VersionSelectorTests
    {
        [Test]
        [TestCase(1, 0, true)]
        [TestCase(1, 1, true)]
        [TestCase(1, 2, true)]
        [TestCase(1, 3, true)]
        [TestCase(1, 4, true)]
        [TestCase(1, 5, true)]
        [TestCase(1, 6, true)]
        [TestCase(1, 7, true)]
        [TestCase(2, 0, false)]
        public void RecognizesSupportedVersions(int major, int minor, bool expected)
        {
            var version = new Version(major, minor);
            var actual = VersionSelector.IsSupportedVersion(version);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(1, 0, "PdfEncodingProvider10")]
        [TestCase(1, 1, "PdfEncodingProvider10")]
        [TestCase(1, 2, "PdfEncodingProvider12")]
        [TestCase(1, 3, "PdfEncodingProvider12")]
        [TestCase(1, 4, "PdfEncodingProvider12")]
        [TestCase(1, 5, "PdfEncodingProvider12")]
        [TestCase(1, 6, "PdfEncodingProvider12")]
        [TestCase(1, 7, "PdfEncodingProvider12")]
        public void CanCreateVersionedInstance(int major, int minor, string expected)
        {
            var version = new Version(major, minor);
            var encodingProvider = VersionSelector.GetVersionedInstance<EncodingProvider>(version);

            Assert.AreEqual(expected, encodingProvider.GetType().Name);
        }
    }
}
