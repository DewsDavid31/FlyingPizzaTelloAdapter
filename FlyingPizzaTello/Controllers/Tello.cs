using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FlyingPizzaTello.Controllers;

public class Tello
{
    // Converted by hand from python code originally from Tello DJI
    
    private readonly IPAddress _telloAddress = IPAddress.Parse("192.168.10.1");

    private readonly int _telloPort = 8889;

    private UdpClient _socket;

    public GeoLocation current;
    
    public string currentState;

    public int battery;

    public double speed;

    public int altitude;

    public string IpAddress = "192.168.10.1";

    public string response;

    public DateTimeOffset elapsed;
    
    private const decimal LongToCm = 1110000m;
    

    public Tello(GeoLocation home, bool offline = false)
    {
        if (offline)
        {
            battery = 100;
            speed = 0.0;
            elapsed = DateTimeOffset.Now;
            altitude = 0;
            response = "Error";
            current = home;
        }
        else
        {
            _socket = new UdpClient(_telloPort);
            _socket.Connect(_telloAddress, _telloPort);
        }
    }

    public void stopSocket()
    {
        _socket.Close();
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


    public async Task<bool> send_command(string command, bool offline = false)
    {
        if (offline)
        {
            var splitCommand = command.Split(" ");
            response = "ERROR";
            if (splitCommand.Length >= 2 && splitCommand[1].Length > 0)
            {
                switch (splitCommand[0])
                {

                    case "up":
                        response = "OK";
                        altitude += int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "down":
                        response = "OK";
                        altitude -= int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "left":
                        response = "OK";
                        current.Latitude -= CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "right":
                        response = "OK";
                        current.Latitude += CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "forward":
                        response = "OK";
                        current.Longitude += CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "back":
                        response = "OK";
                        current.Longitude -= CmToArc(decimal.Parse(splitCommand[1]));
                        return await Task.FromResult(true);
                    case "mdirection":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "speed":
                        response = "OK";
                        speed = int.Parse(splitCommand[1]);
                        return await Task.FromResult(true);
                    case "mon":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "stop":
                        response = "OK";
                        speed = 0;
                        return await Task.FromResult(true);
                    case "cw":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "ccw":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "flip":
                        response = "OK";
                        return await Task.FromResult(true);
                }
            }
            else
            {
                switch (splitCommand[0])
                {

                    case "command":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "takeoff":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "land":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "speed?":
                        response = speed + "";
                        return await Task.FromResult(true);
                    case "battery?":
                        response = battery.ToString();
                        return await Task.FromResult(true);
                    case "time?":
                        response = elapsed.ToString();
                        return await Task.FromResult(true);
                    case "wifi?":
                        response = IpAddress;
                        return await Task.FromResult(true);
                    case "sdk?":
                        response = "Dave's Mock SDK";
                        return await Task.FromResult(true);
                    case "sn?":
                        response = "FAKE-001";
                        return await Task.FromResult(true);
                    case "stop":
                        response = "OK";
                        return await Task.FromResult(true);
                    case "emergency":
                        response = "OK";
                        return await Task.FromResult(true);
                }
            }

            return await Task.FromResult(false);
        }
        else
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


}