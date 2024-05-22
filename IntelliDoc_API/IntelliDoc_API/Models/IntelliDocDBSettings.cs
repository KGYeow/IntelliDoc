using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IntelliDoc_API.Models
{
    public partial class IntelliDocDBSettings
    {
        protected MongoClient client;
        protected IMongoDatabase database;

        public IntelliDocDBSettings(IOptions<IntelliDocDBSettings> options)
        {
            client = new MongoClient(options.Value.ConnectionString);
            database = client.GetDatabase(options.Value.DatabaseName);
        }

        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;

        public virtual IMongoCollection<BsonDocument> Users { get; set; } = null!;
    }
}