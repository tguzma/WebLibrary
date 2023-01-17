using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebLibrary.Models
{
    public class User : MongoIdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalIdentificationNumber { get; set; }
        public string Adress { get; set; }

        [BsonElement("bookIds")]
        [JsonPropertyName("bookIds")]
        public List<string> BookIds { get; set; } = new List<string>();
        public bool IsBanned { get; set; }
        public bool IsApproved{ get; set; }
        public List<HistoryEntry> BookHistory { get; set; } = new List<HistoryEntry>();
    }
}
