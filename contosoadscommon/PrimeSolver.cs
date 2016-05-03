using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace PrimeSolverCommon
{
    /// <summary>
    /// Prime Solver
    /// </summary>
    public class PrimeSolver
    {
        private readonly PrimeNumberCandidatesContext _db = new PrimeNumberCandidatesContext();
        private CloudQueue _primesQueue;

        private static PrimeSolver Instance { get; set; }
        private PrimeSolver()
        {
            InitializeStorage();
        }

        public static PrimeSolver GetPrimeSolver()
        {
            return Instance ?? (Instance = new PrimeSolver());
        }

        public IEnumerable<PrimeNumberCandidate> Get()
        {
            var list = _db.PrimeNumberCandidates;
            return list;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public int GetMaxSolved()
        {
            var max = _db.PrimeNumberCandidates.Any() ? _db.PrimeNumberCandidates.Max(p => p.Number) : 0;
            return max;
        }

        public PrimeNumberCandidate Get(int number)
        {
            var numberCandidate = _db.PrimeNumberCandidates.FirstOrDefault(p=>p.Number == number);
            return numberCandidate;
        }

        public void SolveForPrime(PrimeNumberCandidate primeNumber)
        {
            //_db.PrimeNumberCandidates.Add(primeNumber);
            //await _db.SaveChangesAsync();
            //Trace.TraceInformation("Created Prime {0} in database", primeNumber.Number);

            var primeToTest = primeNumber.Number.ToString();
            var queueMessage = new CloudQueueMessage(primeToTest);
             _primesQueue.AddMessageAsync(queueMessage);
            Trace.TraceInformation("Created queue message for number {0}", primeToTest);
        }

        public void SolveForPrime(int number)
        {
            var primeNumber = new PrimeNumberCandidate(number);
            SolveForPrime(primeNumber);
        }

        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            _primesQueue = queueClient.GetQueueReference("primes");
        }

    }
}
