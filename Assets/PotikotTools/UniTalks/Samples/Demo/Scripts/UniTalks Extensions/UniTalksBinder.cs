using System;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo
{
    public static class UniTalksBinder
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            UniTalksPreferences.EmptyDialogueOptions = Array.Empty<string>();
            
            DialoguesComponents.Variables.Set("name", "Ildus");
        }
    }
}