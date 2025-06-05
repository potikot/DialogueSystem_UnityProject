using System.Threading.Tasks;

namespace PotikotTools.UniTalks
{
    public interface ISaver<T>
    {
        bool Save(string directoryPath, T data, bool refreshAsset = true);
        Task<bool> SaveAsync(string directoryPath, T data, bool refreshAsset = true);
    }
    
    public interface ILoader<T>
    {
        T Load(string directoryPath, string id);
        Task<T> LoadAsync(string directoryPath, string id);
    }

    public interface IPersistence<T> : ISaver<T>, ILoader<T> { }
}