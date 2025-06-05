using System;

namespace PotikotTools.UniTalks
{
    public interface IChangeNotifier
    {
        event Action OnChanged;
    }
}