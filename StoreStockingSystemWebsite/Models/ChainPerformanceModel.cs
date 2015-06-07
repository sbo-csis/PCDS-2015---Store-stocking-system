using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCDSWebsite.Models
{
    public class ChainPerformanceModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<decimal> Values { get; set; }
        public List<decimal> AccPerValues { get; set; }
        public decimal TotalSales { get; set; }
    }

}