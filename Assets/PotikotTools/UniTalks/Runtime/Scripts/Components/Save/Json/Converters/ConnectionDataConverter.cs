using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PotikotTools.UniTalks
{
    public class ConnectionDataConverter : JsonConverter<ConnectionData>
    {
        public override void WriteJson(JsonWriter writer, ConnectionData value, JsonSerializer serializer)
        {
            var obj = new JObject();
            
            if (!string.IsNullOrEmpty(value.Text))
                obj["Text"] = value.Text;

            if (value.From != null && value.To != null)
            {
                obj["From"] = value.From.Id;
                obj["To"] = value.To.Id;
            }
            
            obj.WriteTo(writer);
        }

        public override ConnectionData ReadJson(JsonReader reader, Type objectType, ConnectionData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var connectionData = existingValue ?? new ConnectionData();
            JObject obj = JObject.Load(reader);
            
            if (obj.TryGetValue("Text", out JToken text))
                connectionData.Text = (string)text;
            
            if (obj.TryGetValue("From", out JToken from) && obj.TryGetValue("To", out JToken to))
            {
                int fromId = (int)from;
                int toId = (int)to;

                DialoguesComponents.NodeBinder.AddConnection(fromId, toId);
            }

            return connectionData;
        }
    }
}