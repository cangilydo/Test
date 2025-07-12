using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace ApiService.Handler.Products
{
    public interface IProduct
    {
        Task Process(Guid orderId);
    }
    public class ProductGenerate
    {
        public ConcurrentDictionary<string, Type> Products { get; set; } = new ConcurrentDictionary<string, Type>();

        public ProductGenerate()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var dicts = assembly.GetTypes()
                    .Where(type => typeof(IProduct).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    .ToDictionary(s => s.Name, x => x);

                foreach (var item in dicts)
                {
                    var desObj = item.Value.GetCustomAttribute<DescriptionAttribute>();
                    var typeName = desObj?.Description ?? item.Key;
                    Products.TryAdd(typeName, item.Value);
                }
            }

        }

        public IProduct CreateProduct(string type, IServiceScopeFactory serviceProvider)
        {
            var productType = Products.TryGetValue(type, out var product);
            var instance = productType ? Activator.CreateInstance(product, serviceProvider) : null;

            return instance == null ? null : (IProduct)instance;
        }
    }
}
