using System;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class ProductService
    {
        public static Product GetProduct(int productId)
        {
            using (var context = new StoreStockingContext())
            {
                return context.Products.Find(productId);
            }
        }

        public static int NewProduct(Product product)
        {
            using (var context = new StoreStockingContext())
            {
                context.Products.Add(product);
                context.SaveChanges();
                return product.Id;
            }
        }

        public static int NewProduct(string name, int price)
        {
            return NewProduct(new Product() {Name = name, Price = price});
        }

        public static void UpdateProduct(Product changedProduct)
        {
            using (var context = new StoreStockingContext())
            {
                var currentProduct = (from t in context.Products
                                      where t.Id == changedProduct.Id
                                      select t).FirstOrDefault();

                if (currentProduct != null)
                {
                    currentProduct = changedProduct;
                    context.SaveChanges();
                }
                else
                {
                    throw new ArgumentException("Could not update product. Product id " + changedProduct.Id + " not found.");
                }
            }
        }
    }
}