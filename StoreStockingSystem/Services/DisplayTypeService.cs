using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class DisplayTypeService
    {
        public static DisplayType GetDisplayType(int displayTypeId)
        {
            using (var context = new StoreStockingContext())
            {
                var displayType = (from t in context.DisplayTypes
                                   where t.Id == displayTypeId
                                   select t).FirstOrDefault();

                return displayType;
            }
        }

        public static DisplayType AddDisplayType(string name, int capacity)
        {
            using (var context = new StoreStockingContext())
            {
                var displayType = (new DisplayType
                {
                    Name = name,
                    Capacity = capacity
                });
                context.DisplayTypes.Add(displayType);
                context.SaveChanges();
                return displayType;
            }
        }
    }
}