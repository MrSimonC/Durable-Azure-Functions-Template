namespace DurableTemplate.Entities
{
    public interface IEntityExample
    {
        void SetAvailable(bool available);
        void SetName(string name);
        void SetNotificationSent(bool available);
        void SetOnPromotion(bool available);
        void SetPrice(string amount);
    }
}