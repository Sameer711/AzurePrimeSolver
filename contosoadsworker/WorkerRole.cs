using System.Diagnostics;
using System.Net;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using PrimeSolverCommon;

namespace PrimeSolverWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private CloudQueue _primesQueue;
        //private CloudBlobContainer primesBlobContainer;
        private PrimeNumberCandidatesContext _db;
        //private CloudTable _tableContainer;

        public override void Run()
        {
            Trace.TraceInformation("PrimeSolverWorker entry point called");
            CloudQueueMessage msg = null;

            //todo - try http://stackoverflow.com/a/16739691/2343

            // To make the worker role more scalable, implement multi-threaded and 
            // asynchronous code. See:
            // http://msdn.microsoft.com/en-us/library/ck8bc5c6.aspx
            // http://www.asp.net/aspnet/overview/developing-apps-with-windows-azure/building-real-world-cloud-apps-with-windows-azure/web-development-best-practices#async
            while (true)
            {
                try
                {
                    // Retrieve a new message from the queue.
                    // A production app could be more efficient and scalable and conserve
                    // on transaction costs by using the GetMessages method to get
                    // multiple queue messages at a time. See:
                    // http://azure.microsoft.com/en-us/documentation/articles/cloud-services-dotnet-multi-tier-app-storage-5-worker-role-b/#addcode
                    msg = this._primesQueue.GetMessage();
                    if (msg != null)
                    {
                        ProcessQueueMessage(msg);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (StorageException e)
                {
                    if (msg != null && msg.DequeueCount > 5)
                    {
                        this._primesQueue.DeleteMessage(msg);
                        Trace.TraceError("Deleting poison queue item: '{0}'", msg.AsString);
                    }
                    Trace.TraceError("Exception in PrimeSolverWorker: '{0}'", e.Message);
                    System.Threading.Thread.Sleep(5000);
                }
            }
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

        private void ProcessQueueMessage(CloudQueueMessage msg)
        {
            Trace.TraceInformation("Processing queue message {0}", msg);

            // Queue message contains AdId.
            var numberToTest = int.Parse(msg.AsString);

            var primeNumberCandidate = _db.PrimeNumberCandidates.Find(numberToTest);
            if (primeNumberCandidate == null)
            {
                primeNumberCandidate = new PrimeNumberCandidate(numberToTest)
                /*throw new Exception($"numberToTest {numberToTest} not found, cannot check if prime")*/;
                _db.PrimeNumberCandidates.Add(primeNumberCandidate);
            }

            primeNumberCandidate.IsPrime = IsPrime(numberToTest);
            _db.SaveChanges();

            // Remove message from queue.
            this._primesQueue.DeleteMessage(msg);
        }

        // A production app would also include an OnStop override to provide for
        // graceful shut-downs of worker-role VMs.  See
        // http://azure.microsoft.com/en-us/documentation/articles/cloud-services-dotnet-multi-tier-app-storage-3-web-role/#restarts
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections.
            ServicePointManager.DefaultConnectionLimit = 12;

            // Read database connection string and open database.
            var dbConnString = CloudConfigurationManager.GetSetting("ContosoAdsDbConnectionString");
            _db = new PrimeNumberCandidatesContext(dbConnString);

            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse
                (RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            Trace.TraceInformation("Creating primes blob container");
            //var blobClient = storageAccount.CreateCloudBlobClient();
            //var tableClient = storageAccount.CreateCloudTableClient();

            //_tableContainer = tableClient.GetTableReference("primes");
            //primesBlobContainer = blobClient.GetContainerReference("primes");
            //if (_tableContainer.CreateIfNotExists())
            //{
                // Enable public access on the newly created "primes" container.
                //_tableContainer.SetPermissions(new TablePermissions(), new TableRequestOptions(new TableRequestOptions() {}));
            //}

            Trace.TraceInformation("Creating primes queue");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _primesQueue = queueClient.GetQueueReference("primes");
            _primesQueue.CreateIfNotExists();

            //Trace.TraceInformation("Storage initialized");
            return base.OnStart();
        }
    }
}
