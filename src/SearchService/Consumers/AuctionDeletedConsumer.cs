using AutoMapper;
using Contracts;
using MassTransit;
using SearchService.Data;

namespace SearchService.Consumers
{
    public class AuctionDeletedConsumer(SearchDbContext dbContext) : IConsumer<AuctionDeleted>
    {
        public async  Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            var existingItem = await dbContext.Items.FindAsync(context.Message.Id);

            if (existingItem != null)
            {                
                dbContext.Items.Remove(existingItem);

                var result = await dbContext.SaveChangesAsync() > 0;

                if (!result)
                    throw new MessageException(typeof(AuctionUpdated), "-->Problem deleting mongodb");
            }
            else
            {
                throw new MessageException(typeof(AuctionUpdated), $"-->Problem: Unable to find id {context.Message.Id}");
            }
        }
    }
}
