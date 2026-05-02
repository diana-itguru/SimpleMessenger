using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleMessenger.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "USER";
}

public class UserDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}