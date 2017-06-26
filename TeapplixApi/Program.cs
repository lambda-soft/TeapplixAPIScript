using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;

namespace TeapplixApi
{
    class Program
    {
        static string TEAPPLIXAPIKEY = ConfigurationManager.AppSettings["TEAPPLIXAPIKEY"];
        const string BASEURI = "https://api.teapplix.com/api2";

        static void Main(string[] args)
        {
            GetProducts();
            //GetProductBySKU("094664027312D");
            //UpdateQuantity("094664027312dD",16);

            Console.ReadLine();
        }

        static async void GetProducts()
        {
            var client = new HttpClient();
            var uri = new Uri(BASEURI + @"/Product");

            client.DefaultRequestHeaders.Add("APIToken", TEAPPLIXAPIKEY);

            var response = await client.GetAsync(uri);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);
        }

        static async void GetProductBySKU(string sku)
        {
            var client = new HttpClient();
            var query = @"/Product?ItemName="+sku;
            var uri = new Uri(BASEURI + query);

            client.DefaultRequestHeaders.Add("APIToken", TEAPPLIXAPIKEY);

            var response = await client.GetAsync(uri);
            

            var result = await response.Content.ReadAsStringAsync();


            Console.WriteLine(result);

            dynamic obj = JsonConvert.DeserializeObject(result);
            byte[] byteArray = Convert.FromBase64String((string)obj.Products[0].ShortDescriptionBase64);
            string desc = Encoding.UTF8.GetString(byteArray);

            Console.WriteLine(desc);
        }

        static async void  UpdateQuantity(string sku, int qty)
        {
            var client = new HttpClient();
            var uri = new Uri(BASEURI + @"/ProductQuantity");

            client.DefaultRequestHeaders.Add("APIToken", TEAPPLIXAPIKEY);

            var body = new
            {
                Quantities = new[] {
                    new {
                        ItemName = sku,
                        Quantity = qty,
                        PostType = "in-stock",
                        PostDate = DateTime.Now.ToShortDateString(),
                        PostComment = "Updatd via API"
                    }
                },
                Cleanup = false,
                ProductCrossReference = "reject"
                
            };
            string jsonbody = JsonConvert.SerializeObject(body);
            StringContent bodycontent = new StringContent(jsonbody);
            bodycontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(uri, bodycontent);
            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Status code: {response.StatusCode} \n {result}");
        }
    }
}
