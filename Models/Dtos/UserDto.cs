using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebLibrary.Models.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PersonalIdentificationNumber { get; set; }
        public string Adress { get; set; }
        public string PasswordHash { get; set; }
        public List<string> BookIds { get; set; }
        public bool IsBanned { get; set; }
        public bool IsApproved { get; set; }
    }
}
