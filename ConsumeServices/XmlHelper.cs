using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ConsumeServices
{
    // Create a class to help with XML serialization/deserialization
    [XmlRoot("Products")]
    public class ProductList
    {
        [XmlElement("Product")]
        public List<Product> Products { get; set; }
    }

    public static class XmlHelper
    {
        // URL for the XML API endpoint - you might need to adjust this
        private static readonly string UrlTestXml = "https://xml-product-api.wiremockapi.cloud/products";
        private static readonly HttpClient client = new HttpClient();

        // Serialize object to XML string
        public static string SerializeToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    serializer.Serialize(xmlWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        // Deserialize XML string to object
        public static T DeserializeFromXml<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }

        // Get all products (XML)
        public static async Task GetAllProductsXml()
        {
            Console.WriteLine("===== XML GET All Products =====");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var response = await client.GetAsync(UrlTestXml);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // If the API returns a list directly, we need to wrap it in a root element
            if (!content.TrimStart().StartsWith("<"))
            {
                content = $"<Products>{content}</Products>";
            }

            // Try to deserialize as a product list or as a single product
            List<Product> products;
            try
            {
                var productList = DeserializeFromXml<ProductList>(content);
                products = productList.Products;
            }
            catch
            {
                // If the API returns JSON even when asking for XML, fall back to JSON deserialization
                products = System.Text.Json.JsonSerializer.Deserialize<List<Product>>(content);
            }

            Console.WriteLine("\nProduct List (XML):");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("| ID\t| Name\t\t\t| Price\t|");
            Console.WriteLine("-------------------------------------------------");

            foreach (var product in products)
            {
                Console.WriteLine($"| {product.id,-7}| {product.name,-23}| ${product.price,-7:F2}|");
            }
            Console.WriteLine("-------------------------------------------------");
        }

        // Create product (XML)
        public static async Task CreateProductXml()
        {
            Console.WriteLine("===== XML Create New Product =====");


            var newProduct = new Product {id="3" ,name = "New Product", price = 123 };
            var xml = SerializeToXml(newProduct);
            var content = new StringContent(xml, Encoding.UTF8, "application/xml");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var response = await client.PostAsync(UrlTestXml, content);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"XML POST Status: {response.StatusCode}");
            Console.WriteLine("Product created successfully!");

            // Show updated list
            await GetAllProductsXml();
        }

        // Update product (XML)
        public static async Task UpdateProductXml()
        {
            Console.WriteLine("===== XML Update Product =====");

            // First get all products to show IDs
            await GetAllProductsXml();

            Console.Write("\nEnter the ID of the product to update: ");
            var id = Console.ReadLine();

            Console.Write("Enter new product name: ");
            var name = Console.ReadLine();

            Console.Write("Enter new product price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            var updateProduct = new Product { id = id, name = name, price = price };
            var xml = SerializeToXml(updateProduct);
            var content = new StringContent(xml, Encoding.UTF8, "application/xml");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var response = await client.PutAsync($"{UrlTestXml}/{id}", content);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"XML PUT Status: {response.StatusCode}");
            Console.WriteLine("Product updated successfully!");

            // Show updated list
            await GetAllProductsXml();
        }

        // Delete product (XML)
        public static async Task DeleteProductXml()
        {
            Console.WriteLine("===== XML Delete Product =====");

            // First get all products to show IDs
            await GetAllProductsXml();

            Console.Write("\nEnter the ID of the product to delete: ");
            var id = Console.ReadLine();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var response = await client.DeleteAsync($"{UrlTestXml}/{id}");
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"XML DELETE Status: {response.StatusCode}");
            Console.WriteLine("Product deleted successfully!");

            // Show updated list
            await GetAllProductsXml();
        }
    }
}