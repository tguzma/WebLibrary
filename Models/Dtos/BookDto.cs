using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.Models.Dtos
{
    public class BookDto
    {
        public string BookId { get; set; }
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public int NumberOfPages { get; set; }
        public int YearOfRelease { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        public int AmountAvalible { get; set; }
    }
}
