using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PrimeSolverCommon;

namespace PrimeSolverWeb
{
    public class PrimeHub : Hub
    {
        private PrimeSolver _solver;

        public PrimeHub(PrimeSolver solver)
        {
            _solver = solver;
        }

        public PrimeHub() : this(PrimeSolver.GetPrimeSolver())
        {
            
        }

        public IEnumerable<PrimeNumberCandidate> GetAll()
        {
            return _solver.Get();
        }


        public void Send(int number, bool isPrime)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(number, isPrime);
        }
    }
}