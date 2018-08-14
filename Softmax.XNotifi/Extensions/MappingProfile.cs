using AutoMapper;
using Softmax.XNotifi.Data.Entities;
using Softmax.XNotifi.Models;

namespace Softmax.XNotifi.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Gateway, GatewayModel>().ReverseMap();
            CreateMap<Application, ApplicationModel>().ReverseMap();
            CreateMap<Client, ClientModel>().ReverseMap();
            CreateMap<Request, RequestModel>().ReverseMap();
            CreateMap<Payment, PaymentModel>().ReverseMap();
        }
    }
}
