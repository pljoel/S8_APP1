using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
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
            username = "admin",
            password = "admin"
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

        static async Task<IList<Poll>> GetPolls(int userID)
        {
            IList<Poll> result = null;

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["userId"] = userID.ToString();
            string queryString = query.ToString();

            HttpResponseMessage response = await httpClient.GetAsync("api/GetAvailablePolls?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<IList<Poll>>();
            }

            return result;
        }

        static async Task<PollQuestion> GetQuestion(int userId, PollQuestion question)
        {
            PollQuestion result = null;

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["userId"] = userId.ToString();
            string queryString = query.ToString();

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "api/GetNext?" + queryString, question);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<PollQuestion>();
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
                Console.WriteLine("Connected! UserID : " + userID);

                if(userID < 0)
                {
                    Console.WriteLine("Authentification failed!");
                    return;
                }

                IList<Poll> sondages = await GetPolls(userID);

                if(sondages.Count <= 0)
                {
                    Console.WriteLine("No polls available");
                    return;
                }

                Console.WriteLine("Available polls ({0}): ", sondages.Count);
                for (int i = 0; i < sondages.Count; i++)
                {
                    Console.WriteLine("{0}- {1}", sondages[i].Id, sondages[i].Description);
                }

                Console.WriteLine("Please enter number of the desired poll.");
                int desiredPoll = Int32.Parse(Console.ReadLine());
                while (desiredPoll < sondages[0].Id || desiredPoll > sondages[sondages.Count - 1].Id)
                {
                    Console.WriteLine("Please enter the valid number of the desired poll.");
                    desiredPoll = Int32.Parse(Console.ReadLine());
                }

                PollQuestion question = new PollQuestion
                {
                    PollId = desiredPoll,
                    QuestionId = -1
                };

                question = await GetQuestion(userID, question);

                while(question != null)
                {
                    Console.WriteLine("{0} - {1}", question.QuestionId, question.Text);
                    question.Text = Console.ReadLine();
                    question = await GetQuestion(userID, question);
                }

                Console.WriteLine("Thanks for completing this poll!");



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            Console.WriteLine("Hello World!");
        }
    }
}
