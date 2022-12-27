namespace SolutionTemplate.Interfaces.Base.Entities;

/// <summary>Person</summary>
/// <typeparam name="TKey"></typeparam>
public interface IPerson<out TKey> : IEntity<TKey>
{
    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string Patronymic { get; set; }
}