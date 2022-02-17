namespace FlyingPizzaTello.Entities;

public class TelloAdapter
{
    public Guid badgeNumber { get; set; }
    public GeoLocation location { get; set; }

    public bool IsComplete { get; set; }

}