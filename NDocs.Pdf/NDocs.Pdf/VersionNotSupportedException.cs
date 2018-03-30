using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf
{
    public class VersionNotSupportedException : Exception
    {
        public VersionNotSupportedException(Version version) : base($"Version {version?.Major}.{version?.Minor} is not supported.")
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            Version = (Version) version.Clone();
        }
        
        public Version Version { get; }
    }
}
