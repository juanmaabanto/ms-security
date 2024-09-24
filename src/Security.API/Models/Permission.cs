using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sofisoft.Enterprise.SeedWork.MongoDB.Attributes;
using Sofisoft.Enterprise.SeedWork.MongoDB.Domain;

namespace Sofisoft.Accounts.Security.API.Models
{
    [BsonCollection("permission")]
    public class Permission : Document
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("companyId")]
        public string CompanyId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("actionId")]
        public string ActionId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("groupId")]
        [BsonIgnoreIfNull]
        public string GroupId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        [BsonIgnoreIfNull]
        public string UserId { get; set; }

        [BsonElement("allow")]
        public bool Allow { get; set; }

        [BsonElement("deny")]
        public bool Deny { get; set; }
    }
}