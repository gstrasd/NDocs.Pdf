using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NDocs.Pdf
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    internal class PdfVersionAttribute : Attribute
    {
        private static readonly Regex _versionParser = new Regex(@"^(?<Major>\d+)\.(?<Minor>\d+)$", RegexOptions.Compiled);

        public PdfVersionAttribute(string version)
        {
            var match = _versionParser.Match(version);
            var major = Int32.Parse(match.Groups["Major"].Value);
            var minor = Int32.Parse(match.Groups["Minor"].Value);

            Version = new Version(major, minor);
        }

        public Version Version { get; }
    }
}
