using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System;

namespace WebLibrary.Models
{
    public class Loan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string LoanId { get; set; }
        public string UserId { get; set; }
        public string PersonalId { get; set; }
        public string BookId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
