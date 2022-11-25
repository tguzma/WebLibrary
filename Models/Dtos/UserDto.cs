using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.Models.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [RegularExpression(@"^([0-9]{2})(01|02|03|04|05|06|07|08|09|10|11|12|51|52|53|54|55|56|57|58|59|60|61|62)(([0]{1}[1-9]{1})|([1-2]{1}[0-9]{1})|([3]{1}[0-1]{1}))/([0-9]{3,4})$", ErrorMessage = "Please enter valid Personal Id")]
        public string PersonalIdentificationNumber { get; set; }
        [Required]
        public string Adress { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{4,25}$", ErrorMessage = "Password must be at least 4 char long and must include at least one upper case letter, one lower case letter, and one numeric digit.")]
        public string PasswordHash { get; set; }
        public List<string> BookIds { get; set; }
        public bool IsBanned { get; set; }
        public bool IsApproved { get; set; }
        public List<HistoryEntry> BookHistory { get; set; } = new List<HistoryEntry>();
    }
}
