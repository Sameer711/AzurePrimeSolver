using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrimeSolverRepository;

namespace PrimeSolverCommon
{
    public class PrimeCandidateViewModel
    {
        public int Number { get; set; }
        public bool? IsPrime { get; set; }

        public static PrimeCandidateViewModel FromEntity(PrimeNumberCandidate item)
        {
            return new PrimeCandidateViewModel
            {
                IsPrime = item.IsPrime,
                Number = item.Number
            };
        }
    }
}
