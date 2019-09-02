using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Advert;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;
using AutoMapper;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertApiClient;
        private readonly IMapper _mapper;

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper)
        {
            _fileUploader = fileUploader;
            _advertApiClient = advertApiClient;
            _mapper = mapper;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {

                var createAdvertModel = _mapper.Map<CreateAdvertModel>(model);

                var advertResponse = await _advertApiClient.CreateAsync(createAdvertModel);
                var id = advertResponse.Id;

                if (imageFile != null)
                {
                    var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    var filePath = $"{id}/{fileName}";

                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream).ConfigureAwait(false);

                            if (!result)
                            {
                                throw new Exception("Could not upload file");
                            }
                        }

                        var confirmModel = new ConfirmAdvertModelRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Active
                        };

                        var canConfirm = await _advertApiClient.ConfirmAsync(confirmModel);

                        if (!canConfirm)
                        {
                            throw new Exception($"Cannot confirm advert of id = {id}");
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception e)
                    {
                        var confirmModel = new ConfirmAdvertModelRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Pending
                        };

                        await _advertApiClient.ConfirmAsync(confirmModel);

                    }
                }
            }

            return View(model);
        }

    }
}
