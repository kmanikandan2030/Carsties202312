using AutoMapper;
using Contracts;
using MassTransit;
//using MongoDB.Entities;
using SearchService.Data;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer(IMapper mapper, SearchDbContext dbContext) : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming auction created: " + context.Message.Id);

            var item = mapper.Map<Item>(context.Message);

            if (item.Model == "Foo") throw new ArgumentException("Cannot sell car with name of Foo");

            dbContext.Items.Add(item);
            await dbContext.SaveChangesAsync();
            //await item.SaveAsync();
        }
    }
}
