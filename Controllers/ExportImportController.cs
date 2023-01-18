using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using WebLibrary.Models;
using WebLibrary.Models.Dtos;
using WebLibrary.Services;

namespace WebLibrary.Controllers
{
    public class ExportImportController : Controller
    {
        private MongoClient client;
        IMongoDatabase database;

        private readonly IMongoCollection<Book> _bookCollection;
        private readonly IMongoCollection<Loan> _LoanCollection;
        private readonly IMongoCollection<User> _UserCollection;

        private string _outputBooks; // initialize to the output file
        private string _outputUsers; // initialize to the output file
        private string _outputLoans; // initialize to the output file
        private IWebHostEnvironment _WebHostEnvironment;
        string zipPath;

        public ExportImportController(IOptions<MongoDBSettings> mongoDBSettings, IWebHostEnvironment webHostEnvironment)
        {
            _WebHostEnvironment = webHostEnvironment;
            client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _bookCollection = database.GetCollection<Book>(mongoDBSettings.Value.CollectionNameBook);
            _LoanCollection = database.GetCollection<Loan>(mongoDBSettings.Value.CollectionNameLoan);
            _UserCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionNameUser);
            _outputBooks = @".\Export\books.json";
            _outputLoans = @".\Export\loans.json";
            _outputUsers = @".\Export\users.json";
            zipPath = @".\ExportDb.zip";
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Index), "Book");
        }

        [HttpGet("Export")]
        public async Task<ActionResult> ExportAsync()
        {
            using (var streamWriter = new StreamWriter(_outputBooks))
            {
                await _bookCollection.Find(new BsonDocument())
                    .ForEachAsync(async (document) =>
                    {
                        using (var stringWriter = new StringWriter())
                        using (var jsonWriter = new JsonWriter(stringWriter))
                        {
                            var context = BsonSerializationContext.CreateRoot(jsonWriter);
                            _bookCollection.DocumentSerializer.Serialize(context, document);
                            var line = stringWriter.ToString();
                            await streamWriter.WriteLineAsync(line);
                        }
                    });
            }

            using (var streamWriter = new StreamWriter(_outputLoans))
            {
                await _LoanCollection.Find(new BsonDocument())
                    .ForEachAsync(async (document) =>
                    {
                        using (var stringWriter = new StringWriter())
                        using (var jsonWriter = new JsonWriter(stringWriter))
                        {
                            var context = BsonSerializationContext.CreateRoot(jsonWriter);
                            _LoanCollection.DocumentSerializer.Serialize(context, document);
                            var line = stringWriter.ToString();
                            await streamWriter.WriteLineAsync(line);
                        }
                    });
            }

            using (var streamWriter = new StreamWriter(_outputUsers))
            {
                await _UserCollection.Find(new BsonDocument())
                    .ForEachAsync(async (document) =>
                    {
                        using (var stringWriter = new StringWriter())
                        using (var jsonWriter = new JsonWriter(stringWriter))
                        {
                            var context = BsonSerializationContext.CreateRoot(jsonWriter);
                            _UserCollection.DocumentSerializer.Serialize(context, document);
                            var line = stringWriter.ToString();
                            await streamWriter.WriteLineAsync(line);
                        }
                    });
            }
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(@".\Export", zipPath);

            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpGet("Import")]
        public async Task<ActionResult> ImportAsync()
        {
            if (System.IO.File.Exists(@".\import\books.json") || System.IO.File.Exists(@".\import\loans.json") || System.IO.File.Exists(@".\import\users.json"))
            {
                System.IO.File.Delete(@".\import\books.json");
                System.IO.File.Delete(@".\import\loans.json");
                System.IO.File.Delete(@".\import\users.json");
            }
            ZipFile.ExtractToDirectory(zipPath, @".\import");
            using (var streamReader = new StreamReader(@".\import\books.json"))
            {
                await _bookCollection.DeleteManyAsync(new BsonDocument());

                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    using (var jsonReader = new JsonReader(line))
                    {
                        var context = BsonDeserializationContext.CreateRoot(jsonReader);
                        var document = _bookCollection.DocumentSerializer.Deserialize(context);
                        await _bookCollection.InsertOneAsync(document);
                    }
                }
            }

            using (var streamReader = new StreamReader(@".\import\loans.json"))
            {
                await _LoanCollection.DeleteManyAsync(new BsonDocument());

                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    using (var jsonReader = new JsonReader(line))
                    {
                        var context = BsonDeserializationContext.CreateRoot(jsonReader);
                        var document = _LoanCollection.DocumentSerializer.Deserialize(context);
                        await _LoanCollection.InsertOneAsync(document);
                    }
                }
            }

            using (var streamReader = new StreamReader(@".\import\users.json"))
            {
                await _UserCollection.DeleteManyAsync(new BsonDocument());

                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    using (var jsonReader = new JsonReader(line))
                    {
                        var context = BsonDeserializationContext.CreateRoot(jsonReader);
                        var document = _UserCollection.DocumentSerializer.Deserialize(context);
                        await _UserCollection.InsertOneAsync(document);
                    }
                }
            }

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
