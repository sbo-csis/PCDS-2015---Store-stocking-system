﻿using System.Collections.Generic;
using StoreStockingSystem.Models;

namespace PCDSWebsite.Models
{
    public class ChainViewModel
    {
        public Chain Chain { get; set; }
        public List<StoreStockingSystem.Models.Store> Stores { get; set; }
        public ChainPerformanceModel Performance { get; set; }
        
    
    }
}