using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using ConsumeServices;

namespace WebServiceConsumer
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string UrlTestJson = "https://6822a35ab342dce8004ee31b.mockapi.io/products";

        static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===== Web Service Consumer Demo =====");
                Console.WriteLine("1. Get all products (JSON)");
                Console.WriteLine("2. Create product (JSON)");
                Console.WriteLine("3. Update product (JSON)");
                Console.WriteLine("4. Delete product (JSON)");
                Console.WriteLine("5. Get all products (XML)");
                Console.WriteLine("6. Create product (XML)");
                Console.WriteLine("7. Update product (XML)");
                Console.WriteLine("8. Delete product (XML)");
                Console.WriteLine("9. Exit");
                Console.Write("\nSelect an option: ");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await GetAllProductsJson();
                            break;
                        case "2":
                            await CreateProductJson();
                            break;
                        case "3":
                            await UpdateProductJson();
                            break;
                        case "4":
                            await DeleteProductJson();
                            break;
                        case "5":

                            break;
                        case "6":

                            break;
                        case "7":

                            break;
                        case "8":

                            break;
                        case "9":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }

                    if (!exit)
                    {
                        Console.WriteLine("\nPress any key to return to menu...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("\nPress any key to return to menu...");
                    Console.ReadKey();
                }
            }
        }

        static async Task GetAllProductsJson()
        {
            Console.WriteLine("===== JSON GET All Products =====");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(UrlTestJson);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(content);

            Console.WriteLine("\nProduct List:");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("| ID\t| Name\t\t\t| Price\t|");
            Console.WriteLine("-------------------------------------------------");

            foreach (var product in products)
            {
                Console.WriteLine($"| {product.id,-7}| {product.name,-23}| ${product.price,-7:F2}|");
            }
            Console.WriteLine("-------------------------------------------------");
        }

        static async Task CreateProductJson()
        {
            Console.WriteLine("===== JSON Create New Product =====");

            Console.Write("Enter product name: ");
            var name = Console.ReadLine();

            Console.Write("Enter product price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            var newProduct = new Product { name = name, price = price };
            var json = JsonSerializer.Serialize(newProduct);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync(UrlTestJson, content);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"JSON POST Status: {response.StatusCode}");
            Console.WriteLine("Product created successfully!");

            // Show updated list
            await GetAllProductsJson();
        }

        static async Task UpdateProductJson()
        {
            Console.WriteLine("===== JSON Update Product =====");

            // First get all products to show IDs
            await GetAllProductsJson();

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
            var json = JsonSerializer.Serialize(updateProduct);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PutAsync($"{UrlTestJson}/{id}", content);
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"JSON PUT Status: {response.StatusCode}");
            Console.WriteLine("Product updated successfully!");

            // Show updated list
            await GetAllProductsJson();
        }

        static async Task DeleteProductJson()
        {
            Console.WriteLine("===== JSON Delete Product =====");

            // First get all products to show IDs
            await GetAllProductsJson();

            Console.Write("\nEnter the ID of the product to delete: ");
            var id = Console.ReadLine();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.DeleteAsync($"{UrlTestJson}/{id}");
            response.EnsureSuccessStatusCode();

            Console.WriteLine($"JSON DELETE Status: {response.StatusCode}");
            Console.WriteLine("Product deleted successfully!");

            // Show updated list
            await GetAllProductsJson();
        }
    }
}
