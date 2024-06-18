using AutoMapper;
using FoodBank.CORE.Dtos;

public class FoodItemProfile : Profile
{
    public FoodItemProfile()
    {
        CreateMap<FoodItem, FoodItemDto>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<FoodItemDto, FoodItem>();
    }
}
