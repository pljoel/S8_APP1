using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using USherbrooke.ServiceModel.Sondage;

namespace Client
{
    class Auth
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    class Program
    {

        static HttpClient httpClient = new HttpClient();
        const string API_KEY = "A2D3-HTDG-MLU2-3AM5"; // Need to generate a new one
        static Auth auth = new Auth
        {
            username = "Carey",
            password = "Price"
        };

        static async Task<int> ConnectAsync()
        {
            int result = 0;

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "api/connect", auth);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<int>();
            }
            
            return result;
        }

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

            Console.WriteLine("Hi, tying to connect to the server...");

            httpClient.BaseAddress = new Uri("https://localhost:44364/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Need to check connection
            try
            {
               int  userID = await ConnectAsync();
                Console.WriteLine("UserID : " + userID);

               // IList<Poll> sondages = await GetSondages();
                // maybe validate sondages ??
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            Console.WriteLine("Hello World!");
        }
    }
}
