using SAS.Utilities.TagSystem;

namespace SAS.Pool
{
    public interface ISpawnable
    {
        void OnSpawn<T>(T data);
        void OnDespawn();
    }
}
