using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace RetryPattern
{
    public class ChaosOptions
    {
        private float _probabilityOfChaos;

        public ChaosOptions()
        {
            LatencyInMilliseconds = 0;
            ProbabilityOfChaos = 1;
            ResponseStatusCode = 500;
        }

        public int LatencyInMilliseconds { get; set; }

        public float ProbabilityOfChaos
        {
            get => _probabilityOfChaos;
            set =>
                _probabilityOfChaos = value > 1 
                    ? 1
                    : value < 0 
                        ? 0
                        : value;
        }

        public int ResponseStatusCode { get; set; }

        public bool Apply() => new Random(DateTime.UtcNow.Millisecond).NextDouble() < ProbabilityOfChaos;
    }

    public static class ChaosFunction
    {
        [FunctionName("Chaos")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] ChaosOptions chaos,
            ILogger log)
        {
            log.LogInformation($"Chaos function called with probability of {chaos.ProbabilityOfChaos}.");

            var apply = chaos.Apply();

            if (apply)
            {
                log.LogWarning($"Applying delay of {chaos.LatencyInMilliseconds} milliseconds.");
                await Task.Delay(chaos.LatencyInMilliseconds);

                log.LogWarning($"Returning status code {chaos.ResponseStatusCode}.");
                return new StatusCodeResult(chaos.ResponseStatusCode);
            }

            log.LogInformation("No delay was applied.");
            log.LogInformation("Returning success status code");
            return new OkResult();
        }
    }
}
