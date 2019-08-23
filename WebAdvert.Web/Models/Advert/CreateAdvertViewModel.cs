using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Advert
{
    public class CreateAdvertViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "price is required")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
    }
}
