using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using PrimeSolverCommon;

namespace PrimeSolverWeb.Controllers
{
    public class Broadcaster
    {
        private IHubContext _hubContext;

        public Broadcaster()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<PrimeHub>();
        }

        // Called by a Timer object.
        public void BroadcastNumber(object state)
        {
                _hubContext.Clients.All.broadcastNumber(123);
        }
    }
}