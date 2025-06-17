using PotikotTools.UniTalks.Editor;
using UnityEditor;
using UnityEngine;

namespace PotikotTools.UniTalks.Demo.Editor
{
    [InitializeOnLoad]
    public static class EditorUniTalksBinder
    {
        static EditorUniTalksBinder()
        {
            DialogueEditorWindowsManager.AddNodeType<ChatNodeData, ChatNodeView>();
            DialoguePreviewWindowsManager.AddNodeHandler(typeof(ChatNodeData), new ChatNodeHandler());
            
            DialoguesComponents.Variables.Set("name", "Ildus");
        }
    }
}