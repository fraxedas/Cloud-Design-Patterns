using System;

namespace RetryPattern.Contracts
{
    public class ChaosOptions
    {
        private double _probabilityOfChaos;

        public ChaosOptions()
        {
            LatencyInMilliseconds = 0;
            ProbabilityOfChaos = 1;
            ResponseStatusCode = 500;
        }

        public int LatencyInMilliseconds { get; set; }

        public double ProbabilityOfChaos
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
}
