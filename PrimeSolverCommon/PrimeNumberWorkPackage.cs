using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeSolverCommon
{
    public enum WorkType
    {
        NewWork,
        ContinueWork
    }
    public class PrimeNumberWorkPackage
    {
        public WorkType WorkType { get; set; }
        public int Number { get; set; }
        public int NumEntries { get; set; }
    }
}
