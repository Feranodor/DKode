using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Routing.Constraints;
using CsvHelper;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using CsvHelper.Configuration;
using MyApi.Controllers.Models;
using System.Data.SqlClient;
using Dapper;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        static readonly HttpClient client = new HttpClient();
        static readonly (string uri, string path)[] input =
        {
            ("https://rekturacjazadanie.blob.core.windows.net/zadanie/Products.csv",".\\Products.csv"),
            ("https://rekturacjazadanie.blob.core.windows.net/zadanie/Inventory.csv", ".\\Inventory.csv"),
            ("https://rekturacjazadanie.blob.core.windows.net/zadanie/Prices.csv", ".\\Prices.csv")
        };

        private readonly IDapperContext _context;

        public WarehouseController(IDapperContext context)
        {
            _context = context;
        }

        [HttpGet("firstStep")]
        public async Task<IActionResult> Get()
        {
            var downloadTasks = input.Select(x =>
                {
                    if (System.IO.File.Exists(x.path))
                    {
                        return Task.CompletedTask;
                    }
                    return Download(x.path, x.uri);
                })
                .ToArray();

            await Task.WhenAll(downloadTasks);




            using (var connection = _context.CreateConnection())
            {
                bool? isEmpty = (bool?)connection.ExecuteScalar("IsProductsTableEmpty");

                if (isEmpty is true)
                {
                    var goodProducts = GetData<Product>(".\\Products.csv", ";", true, p =>
                    {
                        var value = Regex.Match(p.shipping, "(\\d\\d)h").Groups[1].Value;
                        bool success = int.TryParse(value, out int number);
                        return p.is_wire is false && success && number <= 24;
                    });
                    await connection.ExecuteAsync("InsertProduct", goodProducts).ConfigureAwait(false);
                }
            }


            using (var connection = _context.CreateConnection())
            {
                bool? isEmpty = (bool?)connection.ExecuteScalar("IsStockTableEmpty");

                if (isEmpty is true)
                {
                    var goodInventory = GetData<Inventory>(".\\Inventory.csv", ",", true, p =>
                    {
                        var value = Regex.Match(p.shipping, "(\\d\\d)h").Groups[1].Value;
                        bool success = int.TryParse(value, out int number);
                        return success && number <= 24;
                    });
                    await connection.ExecuteAsync("InsertStock", goodInventory).ConfigureAwait(false);
                }
            }

            using (var connection = _context.CreateConnection())
            {
                bool? isEmpty = (bool?)connection.ExecuteScalar("IsPricesTableEmpty");

                if (isEmpty is true)
                {
                    var goodPrices = GetData<Prices>(".\\Prices.csv", ",", false, p => true);
                    await connection.ExecuteAsync("InsertPrice", goodPrices).ConfigureAwait(false);
                }
            }


            return Ok();
        }

        private List<T> GetData<T>(string fileCachePath, string delimeter, bool hasHeaderRecord, Predicate<T> isRequired)
        {
            using var reader = new StreamReader(fileCachePath);
            var bad = new List<string>();
            var isRecordBad = false;
            using var csv = new CsvReader(reader, new CsvConfiguration(new CultureInfo("de-DE"))
            {
                HasHeaderRecord = hasHeaderRecord,
                Delimiter = delimeter,
                BadDataFound = context =>
                {
                    isRecordBad = true;
                    bad.Add(context.RawRecord);
                }
            });

            var good = new List<T>();
            while (csv.Read())
            {
                T record = default;
                try
                {
                    record = csv.GetRecord<T>();

                }
                catch (Exception)
                {
                    //log
                    isRecordBad = true;
                }
                if (!isRecordBad && record != null && isRequired(record))
                {
                    good.Add(record);
                }

                isRecordBad = false;
            }

            return good;
        }



        private async Task Download(string filePath, string uri)
        {
            using HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            using Stream input = await response.Content.ReadAsStreamAsync();
            using Stream output = System.IO.File.OpenWrite(filePath);
            input.CopyTo(output);
        }


        [HttpGet("secondStep")]
        public async Task<IActionResult> Get(string sku)
        {
            using var connection = _context.CreateConnection();

            //1131-214YY-FF003
            var result = await connection.QueryAsync<Dtos.Product>("GetProduct", new { SKU = sku }).ConfigureAwait(false);

            if (result is not null && result.Any())
            {
                return Ok(result.First());
            }

            return NotFound();
        }
    }

}
