using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BlazorWasm.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using WebLab.Domain.Entities;
using WebLab.Domain.Models;

namespace BlazorWasm.Services
{
    public class DataService : IDataService
    {
        private HttpClient _httpClient;
        private string _apiUri;
        private int _itemsPerPage;
        private JsonSerializerOptions _serializerOptions;
        private IAccessTokenProvider _accessTokenProvider;

        public DataService(HttpClient httpClient, IConfiguration configuration, IAccessTokenProvider accessTokenProvider)
        {
            _httpClient = httpClient;
            _apiUri = configuration.GetValue<string>("ApiUri")!;
            _itemsPerPage = configuration.GetValue<int>("ItemsPerPage");

            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _accessTokenProvider = accessTokenProvider;
        }

        public List<BeerType> Categories { get; set; } = new List<BeerType>();

        public List<Beer> ObjectsList { get; set; } = new List<Beer>();

        public bool Success { get; set; }

        public string ErrorMessage { get; set; } = "";

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public event Action DataLoaded;

        public async Task GetCategoryListAsync()
        {
            var tokenRequest = await _accessTokenProvider.RequestAccessToken();
            if (tokenRequest.TryGetToken(out var token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Value);
                var response = await _httpClient.GetAsync(new Uri($"{_apiUri}beertypes/"));

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        ResponseData<List<BeerType>>? beerTypes = await response.Content.ReadFromJsonAsync<ResponseData<List<BeerType>>>(_serializerOptions);
                        Categories = beerTypes.Data;
                        DataLoaded?.Invoke();

                    }
                    catch (JsonException ex)
                    {

                        Success = false;
                        ErrorMessage = $"Ошибка: {ex.Message}";
                        return;

                    }
                }


                Success = false;
                ErrorMessage = $"Данные не получены от сервера. Error: {response.StatusCode}";
                return;
            }

        }

        public async Task<Beer?> GetProductByIdAsync(int id)
        {
            var tokenRequest = await _accessTokenProvider.RequestAccessToken();
            if (tokenRequest.TryGetToken(out var token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Value);
                var urlString = new StringBuilder($"{_apiUri}beers/{id}");


                var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = await response.Content.ReadFromJsonAsync<ResponseData<Beer>>(_serializerOptions);
                        DataLoaded?.Invoke();
                        return result.Data;

                    }
                    catch (JsonException ex)
                    {

                        Success = false;
                        ErrorMessage = $"Ошибка: {ex.Message}";
                        return null;
                    }
                }
                Success = false;
                ErrorMessage = $"Данные не получены от сервера. Error: {response.StatusCode}";
                return null;
            }
            return null;
        }

        public async Task GetProductListAsync(string? beerTypeNormalized, int pageNo = 1)
        {
            var tokenRequest = await _accessTokenProvider.RequestAccessToken();
            if (tokenRequest.TryGetToken(out var token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Value);
                var query = new QueryBuilder();
                var urlString = new StringBuilder($"{_apiUri}beers/");

                if (beerTypeNormalized != null)
                {
                    query.Add("beerType", beerTypeNormalized);
                }
                if (pageNo > 1)
                {
                    urlString.Append($"page{pageNo}/");
                }
                if (!_itemsPerPage.Equals(3))
                {
                    query.Add("pageSize", _itemsPerPage.ToString());
                }
                urlString.Append(query);

                var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = (await response.Content.ReadFromJsonAsync<ResponseData<ListModel<Beer>>>(_serializerOptions)).Data!;
                        ObjectsList = result.Items;
                        TotalPages = result.TotalPages;
                        CurrentPage = result.CurrentPage;
                        DataLoaded?.Invoke();

                    }
                    catch (JsonException ex)
                    {
                        Success = false;
                        ErrorMessage = $"Ошибка: {ex.Message}";
                        return;
                    }
                }

                Success = false;
                ErrorMessage = $"Данные не получены от сервера. Error: {response.StatusCode}";
                return;
            }

        }
    }
}
