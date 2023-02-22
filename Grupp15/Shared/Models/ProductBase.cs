using Grupp15.Shared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Grupp15.Shared.Models
{
    [JsonConverter(typeof(ProductConverter))]
    public abstract class ProductBase
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Count { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; } = string.Empty;
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public string Type => ModelType.Replace("Model", "");

        //Helper function(s)
        public abstract ProductBase? FromCSV(string line);

        //Json type handling
        [NotMapped]
        public abstract string ModelType { get; }
    }
}