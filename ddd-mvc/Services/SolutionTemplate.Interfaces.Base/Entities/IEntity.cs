namespace SolutionTemplate.Interfaces.Base.Entities
{
    /// <summary> Generic typed Id Entity </summary>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public interface IEntity<out TKey>
    {
        TKey Id { get; }
    }

    /// <summary> Entity </summary>
    public interface IEntity : IEntity<int>
    {
    }
}