using System;
using UnityEngine.UIElements;

namespace PotikotTools.DialogueSystem
{
    public class CustomFoldout : BindableElement, INotifyValueChanged<bool>
    {
        // resource("Icons/console.warnicon.png")
        
        public bool value { get; set; }

        public void SetValueWithoutNotify(bool newValue)
        {
            throw new NotImplementedException();
        }
    }
}