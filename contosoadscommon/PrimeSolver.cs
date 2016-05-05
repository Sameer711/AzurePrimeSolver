using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using PrimeSolverRepository;

namespace PrimeSolverCommon
{
    /// <summary>
    /// Prime Solver
    /// </summary>
    public class PrimeSolver
    {
        private PrimeNumbersRepository _repository;
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

        public IEnumerable<PrimeCandidateViewModel> Get(int numResults, bool onlyPrime)
        {
            var list = _repository.Get(numResults, onlyPrime);
            return list.Select(PrimeCandidateViewModel.FromEntity);
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
            var max = _repository.Any() ? _repository.GetMaxSolved() :1;
            return max;
        }

        public PrimeCandidateViewModel Get(int number)
        {
            var numberCandidate = _repository.Find(number);
            return PrimeCandidateViewModel.FromEntity(numberCandidate);
        }

        public void SolveForPrime(int number)
        {
            //_repository.Get().Add(primeNumber);
            //await _db.SaveChangesAsync();
            //Trace.TraceInformation("Created Prime {0} in database", primeNumber.Number);

            var primeToTest = number.ToString();
            var queueMessage = new CloudQueueMessage(primeToTest);
             _primesQueue.AddMessageAsync(queueMessage);
            Trace.TraceInformation("Created queue message for number {0}", primeToTest);
        }


        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);
            var dbConnString = CloudConfigurationManager.GetSetting("PrimeSolverDbConnectionString");

            _repository = new PrimeNumbersRepository(dbConnString);
            // Get a reference to the queue.
            _primesQueue = queueClient.GetQueueReference("primes");
        }

        public static bool IsPrime(int candidate)
        {
            // Test whether the parameter is a prime number.
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // Note:
            // ... This version was changed to test the square.
            // ... Original version tested against the square root.
            // ... Also we exclude 1 at the end.
            for (int i = 3; i * i <= candidate; i += 2)
            {
                if (candidate % i == 0)
                {
                    return false;
                }
            }
            return candidate != 1;
        }
    }
}
