using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController(
    AuctionDbContext context,
    IMapper mapper,
    IPublishEndpoint publishEndpoint
    ) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await context.Auctions
        .Include(x => x.Item)
        .OrderBy(x => x.Item.Make)
        .ToListAsync();

        return mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> Create(CreateAuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);
        //TODO: add current user as seller
        auction.Seller = "test";

        context.Auctions.Add(auction);

        var newAuction = mapper.Map<AuctionDto>(auction);
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to the DB");        

        return CreatedAtAction(nameof(GetAuctionById),
        new { auction.Id }, newAuction);

    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        //TODO: Check seller == username

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

        var auctionDto = mapper.Map<AuctionDto>(auction);
        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auctionDto));

        var result = await context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Problem saving changes");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var auction = await context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        //TODO: check seller == username

        context.Auctions.Remove(auction);

        var auctionDto = mapper.Map<AuctionDto>(auction);
        await publishEndpoint.Publish(mapper.Map<AuctionDeleted>(auctionDto));

        var result = await context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Could not delete");
    }
}
