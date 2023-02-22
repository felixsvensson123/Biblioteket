using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp15.Shared.Models;

public class BookModel : ProductBase
{
    public string Author { get; set; } = string.Empty;
    public int Pages { get; set; }

    public override BookModel? FromCSV(string line)
    {
        string[] values = line.Split(",");

        if (values.Length != 5)
            return null;
        else
        {
            return new BookModel()
            {
                Name = values[0],
                Author = values[1],
                Pages = int.Parse(values[2]),
                Count = int.Parse(values[3]),
                Description = values[4],
                Created = DateTime.Now
            };
        }
    }

    [NotMapped]
    public override string ModelType { get; } = nameof(BookModel);
}