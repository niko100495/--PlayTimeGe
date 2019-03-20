using AutoMapper;
using WebApi.Dtos;
using WebApi.Dtos.ForRelationship;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<User, UserRelation>();
            CreateMap<UserRelation, User>();

            CreateMap<Console, ConsoleDto>();
            CreateMap<ConsoleDto, Console>();

            CreateMap<Promotion, PromotionDto>();
            CreateMap<PromotionDto, Promotion>();

            CreateMap<Magazine, MagazineDto>();
            CreateMap<MagazineDto, Magazine>();

            CreateMap<PromotionConsole, PromConsDto>();
            CreateMap<PromConsDto, PromotionConsole>();

            CreateMap<Trade, TradeDto>();
            CreateMap<TradeDto, Trade>();

            CreateMap<ConsolePlay, ConsolePlayDto>();
            CreateMap<ConsolePlayDto, ConsolePlay>();
        }
    }
}