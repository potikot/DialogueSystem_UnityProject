using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PotikotTools.DialogueSystem
{
    public class ConnectionDataConverter : JsonConverter<ConnectionData>
    {
        public override void WriteJson(JsonWriter writer, ConnectionData value, JsonSerializer serializer)
        {
            JObject obj = new();
            
            if (!string.IsNullOrEmpty(value.Text))
                obj["Text"] = value.Text;
            if (value.From != null)
                obj["From"] = value.From.Id;
            if (value.To != null)
                obj["To"] = value.To.Id;
            
            obj.WriteTo(writer);
        }

        public override ConnectionData ReadJson(JsonReader reader, Type objectType, ConnectionData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            ConnectionData connectionData = new();
            JObject obj = JObject.Load(reader);
            
            if (obj.TryGetValue("Text", out JToken text))
                connectionData.Text = (string)text;
            
            // TODO: get node by id
            
            if (obj.TryGetValue("From", out JToken from))
            {
                int fromId = (int)from;
                // DL.Log("fromId: " + fromId);
            }

            if (obj.TryGetValue("To", out JToken to))
            {
                int toId = (int)to;
                // DL.Log("toId: " + toId);
            }

            return connectionData;
        }
    }
}