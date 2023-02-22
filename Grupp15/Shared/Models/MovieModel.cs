using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp15.Shared.Models;

public class MovieModel : ProductBase
{
    public string Director { get; set; } = String.Empty;
    public string Genre { get; set; } = String.Empty;

    public override MovieModel? FromCSV(string line)
    {
        string[] values = line.Split(",");

        if (values.Length != 5)
            return null;
        else
        {
            return new MovieModel()
            {
                Name = values[0],
                Director = values[1],
                Genre = values[2],
                Count = int.Parse(values[3]),
                Description = values[4],
                Created = DateTime.Now
            };
        }
    }

    [NotMapped]
    public override string ModelType { get; } = nameof(MovieModel);
}
