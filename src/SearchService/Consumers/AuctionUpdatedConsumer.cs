using AutoMapper;
using Contracts;
using MassTransit;
using SearchService.Data;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionUpdatedConsumer(IMapper mapper, SearchDbContext dbContext) : IConsumer<AuctionUpdated>
    {
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            var existingItem = await dbContext.Items.FindAsync(context.Message.Id);

            if (existingItem != null)
            {
                var item = mapper.Map(context.Message, existingItem);

                dbContext.Items.Update(item);
                
                var result = await dbContext.SaveChangesAsync() > 0;

                if (!result) 
                    throw new MessageException(typeof(AuctionUpdated), "-->Problem updating mongodb");
            }
            else
            {
                throw new MessageException(typeof(AuctionUpdated), $"-->Problem: Unable to find id {context.Message.Id}");
            }
        }
    }
}
