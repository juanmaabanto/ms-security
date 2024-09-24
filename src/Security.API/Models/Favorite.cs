using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sofisoft.Enterprise.SeedWork.MongoDB.Attributes;
using Sofisoft.Enterprise.SeedWork.MongoDB.Domain;

namespace Sofisoft.Accounts.Security.API.Models
{
    [BsonCollection("favorite")]
    public class Favorite : Document
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("companyId")]
        public string CompanyId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("optionId")]
        public string OptionId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string UserId { get; set; }

    }
}