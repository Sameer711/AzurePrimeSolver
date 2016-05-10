using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PrimeSolverCommon;

namespace PrimeSolverWeb.ApiControllers
{
    [RoutePrefix("Status")]
    public class StatusController : ApiController
    {
        private readonly PrimeSolver _solver = PrimeSolver.GetPrimeSolver();
        [Route("/IsReadyForWork")]
        public bool IsReadyForWork()
        {
            return _solver.IsReadyForWork();
        }

    }
}