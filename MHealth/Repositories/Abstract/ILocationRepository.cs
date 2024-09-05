namespace MHealth.Repositories.Abstract
{
    public interface ILocationRepository
    {
        Task<string> GetCachedLocation();
    }
}
