using Grupp15.Shared.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Grupp15.Shared.Helpers
{
    public class ProductConverter : JsonConverter<ProductBase>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ProductBase).IsAssignableFrom(typeToConvert);
        }

        public override ProductBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                return jsonDoc.RootElement.GetProperty("modelType").GetString() switch
                {
                    nameof(BookModel) => jsonDoc.RootElement.Deserialize<BookModel>(options),
                    nameof(MovieModel) => jsonDoc.RootElement.Deserialize<MovieModel>(options),
                    nameof(EBookModel) => jsonDoc.RootElement.Deserialize<EBookModel>(options),
                    _ => throw new JsonException("'Type' doesn't match a known derived type"),
                };
            }
        }

        public override void Write(Utf8JsonWriter writer, ProductBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}