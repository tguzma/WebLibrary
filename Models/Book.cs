using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebLibrary.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }

        [Display(Name = "Book's name")]
        public string BookName { get; set; }

        [Display(Name = "Author's name")]
        public string AuthorName { get; set; }
        
        [Display(Name = "Number of pages")]
        public int NumberOfPages { get; set; }

        [Display(Name = "Release year")]
        public string YearOfRelease { get; set; }
        public string ImageUrl { get; set; }

        [Display(Name = "Amount avalible")]
        public int AmountAvalible { get; set; }
        public int AmountBorrowed { get; set; } = 0;
    }
}