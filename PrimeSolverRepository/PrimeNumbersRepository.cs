using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace PrimeSolverRepository
{
    public class PrimeNumbersRepository
    {
        private readonly PrimeNumberCandidatesContext _db;
        public PrimeNumbersRepository(string connectionString = null)
        {
            // Read database connection string and open database.
            //var dbConnString = ConfigurationManager.ConnectionStrings["PrimeSolverDbConnectionString"];
            //_db = new PrimeNumberCandidatesContext(dbConnString);
            _db = connectionString == null ? new PrimeNumberCandidatesContext() : new PrimeNumberCandidatesContext(connectionString);
        }

        public IEnumerable<PrimeNumberCandidate> Get()
        {
            return _db.PrimeNumberCandidates;
        }

        public async Task Add(PrimeNumberCandidate primeNumberCandidate)
        {
            _db.PrimeNumberCandidates.AddOrUpdate(primeNumberCandidate);
            await _db.SaveChangesAsync();
        }

        //public PrimeNumberCandidate Find(int numberToTest)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
