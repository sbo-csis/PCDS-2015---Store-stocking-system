using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class DisplayTypeService
    {
        public static DisplayType GetDisplayType(int displayTypeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return  (from t in context.DisplayTypes
                     where t.Id == displayTypeId
                     select t).FirstOrDefault();
        }

        public static DisplayType AddDisplayType(DisplayType display, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.DisplayTypes.Add(display);
            context.SaveChanges();
            return display;
        }

        public static DisplayType AddDisplayType(string name, int capacity, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return AddDisplayType(new DisplayType
            {
                Name = name,
                Capacity = capacity
            }, context);
        }
    }
}