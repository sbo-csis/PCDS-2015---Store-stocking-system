﻿using System.ComponentModel.DataAnnotations;

namespace StoreStockingSystem.Models
{
    public class Chain
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalId { get; set; }
    }
}