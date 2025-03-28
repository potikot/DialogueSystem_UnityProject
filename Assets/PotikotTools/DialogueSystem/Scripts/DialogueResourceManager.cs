using System.Text;

namespace PotikotTools.DialogueSystem
{
    public interface IDialogueResourceManager
    {
        public T GetResource<T>(string name);
    }
    
    public class DialogueResourceManager
    {
        public string GenerateScript()
        {
            StringBuilder script = new();

            // usings
            script.AppendLine("using UnityEngine;");
            script.AppendLine();
            
            // namespace
            script.AppendLine("namespace PotikotTools.DialogueSystem");
            script.AppendLine("{");
            script.AppendLine("\tpublic class DRM : DialogueResourceManager");
            script.AppendLine("\t{");
            script.AppendLine("\t}");
            script.AppendLine("}");
            
            return script.ToString();
        }
    }
}