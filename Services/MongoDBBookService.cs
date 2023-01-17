using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;
using MongoDB.Labs.Search;

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

        public async Task<List<Book>> Search(string term)
        {
            var filters = Builders<Book>.Filter.Where(x => x.BookName.Contains(term)) |
                Builders<Book>.Filter.Where(x => x.AuthorName.Contains(term)) |
                Builders<Book>.Filter.Where(x => x.YearOfRelease.Contains(term));

            return await _bookCollection.Find(filters).ToListAsync(); 
        }

        public async Task<List<string>> Autocomplete()
        {
            var books = await _bookCollection.Find(new BsonDocument()).ToListAsync(); 
            var tags = new List<string>();

            foreach (var book in books)
            {
                if (!tags.Contains(book.BookName))
                {
                    tags.Add(book.BookName);
                }
                if (!tags.Contains(book.AuthorName))
                {
                    tags.Add(book.AuthorName);
                }
                if (!tags.Contains(book.YearOfRelease))
                {
                    tags.Add(book.YearOfRelease);
                }
            }

            return tags;
        }

        public async Task<List<Book>> Sort(string sortType)
        {
            var filter = Builders<Book>.Sort.Ascending(sortType);

            return await _bookCollection.Find(new BsonDocument()).Sort(filter).ToListAsync();
        }
    }
}