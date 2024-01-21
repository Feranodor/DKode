using CsvHelper.Configuration.Attributes;

namespace MyApi.Controllers.Models
{
    internal class Prices
    {

        [Index(0)]
        public string? ID { get; set; }
        [Index(1)]
        public string? SKU { get; set; }
        [Index(2)]
        public decimal? price { get; set; }
        [Index(3)]
        public decimal? discountPrice { get; set; }
        [Index(4)]
        public decimal? vatRate { get; set; }
        [Index(5)]
        public decimal? logisticUnitPrice { get; set; }
    }
}
