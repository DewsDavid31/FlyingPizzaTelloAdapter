using System.Net;
using FlyingPizzaTello.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FlyingPizzaTello.Adapters
{

    public class TelloAdapter
    {
        private HttpListener _httpServer;
        
        private Guid BadgeNumber { get;}
        public TelloController  Controller { get; set; }

        
        public TelloAdapter(Guid badge, GeoLocation home, bool offline=false)
        {
            BadgeNumber = badge;
            Controller = new TelloController(BadgeNumber.GetHashCode(), home, offline);
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
        
        public async Task<IActionResult> AssignDelivery(GeoLocation destination)
        {
            //https://{droneIpAddress}/assigndelivery
            Controller.DeliverOrder(destination);
            return new OkResult();
        }
        
        public async Task<IActionResult> InitRegistration()
        {
            
            //"https://{droneIpAddress}/initregistration
            return new OkResult();
        }
        
        public async Task<IActionResult> CompleteRegistration()
        {
            //"https://{droneIpAddress}/completregistration"
            return new OkResult();
        }

        public void StopSocket()
        {
            _httpServer.Close();
            Controller.StopSocket();
        }

    }


}