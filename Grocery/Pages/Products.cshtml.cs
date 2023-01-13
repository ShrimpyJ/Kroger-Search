using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Grocery.Models;

namespace Grocery.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<Product> Products = new List<Product>();
        public ProductsModel(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

        public async Task<string> GetToken()
        {
            var httpClient = _httpClientFactory.CreateClient("KrogerToken");
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "product.compact"),
            });
            var response = await httpClient.PostAsync(
                "connect/oauth2/token", data);

            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                dynamic? json = JsonConvert.DeserializeObject(str);
                string result = json["access_token"];
                HttpContext.Session.SetString("token", result);
                return result;
            }

            return null;
        }

        public async Task OnGet()
        {
            string locationId = "02600670";
            string term = Request.Query["productSearch"];
            string limit = Request.Query["limit"];

            var token = HttpContext.Session.GetString("token");
            if (token == null || token == "")
            {
                Console.WriteLine("No token, generating one...");
                token = await GetToken();
                if (token == null)
                {
                    Console.WriteLine("Error: could not acquire token");
                    return;
                }
            }

            var httpClient = _httpClientFactory.CreateClient("KrogerProduct");

            httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, "Bearer " + token);

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("filter.term", term),
                new KeyValuePair<string, string>("filter.locationId", locationId),
                new KeyValuePair<string, string>("filter.limit", limit),
            });
            string query = data.ReadAsStringAsync().Result;

            var response = await httpClient.GetAsync("products?" + query);

            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                dynamic? json = JsonConvert.DeserializeObject(str);
                foreach (var item in json["data"])
                {
                    Product product = new Product(item);
                    Products.Add(product);
                }
                Products = Products.OrderBy(x => x.PricePerUnit).ToList();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return;
            }
        }

        public async Task OnPost()
        {
            string locationId = "02600670";
            string term = Request.Form["productSearch"];
            string limit = Request.Form["limit"];

            var token = HttpContext.Session.GetString("token");
            if (token == null || token == "")
            {
                Console.WriteLine("No token, generating one...");
                token = await GetToken();
                if (token == null)
                {
                    Console.WriteLine("Error: could not acquire token");
                    return;
                }
            }

            var httpClient = _httpClientFactory.CreateClient("KrogerProduct");

            httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, "Bearer " + token);

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("filter.term", term),
                new KeyValuePair<string, string>("filter.locationId", locationId),
                new KeyValuePair<string, string>("filter.limit", limit),
            });
            string query = data.ReadAsStringAsync().Result;

            var response = await httpClient.GetAsync("products?" + query);

            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                dynamic? json = JsonConvert.DeserializeObject(str);
                foreach (var item in json["data"])
                {
                    Product product = new Product(item);
                    Products.Add(product);
                }
                Products = Products.OrderBy(x => x.PricePerUnit).ToList();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return;
            }
        }
    }
}