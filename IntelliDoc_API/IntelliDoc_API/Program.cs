using IntelliDoc_API.Models;
using IntelliDoc_API.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

/*var client = new MongoClient(configuration.GetConnectionString("ConnUri"));
var collection = client.GetDatabase("IntelliDocDB").GetCollection<BsonDocument>("Users");
var filter = Builders<BsonDocument>.Filter.Eq("fullName", "Yeow Kok Guan");
var document = collection.Find(filter).First();
Console.WriteLine(document);*/

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<IntelliDocDBContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("ConnStr"));
});
builder.Services.Configure<IntelliDocDBSettings>(options =>
{
    options.ConnectionString = configuration.GetConnectionString("ConnUri");
    options.DatabaseName = "IntelliDocDB";
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["JWT:ValidIssuer"],
        ValidAudience = configuration["JWT:ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
    }
);
builder.Services.AddAuthorization();
builder.Services.AddScoped<ModelService>();
builder.Services.AddScoped<RegExService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Need to remove when using Window Auth Use
app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler("/error");
app.UseStaticFiles();

app.Run();