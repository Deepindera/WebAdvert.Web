using AdvertApi.Models;

namespace WebAdvert.Web.Models.Advert
{
    public class ConfirmAdvertModelRequest
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public AdvertStatus Status { get; set; }
    }
}
