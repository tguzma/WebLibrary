using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace WebLibrary.Models
{
    public class MongoDBSettings : MongoDbSettings
    {
        public string CollectionNameUser { get; set; } 
        public string CollectionNameBook { get; set; } 
    }
}
