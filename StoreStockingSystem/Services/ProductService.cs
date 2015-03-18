using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class ProductService
    {
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
    }
}