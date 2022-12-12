using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace WebLibrary.Models.Dtos
{
    public class LoanDto
    {
        public string LoanId { get; set; }
        [Required]
        public string BookId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public BsonDateTime CreatedAt { get; set; }
    }
}
