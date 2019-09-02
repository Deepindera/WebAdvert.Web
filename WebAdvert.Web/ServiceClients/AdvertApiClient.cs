using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebAdvert.Web.Models.Advert;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public AdvertApiClient(IConfiguration configuration, HttpClient httpClient, IMapper mapper)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _mapper = mapper;

            var createUrl = _configuration.GetSection("AdvertApi").GetValue<string>("Createurl");
            _httpClient.BaseAddress = new Uri(createUrl);
            _httpClient.DefaultRequestHeaders.Add("Content-type", "application/json");
        }

        public async Task<AdvertResponse> CreateAsync(CreateAdvertModel model)
        {
            var advertModel = _mapper.Map<AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertModel);
            var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/create", new StringContent(jsonModel));
            var responseJson = await response.Content.ReadAsStringAsync();
            var createAdvertResponse = JsonConvert.DeserializeObject<CreateAdvertResponse>(responseJson);
            var advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse);

            return advertResponse;
        }

        public async Task<bool> ConfirmAsync(ConfirmAdvertModelRequest model)
        {
            var confirmAdvertModel = _mapper.Map<ConfirmAdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(confirmAdvertModel);
            var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/confirm", new StringContent(jsonModel));

            return response.StatusCode == HttpStatusCode.OK;

        }
    }
}
