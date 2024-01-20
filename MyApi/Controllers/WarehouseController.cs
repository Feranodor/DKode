using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Routing.Constraints;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string[] filePaths =
        {
            "https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv",
            "https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv",
            "https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv"
        };


        [HttpGet("firstStep")]
        public async Task<IActionResult> Get()
        {
            var uriPaths = filePaths.Select(x => new Uri(x))
                .Select(uri => (uri: uri, path: $".\\{Path.GetFileName(uri.LocalPath)}"))
                .ToArray();

            var downloadTasks = uriPaths.Select(x =>
                {
                    if (System.IO.File.Exists(x.path))
                    {
                        return Task.CompletedTask;
                    }
                    return Download(x.path, x.uri);
                })
                .ToArray();

            await Task.WhenAll(downloadTasks);






            return Ok();
        }

        private async Task Download(string filePath, Uri uri)
        {
            using HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            using Stream input = await response.Content.ReadAsStreamAsync();
            using Stream output = System.IO.File.OpenWrite(filePath);
            input.CopyTo(output);
        }


        [HttpGet("secondStep")]
        public IActionResult Get(string sku)
        {
            return Ok();
        }
    }
}
