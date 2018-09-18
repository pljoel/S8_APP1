using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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

        static bool _validateEntry(String entry)
        {
            Regex regex = new Regex("a|b|c|d", RegexOptions.Compiled);
            if (entry.Length == 1) // since we accept only a,b,c or d
            {
                if (regex.Match(entry).Success)
                    return true;
            }
            return false;

        }

        static HttpClient httpClient = new HttpClient();
        const string API_KEY = "A2D3-HTDG-MLU2-3AM5";

        static async Task<int> ConnectAsync(Auth auth)
        {
            int result = 0;

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "api/connect", auth);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<int>();
            } else
            {
                Console.WriteLine(response.ReasonPhrase);
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
            } else
            {
                Console.WriteLine(response.ReasonPhrase);
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
            } else
            {
                Console.WriteLine(response.ReasonPhrase);
            }

            return result;
        }

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();

            Console.WriteLine("Exiting... Press any key to close.");
            Console.ReadLine();
            return;
        }

        static Auth _login()
        {
            String user;
            String pass;
            Console.WriteLine("Username : ");
            user = Console.ReadLine();
            while (user.Length <= 0)
            {
                Console.WriteLine("Please enter a valid username : ");
                user = Console.ReadLine();
            }

            Console.WriteLine("Password : ");
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Console.ReadLine());
            pass = System.Convert.ToBase64String(plainTextBytes);
            while (pass.Length <= 0)
            {
                Console.WriteLine("Please enter a valid password : ");
                plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Console.ReadLine());
                pass= System.Convert.ToBase64String(plainTextBytes);
            }

            return new Auth
            {
                username = user,
                password = pass
            };
        }

        static async Task RunAsync()
        {
            //specify to use TLS 1.2 as default connection
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Console.WriteLine("Hi, tying to connect to the server...");

            httpClient.BaseAddress = new Uri("https://localhost:44364/");
            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(API_KEY);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                int userID = 0;
                int loginTry = 0;

               while (userID <= 0 && loginTry < 3)
                {
                    userID = await ConnectAsync(_login());
                    loginTry++;
                }

               if (loginTry >= 3)
                {
                    Console.WriteLine("Authentification failed !");
                    return;
                }

                Console.WriteLine("Connected! UserID : " + userID);

                IList<Poll> sondages = await GetPolls(userID);

                if(sondages.Count <= 0)
                {
                    Console.WriteLine("No polls available");
                    return;
                }
                String exit = "no";

                while (exit != "yes")
                {
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
                        QuestionId = -1,
                        Text = "GetFirstQuestion"
                    };

                    question = await GetQuestion(userID, question);

                    while (question != null)
                    {
                        Console.WriteLine("{0} - {1}", question.QuestionId, question.Text);
                   
                        String answer = Console.ReadLine();
                        while (!_validateEntry(answer))
                        {
                            Console.WriteLine("Wrong entry, please try again.");
                            Console.WriteLine("{0} - {1}", question.QuestionId, question.Text);
                            answer = Console.ReadLine();
                        }
                        question.Text = answer;
                        question = await GetQuestion(userID, question);
                    }

                    Console.WriteLine("Thanks for completing this poll!");
                    Console.WriteLine("Enter yes to exit or no to continue.");
                    exit = Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
