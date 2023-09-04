using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;
using WebLab.Services.BeerService;

namespace WebLab.Services.BeerTypeService
{
	public class ApiBeerTypeService : IBeerTypeService
	{
		HttpClient _httpClient;
		ILogger _logger;
		JsonSerializerOptions _serializerOptions;

		public ApiBeerTypeService(IHttpClientFactory httpClientFactory, ILogger<ApiBeerTypeService> logger)
		{
			_httpClient = httpClientFactory.CreateClient("API");
			_logger = logger;

			_serializerOptions = new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
		}

		public async Task<ResponseData<List<BeerType>>> GetBeerTypeListAsync()
		{
			var urlString = new StringBuilder($"{_httpClient.BaseAddress!.AbsoluteUri}beertypes");

			var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));
			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not recieved. Error {response.StatusCode}";
				_logger.LogError(errorMessage);

				return new ResponseData<List<BeerType>>(null!)
				{
					IsSuccess = false,
					ErrorMessage = errorMessage,
				};
			}

			var beerList = await response.Content.ReadFromJsonAsync<ResponseData<List<BeerType>>>(_serializerOptions);
			return beerList!;
		}
	}
}
