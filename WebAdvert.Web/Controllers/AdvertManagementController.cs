using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Advert;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly  IFileUploader _fileUploader;

        public AdvertManagementController(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async  Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if(ModelState.IsValid)
            {
                var id = "1111";

                if(imageFile != null)
                {
                    var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    var filePath = $"{id}/{fileName}";

                    try
                    {
                        using(var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream).ConfigureAwait(false);

                            if(!result)
                            {
                                throw new Exception("Could not upload file");
                            }
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return View(model);
        }

    }
}
