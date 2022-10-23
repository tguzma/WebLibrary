using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;

namespace WebLibrary.Services
{
    public class MongoDBBookService
    {
        private readonly IMongoCollection<Book> _bookCollection;

        public MongoDBBookService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _bookCollection = database.GetCollection<Book>(mongoDBSettings.Value.CollectionNameBook);
        }
        
        public async Task<List<Book>> GetAsync()
        {
            return await _bookCollection.Find(new BsonDocument()).ToListAsync();
        } 

        public async Task CreateAsync(Book book) 
        {
            await _bookCollection.InsertOneAsync(book);

            return;
        }

        public async Task UpdatetAsync(string id,UpdateDefinition<Book> updateDefinition)
        {
            FilterDefinition<Book> filter = Builders<Book>.Filter.Eq("Id", id);
            await _bookCollection.UpdateOneAsync(filter, updateDefinition);

            return;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<Book> filter = Builders<Book>.Filter.Eq("Id", id);
            await _bookCollection.DeleteOneAsync(filter);

            return;
        }
    }
}