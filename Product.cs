using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dotnet.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

        [BsonElement("NOME")]
        public string NOME { get; set; }

        [BsonElement("DESCRICAO")]
        public string DESCRICAO { get; set; }

        [BsonElement("VALOR")]
        [BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)]
        public decimal VALOR { get; set; }
    }
}