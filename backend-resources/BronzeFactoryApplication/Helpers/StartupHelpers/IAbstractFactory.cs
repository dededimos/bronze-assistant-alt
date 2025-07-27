namespace BronzeFactoryApplication.Helpers.StartupHelpers
{
    /// <summary>
    /// An Interface representing a Factory of <typeparamref name="T"/> objects
    /// </summary>
    /// <typeparam name="T">The Created Object</typeparam>
    public interface IAbstractFactory<T>
    {
        T Create();
    }

    /// <summary>
    /// An interface representing a Factory of <typeparamref name="T"/> objects created by providing <typeparamref name="U"/> objects
    /// </summary>
    /// <typeparam name="T">The Object Created</typeparam>
    /// <typeparam name="U">The Parameter needed for the Creation Process</typeparam>
    public interface IAbstractFactory<T, U>
    {
        T Create(U parameter);
    }
}