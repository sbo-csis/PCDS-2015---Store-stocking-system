using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class ProductService
    {
        public static Product GetProduct(int productId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return context.Products.Find(productId);
        }

        public static Product NewProduct(Product product, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Products.Add(product);
            context.SaveChanges();

            return product;
        }

        public static Product NewProduct(string name, int price, StoreStockingContext context = null)
        {
            return NewProduct(new Product() {Name = name, Price = price}, context);
        }

        public static void UpdateProduct(Product changedProduct, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var currentProduct = (from t in context.Products
                                    where t.Id == changedProduct.Id
                                    select t).FirstOrDefault();

            if (currentProduct != null)
            {
                currentProduct.Name = changedProduct.Name;
                currentProduct.Price = changedProduct.Price;
                context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Could not update product. Product id " + changedProduct.Id + " not found.");
            }
        }
    }
}