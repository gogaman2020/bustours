using System.IO;
using System.Reflection;
using System.Text;

namespace BusTour.AppServices.Resources
{
    public static class EmbeddedResource
    {
        public static string GetFileContent(string fileName)
        {
            var ns = typeof(EmbeddedResource).GetTypeInfo().Namespace;

            using (var stream = typeof(EmbeddedResource).GetTypeInfo().Assembly.GetManifestResourceStream($"{ns}.{fileName}.json"))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static byte[] GetFileBytes(string fileName)
        {
            var ns = typeof(EmbeddedResource).GetTypeInfo().Namespace;

            using (var stream = typeof(EmbeddedResource).GetTypeInfo().Assembly.GetManifestResourceStream($"{ns}.{fileName}"))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}