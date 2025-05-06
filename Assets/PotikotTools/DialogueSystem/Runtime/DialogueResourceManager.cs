using System.Threading.Tasks;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public class DialogueResourceManager : IDialogueResourceManager
    {
        public T GetResource<T>(string name) where T : Object
        {
            return Resources.Load<T>(name);
        }
        
        public async Task<T> GetResourceAsync<T>(string name) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(name);

            while (!request.isDone)
                await Task.Yield();
            
            return request.asset as T;
        }
    }
}