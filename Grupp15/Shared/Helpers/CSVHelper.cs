using Grupp15.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Grupp15.Shared.Helpers
{
    public class CSVHelper
    {
        public static List<T>? FromStream<T>(Stream stream) where T : ProductBase, new()
        {
            if (stream == null)
                return null;

            string csv = "";

            var reader = new StreamReader(stream);

            while (reader.Peek() >= 0)
                csv += reader.ReadLine() + "\n";

            var type = new T();

            List<T?> list = csv.Split("\n")
                .Skip(1) //remove first line that indicates column names
                .SkipLast(1) //remove last line as its empty
                .Select(t => (T?)type.FromCSV(t))
                .Where(t => t != null)
                .ToList();

            if(list is not null)
            {
                return list!;
            }

            return null;
        }

        public static List<T>? FromFile<T>(IFormFile file) where T : ProductBase, new()
        {
            if (file == null)
                return null;

            byte[] buffer = new byte[file.Length];

            file.OpenReadStream().Read(buffer);

            Stream fileStream = new MemoryStream(buffer);

            return FromStream<T>(fileStream)!;
        }
    }
}