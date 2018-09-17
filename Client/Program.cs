using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using USherbrooke.ServiceModel.Sondage;

namespace Client
{
    class Program
    {
        static HttpClient httpClient = new HttpClient();
        const string API_KEY = "A2D3-HTDG-MLU2-3AM5"; // Need to generate a new one

        static async Task<IList<Poll>> GetSondages()
        {
            IList<Poll> result = null;

            HttpResponseMessage response = await httpClient.GetAsync("api/Sondages");
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<IList<Poll>>(); // wtf suppose to work
            }
            return result;
        }

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();

        }

        static async Task RunAsync()
        {
            //specify to use TLS 1.2 as default connection
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;



            httpClient.BaseAddress = new Uri("https://localhost:44364/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Need to check connection

            IList<Poll> sondages = await GetSondages();

            // maybe validate sondages ??

            Console.WriteLine("Hello World!");
        }
    }
}
