using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PrimeSolverCommon;

namespace PrimeSolverWeb.ApiControllers
{
    public class PrimesController : ApiController
    {
        private readonly PrimeSolver _solver = PrimeSolver.GetPrimeSolver();

        // GET api/<controller>
        public IEnumerable<PrimeCandidateViewModel> GetPrimes()
        {
            return _solver.Get(100, true);
        }

    }
}