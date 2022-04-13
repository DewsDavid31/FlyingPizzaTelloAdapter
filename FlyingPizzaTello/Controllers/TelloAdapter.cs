using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace FlyingPizzaTello.Controllers;

public class TelloAdapter : ControllerBase
{
    private Guid BadgeNumber { get;}
    public Tello.TelloController  Controller { get;}

        
    public TelloAdapter(Guid badge, GeoLocation home, bool offline=false)
    {
        BadgeNumber = badge;
        Controller = new Tello.TelloController(BadgeNumber.GetHashCode(), home, offline);
    }
    [HttpPost("assigndelivery")]
    public async Task<IActionResult> AssignDelivery(GeoLocation destination)
    {
        Controller.DeliverOrder(destination);
        return new OkResult();
    }
    [HttpPost("initregistration")]
    public async Task<IActionResult> InitRegistration()
    {
            
        return new OkResult();
    }
    [HttpPost("completeregistration")]
    public async Task<IActionResult> CompleteRegistration()
    {
        return new OkResult();
    }

    public void StopSocket()
    {
        Controller.StopSocket();
    }

}

public class Tello
{
    // Converted by hand from python code originally from Tello DJI

    private readonly IPAddress _telloAddress = IPAddress.Parse("192.168.10.1");

    private readonly int _telloPort = 8889;

    private readonly UdpClient? _socket;

    private readonly GeoLocation _current;

    public string? CurrentState;

    private readonly int _battery;

    private double _speed;

    private int _altitude;

    private string IpAddress = "192.168.10.1";

    private string _response;

    private readonly DateTimeOffset _elapsed;

    private const decimal LongToCm = 1110000m;


    private Tello(GeoLocation home, bool offline = false)
    {
        if (offline)
        {
            _battery = 100;
            _speed = 0.0;
            _elapsed = DateTimeOffset.Now;
            _altitude = 0;
            _response = "Error";
            _current = home;
        }
        else
        {
            _battery = 0;
            _speed = 0.0;
            _elapsed = DateTimeOffset.Now;
            _altitude = 0;
            _response = "Ready";
            _current = home;
            _socket = new UdpClient(_telloPort);
            _socket.Connect(_telloAddress, _telloPort);
        }
    }

    private void StopSocket()
    {
        _socket?.Close();
    }

    private static decimal CmToArc(decimal value)
    {
        // Converts tello centimeter distance to lat/long.
        return value / LongToCm;
    }


    private async Task<bool> send_command(string command, bool offline = false)
    {
        if (offline)
        {
            var splitCommand = command.Split(" ");
            _response = "ERROR";
            if (splitCommand.Length >= 2 && splitCommand[1].Length > 0)
            {
                switch (splitCommand[0])
                {

                    case "up":
                        _response = "OK";
                        _altitude += int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "down":
                        _response = "OK";
                        _altitude -= int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "left":
                        _response = "OK";
                        _current.Latitude -= CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "right":
                        _response = "OK";
                        _current.Latitude += CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "forward":
                        _response = "OK";
                        _current.Longitude += CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "back":
                        _response = "OK";
                        _current.Longitude -= CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "mdirection":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "speed":
                        _response = "OK";
                        _speed = int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "mon":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "stop":
                        _response = "OK";
                        _speed = 0;
                        return await Task.FromResult(true);
                    case "cw":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "ccw":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "flip":
                        _response = "OK";
                        return await Task.FromResult(true);
                }
            }
            else
            {
                switch (splitCommand[0])
                {

                    case "command":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "takeoff":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "land":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "speed?":
                        _response = _speed + "";
                        return await Task.FromResult(true);
                    case "battery?":
                        _response = _battery.ToString();
                        return await Task.FromResult(true);
                    case "time?":
                        _response = _elapsed.ToString();
                        return await Task.FromResult(true);
                    case "wifi?":
                        _response = IpAddress;
                        return await Task.FromResult(true);
                    case "sdk?":
                        _response = "Dave's Mock SDK";
                        return await Task.FromResult(true);
                    case "sn?":
                        _response = "FAKE-001";
                        return await Task.FromResult(true);
                    case "stop":
                        _response = "OK";
                        return await Task.FromResult(true);
                    case "emergency":
                        _response = "OK";
                        return await Task.FromResult(true);
                }
            }

            return await Task.FromResult(false);
        }

        byte[] bytes = Encoding.UTF8.GetBytes(command);

        if (command == "takeoff")
        {
            // Added since takeoff ignores settling on the floor
            await Task.Delay(5000);
        }

        await _socket?.SendAsync(bytes, bytes.Length)!;
        var response = await _socket.ReceiveAsync();
        var telloResp = Encoding.ASCII.GetChars(response.Buffer);
        if (telloResp.ToString() == "error")
        {
            return false;
        }

        return true;
    }

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
        private string? _direction;
        public GeoLocation Destination { get; private set; }
        private GeoLocation Home { get; set; }
        private int BadgeNumber { get; }
        public string Status { get; private set; }
        public GeoLocation Location { get; private set; }

        private readonly Tello _tello;

        private GeoLocation? CalcDestination { get; set; }

        private GeoLocation? CalcDestinationCm { get; set; }

        private bool Offline { get; set; }



        public TelloController(int badge, GeoLocation home, bool offline = false)
        {
            BadgeNumber = badge;
            Status = "Ready";
            Location = home;
            Destination = home;
            Home = home;
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
            var commands = PointToTelloCommands(telemetry.Latitude, telemetry.Longitude);
            if (Status == "Dead") return;
            foreach (string command in commands)
            {
                var task = _tello.send_command(command, Offline);
                task.Wait();
            }
        }

        private void SendCommand(string command)
        {
            if (Status == "Dead") return;
            // Refuses to take commands if an error occured.
            var task = _tello.send_command(command, Offline);
            task.Wait();
            if (!task.Result)
            {
                // Errors are considered in dead status for now
                Status = "Dead";
            }
        }

        private static decimal ArcToCm(decimal value)
        {
            // Converts lat/long to tello centimeter distance.
            return value * LongToCm;
        }

        // private static decimal CmToArc(decimal value)
        // {
        //     // Converts tello centimeter distance to lat/long.
        //     return value / LongToCm;
        // }

        private IEnumerable<string> PointToTelloCommands(decimal lat, decimal longitude)
        {
            // Unrolls a lat/long command into multiple Tello 200cm directions
            CalcDestination = new GeoLocation
            {
                Latitude = lat,
                Longitude = longitude
            };
            CalcDestinationCm = new GeoLocation
            {
                Latitude = ArcToCm(CalcDestination.Latitude),
                Longitude = ArcToCm(CalcDestination.Longitude)
            };
            decimal tempX = ArcToCm(Location.Latitude);
            decimal tempY = ArcToCm(Location.Longitude);
            decimal amount = 0;
            //double telloFudgeValue = 0.45;
            // This value was added since I observed a command of 111 cm ends up being about 50 cm in distance
            // You may have to calibrate your tello for this factor to be accurate.
            var commands = new List<string>();
            while (tempX != CalcDestinationCm.Latitude && tempY != CalcDestinationCm.Longitude)
            {
                string tempCommand;
                while (tempY < CalcDestinationCm.Latitude - MinDist)
                {
                    _direction = Forward;
                    var diffY = Math.Abs(CalcDestinationCm.Longitude - tempY);
                    if (diffY >= MaxDist)
                    {
                        amount = MaxDist;

                    }

                    if (diffY < MaxDist)
                    {
                        amount = Math.Abs(tempY - CalcDestinationCm.Longitude);

                    }

                    tempY += amount;
                    tempCommand = _direction + amount;
                    amount = 0;
                    commands.Add(tempCommand);
                }

                amount = 0;
                while (tempX > CalcDestinationCm.Latitude + MinDist)
                {

                    _direction = Left;
                    var diffX = Math.Abs(tempX - CalcDestinationCm.Latitude);
                    if (diffX >= MaxDist)
                    {
                        amount = MaxDist;

                    }

                    if (diffX < MaxDist)
                    {
                        amount = Math.Abs(tempY - CalcDestinationCm.Latitude);

                    }

                    tempX -= amount;
                    amount = 0;
                    tempCommand = _direction + amount;
                    commands.Add(tempCommand);
                }

                amount = 0;
                while (tempX < CalcDestinationCm.Latitude - MinDist)
                {

                    _direction = Right;
                    var diffX = Math.Abs(tempX - CalcDestinationCm.Latitude);
                    if (diffX >= MaxDist)
                    {
                        amount = MaxDist;

                    }

                    if (diffX < MaxDist)
                    {
                        amount = Math.Abs(tempY - CalcDestinationCm.Latitude);

                    }

                    tempX += amount;
                    amount = 0;
                    tempCommand = _direction + amount;
                    commands.Add(tempCommand);
                }



                amount = 0;
                while (tempY > CalcDestinationCm.Longitude + MinDist)
                {
                    _direction = Backward;
                    var diffY = Math.Abs(CalcDestinationCm.Longitude - tempY);
                    if (diffY >= MaxDist)
                    {
                        amount = MaxDist;
                    }

                    if (diffY < MaxDist)
                    {
                        amount = Math.Abs(tempY - CalcDestinationCm.Longitude);

                    }

                    tempY -= amount;
                    amount = 0;
                    tempCommand = _direction + amount;
                    commands.Add(tempCommand);
                }

                amount = 0;
                // For now we update the location after unrolling since Tello doesn't keep lat/long.
                Location = new GeoLocation
                {
                    Latitude = CmToArc(tempX),
                    Longitude = CmToArc(tempY)
                };
            }

            return commands.ToArray();
        }

        public override string ToString()
        {
            return $"ID:{BadgeNumber}\nlocation:{Location}\nDestination:{Destination}\nStatus:{Status}";
        }

        public void StopSocket()
        {
            _tello.StopSocket();
        }
    }
}