using Microsoft.AspNetCore.Mvc;

namespace FlyingPizzaTello
{

    public class TelloAdapter
    {
        private Guid BadgeNumber { get;}
        private TelloController  Controller { get;}

        public TelloAdapter(Guid badge, GeoLocation home)
        {
            BadgeNumber = badge;
            Controller = new TelloController(BadgeNumber.GetHashCode(),home);
        }
        
        [HttpPost("/assigndelivery")]
        public async Task<IActionResult> Assign(GeoLocation destination)
        {
            //https://{droneIpAddress}/assigndelivery
            Controller.DeliverOrder(destination);
            return new OkResult();
        }
        
            
        
        
        [HttpPost("/initregistration")]
        public async Task<IActionResult> Init()
        {
            
            //"https://{droneIpAddress}/initregistration
            return new OkResult();
        }
        
        [HttpPost("/completregistration")]
        public async Task<IActionResult> CompleteRegistration()
        {
            //"https://{droneIpAddress}/completregistration"
            return new OkResult();
        }
    }


}