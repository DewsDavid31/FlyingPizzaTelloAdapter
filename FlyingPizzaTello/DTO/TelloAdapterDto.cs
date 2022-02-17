using FlyingPizzaTello.Entities;
namespace FlyingPizzaTello.DTO;

public class TelloAdapterDto
{
    public Guid badgeNumber { get; set; }

    public GeoLocation location { get; set; }
    public bool IsComplete { get; set; }

    public TelloAdapterDto(TelloAdapter telloAdapterObj) =>
        (badgeNumber, location, IsComplete) = (telloAdapterObj.badgeNumber,telloAdapterObj.location, telloAdapterObj.IsComplete);

}