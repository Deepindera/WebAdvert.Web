using System.Threading.Tasks;
using AdvertApi.Models;
using WebAdvert.Web.Models.Advert;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<AdvertResponse> Create(CreateAdvertModel model);
        Task<bool> Confirm(ConfirmAdvertModelRequest model);
    }
}
