using System.Net;
using System.Net.Sockets;
using System.Text;
using Moq;

namespace FlyingPizzaTello.Mocks;

public class MockedTello : Tello
{
    public GeoLocation current;
    
    public string currentState;

    public int battery;

    public double speed;

    public int altitude;

    public string IpAddress = "192.168.10.1";

    public string response;

    public DateTimeOffset elapsed;

    // Version of Tello code with simulated Tello SDK mock baked in instead of UDP calls
    public MockedTello(GeoLocation registeredHome)
    {
        battery = 100;
        speed = 0.0;
        elapsed = DateTimeOffset.Now;
        altitude = 0;
        response = "Error";
        current = registeredHome;
    }

    public override async Task<bool> send_command(string command)
    {
        var splitCommand = command.Split(" ");
        response = "ERROR";
        if (splitCommand.Length >= 2)
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
                    current.Latitude -= decimal.Parse(splitCommand[1]);
                    return await Task.FromResult(true);
                case "right":
                    response = "OK";
                    current.Latitude += decimal.Parse(splitCommand[1]);
                    return await Task.FromResult(true);
                case "forward":
                    response = "OK";
                    current.Longitude += decimal.Parse(splitCommand[1]);
                    return await Task.FromResult(true);
                case "back":
                    response = "OK";
                    current.Longitude -= decimal.Parse(splitCommand[1]);
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

                case "Command":
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
}


