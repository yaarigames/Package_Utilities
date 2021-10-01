namespace SAS.Pool
{
    public interface IPool
    {
        int Capacity { get; }
        int Active { get; }
        int Inactive { get; }
        void Initialize(int count);

        void Clear();
    }

    public interface IDespawnablePool<in T> : IPool
    {
        void Despawn(T item);
    }

    public interface IPool<T> : IDespawnablePool<T>
    {
        T Spawn();
    }
}
