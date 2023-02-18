using System.Collections.Generic;
using System.IO;

namespace BusTour.AppServices.FileConvertingService
{
    public interface IFileConvertingService
    {
        byte[] ConvertToPDF(Stream templateStream, Dictionary<string, string> dict);
    }
}