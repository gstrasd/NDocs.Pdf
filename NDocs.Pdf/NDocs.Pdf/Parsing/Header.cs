using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Parsing
{
    public class Header
    {
        public Header(int majorVersion, int minorVersion, uint magicNumber)
        {
            if (majorVersion < 0) throw new ArgumentOutOfRangeException(nameof(majorVersion), $"{nameof(majorVersion)} must be a positive value.");
            if (minorVersion < 0) throw new ArgumentOutOfRangeException(nameof(minorVersion), $"{nameof(minorVersion)} must be a positive value.");

            Version = new Version(majorVersion, minorVersion);
            MagicNumber = magicNumber;
        }

        public Version Version { get; }
        public uint MagicNumber { get; }
    }
}