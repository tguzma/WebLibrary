using System;

namespace WebLibrary.Models
{
    public class HistoryEntry
    {
        public string BookId { get; set; }
        public DateTime DateBorrowed { get; set; }
        public DateTime? DateReturned { get; set; }

        public HistoryEntry(string bookId, DateTime dateBorrowed, DateTime? dateReturned)
        {
            BookId = bookId;
            DateBorrowed = dateBorrowed;
            DateReturned = dateReturned;
        }
    }
}
