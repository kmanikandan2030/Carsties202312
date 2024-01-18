﻿using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{

    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>()
        .ForMember(dest => dest.Item, option => option.MapFrom(src => src));
        CreateMap<CreateAuctionDto, Item>();

        CreateMap<AuctionDto, AuctionCreated>();
        CreateMap<AuctionDto, AuctionUpdated>();
        CreateMap<AuctionDto, AuctionDeleted>();
    }

}
