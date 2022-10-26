using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebLibrary.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public int NumberOfPages { get; set; }
        public int YearOfRelease { get; set; }
        public string ImageUrl { get; set; }
        public int AmountAvalible { get; set; }

    }
}