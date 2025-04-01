using System;
using Newtonsoft.Json;

namespace PotikotTools.DialogueSystem
{
    public class NodeDataIdConverter : JsonConverter<NodeData>
    {
        public override void WriteJson(JsonWriter writer, NodeData value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.Id);
        }

        public override NodeData ReadJson(JsonReader reader, Type objectType, NodeData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            int nodeId = (int)reader.Value;

            // TODO: Get node by id
            
            return existingValue;
        }
    }
}