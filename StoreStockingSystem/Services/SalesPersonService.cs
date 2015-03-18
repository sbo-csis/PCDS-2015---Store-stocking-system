﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class SalesPersonService
    {
        public static SalesPerson AddSalesPerson(SalesPerson person)
        {
            using (var context = new StoreStockingContext())
            {
                context.SalesPersons.Add(person);
                context.SaveChanges();
                return person;
            }
        }

        public static SalesPerson AddSalesPerson(string name)
        {
            return AddSalesPerson(new SalesPerson() {Name = name});
        }
    }
}