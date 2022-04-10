using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FlyingPizzaTello;

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
    
    public virtual async Task<bool> send_command(string command)
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