using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    /// <summary>
    /// Class for fetching and manipulating display objects. Should be used for getting displays and adding new displays to database.
    /// </summary>
    public static class DisplayTypeService
    {
        /// <summary>
        /// Gets display-type object.
        /// </summary>
        /// <param name="displayTypeId">Display type ID</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static DisplayType GetDisplayType(int displayTypeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return  (from t in context.DisplayTypes
                     where t.Id == displayTypeId
                     select t).FirstOrDefault();
        }

        /// <summary>
        /// Add new displaytype object to database. Returns the displaytype object after database insertion.
        /// </summary>
        /// <param name="display">Display object.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static DisplayType AddDisplayType(DisplayType display, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.DisplayTypes.Add(display);
            context.SaveChanges();
            return display;
        }

        /// <summary>
        /// Create new displaytype. Returns the displaytype object after database insertion.
        /// </summary>
        /// <param name="name">Displaytype name.</param>
        /// <param name="capacity">Displaytype capacity</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
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