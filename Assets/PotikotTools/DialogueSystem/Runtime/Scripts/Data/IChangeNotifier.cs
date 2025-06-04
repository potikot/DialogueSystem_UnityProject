using System;

namespace PotikotTools.DialogueSystem
{
    public interface IChangeNotifier
    {
        event Action OnChanged;
    }
}