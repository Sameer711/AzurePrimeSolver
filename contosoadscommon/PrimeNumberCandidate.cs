using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrimeSolverCommon
{
    public class PrimeNumberCandidate 
    {
        public PrimeNumberCandidate(int numberToTest)
        {
            Number = numberToTest;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int Number { get; set; }

        [DisplayName("Is Prime")]
        public bool? IsPrime { get; set; }

        public PrimeNumberCandidate() { }

    }
}
