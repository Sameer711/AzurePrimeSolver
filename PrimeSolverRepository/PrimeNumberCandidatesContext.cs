using System.Data.Entity;

namespace PrimeSolverRepository
{
    public class PrimeNumberCandidatesContext : DbContext
    {
        public DbSet<PrimeNumberCandidate> PrimeNumberCandidates { get; set; }

        public PrimeNumberCandidatesContext() : base("name=PrimeNumberCandidatesContext")
        {
        }

        public PrimeNumberCandidatesContext(string connString) : base(connString)
        {
        }

    }
}
