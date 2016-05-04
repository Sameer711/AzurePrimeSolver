using System.Data.Entity;

namespace PrimeSolverRepository
{
    public class PrimeNumberCandidatesContext : DbContext
    {
        private DbSet<PrimeNumberCandidate> _primeNumberCandidates;

        public PrimeNumberCandidatesContext() : base("name=PrimeNumberCandidatesContext")
        {
        }

        public PrimeNumberCandidatesContext(string connString) : base(connString)
        {
        }

        public DbSet<PrimeNumberCandidate> PrimeNumberCandidates
        {
            get { return _primeNumberCandidates; }
            set { _primeNumberCandidates = value; }
        }
    }
}
