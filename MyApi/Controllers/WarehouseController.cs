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




            using (var connection = new SqlConnection("Server=127.0.0.1,1433;Database=DatabaseQWERTY;user id=sa;password=Dduppaa!;TrustServerCertificate=True;"))
            {
                bool? isEmpty = (bool?)connection.ExecuteScalar("SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM [Products]) THEN 0 ELSE 1 END AS BIT)");

                if (isEmpty is true)
                {
                    var goodProducts = GetData<Product>(".\\Products.csv", ";",true, p =>
                    {
                        var value = Regex.Match(p.shipping, "(\\d\\d)h").Groups[1].Value;
                        bool success = int.TryParse(value, out int number);
                        return p.is_wire is false && success && number <= 24;
                    });
                    await connection.ExecuteAsync(
                        @"INSERT INTO [Products] (Id, Sku, Name, EAN, Producer_name, Category, Is_wire, Shipping, Available, Is_vendor, Default_image)
                                    VALUES (@ID, @SKU, @name, @EAN, @producer_name, @category, @is_wire, @shipping, @available, @is_vendor, @default_image)",
                        goodProducts).ConfigureAwait(false);
                }
            }


            using (var connection = new SqlConnection("Server=127.0.0.1,1433;Database=DatabaseQWERTY;user id=sa;password=Dduppaa!;TrustServerCertificate=True;"))
            {
                bool? isEmpty = (bool?)connection.ExecuteScalar("SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM [Stock]) THEN 0 ELSE 1 END AS BIT)");

                if (isEmpty is true)
                {
                    var goodInventory = GetData<Inventory>(".\\Inventory.csv", ",",true, p =>
                    {
                        var value = Regex.Match(p.shipping, "(\\d\\d)h").Groups[1].Value;
                        bool success = int.TryParse(value, out int number);
                        return success && number <= 24;
                    });
                    //nie wszystko trzeba do bazy cos do wyjebania bedzie
                    await connection.ExecuteAsync(
                        @"INSERT INTO [Stock] (Id, Sku, Unit, Qty, Manufacturer_name, Manufacturer_ref_num, Shipping, Shipping_cost)
                             VALUES (@product_id, @sku, @unit, @qty, @manufacturer_name, @manufacturer_ref_num, @shipping, @shipping_cost)",
                        goodInventory).ConfigureAwait(false);
                }
            }

            using (var connection = new SqlConnection("Server=127.0.0.1,1433;Database=DatabaseQWERTY;user id=sa;password=Dduppaa!;TrustServerCertificate=True;"))
            {
                bool? isEmpty = (bool?)connection.ExecuteScalar("SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM [Prices]) THEN 0 ELSE 1 END AS BIT)");

                if (isEmpty is true)
                {
                    var goodInventory = GetData<Prices>(".\\Prices.csv", ",",false, p => true);
                    //nie wszystko trzeba do bazy cos do wyjebania bedzie
                    await connection.ExecuteAsync(
                        @"INSERT INTO [Prices] (Id, Sku, Price, DiscountPrice, VatRate, LogisticUnitPrice)
                                       VALUES (@ID, @SKU, @price, @discountPrice, @vatRate, @logisticUnitPrice)",
                        goodInventory).ConfigureAwait(false);
                }
            }


            return Ok();
        }

        private List<T> GetData<T>(string fileCachePath, string delimeter,bool hasHeaderRecord, Predicate<T> isRequired)
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
        public IActionResult Get(string sku)
        {
            return Ok();
        }
    }

}
