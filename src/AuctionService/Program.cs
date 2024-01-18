using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using AuctionService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    // E-Mail Outbox concept - when the consumer is down.
    x.AddEntityFrameworkOutbox<AuctionDbContext>(outbox =>
    {
        outbox.QueryDelay = TimeSpan.FromSeconds(10);
        outbox.UsePostgres();
        outbox.UseBusOutbox();
    });

    // To handle error that occurred in the consumer.
    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction",false));

    x.UsingRabbitMq((context, cfg) =>
    {        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.MapControllers();
try
{
    DbInit.InitDb(app);
}
catch (Exception e)
{

    System.Console.WriteLine(e);
}
app.Run();
