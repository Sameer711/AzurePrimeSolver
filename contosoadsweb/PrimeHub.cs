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

        //public void BroadcastResult(int number, bool isPrime)
        //{
        //    Clients.All.updateResult(number, isPrime);

        //}
        public bool IsReadyForWork()
        {
            return _solver.IsReadyForWork();
        }

        //public void BroadcastProgress(int percent)
        //{
        //    Clients.All.updateProgress(percent);

        //}

    }
}