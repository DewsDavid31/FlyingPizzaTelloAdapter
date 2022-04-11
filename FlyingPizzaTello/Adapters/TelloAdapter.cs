using System.Net;
using System.Net.Sockets;
using FlyingPizzaTello.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.VisualBasic.CompilerServices;

namespace FlyingPizzaTello
{

    public class TelloAdapter
    {
        private static HttpListener _httpServer;
        
        private Guid BadgeNumber { get;}
        public TelloController  Controller { get;}

        
        public TelloAdapter(Guid badge, GeoLocation home, bool offline=false)
        {
            // Offline version
            if (offline)
            {
                BadgeNumber = badge;
                Controller = new TelloController(BadgeNumber.GetHashCode(), home, new MockedTello(home));
            }
            else
            {
                BadgeNumber = badge;
                Controller = new TelloController(BadgeNumber.GetHashCode(),home);
            }
        }

        public async Task Listen(string ip)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(ip);
            listener.Start();
            while (listener.IsListening)
            {
                // Handle events here
            }
        }
        
        [HttpPost("/assigndelivery")]
        public async Task<IActionResult> AssignDelivery(GeoLocation destination)
        {
            //https://{droneIpAddress}/assigndelivery
            Controller.DeliverOrder(destination);
            return new OkResult();
        }

        [HttpPost("/initregistration")]
        public async Task<IActionResult> InitRegistration()
        {
            
            //"https://{droneIpAddress}/initregistration
            return new OkResult();
        }
        
        [HttpPost("/completeregistration")]
        public async Task<IActionResult> CompleteRegistration()
        {
            //"https://{droneIpAddress}/completregistration"
            return new OkResult();
        }

    }


}