using System;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class ProductService
    {
        /// <summary>
        /// Get populated product object.
        /// </summary>
        /// <param name="productId">Product ID.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Product GetProduct(int productId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return context.Products.Find(productId);
        }

        /// <summary>
        /// Add new product to database. Returns the added product object.
        /// </summary>
        /// <param name="product">Product object.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Product NewProduct(Product product, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Products.Add(product);
            context.SaveChanges();

            return product;
        }

        /// <summary>
        /// Add new product to database. Returns the added product object.
        /// </summary>
        /// <param name="name">Product name.</param>
        /// <param name="price">Product price.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Product NewProduct(string name, int price, StoreStockingContext context = null)
        {
            return NewProduct(new Product {Name = name, Price = price}, context);
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="changedProduct">Updated product object.</param>
        /// <param name="context">Optional database context.</param>
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