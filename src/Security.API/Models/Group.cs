using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sofisoft.Enterprise.SeedWork.MongoDB.Attributes;
using Sofisoft.Enterprise.SeedWork.MongoDB.Domain;

namespace Sofisoft.Accounts.Security.API.Models
{
    [BsonCollection("group")]
    public class Group : Document
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("workspaceId")]
        public string WorkspaceId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("parents")]
        public string[] Parents { get; set; }

        [BsonElement("users")]
        public string[] Users { get; set; }
    }
}