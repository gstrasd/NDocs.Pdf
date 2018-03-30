using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NDocs.Pdf
{
    internal static class VersionSelector
    {
        private static readonly List<Version> _supportedVersions;
        private static readonly ILookup<Version, Type> _versionedTypes;

        static VersionSelector()
        {
            var assembly = Assembly.GetExecutingAssembly();
            _supportedVersions = (
                from a in assembly.GetCustomAttributes<PdfVersionAttribute>()
                orderby a.Version.Major, a.Version.Minor
                select a.Version
            ).ToList();
            _versionedTypes = (
                from t in assembly.GetTypes()
                from a in t.GetCustomAttributes<PdfVersionAttribute>()
                select new { a.Version, Type = t }
            ).ToLookup(t => t.Version, t => t.Type);
        }

        public static bool IsSupportedVersion(Version version)
        {
            return _supportedVersions.Contains(version);
        }

        public static Version GetLatestVersion()
        {
            return _supportedVersions.Last();
        }

        public static T GetVersionedInstance<T>(Version version) where T : class
        {
            var type = _versionedTypes[version].FirstOrDefault(t => t.IsSubclassOf(typeof(T)));
            if (type != null) return (T)Activator.CreateInstance(type);
            throw new ArgumentException($"Type {typeof(T).Name} is not versioned.");
        }
    }
}
