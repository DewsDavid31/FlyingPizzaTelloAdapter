using FlyingPizzaTelloTests;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlyingPizzaTello;

public class Drone : IBaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public Guid BadgeNumber { get; set; }

        public string OrderId { get; set; }

        public GeoLocation HomeLocation { get; set; }

        [BsonElement("Location")]
        public GeoLocation CurrentLocation { get; set; }

        public GeoLocation Destination { get; set; }

        public string Status { get; set; }

        public string IpAddress { get; set; }

        public string DispatcherUrl { get; set; }
    }
