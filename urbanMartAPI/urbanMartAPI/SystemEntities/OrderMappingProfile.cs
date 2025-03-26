using AutoMapper;
using urbanMartAPI.SystemEntities;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderItem, OrderItemResponse>();
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.RowVersion, opt => opt.MapFrom(src => src.RowVersion));
    }
}
