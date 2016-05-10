using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeSolverCommon
{
    public class Numbers
    {
        private static Numbers _instance;
        // Lock synchronization object
        private static readonly object syncLock = new object();

        public static Numbers Instance()
        {
            lock (syncLock)
            {
                if (_instance == null)
                {
                    _instance = new Numbers();
                }
            }

            return _instance;
        }

        protected Numbers()
        {
            
        }

       
        private Dictionary<int, bool> Candidates { get; set; } = new Dictionary<int, bool>();

        public void AddNumber(int number, bool isPrime)
        {
            Candidates[number] = isPrime;
        }

        public bool? IsPrime(int number)
        {
            return Candidates?[number];
        }

    }
}
