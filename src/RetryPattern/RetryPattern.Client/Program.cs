using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using RetryPattern.Contracts;

namespace RetryPattern.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Trying to get Data");
            var service = new Service();

            var data = await service.GetData();

            Console.WriteLine(data.StatusCode.ToString());
            Console.ReadLine();
        }
    }

    public class Service
    {
        public async Task<HttpResponseMessage> GetData()
        {
            return await HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response => (int) response.StatusCode == 429)
                .RetryAsync(10)
                .ExecuteAsync(async () =>
                {
                    Console.WriteLine("Trying to fetch remote data...");

                    using var client = new HttpClient();
                    return await client.PostAsync("http://localhost:7071/api/Chaos",
                        new StringContent(JsonConvert.SerializeObject(new ChaosOptions
                        {
                            ProbabilityOfChaos = 0.9,
                            LatencyInMilliseconds = 1000,
                            ResponseStatusCode = 503
                        }), Encoding.UTF8, "application/json")
                    );
                });
        }
    }
}