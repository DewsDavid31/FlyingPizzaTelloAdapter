namespace FlyingPizzaTello;

public class AddOrderDto
{
    public class AddOrderDTO
    {
        public string Id;
        public GeoLocation DeliveryLocation;
        public override string ToString() => $"AddOrderDto:{{Id:{Id},DeliveryLocation:{DeliveryLocation}}}";
    }

    public string Id { get; set; }
}
