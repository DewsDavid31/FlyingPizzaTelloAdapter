
using System.Net;

namespace FlyingPizzaTello.Controllers
{

    public class TelloController
    {

        // Adapter between FlyingPizza dispatcher and Tello SDK Drone
        private const decimal LongToCm = 1110000;
        private const string Command = "command ";
        private const string Right = "right ";
        private const string Left = "left ";
        private const string Backward = "back ";
        private const string Forward = "forward ";
        private const string Takeoff = "takeoff";
        private const string Land = "land";
        private const decimal MaxDist = 200;
        private const decimal MinDist = 20;
        private string _direction;
        public GeoLocation Destination { get; set; }
        private GeoLocation Home { get; set; }
        private new int BadgeNumber { get;}
        public string Status { get; set; }
        public GeoLocation Location { get; set; }
        public string IpAddress { get; }
        
        private Tello _tello;

        private GeoLocation _calcDestination { get; set; }

        private GeoLocation _calcDestinationCm { get; set; }

        private bool Offline { get; set; }



        public TelloController(int badge, GeoLocation home, bool offline = false)
        {
            BadgeNumber = badge;
            Status = "Ready";
            Location = home; 
            Destination = home;
            Home = home;
            IpAddress = "192.168.10.1";
            Offline = offline;
            _tello = new Tello(home, Offline);
            
        }

        public void DeliverOrder(GeoLocation customerLocation)
        {
            Destination = customerLocation;
            SendCommand(Command);
            SendCommand(Takeoff);
            SendCommand(customerLocation);
            SendCommand(Land);
            SendCommand(Takeoff);
            Status = "Returning";
            SendCommand(Home);
            SendCommand(Land);
            Status = "Ready";
        }

        private void SendCommand(GeoLocation telemetry)
        {
            IEnumerable<string> commands = PointToTelloCommands(telemetry.Latitude, telemetry.Longitude);
            if (Status != "Dead")
                // Refuses more commands on any error.
            {
                foreach (string command in commands)
                {
                    var task = _tello.send_command(command, Offline);
                    task.Wait();
                }
            }
        }

        private void SendCommand(string command)
        {
            if (Status != "Dead")
            {
                // Refuses to take commands if an error occured.
                var task = _tello.send_command(command, Offline);
                task.Wait();
                if (!task.Result)
                {
                    // Errors are considered in dead status for now
                    Status = "Dead";
                }
            }
        }

        private static decimal ArcToCm(decimal value)
        {
            // Converts lat/long to tello centimeter distance.
            return value * LongToCm;
        }

        private static decimal CmToArc(decimal value)
        {
            // Converts tello centimeter distance to lat/long.
            return value / LongToCm;
        }

        private IEnumerable<string> PointToTelloCommands(decimal lat, decimal longitude)
        {
            // Unrolls a lat/long command into multiple Tello 200cm directions
            _calcDestination = new GeoLocation();
            _calcDestination.Latitude = lat;
            _calcDestination.Longitude = longitude;
            _calcDestinationCm = new GeoLocation();
            _calcDestinationCm.Latitude = ArcToCm(_calcDestination.Latitude);
            _calcDestinationCm.Longitude = ArcToCm(_calcDestination.Longitude);
            decimal tempX = ArcToCm(Location.Latitude);
            decimal tempY = ArcToCm(Location.Longitude);
            decimal amount = 0;
            //double telloFudgeValue = 0.45;
            // This value was added since I observed a command of 111 cm ends up being about 50 cm in distance
            // You may have to calibrate your tello for this factor to be accurate.
            string tempCommand;
            List<string> commands = new List<string>();
            while (tempX != _calcDestinationCm.Latitude && tempY != _calcDestinationCm.Longitude)
            {
                while (tempY < _calcDestinationCm.Latitude - MinDist)
                {
                    _direction = Forward;
                    decimal diffY = Math.Abs(_calcDestinationCm.Longitude - tempY);
                    if (diffY >= MaxDist)
                    {
                        amount = MaxDist;

                    }

                    if (diffY < MaxDist)
                    {
                        amount = Math.Abs(tempY - _calcDestinationCm.Longitude);

                    }

                    tempY += amount;
                    tempCommand = _direction + amount;
                    amount = 0;
                    commands.Add(tempCommand);
                }

                amount = 0;
                while (tempX > _calcDestinationCm.Latitude + MinDist)
                {

                    _direction = Left;
                    decimal diffX = Math.Abs(tempX - _calcDestinationCm.Latitude);
                    if (diffX >= MaxDist)
                    {
                        amount = MaxDist;

                    }

                    if (diffX < MaxDist)
                    {
                        amount = Math.Abs(tempY - _calcDestinationCm.Latitude);

                    }

                    tempX -= amount;
                    amount = 0;
                    tempCommand = _direction + amount;
                    commands.Add(tempCommand);
                }

                amount = 0;
                while (tempX < _calcDestinationCm.Latitude - MinDist)
                {

                    _direction = Right;
                    decimal diffX = Math.Abs(tempX - _calcDestinationCm.Latitude);
                    if (diffX >= MaxDist)
                    {
                        amount = MaxDist;

                    }

                    if (diffX < MaxDist)
                    {
                        amount = Math.Abs(tempY - _calcDestinationCm.Latitude);

                    }

                    tempX += amount;
                    amount = 0;
                    tempCommand = _direction + amount;
                    commands.Add(tempCommand);
                }



                amount = 0;
                while (tempY > _calcDestinationCm.Longitude + MinDist)
                {
                    _direction = Backward;
                    var diffY = Math.Abs(_calcDestinationCm.Longitude - tempY);
                    if (diffY >= MaxDist)
                    {
                        amount = MaxDist;
                    }

                    if (diffY < MaxDist)
                    {
                        amount = Math.Abs(tempY - _calcDestinationCm.Longitude);

                    }

                    tempY -= amount;
                    amount = 0;
                    tempCommand = _direction + amount;
                    commands.Add(tempCommand);
                }

                amount = 0;
                // For now we update the location after unrolling since Tello doesn't keep lat/long.
                Location = new GeoLocation();
                Location.Latitude = CmToArc(tempX);
                Location.Longitude = CmToArc(tempY);
            }

            return commands.ToArray();
        }

        public override string ToString()
        {
            return $"ID:{BadgeNumber}\nlocation:{Location}\nDestination:{Destination}\nStatus:{Status}";
        }

        public void StopSocket()
        {
            _tello.stopSocket();
        }
    }
}












   
