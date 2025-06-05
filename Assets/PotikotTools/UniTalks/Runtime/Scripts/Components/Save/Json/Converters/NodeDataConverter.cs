using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PotikotTools.UniTalks
{
    public class NodeDataConverter : JsonConverter<NodeData>
    {
        public override void WriteJson(JsonWriter writer, NodeData value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["Type"] = value.GetType().AssemblyQualifiedName,
                ["Id"] = value.Id,
                ["SpeakerIndex"] = value.SpeakerIndex,
                ["ListenerIndex"] = value.ListenerIndex,
                ["Text"] = value.Text,
                ["AudioResourceName"] = value.AudioResourceName,
                ["Commands"] = JToken.FromObject(value.Commands, serializer),
                ["OutputConnections"] = JToken.FromObject(value.OutputConnections, serializer)
            };
            
            obj.WriteTo(writer);
        }

        public override NodeData ReadJson(JsonReader reader, Type objectType, NodeData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}