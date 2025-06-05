using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;

namespace PotikotTools.UniTalks
{
    public class MessagePackDialoguePersistence : IDialoguePersistence
    {
        private readonly MessagePackSerializerOptions _options;

        public MessagePackDialoguePersistence()
        {
            var map = new Dictionary<Type, int>
            {
                { typeof(SingleChoiceNodeData), 0 },
                { typeof(MultipleChoiceNodeData), 1 },
                { typeof(TimerNodeData), 2 }
            };
            
            var resolver = CompositeResolver.Create(
                new DynamicUnionResolver<NodeData>(map),
                StandardResolver.Instance
            );

            _options = MessagePackSerializerOptions.Standard.WithResolver(resolver).WithCompression(MessagePackCompression.Lz4BlockArray);
            MessagePackSerializer.DefaultOptions = _options;
        }

        public bool Save(string directoryPath, DialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = MessagePackSerializer.Serialize(dialogueData, _options);
            return FileUtility.WriteAllBytes(fullPath, binary, refreshAsset);
        }

        public async Task<bool> SaveAsync(string directoryPath, DialogueData dialogueData, bool refreshAsset = true)
        {
            string fullPath = Path.Combine(directoryPath, dialogueData.Name, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = MessagePackSerializer.Serialize(dialogueData, _options);
            return await FileUtility.WriteAllBytesAsync(fullPath, binary, refreshAsset);
        }

        public DialogueData Load(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = FileUtility.ReadAllBytes(fullPath);
            if (binary == null || binary.Length == 0)
                return null;

            return MessagePackSerializer.Deserialize<DialogueData>(binary, _options);
        }

        public async Task<DialogueData> LoadAsync(string directoryPath, string dialogueId)
        {
            string fullPath = Path.Combine(directoryPath, dialogueId, UniTalksPreferences.Data.RuntimeDataFilename);

            byte[] binary = await FileUtility.ReadAllBytesAsync(fullPath);
            if (binary == null || binary.Length == 0)
                return null;

            return MessagePackSerializer.Deserialize<DialogueData>(binary, _options);
        }

        public List<string> LoadTags(string directoryPath, string dialogueId)
        {
            var dialogue = Load(directoryPath, dialogueId);
            return dialogue?.Tags?.ToList() ?? new List<string>();
        }

        public async Task<List<string>> LoadTagsAsync(string directoryPath, string dialogueId)
        {
            var dialogue = await LoadAsync(directoryPath, dialogueId);
            return dialogue?.Tags?.ToList() ?? new List<string>();
        }
    }
}