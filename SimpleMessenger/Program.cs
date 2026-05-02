using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SimpleMessenger.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.SignalR;
using SimpleMessenger;

var builder = WebApplication.CreateBuilder(args);

//MongoDB
var connectionString = builder.Configuration["MongoDB:ConnectionString"];
var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase("SimpleMessenger");
builder.Services.AddSingleton(database.GetCollection<User>("Users")); //критически важная строка, потому что объясняет программе, что такое IMongoCollection<User>

var secretKey = builder.Configuration["JwtSettings:SecretKey"];

//SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton(database.GetCollection<Message>("Messages"));

var app = builder.Build();

//передача статики на html, чтоб видеть в браузере
app.UseDefaultFiles(); 
app.UseStaticFiles();

app.UseMiddleware<AuthMiddleware>(); //вызов Middleware авторизации

//Регистрация
app.MapPost("/register", async (User user, IMongoCollection<User> users) =>
{
    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
    await users.InsertOneAsync(user);
    return Results.Created($"/users/{user.Id}", user);
});

//Логин
app.MapPost("/login", async (UserDto logged, IMongoCollection<User> users) =>
{
    var user = await users.Find(u => u.Username == logged.Username).FirstOrDefaultAsync();

    if (user == null || !BCrypt.Net.BCrypt.Verify(logged.Password, user.PasswordHash))
        return Results.Unauthorized();
    
    var token = TokenService.GenerateToken(user, secretKey);
    return Results.Ok(new { Token = token });
});

// Тестовый эндпоинт
app.MapGet("/getuser", (HttpContext context) =>
{
    var userId = context.Items["UserId"];
    var userRole = context.Items["UserRole"];
    
    if (userId == null) return Results.Json(new { message = "Unauthorized" }, statusCode: 401);
    
    if (userRole?.ToString() != "ADMIN") return Results.Json(new { message = "Forbidden" }, statusCode: 403);

    return Results.Ok(new { 
        message = "Привет, Админ! Ты авторизован.", 
        id = userId, 
        role = userRole 
    });
});

//
app.MapGet("/chat/history", async (IMongoCollection<Message> collection) =>
    await collection.Find(_ => true).Limit(50).ToListAsync());

app.MapHub<ChatHub>("/chathub");

app.Run();