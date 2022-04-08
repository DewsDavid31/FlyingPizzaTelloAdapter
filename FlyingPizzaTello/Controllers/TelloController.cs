using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FlyingPizzaTello
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
        private GeoLocation DestinationCm { get; set; }
        public GeoLocation Destination { get; set; }
        private GeoLocation Home { get;}
        private new int BadgeNumber { get;}
        public string Status { get; set; }
        public GeoLocation Location { get; set; }
        public string IpAddress { get; }
        
        private Tello _tello;
        


        public TelloController(int badge, GeoLocation home)
        {
            BadgeNumber = badge;
            Status = "Ready";
            Location = home; 
            Destination = home;
            Home = home;
            IpAddress = "192.168.10.1";
            _tello = new Tello();
        }
        public TelloController(int badge, GeoLocation home,Tello mockedTello)
        {
            BadgeNumber = badge;
            Status = "Ready";
            Location = home; 
            Destination = home;
            Home = home;
            IpAddress = "192.168.10.1";
            _tello = mockedTello;
        }

        public void DeliverOrder(GeoLocation customerLocation)
        {
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
            Destination = telemetry;
            IEnumerable<string> commands = PointToTelloCommands(telemetry.Latitude, telemetry.Longitude);
            if (Status != "Dead")
                // Refuses more commands on any error.
            {
                foreach (string command in commands)
                {
                    var task = _tello.send_command(command);
                    task.Wait();
                }
            }
        }

        private void SendCommand(string command)
        {
            if (Status != "Dead")
            {
                // Refuses to take commands if an error occured.
                var task = _tello.send_command(command);
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
            Destination = new GeoLocation();
            Destination.Latitude = lat;
            Destination.Longitude = longitude;
            DestinationCm = new GeoLocation();
            DestinationCm.Latitude = ArcToCm(Destination.Latitude);
            DestinationCm.Longitude = ArcToCm(Destination.Longitude);
            decimal tempX = ArcToCm(Location.Latitude);
            decimal tempY = ArcToCm(Location.Longitude);
            decimal amount = 0;
            double telloFudgeValue = 0.45;
            // This value was added since I observed a command of 111 cm ends up being about 50 cm in distance
            // You may have to calibrate your tello for this factor to be accurate.
            string tempCommand;
            List<string> commands = new List<string>();
            
            while (tempY < DestinationCm.Latitude - MinDist)
            {
                _direction = Forward;
                decimal diffY = Math.Abs(DestinationCm.Longitude - tempY);
                if (diffY >= MaxDist)
                {
                    amount = MaxDist;

                }
                if (diffY < MaxDist)
                {
                    amount = Math.Abs(tempY - DestinationCm.Longitude);
                    
                }
                tempY += amount;
                tempCommand = _direction + amount;
                commands.Add(tempCommand);
            }

            while (tempX > DestinationCm.Latitude + MinDist)
            {
                
                _direction = Left;
                decimal diffY = Math.Abs(tempX - DestinationCm.Latitude);
                if (diffY >= MaxDist)
                {
                    amount = MaxDist;

                }
                if (diffY < MaxDist)
                {
                    amount = Math.Abs(tempY - DestinationCm.Latitude);
                    
                }
                tempX -= amount;
                tempCommand = _direction + amount;
                commands.Add(tempCommand);
            }

            while (tempX <  - MinDist)
            {
                _direction = Right;
                var diffX = tempX - DestinationCm.Latitude;
                if (diffX >= MaxDist)
                {
                    amount = MaxDist;
                }
                if (diffX < MaxDist)
                {
                    amount = Math.Abs(tempX - DestinationCm.Latitude);
                }
                tempX += amount;
                tempCommand = _direction + amount;
                commands.Add(tempCommand);
            }

            while (tempY > DestinationCm.Longitude + MinDist)
            {
                _direction = Backward;
                var diffY = Math.Abs(DestinationCm.Longitude - tempY);
                if ( diffY >= MaxDist)
                {
                    amount = MaxDist;
                }
                if ( diffY < MaxDist)
                {
                    amount = Math.Abs(tempY - DestinationCm.Longitude);
                    
                }
                tempY -= amount;
                tempCommand = _direction + amount;
                commands.Add(tempCommand);
            }

            // For now we update the location after unrolling since Tello doesn't keep lat/long.
            Location = new GeoLocation();
            Location.Latitude = CmToArc(tempX);
            Location.Longitude = CmToArc(tempY);
            return commands.ToArray();
        }

        public override string ToString()
        {
            return $"ID:{BadgeNumber}\nlocation:{Location}\nDestination:{Destination}\nStatus:{Status}";
        }
    }
}

public class Tello
{
    // Converted by hand from python code originally from Tello DJI
    
    private readonly IPAddress _telloAddress = IPAddress.Parse("192.168.10.1");

    private readonly int _telloPort = 8889;

    private UdpClient _socket;


    public Tello()
    {
        _socket = new UdpClient(_telloPort);
        _socket.Connect(_telloAddress, _telloPort);
    }
    
    public async Task<bool> send_command(string command)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(command);
        
        if (command == "takeoff")
        {
            // Added since takeoff ignores settling on the floor
            await Task.Delay(5000);
        }

        await _socket.SendAsync(bytes, bytes.Length);
        var response = await _socket.ReceiveAsync();
        var telloResp = Encoding.ASCII.GetChars(response.Buffer);
        if (telloResp.ToString() == "error")
        {
            return false;
        }
        return true;
    }
}










   
