using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;
using WebLab.Services.BeerTypeService;

namespace WebLab.Services.BeerService
{
	public class ApiBeerService : IBeerService
	{
		HttpClient _httpClient;

		JsonSerializerOptions _serializerOptions;
		ILogger _logger;


		int _pageSize;

		public ApiBeerService(IHttpClientFactory httpClientFactory, [FromServices] IConfiguration config, ILogger<ApiBeerService> logger)
		{
			_pageSize = int.Parse(config["BeerPageSize"]!);
			_httpClient = httpClientFactory.CreateClient("API");
			_logger = logger;

			_serializerOptions = new JsonSerializerOptions()
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

		}
		public async Task<ResponseData<Beer>> CreateBeerAsync(Beer beer, IFormFile? formFile)
		{
			var uri = new UriBuilder(_httpClient.BaseAddress!.ToString(), "beers").Uri;

			var response = await _httpClient.PostAsJsonAsync(uri, beer, _serializerOptions);

			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not created. Error: {response.StatusCode}";
				_logger.LogError(errorMessage);

				return new ResponseData<Beer>(null!)
				{
					IsSuccess = false,
					ErrorMessage = errorMessage,
				};

			}

			var data = await response.Content.ReadFromJsonAsync<ResponseData<Beer>>(_serializerOptions);
			return data!;
		}

		public async Task DeleteBeerAsync(int id)
		{
			var uri = new UriBuilder(_httpClient.BaseAddress!.AbsoluteUri, $"beers/{id}").Uri;

			var response = await _httpClient.DeleteAsync(uri);
			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not deleted. Error: {response.StatusCode}";
				_logger.LogError(errorMessage);
			}
		}

		public async Task<ResponseData<ListModel<Beer>>> GetBeerListAsync(string? beerTypeNormalized, int pageNo = 1)
		{
			var urlString = new StringBuilder($"{_httpClient.BaseAddress!.AbsoluteUri}beers");
			if (pageNo > 1)
			{
				urlString.Append($"/page{pageNo}");
			};
			
			var query = new List<KeyValuePair<string, string?>>();
			if (_pageSize != 3)
			{
				query.Add(new("pageSize", _pageSize.ToString()));
			}
			if (beerTypeNormalized != null)
			{
				query.Add(new("beerType", beerTypeNormalized));
			};

			if (query.Count > 0)
			{
				urlString.Append(QueryString.Create(query));
			}

			var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));
			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not recieved. Error {response.StatusCode}";
				_logger.LogError(errorMessage);

				return new ResponseData<ListModel<Beer>>(null!)
				{
					IsSuccess = false,
					ErrorMessage = errorMessage,
				};
			}

			var beerList = await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Beer>>>(_serializerOptions);
			return beerList!;

		}


		public async Task<ResponseData<Beer>> GetBeerByIdAsync(int id)
		{
			var uri = new UriBuilder(_httpClient.BaseAddress!.ToString(), "beers").Uri;

			var response = await _httpClient.GetAsync(uri);

			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not recieved. Error {response.StatusCode}";
				_logger.LogError(errorMessage);

				return new ResponseData<Beer>(null!)
				{
					IsSuccess = false,
					ErrorMessage = errorMessage,
				};

			}

			var data = await response.Content.ReadFromJsonAsync<ResponseData<Beer>>(_serializerOptions);
			return data!;
		}

		public async Task UpdateBeerAsync(int id, Beer beer, IFormFile? formFile)
		{
			var uri = new UriBuilder(_httpClient.BaseAddress!.ToString(), "beers").Uri;

			var response = await _httpClient.PutAsJsonAsync(uri, beer, _serializerOptions);

			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not updated. Error {response.StatusCode}";
				_logger.LogError(errorMessage);
			}
		}
	}
}

