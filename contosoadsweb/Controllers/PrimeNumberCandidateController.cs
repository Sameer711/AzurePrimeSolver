using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using PrimeSolverCommon;

namespace PrimeSolverWeb.Controllers
{
    public class PrimeNumberCandidateController : Controller
    {
        private PrimeNumberCandidatesContext db = new PrimeNumberCandidatesContext();
        private CloudQueue primesQueue;
        private static CloudBlobContainer imagesBlobContainer;

        public PrimeNumberCandidateController()
        {
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            // Get context object for working with blobs, and 
            // set a default retry policy appropriate for a web user interface.
            //var blobClient = storageAccount.CreateCloudBlobClient();
            //blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the blob container.
            //imagesBlobContainer = blobClient.GetContainerReference("primes");

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            primesQueue = queueClient.GetQueueReference("primes");
        }

        // GET: PrimeNumberCandidate
        public async Task<ActionResult> Index()
        {
            // This code executes an unbounded query; don't do this in a production app,
            // it could return too many rows for the web app to handle. For an example
            // of paging code, see:
            // http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
            var adsList = db.PrimeNumberCandidates.AsQueryable();
            return View(await adsList.ToListAsync());
        }

        // GET: PrimeNumberCandidate/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PrimeNumberCandidate ad = await db.PrimeNumberCandidates.FindAsync(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            return View(ad);
        }

        // GET: PrimeNumberCandidate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrimeNumberCandidate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "Number")] PrimeNumberCandidate primeNumber
            //,
            //HttpPostedFileBase imageFile
            )
        {
            //CloudBlockBlob imageBlob = null;
            // A production app would implement more robust input validation.
            // For example, validate that the image file size is not too large.
            if (ModelState.IsValid)
            {
                //if (imageFile != null && imageFile.ContentLength != 0)
                //{
                //    imageBlob = await UploadAndSaveBlobAsync(imageFile);
                //    primeNumber.ImageURL = imageBlob.Uri.ToString();
                //}
                //primeNumber.PostedDate = DateTime.Now;
                db.PrimeNumberCandidates.Add(primeNumber);
                await db.SaveChangesAsync();
                Trace.TraceInformation("Created Prime {0} in database", primeNumber.Number);

                //if (imageBlob != null)
                //{
                var primeToTest = primeNumber.Number.ToString();
                var queueMessage = new CloudQueueMessage(primeToTest);
                    await primesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for number {0}", primeToTest);
                //}
                return RedirectToAction("Index");
            }

            return View();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> CreateMany()
        {
            
            var maxPrime = db.PrimeNumberCandidates.Any() ? db.PrimeNumberCandidates.Max(p => p.Number) : 0;
            for (int i = maxPrime + 1;i <= maxPrime + 1000; i++)
            {
                var primeToTest = i.ToString();
                var queueMessage = new CloudQueueMessage(primeToTest);
                primesQueue.AddMessageAsync(queueMessage);
                Trace.TraceInformation("Created queue message for number {0}", primeToTest);

            }
            return RedirectToAction("Index");
        }
    }
}
