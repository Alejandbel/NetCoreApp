using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
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
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				IncludeFields = true,
			};

		}
		public async Task<ResponseData<Beer>> CreateBeerAsync(Beer beer, IFormFile? formFile)
		{
			var urlString = $"{_httpClient.BaseAddress!.AbsoluteUri}beers";
			var uri = new Uri(urlString);

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

            if (formFile != null && data != null)
            {
                await SaveImageAsync(data.Data.Id, formFile);
            }

            return data!;
		}

		public async Task DeleteBeerAsync(int id)
		{
			var urlString = $"{_httpClient.BaseAddress!.AbsoluteUri}beers/{id}";
			var uri = new Uri(urlString);

			var response = await _httpClient.DeleteAsync(uri);
			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not deleted. Error: {response.StatusCode}";
				_logger.LogError(errorMessage);
			}
		}

		public async Task<ResponseData<ListModel<Beer>>> GetBeerListAsync(string? beerTypeNormalized, int pageNo = 1, int pageSize = -1)
		{
			pageSize = pageSize == -1 ? _pageSize : pageSize;
			var urlString = new StringBuilder($"{_httpClient.BaseAddress!.AbsoluteUri}beers");
			if (pageNo > 1)
			{
				urlString.Append($"/page{pageNo}");
			};

			var query = new List<KeyValuePair<string, string?>>();
			if (pageSize != 3)
			{
				query.Add(new("pageSize", pageSize.ToString()));
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
			var urlString = $"{_httpClient.BaseAddress!.AbsoluteUri}beers/{id}";
			var uri = new Uri(urlString);


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
			var urlString = $"{_httpClient.BaseAddress!.AbsoluteUri}beers/{id}";
			var uri = new Uri(urlString);

			var response = await _httpClient.PutAsJsonAsync(uri, beer, _serializerOptions);

			if (!response.IsSuccessStatusCode)
			{
				var errorMessage = $"Object not updated. Error {response.StatusCode}";
				_logger.LogError(errorMessage);
			}

			if (formFile != null)
			{
                await SaveImageAsync(id, formFile);
            }
        }

		private async Task SaveImageAsync(int id, IFormFile image)
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri($"{_httpClient.BaseAddress!.AbsoluteUri}beers/{id}"),
			};
			var content = new MultipartFormDataContent();
			var streamContent = new StreamContent(image.OpenReadStream());
			content.Add(streamContent, "formFile", image.FileName);
			request.Content = content;
			await _httpClient.SendAsync(request);
		}
	}
}

