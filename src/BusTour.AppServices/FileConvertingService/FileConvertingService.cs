using Aspose.Words;
using Infrastructure.Common.DI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TemplateEngine.Docx;

namespace BusTour.AppServices.FileConvertingService
{
    [InjectAsSingleton]
    public class FileConvertingService : IFileConvertingService
    {
        public byte[] ConvertToPDF(Stream templateStream, Dictionary<string, string> dict)
        {
            using (var memoryStream = new MemoryStream())
            {
                templateStream.CopyTo(memoryStream);

                File.WriteAllBytes("Temp.docx", memoryStream.ToArray());
            }
            
            var valuesToFill = new Content(dict.Select(x => new FieldContent(x.Key, x.Value)).ToArray());

            using (var outputDocument = new TemplateProcessor("Temp.docx").SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();
            }

            var doc = new Document("Temp.docx");
            doc.Save("Output.pdf");

            var content = File.ReadAllBytes("Output.pdf");

            File.Delete("Output.pdf");
            File.Delete("Temp.docx");

            return content;
        }
    }
}
