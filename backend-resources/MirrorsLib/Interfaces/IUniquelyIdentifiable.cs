namespace MirrorsLib.Interfaces
{
    public interface IUniquelyIdentifiable
    {
        string ItemUniqueId { get; }

        void AssignNewUniqueId();
        void AssignNewUniqueId(string uniqueId);
    }
}
