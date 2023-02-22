using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp15.Shared.Models;

public class EBookModel : ProductBase
{
    public string EAuthor { get; set; } = String.Empty;
    public int EPages { get; set; }

    public override EBookModel? FromCSV(string line)
    {
        string[] values = line.Split(",");

        if (values.Length != 4)
            return null;
        else
        {
            return new EBookModel()
            {
                Name = values[0],
                EAuthor = values[1],
                EPages = int.Parse(values[2]),
                Description = values[3],
                Created = DateTime.Now
            };
        }
    }

    [NotMapped]
    public override string ModelType { get; } = nameof(EBookModel);
}