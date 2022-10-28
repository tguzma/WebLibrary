using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebLibrary.Models
{
    /*
        Aplikace uchovává/spravuje seznam zákazníků. 
        U zákazníka uchováváme/spravujeme jméno, příjmení, rodné číslo, adresu, uživatelské jméno a heslo. 
        Na konkrétní účet se pak navazují vypůjčené knihy. 
        Zákaznický účet může vytvářet knihovník nebo sám uživatel. 
        V případě že si ho vytvoří sám uživatel,
        vidí  knihovník upozornění a knihovník musí tento účet schválit před tím než je funkční a umožňuje výpůjčku knih.
        Pokud je účet schválen, může si zákazník půjčovat knihy nebo editovat své vlastní záznamy. 
        Pokud však dojde k editaci záznamů, je učet opět omezen do schválení knihovníka.
     */
    public class User : MongoIdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string PersonalIdentificationNumber { get; set; }
        public string Adress { get; set; }

        [BsonElement("bookIds")]
        [JsonPropertyName("bookIds")]
        public List<string> BookIds { get; set; }
        public bool IsBanned { get; set; }
        public bool IsApproved{ get; set; }
    }
}
