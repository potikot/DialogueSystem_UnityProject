using UnityEditor;
using UnityEngine;

namespace PotikotTools.UniTalks.Editor
{
    public abstract class BaseUniTalksEditorWindow : EditorWindow
    {
        [SerializeField] protected string dialogueName;
        
        protected EditorDialogueData editorData;
        protected bool isSubbed;

        public string DialogueName => dialogueName;
        
        public EditorDialogueData EditorData
        {
            get => editorData;
            set
            {
                if (value == null || editorData == value || value.RuntimeData == null)
                    return;
                
                editorData = value;
                editorData.OnNameChanged += ChangeTitle;
                editorData.OnDeleted += Close;
                isSubbed = true;
                
                dialogueName = value.Name;
                
                ChangeTitle(editorData.Name);
                OnEditorDataChanged();
            }
        }

        protected virtual void OnDestroy()
        {
            if (isSubbed)
            {
                editorData.OnNameChanged -= ChangeTitle;
                editorData.OnDeleted -= Close;
                isSubbed = false;
            }
        }
        
        protected virtual void ChangeTitle(string value)
        {
            titleContent = new GUIContent(value);
        }
        
        protected abstract void OnEditorDataChanged();
    }
}