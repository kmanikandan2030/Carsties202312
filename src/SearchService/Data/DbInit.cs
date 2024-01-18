using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;



namespace SearchService.Data;

public class DbInit
{
  public static async Task InitDb(SearchDbContext searchDbContext, string connStr)
  {

        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(connStr));

        await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text)
        .CreateAsync();

        /*if (searchDbContext.Items.Count() == 0)
        {
            var itemData = await File.ReadAllTextAsync("Data/auctions.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            await searchDbContext.Items.AddRangeAsync(items);
            await searchDbContext.SaveChangesAsync();
        }*/


        
        /* await DB.InitAsync("SearchDb", MongoClientSettings
         .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

         await DB.Index<Item>()
         .Key(x => x.Make, KeyType.Text)
         .Key(x => x.Model, KeyType.Text)
         .Key(x => x.Color, KeyType.Text)
         .CreateAsync();

         var count = await DB.CountAsync<Item>();

         if (count == 0)
         {
           System.Console.WriteLine("No data - will attempt to seed");
           var itemData = await File.ReadAllTextAsync("Data/auctions.json");
           var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
           var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

           await DB.SaveAsync(items);
         }*/

    }
}
