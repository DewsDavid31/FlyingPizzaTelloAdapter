using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;

namespace FlyingPizzaTello
{

    public class TelloAdapter : Drone
    {
        private static HttpListener _httpServer;
        
        private Guid BadgeNumber { get;}
        public TelloController  Controller { get;}

        public TelloAdapter(Guid badge, GeoLocation home)
        {
            BadgeNumber = badge;
            Controller = new TelloController(BadgeNumber.GetHashCode(),home);
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
            // Yes I know it is mispelt, its how the legacy code is currently
            //"https://{droneIpAddress}/completregistration"
            return new OkResult();
        }

    }


}