using SAS.Utilities.TagSystem;

namespace SAS.Pool
{
    public interface ISpawnable
    {
        void OnSpawn(object data);
        void OnDespawn();
    }
}
