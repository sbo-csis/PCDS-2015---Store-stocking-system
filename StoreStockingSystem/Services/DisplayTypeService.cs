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

        public static int AddDisplayType(DisplayType display)
        {
            using (var context = new StoreStockingContext())
            {
                context.DisplayTypes.Add(display);
                context.SaveChanges();
                return display.Id;
            }
        }

        public static int AddDisplayType(string name, int capacity)
        {
            return AddDisplayType(new DisplayType
            {
                Name = name,
                Capacity = capacity
            });
        }
    }
}