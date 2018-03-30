using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf.Tests
{
    public static class TestUtility
    {
        private const string _fileNamespace = "Company.Documents.Pdf.Tests.Files.";

        public static Stream GetFile(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = String.Concat(_fileNamespace, filename);
            var stream = assembly.GetManifestResourceStream(name);

            return stream;
        }
    }
}
