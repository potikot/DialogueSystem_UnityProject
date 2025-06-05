using System.IO;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;

namespace PotikotTools.UniTalks.Editor
{
    public class MessagePackEditorDialoguePersistence : IEditorDialoguePersistence
    {
        private readonly MessagePackSerializerOptions _options;

        public MessagePackEditorDialoguePersistence()
        {
            var resolver = CompositeResolver.Create(
                StandardResolver.Instance
            );

            _options = MessagePackSerializerOptions.Standard.WithResolver(resolver).WithCompression(MessagePackCompression.Lz4BlockArray);
            MessagePackSerializer.DefaultOptions = _options;
        }

        public bool Save(string directoryPath, EditorDialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = MessagePackSerializer.Serialize(dialogueData, _options);
            return FileUtility.WriteAllBytes(fullPath, binary, refreshAsset);
        }

        public async Task<bool> SaveAsync(string directoryPath, EditorDialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = MessagePackSerializer.Serialize(dialogueData, _options);
            return await FileUtility.WriteAllBytesAsync(fullPath, binary, refreshAsset);
        }

        public EditorDialogueData Load(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = FileUtility.ReadAllBytes(fullPath);
            if (binary == null || binary.Length == 0)
                return null;

            return MessagePackSerializer.Deserialize<EditorDialogueData>(binary, _options);
        }

        public async Task<EditorDialogueData> LoadAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = await FileUtility.ReadAllBytesAsync(fullPath);
            if (binary == null || binary.Length == 0)
                return null;

            return MessagePackSerializer.Deserialize<EditorDialogueData>(binary, _options);
        }
    }
}