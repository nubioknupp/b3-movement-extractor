using System.IO;
using System.Text;
using Aspose.Cells;

namespace B3MovementExtractorWeb.Helpers
{
    public class ExcelHelper
    {
        public static async Task<IEnumerable<string>> ToStringFormatCSV(IFormFile file)
        {
            string[] contents;
            var fileCSVPath = $"{Path.GetTempPath()}OutputCSV-{Guid.NewGuid()}.csv";
            var options = new TxtSaveOptions(SaveFormat.Csv)
            {
                Separator = Convert.ToChar(";")
            };

            using var fileStream = new MemoryStream();
            await file.CopyToAsync(fileStream);

            using var workbook = new Workbook(fileStream);
            workbook.Save(fileName: fileCSVPath, saveOptions: options);

            contents = File.ReadAllLines(fileCSVPath, Encoding.UTF8);

            if (File.Exists(fileCSVPath))
            {
                File.Delete(fileCSVPath);
            }

            return contents.Where(line => !line.Contains("Aspose.Cells"));
        }
    }
}
