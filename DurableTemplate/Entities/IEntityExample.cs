namespace DurableTemplate.Entities
{
    public interface IEntityExample
    {
        int CurrentValue { get; set; }

        void Add(int amount);
        int Get();
        void Reset();
    }
}