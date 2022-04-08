namespace FlyingPizzaTello.Mocks;

public class MockedTello
{
    public GeoLocation current;
    public string currentState;

    public MockedTello(GeoLocation home)
    {
        current = home;
        currentState = "READY";
    }
    public async Task<bool> send_command(string command)
    {
        var splitCommand = command.Split(" ");
        switch (splitCommand[0])
        {
            case "command":
                currentState = "READY";
                return true;
            case "takeoff":
                currentState = "FLYING";
                return true;
            case "land":
                currentState = "MOTOR STOPPED";
                return true;
            case "left":
                if (splitCommand.Length < 2)
                {
                    return false;
                }
                current.Latitude -= int.Parse(splitCommand[1]);
                return true;
            case "right":
                if (splitCommand.Length < 2)
                {
                    return false;
                }
                current.Latitude += int.Parse(splitCommand[1]);
                return true;
            case "back":
                if (splitCommand.Length < 2)
                {
                    return false;
                }
                current.Longitude -= int.Parse(splitCommand[1]);
                return true;
            case "forward":
                if (splitCommand.Length < 2)
                {
                    return false;
                }
                current.Longitude += int.Parse(splitCommand[1]);
                return true;
            default:
                return false;
        }
    }        
}

