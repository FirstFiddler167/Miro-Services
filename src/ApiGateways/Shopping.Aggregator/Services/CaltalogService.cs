using Shopping.Aggregator.Extensions;
using Shopping.Aggregator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services
{
    public class CaltalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;

        public CaltalogService(HttpClient httpClient) =>
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        public async Task<IEnumerable<CatalogModel>> GetCatalog() =>
            await (await _httpClient.GetAsync("/api/v1/Catalog")).ReadContentAs<List<CatalogModel>>();
        public async Task<CatalogModel> GetCatalog(string id) =>
            await (await _httpClient.GetAsync($"/api/v1/Catalog/{id}")).ReadContentAs<CatalogModel>();
        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category) =>
             await (await _httpClient.GetAsync($"/api/v1/Catalog/GetProductByCategory/{category}")).ReadContentAs<IEnumerable<CatalogModel>>();
    }
}
