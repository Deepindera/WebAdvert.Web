using AdvertApi.Models;
using AutoMapper;
using WebAdvert.Web.Models.Advert;


namespace WebAdvert.Web.Models
{
    public class AdvertApiMapperProfile : Profile
    {
        public AdvertApiMapperProfile()
        {
            CreateMap<AdvertModel, CreateAdvertModel>().ReverseMap();
            CreateMap<CreateAdvertResponse, AdvertResponse>().ReverseMap();
            CreateMap<ConfirmAdvertModel,ConfirmAdvertModelRequest>().ReverseMap();
        }
    }
}
