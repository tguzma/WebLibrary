using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebLibrary.Models;

namespace WebLibrary.Services
{
    public class MongoDBLoanService
    {
        private readonly IMongoCollection<Loan> _loanCollection;

        public MongoDBLoanService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _loanCollection = database.GetCollection<Loan>(mongoDBSettings.Value.CollectionNameLoan);
        }

        public async Task<List<Loan>> GetAsync()
        {
            return await _loanCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Loan> FindByIdAsync(string idBook, string idUser)
        {
            var filters = Builders<Loan>.Filter.And(Builders<Loan>.Filter.Eq("BookId", idBook),Builders<Loan>.Filter.Eq("UserId",idUser));
            try
            {
                return await _loanCollection.Find(filters).Limit(1).SingleAsync();
            }
            catch
            {
                return new Loan();
            }
        }

        public async Task CreateAsync(Loan loan)
        {
            await _loanCollection.InsertOneAsync(loan);

            return;
        }

        public async Task UpdatetAsync(Loan loan)
        {
            var filter = Builders<Loan>.Filter.Eq("LoanId", loan.LoanId);
            await _loanCollection.ReplaceOneAsync(filter, loan);

            return;
        }

        public async Task DeleteAsync(string idBook, string idUser)
        {
            var filters = Builders<Loan>.Filter.And(Builders<Loan>.Filter.Eq("BookId", idBook), Builders<Loan>.Filter.Eq("UserId", idUser));
            await _loanCollection.DeleteOneAsync(filters);

            return;
        }
    }
}
