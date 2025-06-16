using PotikotTools.UniTalks.Editor;
using UnityEditor;

namespace PotikotTools.UniTalks.Demo.Editor
{
    [InitializeOnLoad]
    public static class EditorUniTalksBinder
    {
        static EditorUniTalksBinder()
        {
            DialogueEditorWindowsManager.AddNodeType<ChatNodeData, ChatNodeView>();
            DialoguePreviewWindowsManager.AddNodeHandler(typeof(ChatNodeData), new ChatNodeHandler());
        }
    }
}