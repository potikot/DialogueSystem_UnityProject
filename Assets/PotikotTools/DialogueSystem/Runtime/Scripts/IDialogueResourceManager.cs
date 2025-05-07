using System.Threading.Tasks;
using UnityEngine;

namespace PotikotTools.DialogueSystem
{
    public interface IDialogueResourceManager
    {
        T GetResource<T>(string name) where T : Object;
        Task<T> GetResourceAsync<T>(string name) where T : Object;
    }
}