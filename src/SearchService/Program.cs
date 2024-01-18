using MassTransit;
using Microsoft.EntityFrameworkCore;
using SearchService.Consumers;
using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var connStr = builder.Configuration.GetConnectionString("MongoDbConnection"); //"mongodb://localhost:27017";

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<SearchDbContext>(opt=>
{
    opt.UseMongoDB(connStr, "SearchDb");
});
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var searchDbContext = scope.ServiceProvider.GetRequiredService<SearchDbContext>();
    await DbInit.InitDb(searchDbContext,connStr);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}


app.Run();
