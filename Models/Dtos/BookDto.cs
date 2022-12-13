using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.Models.Dtos
{
    public class BookDto
    {

        public string BookId { get; set; }
        [Required]
        public string BookName { get; set; }
        [Required]
        public string AuthorName { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage ="Number of pages should be integer number")]
        public int NumberOfPages { get; set; }
        [Required]
        public int YearOfRelease { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Amount shall be an integer")]
        public int AmountAvalible { get; set; }

        //does it need amount borrowed?
        public int AmountBorrowed { get; set; }
    }
}
