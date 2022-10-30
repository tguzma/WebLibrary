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

        public async Task<Book> FindByIdAsync(string id)
        {
            var filter = Builders<Book>.Filter.Eq("BookId", id);
            return await _bookCollection.Find(filter).Limit(1).SingleAsync();
        }

        public async Task CreateAsync(Book book) 
        {
            await _bookCollection.InsertOneAsync(book);

            return;
        }

        public async Task UpdatetAsync(Book book)
        {
            var filter = Builders<Book>.Filter.Eq("BookId", book.BookId);
            await _bookCollection.ReplaceOneAsync(filter, book);

            return;
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Book>.Filter.Eq("BookId", id);
            await _bookCollection.DeleteOneAsync(filter);

            return;
        }
    }
}