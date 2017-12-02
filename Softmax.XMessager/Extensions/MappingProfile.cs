using AutoMapper;
using Softmax.XMessager.Data.Entities;
using Softmax.XMessager.Models;

namespace Softmax.XMessager.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Gateway, GatewayModel>().ReverseMap();
            CreateMap<Application, ApplicationModel>().ReverseMap();
            CreateMap<Client, ClientModel>().ReverseMap();
            CreateMap<Request, RequestModel>().ReverseMap();

        }
    }
}
