using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;

namespace WebLibrary.Services
{
    public class MongoDBUserService
    {
        private readonly IMongoCollection<User> _userCollection;

        public MongoDBUserService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionNameUser);
        }
        
        public async Task<List<User>> GetUsersByRoleAsync(Guid roleId)
        {
            var filter = Builders<User>.Filter.AnyEq("Roles", roleId);
            return await _userCollection.Find(filter).ToListAsync();
        }
    }
}