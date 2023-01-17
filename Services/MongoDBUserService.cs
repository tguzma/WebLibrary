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

        public async Task<List<User>> Search(string term)
        {
            var filters = Builders<User>.Filter.Where(x => x.FirstName.Contains(term)) |
                Builders<User>.Filter.Where(x => x.LastName.Contains(term)) |
                Builders<User>.Filter.Where(x => x.PersonalIdentificationNumber.Contains(term)) |
                Builders<User>.Filter.Where(x => x.Adress.Contains(term));

            return await _userCollection.Find(filters).ToListAsync();
        }

        public async Task<List<string>> Autocomplete()
        {
            var users = await _userCollection.Find(new BsonDocument()).ToListAsync();
            var tags = new List<string>();

            foreach (var user in users)
            {
                if (!tags.Contains(user.FirstName))
                {
                    tags.Add(user.FirstName);
                }
                if (!tags.Contains(user.LastName))
                {
                    tags.Add(user.LastName);
                }
                if (!tags.Contains(user.PersonalIdentificationNumber))
                {
                    tags.Add(user.PersonalIdentificationNumber);
                }
                if (!tags.Contains(user.Adress))
                {
                    tags.Add(user.Adress);
                }
            }

            return tags;
        }

        public async Task<List<User>> Sort(string sortType)
        {
            var filter = Builders<User>.Sort.Ascending(sortType);

            return await _userCollection.Find(new BsonDocument()).Sort(filter).ToListAsync();
        }
    }
}