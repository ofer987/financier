using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Financier.Web.Identity
{
    public static class Constants
    {
        public static class App
        {
            public static string BaseUrl = "http://localhost:5002";
        }
    }

    [Authorize]
    public class AppController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        private HttpClient _httpClient = null;
        protected HttpClient HttpClient
        {
            get
            {
                if (_httpClient != null)
                {
                    return _httpClient;
                }

                return (_httpClient = _clientFactory.CreateClient());
            }
        }

        protected string UserEmail => this.User.Identity.Name;

        protected HttpClient GetClientForApp()
        {
            var client = _clientFactory.CreateClient();
            // TODO should this be called,
            // 1. Account-Name, or
            // 2. User-Email
            // ?????
            client.DefaultRequestHeaders.Add("Account-Name", UserEmail);

            return client;
            // var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5002/{internalRoute}");
        }

        public AppController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index(string route)
        {
            var client = GetClientForApp();
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5002/{route}");
            var response = await client.SendAsync(request);

            try
            {
                var message = response.EnsureSuccessStatusCode();
                var content = await message.Content.ReadAsStringAsync();

                var result = new ContentResult();
                result.Content = content;
                result.ContentType = "text/html";
                result.StatusCode = 200;

                return result;
            }
            catch (HttpRequestException exception)
            {
                var content = $"oops an error happened!\n{exception}";

                var result = new ContentResult();
                result.Content = content;
                result.ContentType = "text/plain";
                result.StatusCode = 400;

                return result;
            }
            catch (Exception exception)
            {
                var content = $"this should not have happened!\n{exception}";

                var result = new ContentResult();
                result.Content = content;
                result.ContentType = "text/plain";
                result.StatusCode = 500;

                return result;
            }
        }
    }
}
