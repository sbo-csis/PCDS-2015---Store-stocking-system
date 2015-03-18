﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StoreStockingSystem.Models
{
    public class SalesPerson
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}