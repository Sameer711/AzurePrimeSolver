using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeSolverRepository
{
    public class PrimeNumberCandidate 
    {
        public PrimeNumberCandidate(int numberToTest)
        {
            Number = numberToTest;
        }

        public PrimeNumberCandidate(int numberToTest, bool isPrime)
        {
            Number = numberToTest;
            IsPrime = isPrime;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Number { get; set; }

        [DisplayName("Is Prime")]
        public bool? IsPrime { get; set; }

        public PrimeNumberCandidate() { }

    }
}
