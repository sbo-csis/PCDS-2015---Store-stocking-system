using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCDSWebsite.Models
{
    public class Store
    {
        public string Name { get; set; }
        public string SalesStatus { get; set; } //Used for mocking
        public List<Stock> StockList { get; set; }
        public string StockStatus { get; set; } //Used for mocking
        public int HistoricSalesSpeed { get; set; } //Used for mocking
        public int CurrentSalesSpeed { get; set; } //Used for mocking
        public string SalesTendency { get; set; } //Used for mocking
        public string ExpectedSalesReachedPercentage { get; set; }  //Used for mocking
    }
}