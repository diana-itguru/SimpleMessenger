using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleMessenger.Models;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    
    public string? Id { get; set; }
    public string Sender { get; set; }
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}